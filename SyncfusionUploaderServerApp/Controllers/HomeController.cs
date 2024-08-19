using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SyncfusionUploaderServerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _uploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploaded Files");

        public async Task<IActionResult> Save(IFormFile UploadFiles)
        {
            if (!IsAuthorized())
            {
                return Unauthorized();
            }

            if (UploadFiles == null || UploadFiles.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            Directory.CreateDirectory(_uploads);

            var filePath = Path.Combine(_uploads, UploadFiles.FileName);
            var append = UploadFiles.ContentType == "application/octet-stream"; // Handle chunk upload
            await SaveFileAsync(UploadFiles, filePath, append);

            return Ok();
        }

        private bool IsAuthorized()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if(string.IsNullOrEmpty(authorizationHeader) || 
                !authorizationHeader.StartsWith("Bearer "))
            {
                return false;
            }
            var token = authorizationHeader["Bearer ".Length..];
            return token == "Your.JWT.Token";
        }

        private async Task SaveFileAsync(IFormFile file, string path, bool append)
        {
            await using var fileStream = new FileStream(path, append ? FileMode.Append : FileMode.Create);
            await file.CopyToAsync(fileStream);
        }

        public IActionResult Remove(string UploadFiles)
        {
            if(!IsAuthorized())
            {
                return Unauthorized();
            }

            try
            {
                var filePath = Path.Combine(_uploads, UploadFiles);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    Response.StatusCode = 200;
                    Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
                }
                else
                {
                    Response.StatusCode = 404;
                    Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File not found";
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = $"Error: {e.Message}";
            }

            return new EmptyResult();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
