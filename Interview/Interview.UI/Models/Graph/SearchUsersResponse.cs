using System.Net;

namespace Interview.UI.Models.Graph
{
    
    public class SearchUsersResponse
    {

        public HttpStatusCode statusCode { get; set; } 

        public string filterClause { get; set; }

        public List<GraphUser> value { get; set; }

        public object badRequest { get; set; }

    }

}
