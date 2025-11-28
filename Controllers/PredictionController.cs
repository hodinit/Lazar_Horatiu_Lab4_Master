using Lazar_Horatiu_Lab4_Master.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using System.Threading.Tasks;
using static Lazar_Horatiu_Lab4_Master.PricePredictionModel;
using Lazar_Horatiu_Lab4_Master.Data;
using Microsoft.EntityFrameworkCore;

namespace Lazar_Horatiu_Lab4_Master.Controllers
{
    public class PredictionController : Controller
    {
        private readonly AppDbContext _context;

        public PredictionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Price()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Price(ModelInput input)
        {
            // Load the ML model
            MLContext mlContext = new MLContext();
            // create prediction engine
            ITransformer mlModel = mlContext.Model.Load("PricePredictionModel.mlnet", out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            // try model on submitted data
            ModelOutput result = predEngine.Predict(input);
            ViewBag.Price = result.Score;

            var history = new PredictionHistory
            {
                PassengerCount = input.Passenger_count,
                TripTimeInSecs = input.Trip_time_in_secs,
                TripDistance = input.Trip_distance,
                PaymentType = input.Payment_type,
                PredictedPrice = result.Score,
                CreatedAt = DateTime.Now
            };

            _context.PredictionHistories.Add(history);
            await _context.SaveChangesAsync();

            return View(input);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var history = await _context.PredictionHistories.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(history);
        }
    }
}
