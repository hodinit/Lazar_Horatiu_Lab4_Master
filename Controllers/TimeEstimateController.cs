using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using static Lazar_Horatiu_Lab4_Master.TimeEstimate;

namespace Lazar_Horatiu_Lab4_Master.Controllers
{
    public class TimeEstimateController : Controller
    {
        [HttpGet("/Prediction/Time")]
        [HttpPost("/Prediction/Time")]
        public IActionResult Time(ModelInput input)
        {
            MLContext mlContext = new MLContext();
            ITransformer mlModelTime = mlContext.Model.Load("TimeEstimate.mlnet", out var modelInputSchemaTime);
            var predEngineTime = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModelTime);

            ModelOutput resultTime = predEngineTime.Predict(input);
            ViewBag.Time = resultTime.Score;

            return View("~/Views/Prediction/Time.cshtml", input);
        }
    }
}
