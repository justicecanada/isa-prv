using ContainerAppsAuth.Auth.ContainerApp;
using Microsoft.AspNetCore.Authentication;

namespace ContainerAppsAuth.Auth.Localhost
{
    
    public static class LocalhostAuthenticationBuilderExtensions
    {

        public const string LOCALHOSTAUTHSCHEMENAME = "LocalHostAuth";

        public static AuthenticationBuilder AddLocalhostAuth(this AuthenticationBuilder builder, Action<LocalhostAuthenticationOptions>? configure = null)
        {
            if (configure == null) configure = o => { };

            return builder.AddScheme<LocalhostAuthenticationOptions, LocalHostAuthenticationHandler>(
                LOCALHOSTAUTHSCHEMENAME,
                LOCALHOSTAUTHSCHEMENAME,
                configure);
        }

    }

}
