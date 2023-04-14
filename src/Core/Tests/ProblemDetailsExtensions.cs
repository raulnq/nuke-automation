using Shouldly;
using System.Text.Json;

namespace Core.Tests;

public static class ProblemDetailsExtensions
{
    public static void Check(this Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails, string errorDetail)
    {
        if (errorDetail == null)
        {
            problemDetails.ShouldBeSuccessful();
        }
        else
        {
            problemDetails.ShouldThrowError(errorDetail);
        }
    }

    public static void ShouldBeSuccessful(this Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails)
    {
        problemDetails.ShouldBeNull();
    }

    public static IDictionary<string, string[]> GetValidationErrors(this Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails)
    {
        var errors = problemDetails.Extensions["errors"];

        if (errors == null || string.IsNullOrEmpty(errors.ToString()))
        {
            return new Dictionary<string, string[]>();
        }

        return JsonSerializer.Deserialize<IDictionary<string, string[]>>(errors.ToString()!)!;
    }

    public static void ShouldThrowError(this Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails, string errorDetail, IDictionary<string, string[]>? errors = null)
    {
        problemDetails.ShouldNotBeNull();

        problemDetails.Detail.ShouldBe(errorDetail);

        if (errors != null)
        {
            var validationErrors = problemDetails.GetValidationErrors();

            foreach (var error in errors)
            {
                validationErrors.ShouldContainKey(error.Key);

                error.Value.SequenceEqual(validationErrors[error.Key]).ShouldBeTrue("Error messages are not equal");
            }
        }
    }
}
