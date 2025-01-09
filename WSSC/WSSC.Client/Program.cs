using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using WSSC.Client.Model;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton<UserData>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
var app = builder.Build();
var userData = app.Services.GetService<UserData>();
if (userData!=null)
{
    userData.Name = "user1";
}
await app.RunAsync();
