using Microsoft.AspNetCore.Authentication;

namespace Interview.UI.Auth
{
    
    public class AuthenticationOptions : AuthenticationSchemeOptions
    {

        public AuthenticationOptions()
        {
            Events = new object();
        }

    }

}
