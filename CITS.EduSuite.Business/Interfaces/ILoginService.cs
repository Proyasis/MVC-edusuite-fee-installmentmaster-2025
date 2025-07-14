using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ILoginService
    {
        LoginModel AttemptLogin(string userName, string password, int RoleKey);

        void Logout();

        LoginModel ChangePassword(LoginModel model);

        LoginModel CheckPasswordExists(string Password);
        LoginModel ForgotPasswordSelect(LoginModel model);
        LoginModel CheckOTP(LoginModel model);
        LoginModel AttempOTPLogin(string userName);
        LoginModel ChangePasswordByOTP(LoginModel model);
        LoginModel CheckOTPExist(LoginModel model);

        List<string> GetMACAddress();
    }
}
