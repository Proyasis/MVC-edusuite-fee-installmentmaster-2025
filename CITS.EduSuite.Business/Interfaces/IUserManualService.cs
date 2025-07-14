using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IUserManualService
    {
        UserManualViewModel GetUserManualById(int? id);
        UserManualViewModel CreateUserManual(UserManualViewModel model);
        UserManualViewModel UpdateUserManual(UserManualViewModel model);
        UserManualViewModel DeleteUserManual(UserManualViewModel model);
        UserManualDetailsViewModel DeleteUserManualDetails(UserManualDetailsViewModel model);
        List<UserManualViewModel> GetUserManual(UserManualViewModel model);
        UserManualViewModel DeleteUserManualDocument(short Id);
        UserManualAllViewModel ViewUserManualAll();
    }
}
