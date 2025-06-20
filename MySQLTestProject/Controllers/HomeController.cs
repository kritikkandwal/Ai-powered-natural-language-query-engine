using Microsoft.AspNetCore.Mvc;
using MySQLTestProject.Data;
using System.Linq;

namespace MySQLTestProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateQuery(string userInput)
        {
            string sqlQuery = "";
            List<Employee> result = new List<Employee>();

            // Very basic rule-based parsing
            if (userInput.ToLower().Contains("between"))
            {
                var words = userInput.Split(' ');
                if (int.TryParse(words[words.Length - 3], out int start) && int.TryParse(words[words.Length - 1], out int end))
                {
                    sqlQuery = $"SELECT * FROM Employee WHERE EmpID BETWEEN {start} AND {end}";
                    result = _context.Employees
                        .Where(e => e.EmpID >= start && e.EmpID <= end)
                        .ToList();
                }
                else
                {
                    sqlQuery = "Could not parse input properly.";
                }
            }
            else
            {
                sqlQuery = "Sorry, I could not understand your query.";
            }

            ViewBag.GeneratedQuery = sqlQuery;
            ViewBag.Result = result;

            return View("Index");
        }
    }
}
