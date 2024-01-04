using Interview.UI.Models.CustomValidation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            string fieldName = expression.Body.ToString().Replace("x.", "").Replace("Convert(", "").Replace(", Nullable`1)", "");
            Type type = html.ViewData.ModelExplorer.Model.GetType();
            var metaData = html.MetadataProvider.GetMetadataForProperty(type, fieldName);
            bool noFutureDates = false;

            result.GenerateId(fieldName, "");
            result.Attributes.Add("name", fieldName);
            result.AddCssClass("form-control");
            result.Attributes.Add("type", "date");

            if (obj != null)
            {

                Type t = obj.GetType();
                PropertyInfo p;
                DateTime date;

                p = t.GetProperty("date");
                date = System.Convert.ToDateTime(p.GetValue(obj, null));
                if (date.ToString("yyyy-MM-dd") != DateTime.MinValue.ToString("yyyy-MM-dd"))
                    result.Attributes.Add("value", date.ToString("yyyy-MM-dd"));

                p = t.GetProperty("min");
                if (p != null)
                {
                    date = System.Convert.ToDateTime(p.GetValue(obj, null));
                    result.Attributes.Add("min", date.ToString("yyyy-MM-dd"));
                }

                p = t.GetProperty("max");
                if (p != null)
                { 
                    date = System.Convert.ToDateTime(p.GetValue(obj, null));
                    result.Attributes.Add("max", date.ToString("yyyy-MM-dd"));
                }

            }

            return result;

        }

        public static TagBuilder JusRichTextBoxFor<TModel>(this IHtmlHelper<TModel> html, Expression<Func<TModel, string?>> expression, object obj = null)
        {

            TagBuilder result = new TagBuilder("textarea");
            string fieldName = expression.Body.ToString().Replace("x.", "").Replace("Convert(", "").Replace(", Nullable`1)", "");
            Type type = html.ViewData.ModelExplorer.Model.GetType();
            var metaData = html.MetadataProvider.GetMetadataForProperty(type, fieldName);

            result.GenerateId(fieldName, "");
            result.Attributes.Add("name", fieldName);
            result.AddCssClass("form-control");
            result.AddCssClass("richTextBox");

            if (obj != null)
            {
                Type t = obj.GetType();
                PropertyInfo p = t.GetProperty("value");
                if (p != null)
                {
                    var value = p.GetValue(obj, null);
                    if (value != null)
                        result.InnerHtml.Append(value.ToString());
                }
            }

            return result;

        }

    }

}
