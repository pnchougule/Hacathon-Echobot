
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using SAPConnect.Models;
using Serilog;
using SAPConnect.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAntiforgery();

// Access configuration
var configuration = builder.Configuration;

// Register HttpClient
builder.Services.AddHttpClient("MyClient")
    .ConfigureHttpClient(client =>
    {
        client.Timeout = Timeout.InfiniteTimeSpan; // Set infinite timeout
    });

// Add services to the container 
builder.Services.AddEndpointsApiExplorer(); // Needed for minimal APIs
builder.Services.AddSwaggerGen();// Register the Swagger generator

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Add Serilog to the ASP.NET Core logging pipeline
builder.Host.UseSerilog();



var app = builder.Build();

// Use the CORS policy
app.UseCors("MyPolicy");

app.MapPost("/ask", async ([FromBody] QueryRequest queryRequest, IHttpClientFactory httpClientFactory) =>
{
    Log.Information("/ask endpoint called ");
    try
    {
        var client = httpClientFactory.CreateClient("MyClient");

        var sapEndPoint = configuration["Endpoint:sapEndPoint"];
        var request = new HttpRequestMessage(HttpMethod.Post, sapEndPoint);

        // Create the JSON content
        var jsonContent = new StringContent(
            $"{{ \"query\": \"{queryRequest.Query}\" }}",
            System.Text.Encoding.UTF8,
            "application/json"
        );

        request.Content = jsonContent;

        // Send the request
        var response = await client.SendAsync(request);

        // Ensure the response is successful
        response.EnsureSuccessStatusCode();

        // Read and return the response content
        var responseData = await response.Content.ReadAsStringAsync();

        string finalResult = String.Empty;
        // Parse the JSON string here using Newtonsoft.Json
        if (!string.IsNullOrEmpty(responseData))
        {
            JObject json = JObject.Parse(responseData);
            //Fetch 'type' from response
            string type = (string)json["response"]["type"];
            if (!string.IsNullOrEmpty(type))
            {
                if (type == "text")
                {
                    finalResult = (string)json["response"]["data"];
                }
                else if (type == "table")
                {
                    finalResult = json["response"]["CardData"].Root.ToString();
                }
            }
        }

        return Results.Ok(finalResult);
    }
    catch (Exception ex)
    {

        // Log the exception (you can add logging here)
        return Results.Problem("An unexpected error occurred." + ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }
});

// Optionally enable Swagger for API documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
