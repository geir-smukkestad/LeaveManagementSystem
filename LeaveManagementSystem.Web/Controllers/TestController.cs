using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            var data = new TestViewModel()
            {
                Name = "Student",
                DateOfBirth = new DateTime(1972, 3, 29)
            };
            return View(data);
        }
    }
}
