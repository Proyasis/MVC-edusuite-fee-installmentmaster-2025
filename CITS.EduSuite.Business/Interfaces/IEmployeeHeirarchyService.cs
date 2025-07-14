using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IEmployeeHeirarchyService
    {
        EmplyeeHeirarchyViewModel GetEmployeeHeirarchyById(long? id);
        EmplyeeHeirarchyViewModel UpdateEmplyeeHeirarchyPermission(EmplyeeHeirarchyViewModel model);
    }
}
