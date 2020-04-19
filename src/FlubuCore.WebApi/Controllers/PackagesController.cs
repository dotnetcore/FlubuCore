using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FlubuCore.WebApi.Controllers.Exceptions;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#if NETCOREAPP3_1
  using Microsoft.Extensions.Hosting;
#else
  using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlubuCore.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PackagesController : ControllerBase
    {
        private readonly string[] _allowedFileExtension = { ".zip", ".7z", ".rar" };
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<PackagesController> _logger;

        public PackagesController(IHostEnvironment hostEnvironment, ILogger<PackagesController> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// Upload specified packages to flubu server.
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse(200)]
        [SwaggerResponse(400, Type = typeof(ErrorModel), Description = "Error codes: FormHasNoContentType, NoFiles")]
        [SwaggerResponse(401)]
        [SwaggerResponse(403, Type = typeof(ErrorModel), Description = "Error codes: FileExtensionNotAllowed")]
        [SwaggerResponse(500, Type = typeof(ErrorModel), Description = "Internal Server error occured.")]
        [HttpPost("Upload")]
        [HttpPost]
        public async Task<IActionResult> UploadPackage()
        {
            if (!Request.HasFormContentType)
            {
                throw new HttpError(HttpStatusCode.BadRequest, "FormHasNoContentType");
            }

            var form = Request.Form;

            if (form == null || form.Files.Count == 0)
            {
                throw new HttpError(HttpStatusCode.BadRequest, "NoFiles");
            }

            var uploadDirectory = GetUploadDirectory();

            foreach (var formFile in form.Files)
            {
                var fileExtension = Path.GetExtension(formFile.FileName);
                if (!_allowedFileExtension.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    if (form.Files.Count == 1)
                    {
                        throw new HttpError(HttpStatusCode.Forbidden, "FileExtensionNotAllowed", $"File extension {fileExtension} not allowed.");
                    }
                }

                if (formFile.Length > 0)
                {
                    var uploadPath = Path.Combine(uploadDirectory, formFile.FileName);
                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(fileStream);
                    }

                    _logger.LogInformation($"Uploaded {uploadPath}");
                }
            }

            return Ok();
        }

        /// <summary>
        /// Delete all packages from flubu server (Cleans packages directory or specified subfolder).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [SwaggerResponse(200)]
        [SwaggerResponse(401)]
        [SwaggerResponse(500, Type = typeof(ErrorModel), Description = "Internal Server error occured.")]
        [HttpDelete]
        public IActionResult CleanPackagesDirectory([FromBody]CleanPackagesDirectoryRequest request)
        {
            var uploadDirectory = Path.Combine(_hostEnvironment.ContentRootPath, "Packages");

            if (!string.IsNullOrWhiteSpace(request.SubDirectoryToDelete))
            {
                var destDirPath = Path.GetFullPath(Path.Combine(uploadDirectory + Path.DirectorySeparatorChar));
                uploadDirectory = Path.GetFullPath(Path.Combine(uploadDirectory, request.SubDirectoryToDelete));

                if (!uploadDirectory.StartsWith(destDirPath))
                {
                    throw new HttpError(HttpStatusCode.Forbidden);
                }
            }

            try
            {
                if (Directory.Exists(uploadDirectory))
                {
                    Directory.Delete(uploadDirectory, true);
                }
            }
            catch (IOException)
            {
                Thread.Sleep(1000);
                Directory.Delete(uploadDirectory, true);
            }

            Directory.CreateDirectory(uploadDirectory);

            return Ok();
        }

        private string GetUploadDirectory()
        {
            var form = Request.Form;
            var uploadDirectory = Path.Combine(_hostEnvironment.ContentRootPath, "Packages");
            if (form.ContainsKey("request"))
            {
                StringValues request = form["request"];
                var json = request[0];
                UploadPackageRequest uploadPackageRequest = null;
                try
                {
                     uploadPackageRequest = JsonConvert.DeserializeObject<UploadPackageRequest>(json);
                }
                catch (System.Exception e)
                {
                    _logger.LogWarning(
                        $"request was present but was not of type UploadPackageRequest. Package will be uploaded to root directory. Excetpion: {e}");
                    return uploadDirectory;
                }

                    if (!string.IsNullOrWhiteSpace(uploadPackageRequest.UploadToSubDirectory))
                    {
                        var destDirPath = Path.GetFullPath(Path.Combine(uploadDirectory + Path.DirectorySeparatorChar));
                        uploadDirectory = Path.GetFullPath(Path.Combine(uploadDirectory, uploadPackageRequest.UploadToSubDirectory));

                        if (!uploadDirectory.StartsWith(destDirPath))
                        {
                            throw new HttpError(HttpStatusCode.Forbidden);
                        }
                    }
            }

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            return uploadDirectory;
        }
    }
}
