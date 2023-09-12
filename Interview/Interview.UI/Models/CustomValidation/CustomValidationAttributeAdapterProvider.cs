using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.CustomValidation
{
    
    public class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {

        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {

            if (attribute is CompareDateTimeOffsets)
                return new CompareDateTimeOffsetsAdapter(attribute as CompareDateTimeOffsets, stringLocalizer);
            else if (attribute is CompareTimeSpans)
                return new CompareTimeSpansAdapter(attribute as CompareTimeSpans, stringLocalizer);
            else
                return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);

        }

    }

    public class CompareDateTimeOffsetsAdapter : AttributeAdapterBase<CompareDateTimeOffsets>
    {

        public CompareDateTimeOffsetsAdapter(CompareDateTimeOffsets attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {

        }

        public override void AddValidation(ClientModelValidationContext context) 
        { 
        
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }

    }

    public class CompareTimeSpansAdapter : AttributeAdapterBase<CompareTimeSpans>
    {

        public CompareTimeSpansAdapter(CompareTimeSpans attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {
                
        }

        public override void AddValidation(ClientModelValidationContext context)
        {

        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }

    }

}
