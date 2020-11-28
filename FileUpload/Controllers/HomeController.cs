using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileUpload.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using FileUpload.Helpers;
using FileUpload.Filters;
using System.Numerics;

namespace FileUpload.Controllers
{
    public class HomeController : Controller
    {
        public BigInteger Hash { get; set; }
        public long Time { get; set; }

        public IActionResult Index()
        {
            ViewBag.Hash = Hash;
            ViewBag.Time = Time;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult RunModule(int points)
        {
            var command = $"ParcsNet.exe -t text.txt -p {points}";
            var pp = new ProcessStartInfo("cmd.exe", "/C" + command)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                WorkingDirectory = "C:\\Module",
            };
            using (var process = Process.Start(pp)) {
                process.WaitForExit();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public HashInfo SetModuleInfo()
        {
            var filePath = "C:\\Module\\result.txt";
            var result = System.IO.File.ReadAllText(filePath);
            if (result.Contains("$"))
            {
                System.IO.File.WriteAllText(filePath, string.Empty);
                var results = result.Split("$");
                return new HashInfo {
                    Hash = results[0],
                    Time = long.Parse(results[1])
                };
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(100000000)]
        public async Task<IActionResult> UploadStreamingFile()
        {
            // full path to file in temp location
            var filePath = "C:\\Module\\text.txt";
            System.IO.File.WriteAllText(filePath, string.Empty);
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await Request.StreamFile(stream);
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok();
        }
    }
}
