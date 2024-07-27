using BlazorBootstrap;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;

namespace JaydenAppUI.Services
{
    public class UserValidationService
    {
        AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        ILocalStorageService LocalStorage { get; set; } = default!;
        HttpClient HttpClient { get; set; }
        NavigationManager NavigationManager { get; set; }

        public UserValidationService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            HttpClient = httpClient;
            AuthStateProvider = authStateProvider;
            LocalStorage = localStorage;
        }
        public async Task<(bool, HttpStatusCode)> AuthenticateUser(string Username, string Password)
        {
            var content = new { Username, Password };

            try
            {
                var Response = await HttpClient.PostAsJsonAsync($"Authenticate", content);

                if (Response.IsSuccessStatusCode)
                {
                    string Token = await Response.Content.ReadAsStringAsync();

                    await LocalStorage.SetItemAsync("token", Token);
                    await AuthStateProvider.GetAuthenticationStateAsync();

                    return (true, HttpStatusCode.OK);

                }
                if (Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return(false, HttpStatusCode.NotFound);
                }

                if (Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return (false, HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "TypeError: Failed to fetch")
                {
                    return (false, HttpStatusCode.BadGateway);
                }
                else
                {
                    return (false, HttpStatusCode.InternalServerError);
                }

            }

            return (false, HttpStatusCode.BadRequest);
        }
        public async Task<bool> VerifyUserSessionIsValid()
        {
            try
            {
                var content = new { Authentication = LocalStorage.GetItemAsStringAsync("token") };

               var verificationStatus = await HttpClient.PostAsJsonAsync($"Authenticate", content);

                if (verificationStatus.IsSuccessStatusCode) return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
