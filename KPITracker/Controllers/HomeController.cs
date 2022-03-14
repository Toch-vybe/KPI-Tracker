using KPITracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KPITracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Calculate cal, dynamic viewBag)
        {
            // The next three lines solves the average rating depending on the number of rating recieved.
            var cummulativeResponse = (cal.FiveStar * 5) + (cal.FourStar * 4) + (cal.ThreeStar * 3) + (cal.TwoStar * 2) + (cal.OneStar * 1);

            var totalResponse = cal.FiveStar + cal.FourStar + cal.ThreeStar + cal.TwoStar + cal.OneStar;

            var result = cummulativeResponse / totalResponse;

            // convert to string
            var answer = result.ToString("0.00");

            var passMark = 4.61;

            // use the formular to calculate for number of 5 stars to be gotten to meet a target score: n = (4.61b - a*b)/0.39
            var a = result;
            var b = totalResponse;

            var nCsat = ((4.61 * b) - a * b) / 0.39;
            var starsNeeded = nCsat.ToString("0");

            // use the formular to calculate for number of Dsat to get to miss the target score: nDsat = (ab - 4.61b) / 3.61
            var nDsat = ((a*b) - (4.61*b)) / 3.61;

            //var starsToAvoid = nDsat.ToString("0");
            decimal starsToAvoid = (decimal)nDsat;

            //round off to the next whole number, so for instance 0.2 will be rounded to 1
            starsToAvoid = Math.Ceiling(starsToAvoid);

            //for showing progress bar, nDsat is multiplied by 10 and used in percentage form
            var csatStrenght = nDsat * 10; 
            var csatStrenghtString = csatStrenght.ToString("0");
            var csatStr = csatStrenghtString + "%";


            if (result < passMark) 
            {  
                TempData["success"] = answer;
                TempData["starsNeeded"] = starsNeeded; 
            }
            else
            {
                TempData["safe"] = answer;
                TempData["starsToAvoid"] = starsToAvoid;
                TempData["csatStr"] = csatStr;
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}