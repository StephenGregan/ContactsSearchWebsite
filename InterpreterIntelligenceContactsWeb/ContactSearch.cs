using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
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
        // Change with contacts information.  This is just a reference.
        public DocumentSearchResult Search(string searchText, string businessTitleFacet, string postingtypeFacet, string salaryRangeFacet,
            string sortType, double lat, double lng, int currentPage, int maxDistance, string maxDistanceLat, string maxDistanceLng)
        {
            try
            {
                SearchParameters sp = new SearchParameters()
                {
                    SearchMode = SearchMode.Any,
                    Top = 10,
                    Skip = currentPage - 1,
                    Select = new List<String>() { "id", "agency", "postin_type", "num_of_positions", "business_title",
                    "salary_range_from", "slaray_range_to", "slary_frequency", "work_location", "job_description",
                    "posting_date", "geo_location", "tags" },
                    // Add Count
                    IncludeTotalResultCount = true,
                    HighlightFields = new List<String>() { "job_description" },
                    HighlightPreTag = "<b>",
                    HighlightPostTag = "</b>",
                    // Add Facets
                    Facets = new List<String>() { "business_title", "posting_type", "level", "slary_range_from,interval:50000"},
                };
                // Define the sort type
                if (sortType == "featured")
                {
                    sp.ScoringProfile = "contactsScoringFeatured"; // Using a scoring profile
                    sp.ScoringParameters = new List<ScoringParameter>();
                    sp.ScoringParameters.Add(new ScoringParameter("featuredParam", new[] { "featured" }));
                    sp.ScoringParameters.Add(new ScoringParameter("mapCenterParam", GeographyPoint.Create(lat, lng)));
                }
                else if (sortType == "salaryDesc")
                {
                    sp.OrderBy = new List<String>() { "slary_range_from desc" };
                }
                else if (sortType == "salaryIncr")
                {
                    sp.OrderBy = new List<String>() { "slary_range_from" };
                }
                else if (sortType == "mostRecent")
                {
                    sp.OrderBy = new List<String>() { "posting_date desc" };
                }

                // Add filtering
                string filter = null;
                if (businessTitleFacet != "")
                {
                    filter = "business_title eq '" + businessTitleFacet + "'";
                    if (postingtypeFacet != "")
                    {
                        if (filter != null)
                        {
                            filter += " and ";
                        }
                        filter += "posting_type eq '" + postingtypeFacet + "'";
                    }
                    if (salaryRangeFacet != "")
                    {
                        if (filter != null)
                        {
                            filter += " and ";
                        }
                        filter += "salary_range_from ge" + salaryRangeFacet + " and salary_range_from lt " + (Convert.ToInt32(salaryRangeFacet));
                    }
                    if (maxDistance > 0)
                    {
                        if (filter != null)
                        {
                            filter += " and ";
                        }
                        filter += "geo.distance(geo_location,geography'POINT(" + maxDistanceLng + " " + maxDistanceLat + ")')le " + maxDistance.ToString();
                    }
                    sp.Filter = filter;
                    return _indexClient.Documents.Search(searchText, sp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}", ex.Message.ToString());
            }
            return null;
        }

        public DocumentSearchResult Suggest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}", ex.Message.ToString());
            }
            return null;
        }

        public Document LookUp(string id)
        {
            try
            {
                return _indexClient.Documents.Get(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }
    }
}