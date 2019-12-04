using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Final.Models;
using Syncfusion.Pdf;
using System.IO;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Syncfusion.Pdf.Parsing;

namespace Final.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //When the form is posted/submitted this will execute.
        [HttpPost]
        public IActionResult ConvertToPDF(List<IFormFile> images, string fileName)
        {
            //Creates a pdf with 0 pages.
            PdfDocument pdf = new PdfDocument();

            foreach (var img in images)
            {
                if (img.Length > 0)
                {
                    //Next two lines puts the image into the new memory stream.
                    MemoryStream file = new MemoryStream();
                    img.CopyTo(file);
                    //Loads the image from the memory stream into the custom PdfImage variable.
                    PdfImage image = PdfImage.FromStream(file);
                    //Adds a new page to our pdf.
                    PdfPage page = page = pdf.Pages.Add();

                    //Draws the image onto the pdf page.
                    page.Graphics.DrawImage(image, new PointF(0, 0));
                    file.Dispose();
                }
            }

            MemoryStream stream = new MemoryStream();
            //Saves the pdf to the new memory stream that will be used to return the result to our user.
            pdf.Save(stream);

            //Set the position as '0'.
            stream.Position = 0;

            //Downloads the pdf document to the browser. Second parameter specifies the type 
            //of file we will return to our user.
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");

            //The parameter fileName needs to have a value if one is not supplied because if not the file
            //will not download with a pdf extension.
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = "Default";
            }

            //Renames the file when downloading to the user. Without this line the pdf will
            //not download onto the users machine, it will open in the tab. We need to keep
            //the .pdf extension or the file will download as a File and will not be able to be opened.
            fileStreamResult.FileDownloadName = fileName + ".pdf";

            return fileStreamResult;
        }
    }
}
