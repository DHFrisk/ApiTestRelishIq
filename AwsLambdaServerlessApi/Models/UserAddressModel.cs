using System.Net.Security;

namespace AwsLambdaServerlessApi.Models
{
    public class UserAddressModel
    {
        public string street {  get; set; }
        public string suite { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public UserAddressGeoModel geo { get; set; }
        
    }
}
