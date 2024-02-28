﻿using Interview.Entities;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Default
{
    
    public class VmInterviewModal : VmBase
    {

        public Guid ProcessId { get; set; }

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
        public TimeSpan VmStartTime { get; set; }

        [Display(Name = "Duration")]
        //[RegularExpression(@"^[0-9]*$", ErrorMessage = "InterviewDurationNumeric")]
        [Required]
        public int? Duration { get; set; }

        public InterviewStates? Status { get; set; }

        [Display(Name = "ContactName")]
        [MaxLength(128, ErrorMessage = "MaxLength")]
        public string? ContactName { get; set; }

        [Display(Name = "ContactNumber")]
        [MaxLength(128, ErrorMessage = "MaxLength")]
        public string? ContactNumber { get; set; }


    }

}
