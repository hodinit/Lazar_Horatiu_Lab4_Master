using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lazar_Horatiu_Lab4_Master.Data;
using Lazar_Horatiu_Lab4_Master.Models;

namespace Lazar_Horatiu_Lab4_Master.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionApixController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PredictionApixController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllPredictions()
        {
            var predictions = _context.PredictionHistories.ToList();
            return Ok(predictions);
        }

        [HttpDelete]
        public IActionResult DeleteThisPrediction(int id = 6)
        {
            var prediction = _context.PredictionHistories.Find(id);
            if (prediction == null)
            {
                return NotFound();
            }
            _context.PredictionHistories.Remove(prediction);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
