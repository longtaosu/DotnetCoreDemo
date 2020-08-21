using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Minio.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BucketObjectController : ControllerBase
    {
        private MinioClient _minioClient;
        public BucketObjectController(MinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        [HttpPost]
        public bool UploadFile(IFormFile file)
        {
            if (file is null)
                return false;
            try
            {
                _minioClient.PutObjectAsync("fds", file.FileName, file.OpenReadStream(), file.Length);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }           
        }

[HttpGet]
public string GetObjectUrl(string fileName)
{
    //获取下载地址
    return _minioClient.PresignedGetObjectAsync("fds", "minio.png",10).Result;
}
    }
}
