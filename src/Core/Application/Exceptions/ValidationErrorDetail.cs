namespace Core.Application;

public class ValidationErrorDetail
{
    public string Code { get; set; }

    public object[]? Parameters { get; set; }

    public ValidationErrorDetail(string code, object[]? parameters)
    {
        Code = code;

        Parameters = parameters;
    }

}
