using Microsoft.AspNetCore.Authentication;

namespace ContainerAppsAuth.Auth.ContainerApp
{

    public static class EasyAuthAuthenticationBuilderExtensions
    {

        public const string EASYAUTHSCHEMENAME = "EasyAuth";

        public static AuthenticationBuilder AddAzureContainerAppsEasyAuth(
            this AuthenticationBuilder builder,
            Action<EasyAuthAuthenticationOptions>? configure = null)
        {
            if (configure == null) configure = o => { };

            return builder.AddScheme<EasyAuthAuthenticationOptions, EasyAuthAuthenticationHandler>(
                EASYAUTHSCHEMENAME,
                EASYAUTHSCHEMENAME,
                configure);
        }

    }

}
