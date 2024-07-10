using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Text.Json;
using IQAirSP_Projekat1;
using System.Runtime.CompilerServices;
using System.Diagnostics;
class Program
{
    // Inicijalizacija Cache-a, api kljuca i sajta
    private static readonly Cache Cache = new Cache();
    private static readonly string ApiKey = "f159ff6f-1f31-4b01-ad29-fc5208f223d6";
    private static readonly string ApiBaseUrl = "http://api.airvisual.com/v2/city";
    private static readonly HttpClient http = new HttpClient();

    static async Task Main()
    {
        // Http Listener
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.WriteLine("Slusam na portu 8080...");

        while (true)
        {
            //Osluskuje promenu i kada do nje dodje poziva Request
            var context = await listener.GetContextAsync();
            _ = Task.Run(() => Request(context));
        }
    }

    static async void Request(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        // Pokupljamo URL
        string url = request.Url.ToString();
        Console.WriteLine($"Request: {url}");
        if (url.Contains("favicon")) return;

        IQAir responseData;

        // Lockujemo Cache i pitamo da li se u cache-u vec nalazi taj request
        Stopwatch sw = new Stopwatch();
        sw.Start();
        try
        {
            if (Cache.Sadrzi(url))
            {
                // Ako se request vec nalazi u cache-u, informacije pribavljamo iz cache-a
                responseData = Cache.CitajIzKesa(url);
                sw.Stop();
                Console.WriteLine($"Vreme potrebno za pribavljanje podataka iz cache-a: {sw.ElapsedMilliseconds}ms"); ;
            }
            else
            {
                // Ako se request ne nalazi u cache-u, informacije pribavljamo sa IQ Air sajta
                responseData = GetData(url).GetAwaiter().GetResult();
                if (responseData == null) return;

                if (responseData.status != "TooManyRequests")
                {
                    Cache.UpisiUKes(url, responseData);
                }

                sw.Stop();
                Console.WriteLine($"Vreme potrebno za pribavljanje podataka bez upotrebe cache-a: {sw.ElapsedMilliseconds}ms");
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            Console.WriteLine(ex.ToString());
            return;
        }

        // Prikaz korisniku na browseru
        HTMLBuilder html = new HTMLBuilder(responseData);
        byte[] buffer = Encoding.UTF8.GetBytes(html.Build());
        response.ContentLength64 = buffer.Length;
        response.ContentType = "text/html";
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    static async Task<IQAir> GetData(string url)
    {
        // Pribavljamo uneti grad
        if (url.Contains("favicon")) return null;
        var query = WebUtility.UrlDecode(url.Substring(url.IndexOf('?') + 1));
        var parameters = query.Split('&');
        var city = "";
        foreach (var parameter in parameters)
        {
            var parts = parameter.Split('=');

            if (parts.Length == 2)
            {
                city = parts[1];
                break;
            }
        }

        if (string.IsNullOrEmpty(city))
        {
            return new IQAir("Error");
        }
        if (city == "Beograd") city = "Belgrade";
        try
        {
            HttpResponseMessage webResponse;
            try
            {
                // Proveravamo da li grad nalazi u centralnoj Srbiji
                query = $"city={city}&state=Central Serbia&country=Serbia";
                string apiUrl = $"{ApiBaseUrl}?{query}&key={ApiKey}";
                webResponse = await http.GetAsync(apiUrl);

                if (!webResponse.IsSuccessStatusCode)
                {
                    throw new Exception(webResponse.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                // Ako se ne nalazi u centralnoj Srbiji, nalazi se u Vojvodini
                query = $"city={city}&state=Autonomna Pokrajina Vojvodina&country=Serbia";
                string apiUrl = $"{ApiBaseUrl}?{query}&key={ApiKey}";
                webResponse = await http.GetAsync(apiUrl);

                if (!webResponse.IsSuccessStatusCode)
                {
                    throw new Exception(webResponse.StatusCode.ToString());
                }
            }
            Stream webResponseStream = await webResponse.Content.ReadAsStreamAsync();
            IQAir responseObj = await JsonSerializer.DeserializeAsync<IQAir>(webResponseStream);
            return responseObj;
        }
        catch (Exception ex)
        {
            return new IQAir(ex.Message);
        }
    }
}