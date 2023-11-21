using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Security
{
    public class MenuConfiguration
    {
        public string MenuName { get; set; }
        public string MenuType { get; set; }
        public List<UserRoles> MenuRoles { get; set; }       
    }
}
