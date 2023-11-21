using AutoMapper;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;


namespace ISS.Repository.Utility
{
    public class DbHelper
    {
        public List<T> ReadData<T>(IDataReader reader) where T : class
        {
            using (reader)
            {
               return FillCollection<T>(reader);

               //DataTable dt = new DataTable();
               //dt.Load(reader);
               //var tt1= Mapper.Map<EnumerableRowCollection, List<T>>(dt.AsEnumerable());
               //return tt;
            }
        }

        public List<T> GetData<T>(OracleDataReaderWrapper reader) where T : class
        {
            
            using (reader)
            {
                return Mapper.Map<OracleDataReaderWrapper, List<T>>(reader);
            }

        }
        

        private  List<T> FillCollection<T>( IDataReader dr)
        {
            Type sourceType = typeof(T);
            T genericObject ;
            List<T> genericList = new List<T>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(sourceType);

            while (dr.Read())
            {
                genericObject = Activator.CreateInstance<T>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    var fieldName = dr.GetName(i);
                    try
                    {                       
                        var property = properties.Find(fieldName, true);
                        
                        if (property != null && dr[i] != DBNull.Value)
                        {
                            property.SetValue(genericObject, dr[i]);
                        }
                    }
                    catch (Exception ee)
                    {
                       var tt = true;
                        if(tt)
                            throw ee;
                    }

                }           
                genericList.Add(genericObject);
            }
            if (dr != null)
                if (!dr.IsClosed) dr.Close();
            return genericList;
        }


        /// <summary>
        /// If it can be converted, this function will figure out how. Given a source
        /// object, tries its best to convert it to the target type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="targetType">The type to convert the source object to.</param>
        /// <returns></returns>
        public static object Convert(object source, Type targetType)
        {
            if (source == null || source == DBNull.Value)
            {
                // Returns the default(T) of the type
                return targetType.IsValueType
                    ? Activator.CreateInstance(targetType)
                    : null;
            }

            var sourceType = source.GetType();

            // Try casting
            if (targetType.IsAssignableFrom(sourceType))
                return source;

            // Try type descriptors
            var targetConverter = TypeDescriptor.GetConverter(targetType);
            if (targetConverter.CanConvertFrom(sourceType))
            {
                return targetConverter.ConvertFrom(source);
            }

            var sourceConverter = TypeDescriptor.GetConverter(sourceType);
            if (sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(source, targetType);
            }

            // Find an implicit assignment converter
            var implicitAssignment = targetType.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static)
                .Where(x => x.Name == "op_Implicit")
                .Where(x => targetType.IsAssignableFrom(x.ReturnType))
                .FirstOrDefault();

            if (implicitAssignment != null)
            {
                return implicitAssignment.Invoke(null, new[] { source });
            }

            // Hope and pray
            return source;
        }


    }
}
