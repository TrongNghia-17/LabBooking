var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var isDevelopment = builder.Environment.IsDevelopment();

// Add services to the container.
builder.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration, isDevelopment);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeLoggingMiddleware>();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapGroup("api/identity")
   .WithTags("Identity")
   .MapIdentityApi<User>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
