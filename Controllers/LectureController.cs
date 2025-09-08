using Microsoft.AspNetCore.Mvc;

namespace part_1.Controllers
{
    public class LectureController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //submit claim view
        public IActionResult SubmitClaim()
        {
            return View();
        }

        //track claim view
        public IActionResult TrackClaim()
        {
            return View();
        }
    }
}
