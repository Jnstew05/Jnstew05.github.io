using BlazorBootstrap;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net.Http;
using JaydenAppUI.Authentication;
using System.Net;

namespace JaydenAppUI.Pages
{

    public partial class Login
    {
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] ILocalStorageService LocalStorage { get; set; } = default!;
        [Inject] JsonWebTokenHandler _tokenHandler { get; set; } = default!;
        [Inject] Services.UserValidationService UserValidationService { get; set; } = default!;
        public ToastMessage ToastMessage { get; set; } = default!;
        public string? Username { get; set; }
        public string? Password { get; set; }
        List<ToastMessage> messages = new List<ToastMessage>();
        public string Message = "User not Authorised";

        protected async override Task OnInitializedAsync()
        {
            await CheckIfAlreadyVerified();
        }

        private void ShowMessage(ToastType toastType, string message) => messages.Add(CreateToastMessage(toastType, message));

        private ToastMessage CreateToastMessage(ToastType toastType, string message)
        => new ToastMessage
        {
            Type = toastType,
          Message = message,
        };

        public async Task CheckIfAlreadyVerified()
        {
            string? tokenFromStorage = await LocalStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrEmpty(tokenFromStorage))
            {
                return;
            }
            else
            {
                string formattedToken;
                try
                {
                    formattedToken = tokenFromStorage.Replace("\"", string.Empty);

                }
                catch (Exception)
                {
                    return;
                }

                JsonWebToken token;
                try
                {
                    token = _tokenHandler.ReadJsonWebToken(formattedToken);
                }
                catch (Exception)
                {
                    return;
                }

                if (token.ValidTo > DateTime.UtcNow)
                {
                    NavigateToMain();
                }
                else
                {
                    return;
                }
            }
        }

        public async Task ValidateUser()
        {
            if (Username == null || Password == null) { return; }


            var result = await UserValidationService.AuthenticateUser(Username, Password);

            if (result.Item1)
            {
                NavigateToMain();
                return;
            }

            switch (result.Item2)
            {
                case HttpStatusCode.NotFound:
                    ShowMessage(ToastType.Warning, "User not found");
              break;
                case HttpStatusCode.BadRequest:
                    ShowMessage(ToastType.Warning, "An unknown error has occured");
                    break;
                case HttpStatusCode.BadGateway:
                    ShowMessage(ToastType.Warning, "An error has occured between the server and the client.");
                    break;
                default:
                    ShowMessage(ToastType.Warning, "An REALLY unknown error has occured ����‍♂️");
                    break;

            }

/*            ToastMessages.Clear();
*/        }
        public void NavigateToMain()
        {
            NavigationManager.NavigateTo("/home");
        }

    }

}
