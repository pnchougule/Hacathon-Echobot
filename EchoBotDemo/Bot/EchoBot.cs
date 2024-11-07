using EchoBotDemo.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text.Json.Nodes;

namespace EchoBotDemo.Bot;

public class EchoBot : TeamsActivityHandler
{
    private readonly HttpClient _httpClient;
    private const string LoadingMessage = "Loading...";
    public EchoBot(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MyClient");
    }



    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        // Show typing indicator
        await turnContext.SendActivityAsync(new Activity(type: ActivityTypes.Typing), cancellationToken);

        // Start showing the loader
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken1 = cancellationTokenSource.Token;

        // Start a task that can be canceled
       await  LongRunningOperationAsync(cancellationToken1);

        try
        {

            var handler = new HttpClientHandler();
            using (HttpClient _httpClient  = new HttpClient(handler) {Timeout =Timeout.InfiniteTimeSpan })
            {
                await Task.Delay(2000);
                QueryRequest queryRequest = new QueryRequest();
                queryRequest.Query = turnContext.Activity.Text;
                queryRequest.BotId = turnContext.Activity.Id;
                var request = new HttpRequestMessage(HttpMethod.Post, "https://todoapi20241017212742.azurewebsites.net/ask");
                // Serialize to JSON
                string queryRequestString = System.Text.Json.JsonSerializer.Serialize(queryRequest);

                var content = new StringContent(queryRequestString, null, "application/json");

                request.Content = content;
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response into a QueryResponse object
                    var queryResponse = JsonConvert.DeserializeObject<QueryResponse>(jsonResponse);
                    if(queryResponse != null)
                    {
                        if(queryResponse.ResponseType!="table")
                        {
                            // Send the response
                            await turnContext.SendActivityAsync($" {queryResponse.Response}", cancellationToken: cancellationToken);

                        }
                        else
                        {
                            var dynamicList = new List<dynamic>();
                            JsonReader reader1 = new JsonTextReader(new StringReader(queryResponse.Response));
                            using (JsonReader reader = new JsonTextReader(new StringReader(queryResponse.Response)))
                            {
                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonToken.StartObject)
                                    {
                                        var item = new ExpandoObject() as IDictionary<string, Object>;

                                        while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                                        {
                                            string propertyName = reader.Value.ToString();
                                            reader.Read(); // Move to the value
                                            var propertyValue = reader.Value;

                                            item[propertyName] = propertyValue; // Add to the ExpandoObject
                                        }

                                        dynamicList.Add(item); // Add the dynamic object to the list
                                    }
                                }
                            }
                            AdaptiveCardGenerator adaptiveCardGenerator = new AdaptiveCardGenerator();
                            var obj = await adaptiveCardGenerator.GenerateHtmlTable<dynamic>(dynamicList);
                            var message = MessageFactory.Text(obj);
                            message.TextFormat = TextFormatTypes.Xml;
                            await turnContext.SendActivityAsync(message,cancellationToken);
                        }
                    }
                    
                   
                }

            }

        }
        catch (HttpRequestException ex)
        {
            // Log the exception message
            Console.WriteLine($"Request error: {ex.Message}");
        }
        catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Request timed out.");
        }
        catch (Exception ex)
        {
            // Catch other exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            cancellationTokenSource.Cancel();
        }




    }


    static async Task LongRunningOperationAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Simulate work
            Console.WriteLine("Working...");
            await Task.Delay(1000); // Delay for 1 second
        }
    }

    
    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        var welcomeText = "Hi there! I'm a Teams bot that will echo what you said to me.";
        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText), cancellationToken);
            }
        }
    }
}

