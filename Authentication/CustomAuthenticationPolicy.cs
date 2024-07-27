using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace JaydenAppUI.Authentication
{
    public class CustomAuthenticationPolicy
    {
        public class ApiAuthorizationRequirement : IAuthorizationRequirement
        {

            public ApiAuthorizationRequirement()
            {
            }
        }
        public class ApiAuthorizationHandler : AuthorizationHandler<ApiAuthorizationRequirement>
        {

            private readonly JsonWebTokenHandler _jwtHandler;

            private readonly ILocalStorageService _localStorage;

            public ApiAuthorizationHandler(ILocalStorageService localStorage, JsonWebTokenHandler jwtHandler)
            {
                _localStorage = localStorage;
                _jwtHandler = jwtHandler;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAuthorizationRequirement requirement)
            {

                string? tokenFromStorage = await _localStorage.GetItemAsStringAsync("token");

                if (string.IsNullOrEmpty(tokenFromStorage))
                {
                    context.Fail();
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
                        context.Fail();
                        return;
                    }

                    JsonWebToken token;
                    try
                    {
                        token = _jwtHandler.ReadJsonWebToken(formattedToken);
                    }
                    catch (Exception)
                    {
                        context.Fail();
                        return;
                    }

                    if (token.ValidTo > DateTime.UtcNow)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }

            }

        }
    }
}
