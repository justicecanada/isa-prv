using Microsoft.AspNetCore.Authentication;

namespace Interview.UI.Auth
{
    
    public static class AuthenticationBuilderExtensions
    {

        public const string AUTHSCHEMENAME = "HostAuth";

        public static AuthenticationBuilder AddAuth(this AuthenticationBuilder builder, Action<AuthenticationOptions>? configure = null)
        {
            if (configure == null) configure = o => { };

            return builder.AddScheme<AuthenticationOptions, AuthenticationHandler>(
                AUTHSCHEMENAME,
                AUTHSCHEMENAME,
                configure);
        }

    }

}
