var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.MapGroup("api/identity")
//   .WithTags("Identity")
//   .MapIdentityApi<User>();

app.UseAuthentication();
app.UseAuthorization();

//app.MapGroup("api/identity")
//   .WithTags("Identity")
//   .MapIdentityApi<User>();

app.MapControllers();

app.Run();
