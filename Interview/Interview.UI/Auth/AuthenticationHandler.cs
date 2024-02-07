using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Interview.UI.Auth
{

    public class AuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
    {

        // https://johnnyreilly.com/azure-container-apps-easy-auth-and-dotnet-authentication

        #region Declarations

        private bool _isDevelopment;

        #endregion

        #region Constructors

        public AuthenticationHandler(IOptionsMonitor<AuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IWebHostEnvironment hostEnvironment)
            : base(options, logger, encoder, clock)
        {

            _isDevelopment = hostEnvironment.EnvironmentName.ToLower() == "development";

        }

        #endregion

        #region Properties

        private bool AuthenticateDevelopmentRequests
        {
            get
            {
                bool result = false;
                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.Development.json")
                    .Build();

                result = config.GetValue<bool>("AuthenticateRequests");

                return result;
            }
        }

        #endregion

        #region Protected Methods

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            try
            {

                AuthenticationTicket ticket = null;

                if (_isDevelopment)
                {
                    if (AuthenticateDevelopmentRequests)
                    {
                        ticket = await AddClaimsPrincipalFromJson();
                        if (ticket == null)
                            return AuthenticateResult.NoResult();
                        await AddClaimsPrincipalWithRolesFromInterviewDb();
                    }
                    else
                        return AuthenticateResult.NoResult();
                }
                else
                {
                    ticket = await AddClaimsPrincipalFromEasyAuthHeaders();
                    if (ticket == null)
                        return AuthenticateResult.NoResult();
                    await AddClaimsPrincipalWithRolesFromInterviewDb();
                }

                var success = AuthenticateResult.Success(ticket);

                return success;

            }
            catch (Exception ex)
            {
                var msgObj = new { message = ex.Message, stacktrace = ex.StackTrace };
                var msg = JsonSerializer.Serialize(msgObj);
                Logger.LogError(ex.Message, ex);

                return AuthenticateResult.Fail(ex);
            }

        }

        #endregion

        #region Private Methods

        private async Task<AuthenticationTicket> AddClaimsPrincipalFromJson()
        {

            AuthenticationTicket result = null;
            string wwwRootPath = Path.GetFullPath("wwwroot");
            string jsonFile = Path.Combine(wwwRootPath, "EasyAuthHeaders.json");
            string json = System.IO.File.ReadAllText(jsonFile);
            var headers = JsonSerializer.Deserialize<List<Models.Auth.Header>>(json);
            var easyAuthProvider = headers.Where(x => x.Key == "X-MS-CLIENT-PRINCIPAL-IDP").FirstOrDefault();
            var msClientPrincipalEncoded = headers.Where(x => x.Key == "X-MS-CLIENT-PRINCIPAL").FirstOrDefault();

            var decodedBytes = Convert.FromBase64String(msClientPrincipalEncoded.Value.First());
            using var memoryStream = new MemoryStream(decodedBytes);
            var clientPrincipal = await System.Text.Json.JsonSerializer.DeserializeAsync<MsClientPrincipal>(memoryStream);

            var claims = clientPrincipal.Claims.Select(claim => new Claim(claim.Type, claim.Value));
            if (clientPrincipal == null || !clientPrincipal.Claims.Any())
                return result;

            // remap "roles" claims from easy auth to the more standard ClaimTypes.Role / "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            var easyAuthRoleClaims = claims.Where(claim => claim.Type == "roles");
            var claimsAndRoles = claims.Concat(easyAuthRoleClaims.Select(role => new Claim(ClaimTypes.Role, role.Value)));

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claimsAndRoles, clientPrincipal.AuthenticationType, clientPrincipal.NameType, ClaimTypes.Role));
     
            Context.User = principal;
            result = new AuthenticationTicket(principal, easyAuthProvider.Value.First());

            return result;

        }

        private async Task<AuthenticationTicket> AddClaimsPrincipalFromEasyAuthHeaders()
        {

            AuthenticationTicket result = null;
            var easyAuthProvider = Context.Request.Headers["X-MS-CLIENT-PRINCIPAL-IDP"].FirstOrDefault() ?? "aad";
            var msClientPrincipalEncoded = Context.Request.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(msClientPrincipalEncoded))
                return result;

            var decodedBytes = Convert.FromBase64String(msClientPrincipalEncoded);
            using var memoryStream = new MemoryStream(decodedBytes);
            var clientPrincipal = await JsonSerializer.DeserializeAsync<MsClientPrincipal>(memoryStream);

            if (clientPrincipal == null || !clientPrincipal.Claims.Any())
                return result;

            var claims = clientPrincipal.Claims.Select(claim => new Claim(claim.Type, claim.Value));

            // remap "roles" claims from easy auth to the more standard ClaimTypes.Role / "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            var easyAuthRoleClaims = claims.Where(claim => claim.Type == "roles");
            var claimsAndRoles = claims.Concat(easyAuthRoleClaims.Select(role => new Claim(ClaimTypes.Role, role.Value)));

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claimsAndRoles, clientPrincipal.AuthenticationType, clientPrincipal.NameType, ClaimTypes.Role));

            Context.User = principal;
            result = new AuthenticationTicket(principal, easyAuthProvider);

            return result;

        }

        private async Task AddClaimsPrincipalWithRolesFromInterviewDb()
        {

            // Inject Role from InternalUser
            var dal = Context.RequestServices.GetService<DalSql>();
            Guid entraId = new Guid(Context.User.Claims.FirstOrDefault(x => x.Type == Constants.EntraIdClaimKey).Value);
            var internalUser = await dal.GetInternalUserByEntraId(entraId);
            if (internalUser != null)
            {
                // https://stackoverflow.com/questions/53292286/is-there-a-way-to-add-claims-in-an-asp-net-core-middleware-after-authentication
                var claim = new Claim(ClaimTypes.Role, internalUser.RoleType.ToString(), "http://www.w3.org/2001/XMLSchema#string", "LOCAL AUTHORITY", "LOCAL AUTHORITY");
                var interviewClaims = new List<Claim>();
                interviewClaims.Add(claim);
                var appIdentity = new ClaimsIdentity(interviewClaims);
                Context.User.AddIdentity(appIdentity);
            }

        }

        #endregion

    }

}
