using FluentValidation.Results;

namespace BuildingBlocks.Common.Exceptions;
public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
        ErrorMessages = Array.Empty<string>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        ErrorMessages = failures.Select(e => e.ErrorMessage).ToArray();
    }

    public IDictionary<string, string[]> Errors { get; }
    public string[] ErrorMessages { get; }
}

