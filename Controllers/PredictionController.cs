using Lazar_Horatiu_Lab4_Master.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using System.Threading.Tasks;
using static Lazar_Horatiu_Lab4_Master.PricePredictionModel;
using Lazar_Horatiu_Lab4_Master.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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
        public async Task<IActionResult> History(
        string? paymentType,
        float? minPrice,
        float? maxPrice,
        DateTime? dataInitiala,
        DateTime? dataFinala,
        string? sortOrder)
        {
            var query = _context.PredictionHistories.AsQueryable();
            if (!string.IsNullOrEmpty(paymentType))
            {
                query = query.Where(p => p.PaymentType == paymentType);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.PredictedPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.PredictedPrice <= maxPrice.Value);
            }
            if (dataInitiala.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= dataInitiala.Value);
            }
            if (dataFinala.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= dataFinala.Value);
            }

            query = sortOrder switch
            {
                "price_asc" => query.OrderBy(p => p.PredictedPrice),
                "price_desc" => query.OrderByDescending(p => p.PredictedPrice),
                "date_asc" => query.OrderBy(p => p.CreatedAt),
                "date_desc" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.PredictedPrice) //sortare default
            };

            ViewBag.CurrentPaymentType = paymentType;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentDataInitiala = dataInitiala?.ToString("yyyy-MM-dd");
            ViewBag.CurrentDataFinala = dataFinala?.ToString("yyyy-MM-dd");
            ViewBag.CurrentSortOrder = sortOrder;
            var result = await query.ToListAsync();
            return View(result);
        }
    }
}
