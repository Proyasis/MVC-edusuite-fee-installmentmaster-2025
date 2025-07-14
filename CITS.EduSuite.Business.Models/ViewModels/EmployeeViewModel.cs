using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeViewModel : BaseModel
    {
        public EmployeeViewModel()
        {
            EmployeePersonalDetails = new EmployeePersonalViewModel();
            EmployeeSubjectAllocationDetails = new List<EmployeeSubjectDetailsModel>();
            EmployeeSalaryAdvanceDetails = new List<EmployeeSalaryAdvanceViewModel>();
            EmployeeSalaryDetails = new List<EmployeeSalaryMasterViewModel>();
            WorkScheduleDetails = new List<WorkscheduleSubjectmodel>();
            EmployeeSalaryAdvanceReturnDetails = new List<EmployeeSalaryAdvanceReturnViewModel>();
            EmployeeSalaryPaymentDetails = new List<PaymentWindowViewModel>();
        }
        public EmployeePersonalViewModel EmployeePersonalDetails { get; set; }

        public List<EmployeeSubjectDetailsModel> EmployeeSubjectAllocationDetails { get; set; }

        public List<EmployeeSalaryAdvanceViewModel> EmployeeSalaryAdvanceDetails { get; set; }
        public List<EmployeeSalaryMasterViewModel> EmployeeSalaryDetails { get; set; }
        public List<WorkscheduleSubjectmodel> WorkScheduleDetails { get; set; }
        public long EmployeeKey { get; set; }
        public List<EmployeeSalaryAdvanceReturnViewModel> EmployeeSalaryAdvanceReturnDetails { get; set; }
        public List<PaymentWindowViewModel> EmployeeSalaryPaymentDetails { get; set; }


    }
}
