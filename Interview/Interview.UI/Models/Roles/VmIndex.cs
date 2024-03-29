﻿using Interview.Entities;
using Interview.UI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Roles
{
    
    public class VmIndex
    {

        #region User Type Section

        [Display(Name = "UserType")]
        [Required(ErrorMessage = "RequiredError")]      
        public UserTypes? UserType { get; set; }

        [Display(Name = "InternalName")]
        [RequiredIf("UserType", "UserType", UserTypes.Internal, ErrorMessage = "RequiredError")]      
        public string? InternalName { get; set;  }

        public Guid? InternalId { get; set;  }

        [Display(Name = "ExistingExternalName")]
        [RequiredIf("UserType", "UserType", UserTypes.ExistingExternal, ErrorMessage = "RequiredError")]        
        public string? ExistingExternalEmail { get; set; }

        [Display(Name = "NewExternalFirstName")]
        [RequiredIf("UserType", "UserType", UserTypes.NewExternal, ErrorMessage = "RequiredError")]       
        public string? NewExternalFirstName { get; set; }

        [Display(Name = "NewExternalLastName")]
        [RequiredIf("UserType", "UserType", UserTypes.NewExternal, ErrorMessage = "RequiredError")]
        public string? NewExternalLastName { get; set; }

        [Display(Name = "NewExternalEmail")]
        [RequiredIf("UserType", "UserType", UserTypes.NewExternal, ErrorMessage = "RequiredError")]
        [EmailAddress(ErrorMessage = "InvalidEmail")]
        public string? NewExternalEmail { get; set; }

        #endregion

        [Display(Name = "RoleId")]
        [Required(ErrorMessage = "RequiredError")]
        public VmRoleUserTypes? RoleUserType { get; set;  }

        [Display(Name = "UserLanguageId")]
        public VmLanguageTypes? LanguageType { get; set; }

        public List<VmEquity>? Equities { get; set; }

        public VmRoleUser? RoleUserToEdit { get; set;  }

    }

}
