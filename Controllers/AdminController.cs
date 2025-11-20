using Microsoft.AspNetCore.Mvc;
using part_1.Models;

namespace part_1.Controllers
{
    public class AdminController : Controller
    {
        // program manager

        //index view 
        public IActionResult ProgramManager()
        {
            return View("/Views/ProgramManager/Index.cshtml");
        }

        //  approve claims
        public IActionResult ApproveClaims()
        {

            ViewBag.id = HttpContext.Session.GetString("id");
            ViewBag.role = HttpContext.Session.GetString("role");

            all_method method = new all_method();

            var claimsList = method.get_all_claims();


            return View("/Views/ProgramManager/ApproveClaims.cshtml",claimsList);
        }
        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            all_method method = new all_method();
            method.update_claim_status(id, "Approved");
            return RedirectToAction("ApproveClaims");
        }




        [HttpPost]
        public IActionResult RejectClaims(int id)
        {
            all_method method = new all_method();
            method.update_claim_status(id, "Rejected");
            return RedirectToAction("ApproveClaims");
        }
        // lectures view
        public IActionResult Lectures()
        {
            return View("/Views/ProgramManager/Lectures.cshtml");
        }

        //program coordinator
        //index view 
        public IActionResult ProgramCoordinator()
        {

            return View("/Views/ProgramCoordinator/Index.cshtml");
        }

        public IActionResult PreApproveClaims()
        {
            ViewBag.id = HttpContext.Session.GetString("id");
            ViewBag.role = HttpContext.Session.GetString("role");

            all_method method = new all_method();

            var claimsList = method.get_all_claim();
       

            return View("/Views/ProgramCoordinator/PreApproveClaims.cshtml", claimsList);
        }

        [HttpPost]
        public IActionResult DeleteClaim(int id)
        {
            all_method method = new all_method();
            method.delete_claim(id);
            return RedirectToAction("/Views/ProgramCoordinator/PreApproveClaims.cshtml");
        }

   
        [HttpPost]
        public IActionResult PreApproveClaim(int id)
        {
            all_method method = new all_method();
            method.update_claim_status(id, "Pre-approved");
            return RedirectToAction("PreApproveClaims");
        }




        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            all_method method = new all_method();
            method.update_claim_status(id, "Rejected");
            return RedirectToAction("PreApproveClaims");
        }



    }
}
