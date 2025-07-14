using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.Security
{
   public interface ICITSEduSuitePrincipal:IPrincipal
    {
       long UserKey { get; set; }
         string Name { get; set; }
    }
}
