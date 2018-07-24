using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlockchainLuckyDraw.Models;
using System.Security.Cryptography;
using System.Numerics;
using System.Text;

namespace BlockchainLuckyDraw.Controllers
{
    public class HomeController : Controller
    {
        private int N = 300;

        private static HashSet<int> drawedNumbers = new HashSet<int>();

        private static List<string> histories = new List<string>();

        private static MD5 md5 = MD5.Create();

        private Random seed = new Random();

        private int LuckyCount = 10;

        public IActionResult Index()
        {
            return View();
        }

        private int GetNumber(byte[] data)
        {
            return (int)(BitConverter.ToUInt32(data) % N);
        }

        private int NextRandomNumber(string lastInput, long time, ref string history)
        {
            var nextInput = md5.ComputeHash(Encoding.UTF8.GetBytes(lastInput + time.ToString()));
            var nextNumber = GetNumber(nextInput);
            while (drawedNumbers.Contains(nextNumber))
            {
                history += $"-{nextNumber}, ";
                nextInput = md5.ComputeHash(Encoding.UTF8.GetBytes(nextInput + time.ToString()));
                nextNumber = GetNumber(nextInput);
            }

            history += $"+{nextNumber}, ";

            drawedNumbers.Add(nextNumber);
            return nextNumber;
        }

        public IActionResult Draw(int drawNumber)
        {
            LuckyCount = drawNumber;
            int[] luckyNumbers = new int[LuckyCount + 1];
            string history = "";
            luckyNumbers[0] = seed.Next(N);
            for (int i = 1; i <= LuckyCount; i++)
            {
                luckyNumbers[i] = NextRandomNumber(luckyNumbers[i-1].ToString(), DateTime.UtcNow.Millisecond, ref history);
            }

            luckyNumbers = luckyNumbers.Skip(1).OrderBy(n => n).ToArray();
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
