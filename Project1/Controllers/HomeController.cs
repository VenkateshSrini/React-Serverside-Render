using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project1.ViewModel;

namespace Project1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var vm = new ReactAppVM(HttpContext);

            return View(vm);
        }
    }
}
