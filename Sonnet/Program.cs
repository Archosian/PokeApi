using System.Net.Mime;
using System.Text.Json.Serialization;
using Sonnet.Clients.FunTranslation;
using Sonnet.Clients.PokeApi;
using Sonnet.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IPokeApiClient, PokeApiClient>();
builder.Services.AddSingleton<IFunTranslationClient, FunTranslationClient>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Uses a Memory Cache for the IDistributedCache. Swap out here when ready for the real deal!
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        //TODO lovely place for some proper exception handling and error propagation/useful error messages
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        var errorResponse = ApiResponse<string>.ErrorResponse("An error occurred.");
        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});

//TODO metrics and timing middleware

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
