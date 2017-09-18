using Microsoft.AspNetCore.Mvc;

namespace Plugin
{
    public class HelloController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
