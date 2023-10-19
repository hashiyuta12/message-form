using Microsoft.AspNetCore.Server.Kestrel.Core;

#region Member
IConfigurationRoot configs = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(path: "appsettings.json")
            .Build();
var builder = WebApplication.CreateBuilder(args);
string url = configs["Server:Host"];
#endregion

#region Method
// Host Configs
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(endpointOptions =>
    {
        endpointOptions.Protocols = HttpProtocols.Http2;
    });
});
// Choose the URL
if (url != "") builder.WebHost.UseUrls(url);

// Choose Host Services
builder.Services.AddGrpc();
builder.Services.AddMagicOnion();

// Host Run
var app = builder.Build();
app.MapMagicOnionService();
app.Run();
#endregion