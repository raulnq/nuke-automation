using Shouldly;
using System.Net;

namespace Core.Tests;

public static class ObjectExtensions
{
    public static void Check<TResponse>(this (HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result, string? errorDetail = null, IDictionary<string, string[]>? errors = null, Action<TResponse>? successAssert = null, Action<Microsoft.AspNetCore.Mvc.ProblemDetails>? errorAssert = null) where TResponse : class
    {
        if (errorDetail == null)
        {
            result.ShouldBeSuccessful();
            successAssert?.Invoke(result.Response!);
        }
        else
        {
            result.ShouldThrowError(errorDetail, errors);
            errorAssert?.Invoke(result.Error!);

        }
    }

    public static void Check(this (HttpStatusCode StatusCode, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result, string? errorDetail = null, IDictionary<string, string[]>? errors = null, Action<Microsoft.AspNetCore.Mvc.ProblemDetails>? errorAssert = null)
    {
        if (errorDetail == null)
        {
            result.ShouldBeSuccessful();
        }
        else
        {
            result.ShouldThrowError(errorDetail, errors);
            errorAssert?.Invoke(result.Error!);

        }
    }

    public static void ShouldBeSuccessful<TResponse>(this (HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result) where TResponse : class
    {
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();
        result.Response.ShouldNotBeNull();
    }

    public static void ShouldBeSuccessful(this (HttpStatusCode StatusCode, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result)
    {
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();
    }

    public static void ShouldThrowError<TResponse>(this (HttpStatusCode StatusCode, TResponse? Response, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result, string errorDetail, IDictionary<string, string[]>? errors = null) where TResponse : class
    {
        result.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        result.Error!.ShouldThrowError(errorDetail, errors);
        result.Response.ShouldBeNull();
    }

    public static void ShouldThrowError(this (HttpStatusCode StatusCode, Microsoft.AspNetCore.Mvc.ProblemDetails? Error) result, string errorDetail, IDictionary<string, string[]>? errors = null)
    {
        result.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        result.Error!.ShouldThrowError(errorDetail, errors);
    }
}
