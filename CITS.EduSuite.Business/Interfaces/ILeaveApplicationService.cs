using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ILeaveApplicationService
    {
        List<LeaveApplicationViewModel> GetLeaveApplication(LeaveApplicationViewModel model);

        LeaveApplicationViewModel GetLeaveApplicationById(LeaveApplicationViewModel model);

        LeaveApplicationViewModel CreateLeaveApplication(LeaveApplicationViewModel model);

        LeaveApplicationViewModel UpdateLeaveApplication(LeaveApplicationViewModel model);

        LeaveApplicationViewModel DeleteLeaveApplication(LeaveApplicationViewModel model);

        LeaveApplicationViewModel GetBranches(LeaveApplicationViewModel model);
        LeaveApplicationViewModel GetEmployeesByBranchId(LeaveApplicationViewModel model);
        LeaveApplicationViewModel ApproveLeaveApplication(LeaveApplicationViewModel model);
    }
}
