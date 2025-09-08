using Microsoft.AspNetCore.Mvc;

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
            return View("/Views/ProgramManager/ApproveClaims.cshtml");
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

        // pre approve claims 
        public IActionResult PreApproveClaims()
        {
            return View("/Views/ProgramCoordinator/PreApproveClaims.cshtml");
        }

    }
}
