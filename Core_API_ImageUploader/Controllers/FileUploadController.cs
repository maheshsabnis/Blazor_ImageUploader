using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Core_API_ImageUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        
        [HttpPost("file/upload")]
        public async Task<IActionResult> Upload([FromForm]IFormFile file)
        {
            try
            {
                
                if (!CheckFileExtension(file))
                {
                    return BadRequest($"The File does not have an extension or it is not image. " +
                        $"The Expected extension is .jpg/.png/.bmp");
                }

                if (!CheckFileSize(file))
                {
                    return BadRequest($"The size of file is more than 10 mb, " +
                        $"please make sure that the file size must be less than 10 mb");
                }
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                 
                if (file.Length > 0)
                {
                   
                    var postedFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition)
                        .FileName.Trim('"');
                    
                    
                    var finalPath = Path.Combine(folder, postedFileName);
                    using (var fs = new FileStream(finalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }
                    return Ok($"File is uploaded Successfully");
                }
                else
                {
                    return BadRequest("The File is not received.");
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Some Error Occcured while uploading File {ex.Message}");
            }
        }
        /// <summary>
        /// The file extension must be jpg/bmp/png
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckFileExtension(IFormFile file)
        {
            string[] extensions = new string[] { "jpg", "bmp", "png" };

            var fileNameExtension = file.FileName.Split(".")[1];
            if (string.IsNullOrEmpty(fileNameExtension) || !extensions.Contains(fileNameExtension))
            {
                return false;
            }
           
            return true;
        }
        /// <summary>
        /// Check the file size, it must be less than 10 mb
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckFileSize(IFormFile file)
        {
            if (file.Length > 1e+7)
            {
                return false;
            }
            return true;
        }
    }
}
