using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JsonLoader
{
    class Program
    {
        private static string TargetSearchServiceName = ConfigurationManager.AppSettings["TargetSearchServiceName"];
        private static string TargetSearchServiceApiKey = ConfigurationManager.AppSettings["TargetSearchServiceApiKey"];
        private static HttpClient HttpClient;
        private static Uri ServiceUri;

        static void Main(string[] args)
        {
            try
            {
                ServiceUri = new Uri("https://" + TargetSearchServiceName + ".search.windows.net");
                HttpClient = new HttpClient();
                HttpClient.DefaultRequestHeaders.Add("api-key", TargetSearchServiceApiKey);

                LaunchImportProcess("allcontacts");

                Console.WriteLine("Press any key to continue.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
            Console.ReadLine();
        }

        private static void LaunchImportProcess(string IndexName)
        {
            Console.WriteLine("Deleting " + IndexName + "index...");
            DeleteIndex(IndexName);
            Console.WriteLine("Creating " + IndexName + "index...");
            CreateIndex(IndexName);
            Console.WriteLine("Uploading data to " + IndexName + "...");
            ImportFromJson(IndexName);
        }

        private static void DeleteIndex(string IndexName)
        {
            try
            {
                try
                {
                    Uri uri = new Uri(ServiceUri, "/indexes" + IndexName);
                    HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpClient, HttpMethod.Delete, uri);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting index: {0}\r\n", ex.Message);
            }
        }

        static void CreateIndex(string IndexName)
        {
            string json = File.ReadAllText("" + IndexName + ".schema");
            try
            {
                Uri uri = new Uri(ServiceUri, "/indexes");
                HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpClient, HttpMethod.Post, uri, json);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message.ToString());
            }
        }

        static void ImportFromJson(string IndexName)
        {
            try
            {
                foreach (string fileName in Directory.GetFiles("", IndexName + "*.json"))
                {
                    Console.WriteLine("Uploading documents from file: {0}", fileName);
                    string json = File.ReadAllText(fileName);
                    Uri uri = new Uri(ServiceUri, "/indexes/" + IndexName + "/docs/index");
                    HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpClient, HttpMethod.Post, uri, json);
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message.ToString());
            }
        }
    }
}
