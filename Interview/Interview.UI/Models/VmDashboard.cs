using Interview.Entities;
using Interview.UI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
	public class VmDashboard : VmBase
	{
		[Required]
		[Display(Name = "StartDate")]
		public DateTime? StartDate { get; set; }
	}
}
