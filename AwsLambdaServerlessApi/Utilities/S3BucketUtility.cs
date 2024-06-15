using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwsLambdaServerlessApi.Utilities
{
    public class S3BucketUtility
    {
        IAmazonS3 client;

        public S3BucketUtility()
        {
            this.client = new AmazonS3Client();
        }

        public async Task<(bool, string)> UploadFileAsync(
                string bucketName,
                string objectName,
                string file)
        {
            PutObjectResponse response = null;
            try
            {
                Amazon.S3.Model.PutObjectRequest request = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectName,
                    ContentBody = file
                };

                response = await this.client.PutObjectAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return (true, response.HttpStatusCode.ToString());
                }

                return (false, JsonSerializer.Serialize(response).ToString());
            }
            catch
            (Exception ex)
            {
                throw new Exception(ex.ToString() + " ------> " + JsonSerializer.Serialize(response).ToString());
            }
        }
    }
}
