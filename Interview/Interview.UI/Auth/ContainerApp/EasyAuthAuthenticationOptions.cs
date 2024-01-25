using Microsoft.AspNetCore.Authentication;

namespace Interview.UI.Auth.ContainerApp
{
    
    public class EasyAuthAuthenticationOptions : AuthenticationSchemeOptions
    {

        public EasyAuthAuthenticationOptions()
        {
            Events = new object();
        }

    }

}
