using Microsoft.AspNetCore.Authentication;

namespace ContainerAppsAuth.Auth.ContainerApp
{

    public class EasyAuthAuthenticationOptions : AuthenticationSchemeOptions
    {

        public EasyAuthAuthenticationOptions()
        {
            Events = new object();
        }

    }

}
