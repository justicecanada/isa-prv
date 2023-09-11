using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Interview.UI.Helpers
{
    
    public static class HtmlExtensions
    {

        public static TagBuilder JusLabelFor<TModel>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TModel>> expression)
        {

            TagBuilder result = new TagBuilder("label");
            Type type = html.ViewData.ModelExplorer.Container.Model.GetType();
            string propertyName = ((Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper)html).ViewData.ModelMetadata.Name;
            var metaData = html.MetadataProvider.GetMetadataForProperty(type, propertyName);
            bool isRequired = false;

            if (metaData.ContainerType != null)
            {
                isRequired = metaData.ContainerType.GetProperty(metaData.PropertyName).GetCustomAttributes(typeof(RequiredAttribute), false).Length == 1;
            }

            result.InnerHtml.SetContent(metaData.DisplayName);
            result.Attributes.Add("for", metaData.Name);
            if (isRequired)
                result.AddCssClass("required");

            return result;

        }

    }

}
