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
using System.Net.Http;
using Newtonsoft.Json;

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

        private int NextRandomNumber(string lastInput, ref DateTime time, ref string history)
        {
            var nextInput = md5.ComputeHash(Encoding.UTF8.GetBytes(lastInput + time.Ticks.ToString()));
            var nextNumber = GetNumber(nextInput);
            while (drawedNumbers.Contains(nextNumber))
            {
                time.AddTicks(10);
                history += $"-{nextNumber}, ";
                nextInput = md5.ComputeHash(Encoding.UTF8.GetBytes(nextInput + time.Ticks.ToString()));
                nextNumber = GetNumber(nextInput);
            }

            history += $"+{nextNumber}, ";

            drawedNumbers.Add(nextNumber);
            return nextNumber;
        }

        public IActionResult Draw(int drawNumber)
        {
            LuckyCount = drawNumber;
            int[] luckyNumbers = new int[LuckyCount];
            string history = "";
            luckyNumbers[0] = seed.Next(N);
            DateTime now = DateTime.Now;
            for (int i = 0; i < LuckyCount; i++)
            {
                if (i == 0)
                {
                    var blockTimeStamp = GetBlockchainCreatedTimestamp(now).ToString();
                    history = $"(time={now.Ticks}, block_time={blockTimeStamp}) ";
                    luckyNumbers[0] = NextRandomNumber(blockTimeStamp, ref now, ref history);
                }
                else
                {
                    luckyNumbers[i] = NextRandomNumber(luckyNumbers[i - 1].ToString(), ref now, ref history);
                }
            }

            luckyNumbers = luckyNumbers.OrderBy(n => n).ToArray();
            histories.Add(history);
            return View("Index", new LuckyNumberModel()
            {
                LuckyNumbers = luckyNumbers,
                Histories = histories
            });
        }

        private long GetBlockchainCreatedTimestamp(DateTime now)
        {
            HttpClient client = new HttpClient();
            var requestUri = "http://13.77.152.248";
            var content = JsonConvert.SerializeObject(new BlockchainPayload()
            {
                timestamp = now.Ticks
            });
            var result = client.PostAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));
            var resultStr = result.Result.Content.ReadAsStringAsync().Result;
            var response = JsonConvert.DeserializeObject<BlockchainResponse>(resultStr);

            return response.block_timestamp;
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
