namespace Interview.UI.Services.State
{
    
    public interface IState
    {

        public Guid? ProcessId { get; set;  }

        public string NoticationMessage { get; set; }

    }

}
