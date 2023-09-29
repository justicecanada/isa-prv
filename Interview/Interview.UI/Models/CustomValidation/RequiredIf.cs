using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.CustomValidation
{

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RequiredIf : ValidationAttribute
    {

        private readonly RequiredAttribute _innerAttribute;
        public string PropertyName { get; set; }
        public string TargetName { get; set; }
        public object DesiredValue { get; set; }

        public RequiredIf(string propertyName, string targetName, object desiredValue)
        {

            PropertyName = propertyName;
            TargetName = targetName;
            DesiredValue = desiredValue;
            _innerAttribute = new RequiredAttribute();

        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {

            var dependentValue = context.ObjectInstance.GetType().GetProperty(PropertyName).GetValue(context.ObjectInstance, null);
            //var contextAccessor = context.GetService(typeof(IHttpContextAccessor));

            if (dependentValue != null)
            {
                if (DesiredValue is bool)
                {
                    if (dependentValue.ToString() == DesiredValue.ToString())
                    {
                        if (!_innerAttribute.IsValid(value))
                            return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
                    }
                }
                else if (DesiredValue is string)
                {
                    List<string> desiredValues = ((string)DesiredValue).Split('|').ToList();
                    if (desiredValues.Contains(dependentValue))
                    {
                        if (!_innerAttribute.IsValid(value))
                            return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
                    }
                }
                else if (DesiredValue is Enum)
                {
                    if (DesiredValue.ToString() == dependentValue.ToString())
                    {
                        if (!_innerAttribute.IsValid(value))
                        {
                            return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
                        }
                    }
                }
            }

            return ValidationResult.Success;

        }

    }

}
