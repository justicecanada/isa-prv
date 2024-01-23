using Microsoft.AspNetCore.Authentication;

namespace Interview.UI.Auth.Localhost
{
    
    public class LocalhostAuthenticationOptions : AuthenticationSchemeOptions
    {

        public LocalhostAuthenticationOptions()
        {
            Events = new object();
        }

    }

}
