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

        public Guid? ContestId
        {
            get
            {
                if (_session.Keys.Contains(Constants.ContestId))
                    return new Guid(_session.GetString(Constants.ContestId));
                else
                    return null;
            }
            set
            {
                if (value == null)
                    _session.Remove(Constants.ContestId);
                else
                    _session.SetString(Constants.ContestId, value.ToString());
            }
        }

    }

}
