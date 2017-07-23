using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using FlubuCore.IO.Wrappers;
using FlubuCore.WebApi.Controllers.Exception;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PackagesController : ControllerBase
    {
        private readonly string[] allowedFileExtension = new []{"zip", "7z", "rar"};
        
        private readonly IHostingEnvironment _hostingEnvironment;

        public PackagesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

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
         
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "packages");

            foreach (var formFile in form.Files)
            {
                var fileExtension = Path.GetExtension(formFile.FileName);
                if (!allowedFileExtension.Contains(fileExtension))
                {
                    if (form.Files.Count == 1)
                    {
                        throw new HttpError(HttpStatusCode.Forbidden, "FileExtensionNotAllowed", $"File extension {fileExtension} not allowed.");
                    }
                }

                if (formFile.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, formFile.FileName), FileMode.Create))
                    {
                        await formFile.CopyToAsync(fileStream);
                    }
                }
            }

            return Ok();
        }

        [HttpDelete]
        public IActionResult CleanPackagesDirectory()
        {
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "packages");
            System.IO.DirectoryInfo di = new DirectoryInfo(uploads);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            return Ok();
        }
    }
}
