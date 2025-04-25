using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueRunning.Models;

namespace QueueRunning.Controllers
{
    public class QueueController : Controller
    {
        private readonly AppDbContext _context;

        private readonly ILogger<QueueController> _logger;

        public QueueController(AppDbContext context, ILogger<QueueController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // IT 05-1
        public IActionResult Index()
        {
            return View("it051");
        }

        // IT 05-2
        public IActionResult PageDisplayQueue(string queue)
        {
            ViewBag.Queue = queue;
            return View("it052");
        }

        // IT 05-2
        public async Task<IActionResult> PageClearQueue(string queue)
        {
            queue = (queue == "00") ? queue : await GetLastQueue();
            ViewBag.Queue = queue;
            return View("it053");
        }

        public async Task<string> GetLastQueue()
        {
            try
            {
                var lastQueue = await _context.QueueNumbers
                    .OrderByDescending(q => q.Id)
                    .FirstOrDefaultAsync();
                return lastQueue?.CurrentQueue ?? "A0";
            }
            catch (Exception ex)
            {
                return "A0";
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetQueue()
        {
            try
            {
                // LOCK การทำงาน ป้องกันการกดพร้อมกัน
                var lastQueue = await GetLastQueue();

                string nextQueue = CalculateNextQueue(lastQueue);

                var newQueue = new QueueNumber
                {
                    CurrentQueue = nextQueue
                };

                _context.QueueNumbers.Add(newQueue);
                await _context.SaveChangesAsync();

                return RedirectToAction("PageDisplayQueue", new { queue = nextQueue });
            }
            catch (Exception ex)
            {
                return RedirectToAction("PageClearQueue");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearQueue()
        {
            _context.QueueNumbers.RemoveRange(_context.QueueNumbers);
            await _context.SaveChangesAsync();

            ViewBag.Queue = "00";
            return RedirectToAction("PageClearQueue", new { queue = ViewBag.Queue });
        }

        private string CalculateNextQueue(string current)
        {
            char letter = current[0];
            int number = int.Parse(current[1].ToString());

            if (number == 9)
            {
                letter = letter == 'Z' ? 'A' : (char)(letter + 1);
                number = 0;
            }
            else
            {
                number++;
            }
            return $"{letter}{number}";
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
