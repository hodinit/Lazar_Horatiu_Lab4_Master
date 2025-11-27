using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using static Lazar_Horatiu_Lab4_Master.PricePredictionModel;

namespace Lazar_Horatiu_Lab4_Master.Controllers
{
    public class PredictionController : Controller
    {
        public IActionResult Price(ModelInput input)
        {
            // Load the ML model
            MLContext mlContext = new MLContext();
            // create prediction engine
            //ITransformer mlModel = mlContext.Model.Load(@"..\PricePredictionModel.mlnet", out var modelInputSchema);
            ITransformer mlModel = mlContext.Model.Load("PricePredictionModel.mlnet", out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            //try model on sample data
            ModelOutput result = predEngine.Predict(input);
            ViewBag.Price = result.Score;
            return View(input);
        }
    }
}
