using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Lab5.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        //private readonly string _processingPath;

        public DataController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory, IConfiguration conf)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(conf.GetValue<string>("ProcessingPath"));
            _httpClient.Timeout = TimeSpan.FromSeconds(5000);
        }

        [HttpPost]
        public async Task<FinalResult> ProceedWithCalculation([FromBody] string data)
        {
            var splitted = data.Split(new char[] { ' ', '\n', '\t' });

            var tasks = new List<Task<CalcResponse>>();

            var stopwatch = Stopwatch.StartNew();
            foreach (var word in splitted)
            {
                tasks.Add(ProceedWithWord(word));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            var responses = new List<CalcResponse>();
            foreach (var finished in tasks)
            {
                responses.Add(finished.Result);
            }

            return new FinalResult
            {
                ActiveThreadAmount = responses.Select(el => el.ThreadId).Distinct().Count(),
                AverageElapsed = responses.Select(el => el.ElapsedMs).Average(),
                SumResult = responses.Select(el => el.Result).Sum(),
                TasksCount = splitted.Length,
                TotalElapsed = stopwatch.ElapsedMilliseconds
            };
        }

        private async Task<CalcResponse> ProceedWithWord(string word)
        {
            
            var jsonString = JsonConvert.SerializeObject(word);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"Calculation", content);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CalcResponse>(result);
        }

        public class CalcResponse
        {
            public double Result { get; set; }
            public int ThreadId { get; set; }
            public long ElapsedMs { get; set; }
        }

        public class FinalResult
        {
            public int ActiveThreadAmount { get; set; }
            public long TotalElapsed { get; set; }
            public int TasksCount { get; set; }
            public double AverageElapsed { get; set; }
            public double SumResult { get; set; }
        }
    }
}
