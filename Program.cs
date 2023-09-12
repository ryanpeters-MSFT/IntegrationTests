using System.Net.Http.Headers;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
        ["urls"] = "http://localhost:8222"
    })
    .Build();

var testServer = new TestServer(
        new WebHostBuilder()
            //.UseUrls("http://localhost:8211")
            // .UseKestrel(options =>
            // {
            //     options.ListenLocalhost(8123);W
            // })
            //.UseConfiguration(config)
            .ConfigureServices(services =>
            {
                services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.InputFormatters.Add(new TextRequestBodyFormatter());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            })
            .Configure(app =>
            {
                app.UseMvc();
            }));


var client = testServer.CreateClient();

var serialized = JsonConvert.SerializeObject(new
{
    name = "ryan"
});

var content = new StringContent(serialized);

//var getResponse = await client.GetAsync("api/manifest");

content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

var response = await client.PostAsync("api/manifest", content);

Console.WriteLine(response.StatusCode);
Console.WriteLine(client.BaseAddress);