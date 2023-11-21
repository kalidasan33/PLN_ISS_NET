using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISS.Common
{
    public static class DataTableExtension
    {
        public static List<dynamic> ToList(this DataTable dataTable)
        {
            var columns = dataTable.Columns.Cast<DataColumn>();
            var columDefinitions = columns.Select(x => new { FieldName = x.ColumnName, FieldType = x.DataType }).ToList<dynamic>();
            var xyz = columns.Select(x => x.ColumnName ).ToList<string>();
            var names = string.Join(",",xyz.ToArray());
            Type myClass = CompileResultType(columDefinitions, dataTable.TableName);
            object myObject = null;
            try
            {
                myObject = Activator.CreateInstance(myClass);
            }
            catch { }
            var dataList = new List<dynamic>();
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in myObject.GetType().GetProperties(flags)
                                 select new
                                 {
                                     FieldName = aProp.Name,
                                     FieldType = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                                 }).ToList();

            var commonFields = objFieldNames.Intersect(columDefinitions).ToList();
            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var aTSource = Activator.CreateInstance(myClass);
                foreach (var aField in commonFields)
                {
                    try
                    {
                        PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField.FieldName);
                        if (!dataRow.IsNull(aField.FieldName))
                        {
                            propertyInfos.SetValue(aTSource, dataRow[aField.FieldName], null);
                        }
                    }
                    catch { }
                }
                dataList.Add(aTSource);
            }
            return dataList;
        }

        public static Type CompileResultType(IList<dynamic> columnDefinitions, string tableName)
        {
            TypeBuilder tb = GetTypeBuilder(tableName);
            // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
            foreach (var field in columnDefinitions)
                CreateProperty(tb, field.FieldName, field.FieldType);
            Type objectType = tb.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder(string tableName)
        {
            var typeSignature = string.Format("StyleNavigator.DTO.Custom.{0}", tableName);
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {

            propertyType = GetRequiredType(propertyType);
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        private static Type GetRequiredType(Type propertyType)
        {
            return propertyType == typeof(DateTime) ? typeof(DateTime?) : propertyType;
        }

    }
}
