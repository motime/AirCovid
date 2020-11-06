namespace AirCovid.Api.Services.Commands.Validation
{
    public class ValidationResult
    {
        public static ValidationResult NoProblem = new ValidationResult(true);

        public ValidationResult(bool passed, Problem problem, string message)
        {
            Passed = passed;
            Problem = problem;
            Message = message;
        }

        public ValidationResult(bool passed)
        {
            Passed = passed;
        }

        public bool Passed { get; }

        public Problem Problem { get; }

        public string Message { get; }
    }
}