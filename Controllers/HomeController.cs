using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuotesAPI.Models;

namespace QuotesAPI.Controllers
{
   public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}