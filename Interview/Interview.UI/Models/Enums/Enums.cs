using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Enums
{

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum VmRoleTypes
    {
        [Description("HR Desc")]
        HR = 1,
        [Description("INTERVIEWER Desc")]
        INTERVIEWER = 2,
        [Description("LEAD Desc")]
        LEAD = 3,
        [Description("ASSISTANT Desc")]
        ASSISTANT = 4,
        [Description("CANDIDATE Desc")]
        CANDIDATE = 5,
        [Description("ADMIN Desc")]
        ADMIN = 6
    }

}
