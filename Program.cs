using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Monydragons_Portfolio;
using Monydragons_Portfolio.Services;
using Monydragons_Portfolio.Services.Content.Interface;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IContentService, BlogContentService>();
builder.Services.AddScoped<IFilteringSortingService, FilteringSortingService>();
builder.Services.AddScoped<IPaginationService, PaginationService>();

await builder.Build().RunAsync();