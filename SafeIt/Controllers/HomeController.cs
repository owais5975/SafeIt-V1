using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeIt.Models;
using SafeIt.Models;
using SafeIt.Services;
using System.Security.Claims;

namespace SafeIt.Controllers
{
    public class HomeController : Controller
    {
        //private ApplicationDBContext _context;
        private readonly INotyfService _notyf;
        private readonly IFileService _fileService;

        public HomeController(INotyfService notyf, IFileService fileService)
        {
            _fileService = fileService;
            _notyf = notyf;
        }

        [Authorize]
        [HttpGet("Encrypt")]
        public IActionResult Encrypt()
        {
            return View();
        }

        [Authorize]
        [HttpPost("Encrypt")]
        public async Task<IActionResult> Encrypt(FileModel file)
        {
            try
            {
                if (!ModelState.IsValid) return View();

                if (!_fileService.IsExist(file.Document.FileName, file.Key))
                {
                    if(file.Document.Length > 5000000)
                    {
                        _notyf.Error("File size should be less than 5MB");
                        return View();
                    }
                    //Encryption
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EncryptedFiles", file.Document.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.Document.CopyToAsync(stream);
                    }
                    ExpressEncription.AESEncription.AES_Encrypt(filePath, file.Key);

                    FileInfo f = new FileInfo(filePath);
                    if (f.Exists) f.Delete();

                    FileInfo move = new FileInfo(filePath + ".aes");
                    if (move.Exists) move.MoveTo(filePath);

                    bool isSaved = await _fileService.AddFile(new AddFile
                    {
                        Name = file.Document.FileName,
                        Key = file.Key,
                    });
                    if (isSaved)
                    {
                        _notyf.Success("File encrypted successfully.");
                        return RedirectToAction("Home");
                    }
                    else
                    {
                        _notyf.Error("Failed to encrypt.");
                        return View();
                    }
                }
                _notyf.Warning("File already exist.");
                return View();

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message);
                return View();
            }
        }

        [HttpGet("Decrypt")]
        public IActionResult Decrypt()
        {
            ViewBag.Download = string.Empty;
            return View();
        }

        [HttpPost("Decrypt")]
        public async Task<IActionResult> Decrypt(FileModel file)
        {
            try
            {
                if (!ModelState.IsValid) return View();

                if(_fileService.IsExist(file.Document.FileName, file.Key))
                {
                    if (file.Document.Length > 5000000)
                    {
                        _notyf.Error("File size should be less than 5MB");
                        return View();
                    }
                    //Decryption
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DecryptedFiles", file.Document.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.Document.CopyToAsync(stream);
                    }
                    ExpressEncription.AESEncription.AES_Decrypt(filePath, file.Key);

                    FileInfo f = new FileInfo(filePath);
                    if (f.Exists) f.Delete();

                    FileInfo move = new FileInfo(filePath + ".decrypted");
                    if (move.Exists) move.MoveTo(filePath);
                    ViewBag.Download = "/DecryptedFiles/" + file.Document.FileName;
                    _notyf.Success("File Decrypted successfully");
                    return View();
                }
                _notyf.Error("Incorrect key.");
                return View();
            }
            catch (Exception ex)
            {
                _notyf.Success("Failed to decrypt.");
                return View();
            }
        }

        //public IActionResult Download(string filepath)
        //{
        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    return File(memory, GetContentType(path), Path.GetFileName(path));
        //}

        [Authorize]
        [HttpGet("Home")]
        public IActionResult Home()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetData()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                //var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                //var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = 10;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                var jsonData = _fileService.AllFiles(draw, skip, pageSize);
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        public IActionResult Error()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
    }
}
