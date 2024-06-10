using Microsoft.AspNetCore.Mvc;
using ApiTestRelishIq.Constants;
//using Newtonsoft.Json;
using System.Text.Json;
using ApiTestRelishIq.Models;

namespace ApiTestRelishIq.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestRelishIqController : ControllerBase
    {

        

        // GET api/<TestRelishIqController>/5
        [HttpGet("photos/{id}")]
        public async Task<string> Get(int id)
        {
            HttpResponseMessage response;
            HttpRequestMessage request;
            HttpClient client = new HttpClient();
            PhotoModel photo = new PhotoModel();
            AlbumModel album = new AlbumModel();
            UserModel user = new UserModel();
            UnifiedDataModel unifiedData;

            // Get single photo
            request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{Constants.Constants.urlPhotos}/{id}"
                );
            response = await client.SendAsync(request);
            if (response != null && response.IsSuccessStatusCode)
            {
                using var streamResponse = await response.Content.ReadAsStreamAsync();
                photo = await JsonSerializer.DeserializeAsync<PhotoModel>(streamResponse);
            }

            // Get photo's album data
            request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{Constants.Constants.urlAlbums}/{photo.albumId}"
                );
            response = await client.SendAsync(request);
            if (response != null && response.IsSuccessStatusCode)
            {
                using var streamResponse = await response.Content.ReadAsStreamAsync();
                album = await JsonSerializer.DeserializeAsync<AlbumModel>(streamResponse);
            }

            // Get album's user data
            request = new HttpRequestMessage(
               HttpMethod.Get,
               $"{Constants.Constants.urlUsers}/{album.userId}"
               );
            response = await client.SendAsync(request);
            if (response != null && response.IsSuccessStatusCode)
            {
                using var streamResponse = await response.Content.ReadAsStreamAsync();
                user = await JsonSerializer.DeserializeAsync<UserModel>(streamResponse);
            }

            // Merge data into one object
            unifiedData = new UnifiedDataModel(photo, album, user);

            return JsonSerializer.Serialize<UnifiedDataModel.Photo>(unifiedData.GetData());
        }

    }
}
