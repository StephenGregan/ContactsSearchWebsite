using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace InterpreterIntelligenceContactsWeb
{
    public class ContactSearch
    {
        private static SearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClient;
        private static string IndexName = "[AddNameOfIndex]";

        public static string errorMessage;

        static ContactSearch()
        {
            try
            {
                string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
                string apiKey = ConfigurationManager.AppSettings["SearchServiceApiKey"];

                _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
                _indexClient = _searchClient.Indexes.GetClient(IndexName);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message.ToString();
            }
        }

        public DocumentSearchResult Search()
        {

            return null;
        }

        public DocumentSearchResult Suggest()
        {
            return null;
        }

        public Document LookUp()
        {

            return null;
        }
    }
}