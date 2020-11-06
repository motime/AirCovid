namespace AirCovid.Domain
{
    public class Policy
    {
        public double AllowedTotalWeight { get; set; } = double.MaxValue;

        public int AllowedBagsPerPassenger { get; set; } = int.MaxValue;

        public double AllowedWeightPerPassenger { get; set; } = double.MaxValue;

        public int AllowedSeats { get; set; } = int.MaxValue;
        
    }
}