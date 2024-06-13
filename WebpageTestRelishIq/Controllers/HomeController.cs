using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using WebpageTestRelishIq.Models;

namespace WebpageTestRelishIq.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Gallery()
        {
            List<UnifiedDataModel.Photo> photos;
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpResponseMessage response;

            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"{Constants.Constants.apiUrl}/all");

            response = await client.SendAsync(request);

            if (response == null || !response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var streamResponse = await response.Content.ReadAsStreamAsync();
            photos = JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(streamResponse);

            return View("Gallery", photos);
        }
        
        [HttpGet]
        public async Task<IActionResult> FilterGallery(
            string photoTitle = "",
            string albumTitle = "",
            string userEmail = "",
            int limit = 25,
            int offset = 0)
        {
            List<UnifiedDataModel.Photo> photos;
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpResponseMessage response;
            string apiParameters = string.Empty;

            apiParameters += "limit=" + limit.ToString();
            apiParameters += "&offset=" + offset.ToString();
            apiParameters = !string.IsNullOrEmpty(photoTitle) ? apiParameters + "&title=" + photoTitle : apiParameters;
            apiParameters = !string.IsNullOrEmpty(albumTitle) ? apiParameters + "&album.title=" + albumTitle : apiParameters;
            apiParameters = !string.IsNullOrEmpty(userEmail) ? apiParameters + "&album.user.email=" + userEmail : apiParameters;

            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"{Constants.Constants.apiUrl}?{apiParameters}");

            response = await client.SendAsync(request);

            if (response == null || !response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var streamResponse = await response.Content.ReadAsStreamAsync();
            photos = JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(streamResponse);

            return View("Gallery", photos);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
