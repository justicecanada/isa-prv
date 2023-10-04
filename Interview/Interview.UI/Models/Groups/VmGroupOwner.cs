using Interview.UI.Services.Mock.Identity;

namespace Interview.UI.Models.Groups
{
    
    public class VmGroupOwner : VmBase
    {

        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public bool? HasAccessEE { get; set; }

        public VmGroup Group { get; set; }

        public MockUser MockUser { get; set; }

    }

}
