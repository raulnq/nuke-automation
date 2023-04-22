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
        return _appDsl.Product.Register(command =>
        {
            command.Name = new string('*', 500);
            command.Description = new string('*', 3500);
        });
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
    public Task register_should_throw_an_error_when_nae_too_long()
    {
        return _appDsl.Product.Register(command =>
        {
            command.Name = new string('*', 501);
            command.Description = new string('*', 3501);
        }, errorDetail: "ValidationErrorDetail", new Dictionary<string, string[]> { 
            { "name", new string[] { "MaximumLengthValidator" } },
            { "description", new string[] { "MaximumLengthValidator" } }
        });
    }
}