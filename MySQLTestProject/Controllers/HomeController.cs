using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySQLTestProject.Data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace MySQLTestProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public HomeController(
            AppDbContext context,
            IHttpClientFactory clientFactory,
            IConfiguration config)
        {   
            _context = context;
            _clientFactory = clientFactory;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateQuery(string userInput)
        {
            string sqlQuery = "Error: Could not generate SQL";
            List<Employee> result = new List<Employee>();
            string modelError = null;
            string executionError = null;

            try
            {
                // Call NLP API
                var apiUrl = _config["NlpApi:BaseUrl"] + "/generate-sql";
                var client = _clientFactory.CreateClient();
                var requestData = new { query = userInput };
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(requestData),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(apiUrl, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic apiData = JsonConvert.DeserializeObject(jsonResponse);
                    sqlQuery = apiData.sql?.ToString() ?? sqlQuery;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    modelError = $"API Error ({response.StatusCode}): {errorContent}";
                }

                // Execute SQL if valid
                if (string.IsNullOrEmpty(modelError) &&
                    sqlQuery.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        result = await _context.Employees
                            .FromSqlRaw(sqlQuery)
                            .AsNoTracking()
                            .ToListAsync();
                    }
                    catch (Exception ex)
                    {
                        executionError = $"SQL Execution failed: {ex.Message}";
                    }
                }
            }
            catch (Exception ex)
            {
                modelError = $"Unhandled Exception: {ex.Message}";
            }

            // Pass everything to frontend
            ViewBag.GeneratedQuery = sqlQuery;
            ViewBag.Result = result;
            ViewBag.ModelError = modelError;
            ViewBag.ExecutionError = executionError;

            return View("Index");
        }
    }
}
