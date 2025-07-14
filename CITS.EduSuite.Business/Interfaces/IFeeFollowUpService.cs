using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IFeeFollowUpService
    {
        ApplicationFeeFollowupViewModel GetFollowup(ApplicationFeeFollowupViewModel model);
        ApplicationFeeFollowupDetailsViewModel GetFeeFollowupById(ApplicationFeeFollowupDetailsViewModel model);
        ApplicationFeeFollowupDetailsViewModel CreateFeeFollowup(ApplicationFeeFollowupDetailsViewModel model);
        ApplicationFeeFollowupDetailsViewModel UpdateFeeFollowup(ApplicationFeeFollowupDetailsViewModel model);
        ApplicationFeeFollowupDetailsViewModel DeleteFeeFollowup(ApplicationFeeFollowupDetailsViewModel model);
        void FillSearchProcessStatus(ApplicationFeeFollowupViewModel model);

    }
}
