using ContainerAppsAuth.Auth.ContainerApp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ContainerAppsAuth.Auth.Localhost
{

    public class LocalHostAuthenticationHandler : AuthenticationHandler<LocalhostAuthenticationOptions>
    {

        private bool _authenticateRequests;

        public LocalHostAuthenticationHandler(IOptionsMonitor<LocalhostAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IWebHostEnvironment hostEnvironment)
            : base(options, logger, encoder, clock)
        {

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            _authenticateRequests = config.GetValue<bool>("AuthenticateRequests");

        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!_authenticateRequests)
                return AuthenticateResult.NoResult();

            try
            {

                string wwwRootPath = Path.GetFullPath("wwwroot");
                string jsonFile = Path.Combine(wwwRootPath, "EasyAuthHeaders.json");
                string json = System.IO.File.ReadAllText(jsonFile);
                var headers = JsonConvert.DeserializeObject<List<Models.Header>>(json);
                var easyAuthProvider = headers.Where(x => x.Key == "X-MS-CLIENT-PRINCIPAL-IDP").FirstOrDefault();
                var msClientPrincipalEncoded = headers.Where(x => x.Key == "X-MS-CLIENT-PRINCIPAL").FirstOrDefault();

                var decodedBytes = Convert.FromBase64String(msClientPrincipalEncoded.Value.First());
                using var memoryStream = new MemoryStream(decodedBytes);
                var clientPrincipal = await System.Text.Json.JsonSerializer.DeserializeAsync<MsClientPrincipal>(memoryStream);

                if (clientPrincipal == null || !clientPrincipal.Claims.Any())
                    return AuthenticateResult.NoResult();

                var claims = clientPrincipal.Claims.Select(claim => new Claim(claim.Type, claim.Value));

                // remap "roles" claims from easy auth to the more standard ClaimTypes.Role / "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                var easyAuthRoleClaims = claims.Where(claim => claim.Type == "roles");
                var claimsAndRoles = claims.Concat(easyAuthRoleClaims.Select(role => new Claim(ClaimTypes.Role, role.Value)));

                var principal = new ClaimsPrincipal();
                principal.AddIdentity(new ClaimsIdentity(claimsAndRoles, clientPrincipal.AuthenticationType, clientPrincipal.NameType, ClaimTypes.Role));

                var ticket = new AuthenticationTicket(principal, easyAuthProvider.Value.First());
                var success = AuthenticateResult.Success(ticket);
                Context.User = principal;

                return success;
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }

        }

    }

}
