using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterpreterIntelligenceContactsWeb.Models
{
    public class Contacts
    {
        public FacetResults Facets { get; set; }
        public IList<SearchResult> Results { get; set; }
        public int? Count { get; set; }
    }

    public class ContactslookUp
    {
        public Document Result { get; set; }
    }    
}