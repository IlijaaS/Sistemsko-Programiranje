using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQAirSP_Projekat1
{
    public class HTMLBuilder
    {
        private IQAir responseObj;

        public HTMLBuilder(IQAir responseObj)
        {
            this.responseObj = responseObj;
        }

        public string Build()
        {
            string htmlResponse = "<!DOCTYPE html><html><head><title>Air Pollution Data</title><style>\n";
            if (responseObj.status == "success") // ako su informacije uspesno pribavljene
            {
                htmlResponse += "div { text-align:center; }\nh1{color: green;}\n </style></head><body>";
                htmlResponse += "<div><h1>Air Pollution Data</h1></div>";
                htmlResponse += "<div><h2>" + "Grad: " + responseObj.data.city + "</h2></div></pre>";
                htmlResponse += "<div><h2>" + "Zemlja: " + responseObj.data.country + "</h2></div></pre>";

                // mogao je switch ovde
                if (responseObj.data.current.pollution.aqius <= 50) htmlResponse += "<div><h2 style=\"color:green;\">" + "Nivo Zagadjenja: " + responseObj.data.current.pollution.aqius + "</h2></div></pre>";
                else if (responseObj.data.current.pollution.aqius <= 100) htmlResponse += "<div><h2 style=\"color:yellow;\">" + "Nivo Zagadjenja: " + responseObj.data.current.pollution.aqius + "</h2></div></pre>";
                else if (responseObj.data.current.pollution.aqius <= 150) htmlResponse += "<div><h2 style=\"color:orange;\">" + "Nivo Zagadjenja: " + responseObj.data.current.pollution.aqius + "</h2></div></pre>";
                else if (responseObj.data.current.pollution.aqius <= 200) htmlResponse += "<div><h2 style=\"color:red;\">" + "Nivo Zagadjenja: " + responseObj.data.current.pollution.aqius + "</h2></div></pre>";
                else if (responseObj.data.current.pollution.aqius <= 300) htmlResponse += "<div><h2 style=\"color:purple;\">" + "Nivo Zagadjenja: " + responseObj.data.current.pollution.aqius + "</h2></div></pre>";
                else if (responseObj.data.current.pollution.aqius > 300) htmlResponse += "<div><h2 style=\"color:#32012F;\">" + "Nivo Zagadjenja: " + responseObj.data.current.pollution.aqius + "</h2></div></pre>";
                htmlResponse += "<div><h2>" + "Trenutna temperatura: " + responseObj.data.current.weather.tp + " &#176;C" + "</h2></div></pre>";
                htmlResponse += "<div><h2>" + "Vlaznost vazduha: " + responseObj.data.current.weather.hu + "%" + "</h2></div></pre>";
                htmlResponse += "<div><h2>" + "Brzina vetra: " + responseObj.data.current.weather.ws + "m/s" + "</h2></div></pre>";
                htmlResponse += "<div><h2>" + "Vreme izmerenih parametara: " + responseObj.data.current.pollution.ts.ToLongDateString() + " " + responseObj.data.current.pollution.ts.ToLongTimeString() + "</h2></div></pre>";
                htmlResponse += "</body></html>";
            }
            else // ako je doslo do greske
            {
                htmlResponse += "div { text-align:center; }\nh1 { color: red; text-align:center; }\n </style></head><body>";
                htmlResponse += "<div><h1>Air Pollution Data</h1></div>";
                htmlResponse += "<div><h4>" + "Grad koji ste uneli ne postoji ili ne postoje informacije o zagadjenu u ovom gradu!" + "</h4></div></pre>";
                htmlResponse += "</body></html>";
            }

            return htmlResponse;
        }
    }
}
