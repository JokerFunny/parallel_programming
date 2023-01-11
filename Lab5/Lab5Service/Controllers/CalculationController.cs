using Microsoft.AspNetCore.Mvc;

namespace Lab5Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculationController : Controller
    {
        private readonly ILogger<CalculationController> _logger;

        public CalculationController(ILogger<CalculationController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public CalcResponse Calculate([FromBody] string data)
        {
            _logger.LogInformation($"Start processing '''{data}''' string");
            var stopwatch = new System.Diagnostics.Stopwatch();

            if (data == null)
            {
                return new CalcResponse();
            }

            stopwatch.Start();
            var chars = data.ToCharArray();

            double[] processing = chars.Select(el => 1.0 * el).ToArray();
            for (int i = 0; i < 333; i++)
            {
                var sum = 0;
                for (int j = 0; j < 1000000; j++)
                {
                    sum += j;
                }
                processing = processing.Select(el => el * Math.Sqrt(sum) / Math.Log10(sum) * Math.PI * Math.E * 1.5 * Math.ReciprocalSqrtEstimate(sum)).ToArray();

                //processing = processing
                //    .Select((c, index) => Math.Log(c) * Math.PI * Math.Sqrt((index + 100)) / Math.Cbrt(c * (index + 100)) * 100 / (163 / Math.Max(10, (index + 100)) / Math.Max(10, c)))
                //    .Select((c, index) => Math.BitDecrement(c) * Math.PI * Math.Tanh((index + 100)) / Math.Cbrt(c * (index + 100)) * 100 / (Math.Max(10, (index + 100)) / Math.Max(10, c)))
                //    .Select((c, index) => Math.Sin(c) * Math.E * Math.ReciprocalSqrtEstimate((index + 100)) / Math.Cbrt(c * (index + 100)) * 100 / (Math.Max(10, (index + 100)) / Math.Max(10, c)))
                //    .Select((c, index) => Math.SinCos(c).Sin * Math.Tau * Math.Acosh((index + 100)) / Math.Log10(c * (index + 100)) * 100 / (Math.Max(10, (index + 100)) / Math.Max(10, c)))
                //    .Select((c, index) => Math.Tan(c) * Math.E * Math.IEEERemainder((index + 100), c) / Math.Cbrt(c * (index + 100)) * 100 / (Math.Max(10, (index + 100)) / Math.Max(10, c)))
                //    .ToArray();
            }
            double res = 0;
            foreach (var el in processing)
            {
                res = el * 100;
            }
            
            stopwatch.Stop();
            _logger.LogInformation($"Done processing '''{data}''' string. Result {res}. Took {stopwatch.ElapsedMilliseconds}");
            //return (res, Thread.CurrentThread.ManagedThreadId);
            return new CalcResponse
            {
                Result = res,
                ElapsedMs = stopwatch.ElapsedMilliseconds,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };
        }

        public class CalcResponse
        {
            public double Result { get; set; }
            public int ThreadId { get; set; }
            public long ElapsedMs { get; set; }
        }
    }
}
