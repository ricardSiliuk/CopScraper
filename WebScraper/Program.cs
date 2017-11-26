using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebScraper
{
    
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        async static Task getResponse(string vnr)
        {
            // Get the response.  
            var values = new Dictionary<string, string>
            {
                {
                    "vnr",vnr
                }
            };
            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.epolicija.lt/itpr_paieska/transportas_lt.php", content);

            var responseString = await response.Content.ReadAsStringAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);

            var tables = htmlDoc.DocumentNode.SelectNodes("//table");

            //Remove last table (table for another request)
            tables.RemoveAt(tables.Count - 1);

            if (tables.Count == 0)
            {
                Console.WriteLine("Valstybinis numeris nėra ieškomas");
                return;
            }

            Console.WriteLine("Ieskomi:");
            Console.WriteLine("==============");

            foreach (var table in tables)
            {
                // Select all rows
                var tableHTML = new HtmlDocument();
                tableHTML.LoadHtml(table.InnerHtml);
                var tableRows = tableHTML.DocumentNode.SelectNodes("//tr");
                // For each row
                foreach (var row in tableRows)
                {
                    // Select all cols
                    var rowHTML = new HtmlDocument();
                    rowHTML.LoadHtml(row.InnerHtml);
                    var tableCols = rowHTML.DocumentNode.SelectNodes("//td");
                    // For each col
                    foreach (var col in tableCols)
                    {
                        // Write elements in line
                        Console.Write(col.InnerHtml);
                    }
                    // Add newline 
                    Console.WriteLine("");
                }
                // Separate different tables
                Console.WriteLine("==============");
            }
        }

        static void Main(string[] args)
        {
            string vnr;
            System.Console.WriteLine("Ieskomas valstybes nr:");
            vnr = System.Console.ReadLine();
            Task x = getResponse(vnr);
            x.Wait();
            Console.WriteLine("Press any key to exit");
            System.Console.Read();
        }
    }
}
