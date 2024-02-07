using Microsoft.AspNetCore.Http;

namespace Interview.UI.Services.State
{
    
    public class SessionState : IState
    {

        private ISession _session;

        public SessionState(IHttpContextAccessor accessor)
        {
            _session = accessor.HttpContext.Session;
        }

        public Guid? ProcessId
        {
            get
            {
                if (_session.Keys.Contains(Constants.ProcessId))
                    return new Guid(_session.GetString(Constants.ProcessId));
                else
                    return null;
            }
            set
            {
                if (value == null)
                    _session.Remove(Constants.ProcessId);
                else
                    _session.SetString(Constants.ProcessId, value.ToString());
            }
        }

    }

}
