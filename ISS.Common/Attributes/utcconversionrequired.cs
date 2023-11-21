using System;
 

namespace ISS.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UTCConversionNotRequiredAttribute : Attribute
    {
    }
}
