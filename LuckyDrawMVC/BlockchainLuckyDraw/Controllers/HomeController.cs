using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlockchainLuckyDraw.Models;

namespace BlockchainLuckyDraw.Controllers
{
    public class HomeController : Controller
    {
        private int N = 300;

        private static HashSet<int> drawedNumbers = new HashSet<int>();

        private static List<string> histories = new List<string>();

        private Random seed = new Random();

        private int LuckyCount = 10;

        public IActionResult Index()
        {
            return View();
        }

        private int NextRandomNumber(ref string history)
        {
            var newNumber = seed.Next(N);
            while (drawedNumbers.Contains(newNumber))
            {
                history += $"-{newNumber}, ";
                newNumber = seed.Next(N);
            }

            history += $"+{newNumber}, ";

            drawedNumbers.Add(newNumber);
            return newNumber;
        }

        public IActionResult Draw(int drawNumber)
        {
            LuckyCount = drawNumber;
            int[] luckyNumbers = new int[LuckyCount];
            string history = "";
            for (int i = 0; i < LuckyCount; i++)
            {
                luckyNumbers[i] = NextRandomNumber(ref history);
            }

            luckyNumbers = luckyNumbers.OrderBy(n => n).ToArray();
            histories.Add(history);
            return View("Index", new LuckyNumberModel()
            {
                LuckyNumbers = luckyNumbers,
                Histories = histories
            });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
