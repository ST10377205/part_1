using Microsoft.AspNetCore.Mvc;
using part_1.Models;

namespace part_1.Controllers
{
    public class LectureController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //submit claim view
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitClaim(claims claiming)
        {
            ViewBag.id = HttpContext.Session.GetString("id");
            ViewBag.role = HttpContext.Session.GetString("role");

            if (ModelState.IsValid)
            {
                // Handle optional file upload (SupportingDocument)
                if (claiming.SupportingDocument != null && claiming.SupportingDocument.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(claiming.SupportingDocument.FileName)}";
                    var filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        claiming.SupportingDocument.CopyTo(stream);
                    }

                    claiming.SupportingDocumentPath = "/uploads/" + uniqueFileName;
                }

                claiming.LecturerID = Convert.ToInt32(HttpContext.Session.GetString("id"));

                all_method method = new all_method();

                string result = method.claims_submit(claiming);

                ViewBag.message = result;
                return View(claiming);
            }

            return View(claiming);
        }

        //track claim view
        public IActionResult TrackClaim()
        {
            ViewBag.id = HttpContext.Session.GetString("id");
            ViewBag.role = HttpContext.Session.GetString("role");

            all_method method = new all_method();

            // Get all claims for the logged-in lecturer
            var claimsList = method.get_all_claims(ViewBag.id);

            return View(claimsList);
        }

        [HttpPost]
        public IActionResult DeleteClaim(int id)
        {
            all_method method = new all_method();
            method.delete_claim(id);
            return RedirectToAction("TrackClaim");
        }

    }
}
