using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface IApplicationFeeInstallmentService
    {

        ApplicationFeeInstallmentViewModel GetFeeInstallmentById(ApplicationFeeInstallmentViewModel model);

       
        ApplicationFeeInstallmentViewModel UpdateFeeInstallment(ApplicationFeeInstallmentViewModel model);

        ApplicationFeeInstallmentViewModel DeleteFeeInstallment(FeeInstallmentModel model);


    }
}
