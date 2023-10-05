using Interview.UI.Models.CustomValidation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
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
                isRequired = metaData.ContainerType.GetProperty(metaData.PropertyName).GetCustomAttributes(typeof(RequiredAttribute), false).Length == 1 ||
                    metaData.ContainerType.GetProperty(metaData.PropertyName).GetCustomAttributes(typeof(RequiredIf), false).Length != 0;
            }

            result.InnerHtml.SetContent(metaData.DisplayName);
            result.Attributes.Add("for", metaData.Name);
            if (isRequired)
            {
                if (System.Globalization.CultureInfo.CurrentCulture.Name == Constants.EnglishCulture)
                    result.AddCssClass("required-en");
                else if (System.Globalization.CultureInfo.CurrentCulture.Name == Constants.FrenchCulture)
                    result.AddCssClass("required-fr");
            }

            return result;

        }

        public static TagBuilder JusDatePickerFor<TModel>(this IHtmlHelper<TModel> html, Expression<Func<TModel, DateTime?>> expression, object obj = null)
        {

            TagBuilder result = new TagBuilder("input");
            string fieldName = expression.Body.ToString().Replace("x.", "");
            Type type = html.ViewData.ModelExplorer.Model.GetType();
            //string propertyName = ((Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper)html).ViewData.ModelMetadata.Name;
            var metaData = html.MetadataProvider.GetMetadataForProperty(type, fieldName);
            bool noFutureDates = false;

            //if (metaData.ContainerType != null)
            //    noFutureDates = metaData.ContainerType.GetProperty(metaData.PropertyName).GetCustomAttributes(typeof(NoFutureDateAttribute), false).Length == 1;

            if (noFutureDates)
            {
                result.AddCssClass("noFutureDates");
                result.Attributes.Add("data-date-end-date", "0d");
            }

            result.GenerateId(fieldName, "");
            result.Attributes.Add("name", fieldName);
            //result.Attributes.Add("type", "text");
            //result.Attributes.Add("placeholder", Constants.DateFormat.ToLower());
            //result.AddCssClass("date date-picker form-control");
            result.AddCssClass("form-control");
            result.Attributes.Add("type", "date");

            if (obj != null)
            {
                Type t = obj.GetType();
                PropertyInfo p = t.GetProperty("date");
                DateTime value = System.Convert.ToDateTime(p.GetValue(obj, null));
                if (value.ToString("yyyy-MM-dd") != DateTime.MinValue.ToString("yyyy-MM-dd"))
                    result.Attributes.Add("value", value.ToString("yyyy-MM-dd"));
            }

            return result;

        }

    }

}
