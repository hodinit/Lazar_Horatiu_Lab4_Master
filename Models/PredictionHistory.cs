namespace Lazar_Horatiu_Lab4_Master.Models
{
    public class PredictionHistory
    {
        public int Id { get; set; }
        public float PassengerCount { get; set; }
        public float TripTimeInSecs { get; set; }
        public float TripDistance { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public float PredictedPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
