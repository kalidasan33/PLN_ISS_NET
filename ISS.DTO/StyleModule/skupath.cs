using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISS.DTO.StyleModule
{
    public class SkuPath
    {
        public string STYLE_CD { get; set; }
        public string COLOR_CD { get; set; }
        public string ATTRIBUTE_CD { get; set; }
        public string SIZE_CD { get; set; }
        public long Count { get; set; }
    }

    //public class SkuPathComparer : IEqualityComparer<PathBom>
    //{
    //    // Products are equal if their names and product numbers are equal. 
    //    public bool Equals(PathBom x, PathBom y)
    //    {

    //        //Check whether the compared objects reference the same data. 
    //        if (Object.ReferenceEquals(x, y)) return true;

    //        //Check whether any of the compared objects is null. 
    //        if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
    //            return false;

    //        //Check whether the products' properties are equal. 
    //        return x.STYLE_CD == y.STYLE_CD && x.COLOR_CD == y.COLOR_CD && x.ATTRIBUTE_CD == y.ATTRIBUTE_CD && x.SIZE_CD == y.SIZE_CD;
    //    }

    //    // If Equals() returns true for a pair of objects  
    //    // then GetHashCode() must return the same value for these objects. 

    //    public int GetHashCode(PathBom sPath)
    //    {
    //        //Check whether the object is null 
    //        if (Object.ReferenceEquals(sPath, null)) return 0;

    //        int hashStyleCode = sPath.STYLE_CD.GetHashCode();

    //        int hashColorCode = sPath.COLOR_CD.GetHashCode();

    //        int hashAttrCode = sPath.ATTRIBUTE_CD.GetHashCode();

    //        int hashSizeCode = sPath.SIZE_CD.GetHashCode();

    //        return hashStyleCode ^ hashColorCode ^ hashAttrCode ^ hashSizeCode;
    //    }

    //}
}