using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISS.Common; 
using System.Collections; 
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc.Html
{
    public static class HtmlExtension
    {

        public static MvcHtmlString CustomCheckBoxFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
       Expression<Func<TModel, TValue>> expression, bool editable = true, bool disabled = false)
        {


            var htmlString = "<table class='cbwraptbl'><tr><td>{0}</td><td>{1}</td></tr></table>";
            var labeltext = string.Empty;

            var classNameValue = "lb4";

            labeltext = System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = classNameValue }).ToHtmlString();

            String chk = null;
            if (!editable)
            {
                 chk = System.Web.Mvc.Html.InputExtensions.CheckBoxFor(htmlHelper, Expression.Lambda<Func<TModel, Boolean>>(expression.Body, expression.Parameters), new { @class = "cb", @readonly="readonly" }).ToHtmlString();
            }
            else
            {
                 chk = System.Web.Mvc.Html.InputExtensions.CheckBoxFor(htmlHelper, Expression.Lambda<Func<TModel, Boolean>>(expression.Body, expression.Parameters), new { @class = "cb" }).ToHtmlString();
            }

            htmlString = string.Format(htmlString, chk, labeltext);
            var html = new MvcHtmlString(htmlString);
            return html;


        }

        public static MvcHtmlString CustomLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
       Expression<Func<TModel, TValue>> expression,String addclass=""  )
        {
            var labeltext = string.Empty;
            var classNameValue = "mndtastx";
            var me = (MemberExpression)expression.Body;
            RequiredAttribute attr=me.Member.GetCustomAttribute<RequiredAttribute>();
            if(attr!=null)
                labeltext = System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = classNameValue + " " + addclass }).ToHtmlString();
            else
                if (addclass!="")
                labeltext = System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class =   addclass }).ToHtmlString();
                else
                    labeltext = System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression ).ToHtmlString();

            var html = new MvcHtmlString(labeltext);
            return html;


        }
    }

}