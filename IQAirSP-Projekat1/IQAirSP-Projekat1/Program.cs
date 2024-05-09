﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Text.Json;
using IQAirSP_Projekat1;
class Program
{
    // Inicijalizacija Cache-a, api kljuca i sajta
    private static readonly Cache Cache = new Cache();
    private static readonly string ApiKey = "f159ff6f-1f31-4b01-ad29-fc5208f223d6";
    private static readonly string ApiBaseUrl = "http://api.airvisual.com/v2/city";
    private static readonly HttpClient http = new HttpClient();

    static void Main()
    {
        // Http Listener
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.WriteLine("Slusam na portu 8080...");

        while (true)
        {
            //Osluskuje promeni i kada do nje dodje poziva Request
            ThreadPool.QueueUserWorkItem(Request, listener.GetContext());
        }
    }

    static void Request(object state)
    {
        var context = (HttpListenerContext)state;
        var request = context.Request;
        var response = context.Response;

        // Pokupljamo URL
        string url = request.Url.ToString();
        Console.WriteLine($"Request: {url}");

        IQAir responseData;

        // Lockujemo Cache i pitamo da li se u cache-u vec nalazi taj request
        lock (Cache)
        {
            if (Cache.Sadrzi(url))
            {
                // Ako se request vec nalazi u cache-u, informacije pribavljamo iz cache-a
                responseData = Cache.CitajIzKesa(url);

            }
            else
            {
                // Ako se request ne nalazi u cache-u, informacije pribavljamo sa IQ Air sajta
                responseData = GetData(url);
                if (responseData == null) return;
                Cache.UpisiUKes(url, responseData);
            }
        }

        // Prikaz korisniku na browseru
        HTMLBuilder html = new HTMLBuilder(responseData);
        byte[] buffer = Encoding.UTF8.GetBytes(html.Build());
        response.ContentLength64 = buffer.Length;
        response.ContentType = "text/html";
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    static IQAir GetData(string url)
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
                webResponse = http.GetAsync(apiUrl).Result;

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
                webResponse = http.GetAsync(apiUrl).Result;

                if (!webResponse.IsSuccessStatusCode)
                {
                    throw new Exception(webResponse.StatusCode.ToString());
                }
            }
            using (var streamReader = new StreamReader(webResponse.Content.ReadAsStream()))
            {
                var responseText = streamReader.ReadToEnd();

                // Parsiramo iz JSON-a u IQAir objekat i vracamo
                IQAir responseObj = JsonSerializer.Deserialize<IQAir>(responseText);
                return responseObj;
            }
        }
        catch (Exception ex)
        {
            return new IQAir("Error: " + ex);
        }
    }
}