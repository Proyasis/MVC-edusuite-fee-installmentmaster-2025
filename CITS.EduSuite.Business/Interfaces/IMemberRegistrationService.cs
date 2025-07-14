using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IMemberRegistrationService
    {
        List<MemberRegistrationViewModel> GetMemberRegistration(string SearchText, short? BranchKey);

        MemberRegistrationViewModel GetMemberRegistrationById(Int32? Id);

        MemberRegistrationViewModel CreateMemberRregistration(MemberRegistrationViewModel model);

        MemberRegistrationViewModel UpdateMemberRregistration(MemberRegistrationViewModel model);

        MemberRegistrationViewModel DeleteMemberRegistration(MemberRegistrationViewModel model);

        MemberRegistrationViewModel CheckPhoneExists(string PhoneNo, int RowKey);

        MemberRegistrationViewModel CheckEmailExists(string EmailNo, int RowKey);

        MemberRegistrationViewModel GetFeesById(MemberRegistrationViewModel model);

        MemberRegistrationViewModel CheckIdentificationExists(MemberRegistrationViewModel model);
        void FillBranches(MemberRegistrationViewModel model);
    }
}
