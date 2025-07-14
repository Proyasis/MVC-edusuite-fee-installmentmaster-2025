using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ISharedService
    {
        List<SelectListModel> GetBranches();

        List<MenuViewModel> GetMenuByUserId(MenuViewModel model);
        bool ShowUniversity();
        bool ShowAcademicTerm();
        bool CheckMenuActive(string MenuCode);
        string GetBakup(string DBName, string Location);

        ApplicationPersonalViewModel FillApplicationDetails(string AdmissionNo, long? ApplicationKey);
        EmployeePersonalViewModel FillEmployeeDetails(string EmployeeCode, long? EmployeeKey);
        long GetApplicationKeyByCode(string RollNoCode);

        decimal CheckShortBalance(short PaymentModeKey, long BankAccountKey, short branchKey);
        MemberPlanDetailsViewModel FillLibraryDetails(string CardId, long? MemberPlanKey);
        string GetMemberPlanKeyByCardId(string CardId);
		 BranchViewModel GetBranchDetailById(short? id);
    }
}
