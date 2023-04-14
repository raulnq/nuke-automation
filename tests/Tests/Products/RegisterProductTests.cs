namespace Tests.Products;

public class RegisterProductTests : IAsyncLifetime
{
    private readonly AppDsl _appDsl;

    public RegisterProductTests()
    {
        _appDsl = new AppDsl();
    }

    public Task DisposeAsync()
    {
        return _appDsl.DisposeAsync().AsTask();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public Task register_should_be_ok()
    {
        return _appDsl.Product.Register();
    }

    [Fact]
    public Task register_should_throw_an_error_when_name_empty()
    {
        return _appDsl.Product.Register(command =>
        {
            command.Name = null;
        }, errorDetail: "ValidationErrorDetail", new Dictionary<string, string[]> { { "name", new string[] { "NotEmptyValidator" } } });
    }

    [Fact]
    public Task register_should_throw_an_error_when_name_too_long()
    {
        return _appDsl.Product.Register(command =>
        {
            command.Name = new string('*', 256); ;
        }, errorDetail: "ValidationErrorDetail", new Dictionary<string, string[]> { { "name", new string[] { "MaximumLengthValidator" } } });
    }
}