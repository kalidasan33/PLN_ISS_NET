using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
   public class SOStyleValidaion
    {

     public bool VendorIsValid   {set;get;}
     public bool DCPathIsValid   {set;get;}
     public bool SewpathIsValid  {set;get;}
     public bool ErrorStatus   {set;get;}
     public String ErrorMessage  {set;get;}

     public decimal  PlannedLeadTime      {set;get;}
     public decimal  TransportationTime   {set;get;}

    }
}
