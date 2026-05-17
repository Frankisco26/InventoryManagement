using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using InventoryManagement.Data;

namespace InventoryManagement.Web.Services
{
    public class AuthService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;

        private string? _token;

        public AuthService(
            AuthenticationStateProvider authenticationStateProvider,
            UserManager<ApplicationUser> userManager,
            JwtService jwtService)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token))
                return _token;

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            var principal = authState.User;

            if (principal.Identity?.IsAuthenticated != true)
                return string.Empty;

            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
                return string.Empty;

            _token = await _jwtService.GenerateToken(user);

            return _token;
        }

        public Task LogoutAsync()
        {
            _token = null;
            return Task.CompletedTask;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
    }
}