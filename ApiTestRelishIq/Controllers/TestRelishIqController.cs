using Microsoft.AspNetCore.Mvc;
using ApiTestRelishIq.Constants;
//using Newtonsoft.Json;
using System.Text.Json;
using ApiTestRelishIq.Models;
using NuGet.DependencyResolver;
using static ApiTestRelishIq.Models.UnifiedDataModel;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ApiTestRelishIq.Controllers
{
    [Route("api/[controller]/photos/")]
    [ApiController]
    public class TestRelishIqController : ControllerBase
    {
        private HttpResponseMessage response;
        private HttpRequestMessage request;
        private HttpClient client = new HttpClient();


        // Get single photo nesting its album and user data
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            PhotoModel photo = new PhotoModel();
            AlbumModel album = new AlbumModel();
            UserModel user = new UserModel();
            UnifiedDataModel unifiedData;

            try
            {
                // Get single photo
                this.request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"{Constants.Constants.urlPhotos}/{id}"
                    );
                this.response = await this.client.SendAsync(request);
                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                var streamResponse = await this.response.Content.ReadAsStreamAsync();
                photo = JsonSerializer.Deserialize<PhotoModel>(streamResponse);

                // Get photo's album data
                this.request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"{Constants.Constants.urlAlbums}/{photo.albumId}"
                    );
                this.response = await this.client.SendAsync(this.request);
                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                streamResponse = await this.response.Content.ReadAsStreamAsync();
                album = JsonSerializer.Deserialize<AlbumModel>(streamResponse);

                // Get album's user data
                this.request = new HttpRequestMessage(
                   HttpMethod.Get,
                   $"{Constants.Constants.urlUsers}/{album.userId}"
                   );
                this.response = await client.SendAsync(this.request);
                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                streamResponse = await response.Content.ReadAsStreamAsync();
                user = JsonSerializer.Deserialize<UserModel>(streamResponse);

                // Merge data into one object
                unifiedData = new UnifiedDataModel(photo, album, user);
                return JsonSerializer.Serialize<UnifiedDataModel.Photo>(unifiedData.GetData());
            }
            catch (Exception ex)
            {
                // Register the exception somehwere
            }

            return null;
        }

        [HttpGet]
        public async Task<string> Get(
            [FromQuery(Name = "title")] string photoTitle = "",
            [FromQuery(Name = "album.title")] string albumTitle = "",
            [FromQuery(Name = "album.user.email")] string userEmail = "",
            [FromQuery(Name = "limit")] int limit = 25,
            [FromQuery(Name = "offset")] int offset = 0)
        {
            List<UnifiedDataModel.Photo> data;
            IEnumerable<UnifiedDataModel.Photo> filteredData;
            List<UnifiedDataModel.Photo> result;

            // Nothing is received
            if (string.IsNullOrEmpty(photoTitle) && string.IsNullOrEmpty(albumTitle) && string.IsNullOrEmpty(userEmail))
            {
                return null;
            }

            // Only photo title is received
            else if (!string.IsNullOrEmpty(photoTitle) && string.IsNullOrEmpty(albumTitle) && string.IsNullOrEmpty(userEmail))
            {
                result = JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(await this.GetPhotosByTitle(photoTitle));
                return await this.CutLength(limit, offset, result);
            }

            // Only album title is received
            else if (!string.IsNullOrEmpty(albumTitle) && string.IsNullOrEmpty(photoTitle) && string.IsNullOrEmpty(userEmail))
            {
                result = JsonSerializer.Deserialize <List<UnifiedDataModel.Photo>>(await this.GetPhotosByAlbumTitle(albumTitle));
                return await this.CutLength(limit, offset, result);
            }

            // Only user email is received
            else if (!string.IsNullOrEmpty(userEmail) && string.IsNullOrEmpty(photoTitle) && string.IsNullOrEmpty(albumTitle))
            {
                result = JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(await this.GetPhotosByUserEmail(userEmail));
                return await this.CutLength(limit, offset, result);
            }

            // Photo and album titles' are received
            else if (!string.IsNullOrEmpty(photoTitle) && !string.IsNullOrEmpty(albumTitle) && string.IsNullOrEmpty(userEmail))
            {
                data = JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(await this.GetPhotosByAlbumTitle(albumTitle));

                if (data == null || data.Count == 0)
                {
                    return null;
                }

                filteredData =
                    from photo in data
                    where photo.title.Contains(photoTitle)
                    select photo;

                return await this.CutLength(limit, offset, filteredData.ToList());
            }

            // Photo title and user email are received
            else if (!string.IsNullOrEmpty(photoTitle) && !string.IsNullOrEmpty(userEmail) && string.IsNullOrEmpty(albumTitle))
            {
                data = JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(await this.GetPhotosByTitle(photoTitle));

                filteredData =
                    from photo in data
                    where photo.album.user.email == userEmail
                    || photo.album.user.email.Contains(userEmail)
                    select photo;

                return await this.CutLength(limit, offset, filteredData.ToList());
            }

            // Album title and user email are received
            else if (!string.IsNullOrEmpty(albumTitle) && !string.IsNullOrEmpty(userEmail) && string.IsNullOrEmpty(photoTitle))
            {
                data = JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(await this.GetPhotosByAlbumTitle(albumTitle));

                filteredData =
                    from photo in data
                    where (photo.album.title == albumTitle
                        || photo.album.title.Contains(albumTitle))
                    && (photo.album.user.email == userEmail
                        || photo.album.user.email.Contains(userEmail))
                    select photo;

                return await this.CutLength(limit, offset, filteredData.ToList());
            }

            // 3 parameters are recevied
            else if (!string.IsNullOrEmpty(albumTitle) && !string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(photoTitle))
            {
                data = JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(await this.GetPhotosByTitle(photoTitle));

                filteredData =
                    from photo in data
                    where (photo.title == photoTitle
                        || photo.title.Contains(photoTitle))
                    && (photo.album.title == albumTitle
                        || photo.album.title.Contains(albumTitle))
                    && (photo.album.user.email == userEmail
                        || photo.album.user.email.Contains(userEmail))
                    select photo;

                return await this.CutLength(limit, offset, filteredData.ToList());
            }

            return null;
        }




        #region Photos

        // Get photos by title coincidence
        [NonAction]
        public async Task<string> GetPhotosByTitle(string title)
        {
            List<PhotoModel> photos = new List<PhotoModel>();
            IEnumerable<PhotoModel> filteredPhotos;
            List<UnifiedDataModel.Photo> filteredPhotos2 = new List<UnifiedDataModel.Photo>();

            try
            {
                // Get all pictures
                this.request = new HttpRequestMessage(
                    HttpMethod.Get,
                    Constants.Constants.urlPhotos
                    );

                this.response = await this.client.SendAsync(request);

                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                var streamResponse = await this.response.Content.ReadAsStreamAsync();
                photos = JsonSerializer.Deserialize<List<PhotoModel>>(streamResponse);

                //foreach(PhotoModel photo in photos)
                //{
                //    if (photo.title.Contains(title))
                //    {
                //        filteredPhotos.Add(photo);
                //    }
                //}

                // I like LINQ :)
                filteredPhotos =
                    from photo in photos
                    where photo.title.Contains(title)
                    select photo;

                foreach (PhotoModel photo in filteredPhotos)
                {
                    filteredPhotos2.Add(
                        JsonSerializer.Deserialize<UnifiedDataModel.Photo>(await this.Get(photo.id)));
                }

                return JsonSerializer.Serialize<List<UnifiedDataModel.Photo>>(filteredPhotos2);
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        [HttpGet("all")]
        public async Task<string> GetAllPhotos()
        {
            List<PhotoModel> photos = new List<PhotoModel>();
            IEnumerable<PhotoModel> filteredPhotos;
            List<UnifiedDataModel.Photo> filteredPhotos2 = new List<UnifiedDataModel.Photo>();

            try
            {
                // Get all pictures
                this.request = new HttpRequestMessage(
                    HttpMethod.Get,
                    Constants.Constants.urlPhotos
                    );

                this.response = await this.client.SendAsync(request);

                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                var streamResponse = await this.response.Content.ReadAsStreamAsync();
                photos = JsonSerializer.Deserialize<List<PhotoModel>>(streamResponse);

                for (int i = 0; i < 25; i++)
                {
                    PhotoModel photo = photos[i];
                    filteredPhotos2.Add(JsonSerializer.Deserialize<UnifiedDataModel.Photo>(await this.Get(photo.id)));
                }

                return JsonSerializer.Serialize(filteredPhotos2);
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        #endregion Photos


        #region Albums

        // Get photos by album title coincidences
        [NonAction]
        public async Task<string> GetPhotosByAlbumTitle(string title)
        {
            List<AlbumModel> albums = new List<AlbumModel>();
            IEnumerable<int> filteredAlbumsIds;
            List<PhotoModel> photos = new List<PhotoModel>();
            IEnumerable<int> filteredPhotosIds;
            List<UnifiedDataModel.Photo> filteredData = new List<UnifiedDataModel.Photo>();

            try
            {
                this.request = new HttpRequestMessage(
                HttpMethod.Get,
                Constants.Constants.urlAlbums
                );
                this.response = await this.client.SendAsync(request);

                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                var streamResponse = await this.response.Content.ReadAsStreamAsync();
                albums = JsonSerializer.Deserialize<List<AlbumModel>>(streamResponse);

                filteredAlbumsIds =
                    from album in albums
                    where album.title.Contains(title)
                    select album.id;

                this.request = new HttpRequestMessage(
                HttpMethod.Get,
                Constants.Constants.urlPhotos
                );

                this.response = await this.client.SendAsync(request);

                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                streamResponse = await this.response.Content.ReadAsStreamAsync();
                photos = JsonSerializer.Deserialize<List<PhotoModel>>(streamResponse);

                filteredPhotosIds =
                    from photo in photos
                    where filteredAlbumsIds.Contains(photo.albumId)
                    select photo.id;

                foreach (int id in filteredPhotosIds)
                {
                    filteredData.Add(JsonSerializer.Deserialize<UnifiedDataModel.Photo>(await this.Get(id)));
                }

                return JsonSerializer.Serialize(filteredData);

            }
            catch (Exception ex)
            {

            }

            return null;

        }

        #endregion Albums

        #region Users
        
        // Get photos by user id
        [NonAction]
        public async Task<string> GetPhotosByUserId(int userId)
        {
            List<AlbumModel> albums = new List<AlbumModel>();
            IEnumerable<int> filteredAlbumsIds;
            List<PhotoModel> photos = new List<PhotoModel>();
            IEnumerable<int> filteredPhotosIds;
            List<UnifiedDataModel.Photo> filteredData = new List<UnifiedDataModel.Photo>();

            try
            {
                this.request = new HttpRequestMessage(
                HttpMethod.Get,
                Constants.Constants.urlAlbums
                );
                this.response = await this.client.SendAsync(request);

                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                var streamResponse = await this.response.Content.ReadAsStreamAsync();
                albums = JsonSerializer.Deserialize<List<AlbumModel>>(streamResponse);

                filteredAlbumsIds =
                    from album in albums
                    where album.userId == userId
                    select album.id;

                this.request = new HttpRequestMessage(
                HttpMethod.Get,
                Constants.Constants.urlPhotos
                );

                this.response = await this.client.SendAsync(request);

                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                streamResponse = await this.response.Content.ReadAsStreamAsync();
                photos = JsonSerializer.Deserialize<List<PhotoModel>>(streamResponse);

                filteredPhotosIds =
                    from photo in photos
                    where filteredAlbumsIds.Contains(photo.albumId)
                    select photo.id;

                foreach (int id in filteredPhotosIds)
                {
                    filteredData.Add(JsonSerializer.Deserialize<UnifiedDataModel.Photo>(await this.Get(id)));
                }

                return JsonSerializer.Serialize(filteredData);

            }
            catch (Exception ex)
            {

            }

            return null;

        }


        // Get photos by user email
        [NonAction]
        public async Task<string> GetPhotosByUserEmail(string email)
        {
            List<UserModel> users = new List<UserModel>();
            IEnumerable<int> filteredUsersIds;
            List<PhotoModel> photos = new List<PhotoModel>();
            IEnumerable<UnifiedDataModel.Photo> filteredData = new List<UnifiedDataModel.Photo>();

            try
            {
                this.request = new HttpRequestMessage(
                HttpMethod.Get,
                Constants.Constants.urlUsers
                );
                this.response = await this.client.SendAsync(request);

                if (this.response == null || !this.response.IsSuccessStatusCode)
                {
                    throw new Exception("Error while getting data!");
                }

                var streamResponse = await this.response.Content.ReadAsStreamAsync();
                users = JsonSerializer.Deserialize<List<UserModel>>(streamResponse);


                // idk if is possible that more than 1 user could have the same email
                // (which btw shouldn't be the case, but you never know)
                // but in case of duplicity i will set it as a list, just in case
                filteredUsersIds =
                    from user in users
                    where user.email == email
                    || user.email.Contains(email)
                    select user.id;

                foreach (int id in filteredUsersIds)
                {
                    filteredData = filteredData.
                        Union(JsonSerializer.Deserialize<List<UnifiedDataModel.Photo>>(await this.GetPhotosByUserId(id)));
                }

                return JsonSerializer.Serialize(filteredData.ToList());
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        #endregion Users


        // Return a new list based on the received (or default) limit and offset
        [NonAction]
        public async Task<string> CutLength(int limit, int offset, List<UnifiedDataModel.Photo> photos)
        {
            List<UnifiedDataModel.Photo> filteredPhotos = new List<UnifiedDataModel.Photo>();

            if (offset > photos.Count())
            {
                return JsonSerializer.Serialize("Offset starting position is greater than photos array length!");
            }

            if (limit > (photos.Count() - offset))
                limit = photos.Count() - offset;

            for (int i = offset; i < (limit + offset); i++)
            {
                filteredPhotos.Add(photos[i]);
            }

            return JsonSerializer.Serialize(filteredPhotos);
        }
    }
}
