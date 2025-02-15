using OrderManager.API.Database;
using OrderManager.API.Dispatchers;
using OrderManager.API.Repositories;
using OrderManager.API.Services;

const string CORS_POLICY = "OrderManagerApiPolicy";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDatabase();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddHandlers();
builder.Services.AddProblemDetails();
builder.Services.AddCors(c =>
    c.AddPolicy(CORS_POLICY, policy =>
        policy.AllowAnyHeader()
              .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
              .WithOrigins(builder.Configuration.GetValue<string>("Frontend") ?? throw new InvalidOperationException("Frontend url was not provided"))
));

var app = builder.Build();
app.UseCors(CORS_POLICY);
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.Run();
