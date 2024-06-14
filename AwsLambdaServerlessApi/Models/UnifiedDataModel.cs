namespace AwsLambdaServerlessApi.Models
{
    public class UnifiedDataModel
    {
        /* 
         * All these vars are created because the test requires an specific
         * format/way of returning the json object, which dismisses the ids
         * that are not from the object:
         * Photo var: albumId
         * Album var: userId
        */

        public Photo photo2 = new Photo();
        public Album album2 = new Album();
        public User user2 = new User();

        public class Photo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public string thumbnailUrl { get; set; }
            public Album album { get; set; }

        }

        public class Album
        {
            public int id { get; set; }
            public string title { get; set; }
            public User user { get; set; }

        }

        public class User
        {
            public int id { get; set; }
            public string name { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public UserAddressModel address { get; set; }
            public string phone { get; set; }
            public string website { get; set; }
            public UserCompanyModel company { get; set; }

        }

        public UnifiedDataModel (PhotoModel photo, AlbumModel album, UserModel user)
        {
            this.user2.id = user.id;
            this.user2.name = user.name;
            this.user2.username = user.username;
            this.user2.email = user.email;
            this.user2.address = user.address;
            this.user2.phone = user.phone;
            this.user2.website = user.website;
            this.user2.company = user.company;

            this.album2.id = album.id;
            this.album2.title = album.title;
            this.album2.user = user2;

            this.photo2.title = photo.title;
            this.photo2.id = photo.id;
            this.photo2.url = photo.url;
            this.photo2.thumbnailUrl = photo.thumbnailUrl;
            this.photo2.album = album2;
        }

        public Photo GetData()
        {
            return this.photo2;
        }

    }
}
