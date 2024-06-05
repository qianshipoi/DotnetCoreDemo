using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace LoggingGlobal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> PostFromData([FromForm] FormData data)
        {
            //using var stream = data.File.OpenReadStream();
            //var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", data.File.FileName);
            //Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            //using var fs = new FileStream(path, FileMode.OpenOrCreate);
            //stream.CopyTo(fs);
            //await fs.FlushAsync();

            var fs = new FileStream("D:\\Desktop\\gps\\gps.csv", FileMode.OpenOrCreate);

            return File(fs, "application/octet-stream");
        }

        [HttpPut]
        public IActionResult PutJsonData([FromBody]FormData data)
        {
            return Ok(data);
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get(int key = 10)
        {
            if(key < 5)
            {
                throw new Exception("Random exception");
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
    public class FormData
    {
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public IFormFile File { get; set; }
    }
}
