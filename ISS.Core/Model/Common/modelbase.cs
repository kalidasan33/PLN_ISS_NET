using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace ISS.Core.Model.Common
{
   public  class ModelBase
    {

       public T CloneModel<T>()
       {
           T domainModel = Activator.CreateInstance<T>();
           var modelProperties = domainModel.GetType().GetProperties();

           Type processType = this.GetType();
           PropertyInfo[] processProperties = processType.GetProperties();

           foreach (var property in modelProperties)
           {
               var p = processType.GetProperty(property.Name);
               if (p != null)
               {
                   try
                   {
                       property.SetValue(domainModel, p.GetValue(this, null), null);
                   }
                   catch { }
               }
           }

           return domainModel;
       }

    }
}
