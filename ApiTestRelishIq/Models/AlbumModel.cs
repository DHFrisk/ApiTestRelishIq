using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace ApiTestRelishIq.Models
{
    public class AlbumModel
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }

    }
}
