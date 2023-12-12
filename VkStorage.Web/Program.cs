var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var exceptionMiddleware = new CustomExceptionHandler();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
//builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddApiVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception e)
    {
        await exceptionMiddleware.TryHandleAsync(context, e, new CancellationToken());
    }
});

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FilesController}/{action=Index}");
app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());
//app.MapEndpoints();

app.Run();
