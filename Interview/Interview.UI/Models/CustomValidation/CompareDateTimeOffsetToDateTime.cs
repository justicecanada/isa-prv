using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.CustomValidation
{
    
    public class CompareDateTimeOffsetToDateTime : ValidationAttribute
    {

        public string Operator { get; set; }
        public string Propertyname { get; set; }
        public string ConditinalPropertyName { get; set; }

        public CompareDateTimeOffsetToDateTime(string compareOperator, string propertyName, string conditionalPropertyName = null)
        {

            Operator = compareOperator;
            Propertyname = propertyName;
            ConditinalPropertyName = conditionalPropertyName;

        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {

            if (value == null)
                return ValidationResult.Success;

            var dependentValue = context.ObjectInstance.GetType().GetProperty(Propertyname).GetValue(context.ObjectInstance, null);
            bool conditionalValue = true;
            bool evaluationResult;

            if (!string.IsNullOrEmpty(ConditinalPropertyName))
            {
                var conditional = context.ObjectInstance.GetType().GetProperty(ConditinalPropertyName).GetValue(context.ObjectInstance, null);
                bool canConvert = bool.TryParse(conditional.ToString(), out conditionalValue);
                if (!canConvert)
                    throw new Exception(string.Format("Cannot convert value for field {0} to a boolean", ConditinalPropertyName));
            }

            if ((dependentValue != null) && (value != null) && conditionalValue)
            {

                DateTimeOffset valueDate = DateTimeOffset.Parse(value.ToString());
                DateTime dependentDate = Convert.ToDateTime(dependentValue);

                switch (Operator)
                {
                    case "==":
                        evaluationResult = valueDate == dependentDate;
                        break;
                    case "!=":
                        evaluationResult = valueDate != dependentDate;
                        break;
                    case "<=":
                        evaluationResult = valueDate <= dependentDate;
                        break;
                    case ">=":
                        evaluationResult = valueDate >= dependentDate;
                        break;
                    case ">":
                        evaluationResult = valueDate > dependentDate;
                        break;
                    case "<":
                        evaluationResult = valueDate < dependentDate;
                        break;
                    default:
                        throw new Exception(string.Format("Cannot use {0} as an operator for CompareDatesAttribute", Operator));
                }

                if (!evaluationResult)
                    return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });

            }

            return ValidationResult.Success;

        }

    }

}
