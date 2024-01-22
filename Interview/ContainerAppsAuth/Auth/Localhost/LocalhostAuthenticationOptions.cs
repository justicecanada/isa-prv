using Microsoft.AspNetCore.Authentication;

namespace ContainerAppsAuth.Auth.Localhost
{
    
    public class LocalhostAuthenticationOptions : AuthenticationSchemeOptions
    {

        public LocalhostAuthenticationOptions()
        {
            Events = new object();
        }

    }

}
