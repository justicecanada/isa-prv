using Interview.Entities;
using Interview.UI.Models.Default;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
    
    public class VmInterview : VmBase
    {

        public VmInterview()
        {
            //InterviewUsers = new List<VmInterviewUser>();
            VmInterviewerUserIds = new VmInterviewerUserIds();
        }

        public Guid ContestId { get; set; }

        [Display(Name = "Room")]
		[Required]
		[MaxLength(128, ErrorMessage = "MaxLength")]
        public string? Room { get; set; }

        [Display(Name = "Location")]
		[Required]
		[MaxLength(128, ErrorMessage = "MaxLength")]
        public string? Location { get; set; }

        [Display(Name = "StartDate")]
		[Required]
		//[MaxLength(128, ErrorMessage = "MaxLength")]
		public DateTime VmStartDate { get; set; }

        [Display(Name = "StartTime")]
		[Required]
		public TimeSpan VmStartTime { get; set;  }

        [Display(Name = "Duration")]
        //[RegularExpression(@"^[0-9]*$", ErrorMessage = "InterviewDurationNumeric")]
        public int? Duration { get; set; }

        public int? Status { get; set; }

        [Display(Name = "ContactName")]
        [MaxLength(128, ErrorMessage = "MaxLength")]
        public string? ContactName { get; set; }

        [Display(Name = "ContactNumber")]
        [MaxLength(128, ErrorMessage = "MaxLength")]
        public string? ContactNumber { get; set; }

        //public List<VmInterviewUser>? InterviewUsers { get; set; }

        public VmInterviewerUserIds? VmInterviewerUserIds { get; set; }

    }

}
