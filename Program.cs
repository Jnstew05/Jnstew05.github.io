using Blazored.LocalStorage;
using JaydenAppUI;
using JaydenAppUI.Authentication;
using JaydenAppUI.Configuration;
using JaydenAppUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var test = Environment.GetEnvironmentVariables();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(BaseUriAddress.BaseUrl) });
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddSingleton<JsonWebTokenHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationHandler, CustomAuthenticationPolicy.ApiAuthorizationHandler>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<JaydenAppUI.Services.UserValidationService>();
builder.Services.AddScoped<ChatHubService>();


builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("MustBeAuthenticated", policy =>

    {
        policy.AddRequirements(new CustomAuthenticationPolicy.ApiAuthorizationRequirement());
        policy.RequireRole("User", "Admin");
        policy.RequireAuthenticatedUser();
    });

    options.AddPolicy("MustBeAnAdmin", policy => policy.RequireRole("Admin"));
});

await builder.Build().RunAsync();
