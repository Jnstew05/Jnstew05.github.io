using Blazored.LocalStorage;
using JaydenAppModels.Models;
using JaydenAppUI.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.SignalR.Client;

namespace JaydenAppUI.Services
{
    public class ChatHubService(NavigationManager navigationManager, ILocalStorageService localStorageService)
    {
        public NavigationManager NavigationManager = navigationManager;
        public ILocalStorageService LocalStorageService = localStorageService;

        public EventHandler<GlobalChatMessages>? ReceiveGlobalMessage;
        private HubConnection? _hubConnection;

        private async Task<string?> GetBearerToken()
        {
            return await localStorageService.GetItemAsStringAsync("token");
        }
        public async Task InitiateConnection()
        {
            //Fix
            var token = await GetBearerToken();
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{BaseUriAddress.BaseUrl}/ChatHub")
                .Build();
            
            _hubConnection.On<GlobalChatMessages>("ReceiveGlobalMessage", (chatLog) =>

            {
                ReceiveGlobalMessage!.Invoke(this, (chatLog));
            });

            await _hubConnection.StartAsync();
        }
        public async Task SendGlobalMessage(AuthenticatedUser user, string message)
        {
            await _hubConnection!.SendAsync("SendGlobalMessage", user, message);
        }

        public async Task TerminateConnection()
        {
            await _hubConnection!.StopAsync();
        }
        
    } 
}

