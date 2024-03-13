namespace Interview.UI.Models.AppSettings
{
    
    public class SessionTimeoutOptions
    {

        public string CookieName { get; set; }

        public bool Enabled { get; set; }

        public int IdleTimeoutInMinutes { get; set; }

        public int InactivityInMilliseconds { get; set; }

        public int ReactionTimeInMilliseconds { get; set; }

        public int SessionAliveInMilliseconds { get; set; }

        public int RefreshLimitInMilliseconds { get; set; }

        public bool RefreshOnClick { get; set; }

    }

}
