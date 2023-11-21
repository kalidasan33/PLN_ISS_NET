using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using ISS.Common;
using System.Reflection;
namespace ISS.Web.Helpers
{

    public class ISSModelBinding : DefaultModelBinder
    {


        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            //if (model is System.Collections.IEnumerable)
            //{
            //    var enu = (model as System.Collections.IEnumerable).GetEnumerator();
            //    if (enu.MoveNext())
            //    {
            //        var props = getUTCRequiredProps(enu.Current);
            //        setUTCToLocal(enu.Current, props);
            //        while (enu.MoveNext())
            //        {
            //            setUTCToLocal(enu.Current, props);
            //        }
            //    }
            //}
            //else
            //{
            //    var props = getUTCRequiredProps(model);
            //    setUTCToLocal(model, props);
            //}
            return model;
        }

        protected List<String> getUTCRequiredProps(Object model)
        {
            if (model != null)
            {
                var properties = model.GetType().GetProperties();
                var list = properties.Where(property => ((property.GetCustomAttributes(typeof(UTCConversionNotRequiredAttribute), true).Length == 1)
                    )).Select(e => e.Name).ToList();
              
                return list;
            }
            return null;
        }

        protected void setUTCToLocal(Object model, List<String> props)
        {
            if (model != null && props != null && props.Count>0)
            {
                props.ForEach(name=> {
                    var property = model.GetType().GetProperty(name);                
                    DateTime dateTime = (DateTime)property.GetValue(model, null);
                   // property.SetValue(model, dateTime.ToUniversalTime(), null);
                    var tt = 00;
                });
            }
        }
    }

}