using Microsoft.AspNetCore.Mvc;
using StressFree.Disney.Application;

namespace StressFree.Disney.Web.Controllers
{
    [Route("api/[controller]")]
    public class WordsController : Controller
    {
        readonly IWordApplication wordApplication;

        public WordsController(IWordApplication wordApplication)
        {
            this.wordApplication = wordApplication;
        }

        [HttpGet("board")]
        public IActionResult Board()
        {
            var response = wordApplication.GetInitialBoard();
            return Json(response);
        }
    }
}