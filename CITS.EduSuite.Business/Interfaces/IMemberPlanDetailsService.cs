using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IMemberPlanDetailsService
    {
        List<MemberPlanDetailsViewModel> GetMemberPlanDetails(MemberPlanDetailsViewModel model, out long TotalRecords);
        MemberPlanDetailsViewModel GetMemberPlanDetailsById(long? id);

        MemberPlanDetailsViewModel UpdateMemberPlanDetails(MemberPlanDetailsViewModel model);
        MemberPlanDetailsViewModel DeleteMemberPlanDetails(MemberPlanDetailsViewModel model);
        //MemberPlanDetailsViewModel FillCourses(MemberPlanDetailsViewModel model);
        //MemberPlanDetailsViewModel FillApplication(MemberPlanDetailsViewModel model);
        void FillBranches(MemberPlanDetailsViewModel model);

        void FillDropdownLists(MemberPlanDetailsViewModel model);
        List<MemberPlanDetailsViewModel> GetMemberPlan(MemberPlanDetailsViewModel model, out long TotalRecords);
        MemberPlanDetailsViewModel CreateMemberPlans(List<MemberPlanDetailsViewModel> modelList);
        void FillMemberType(MemberPlanDetailsViewModel model);

        void FillBorrowerType(MemberPlanDetailsViewModel model);
    }
}
