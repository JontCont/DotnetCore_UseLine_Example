using Microsoft.AspNetCore.Mvc;

namespace StartFMS_BackendAPI.Controllers {
    public class UserAuthorizedController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
