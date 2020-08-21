using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio.DataModel;

namespace Minio.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        private MinioClient _minioClient;
        public BucketController(MinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        [HttpGet]
        public bool MakeBucket(string bucketName)
        {
            _minioClient.MakeBucketAsync(bucketName).Wait();
            return true;
        }

[HttpGet]
public bool IsBucketExist(string bucketName)
{
    var isExist = _minioClient.BucketExistsAsync(bucketName).Result;
    return isExist;
}

        [HttpGet]
        public List<Bucket> GetBucketList()
        {
            var list = _minioClient.ListBucketsAsync().Result;
            return list.Buckets;
        }

        [HttpGet]
        public bool RemoveBucket(string bucketName)
        {
            _minioClient.RemoveBucketAsync(bucketName).Wait();
            return true;
        }
    }
}
