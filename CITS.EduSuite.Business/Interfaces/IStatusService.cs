using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IStatusService
    {
        List<StatusViewModel> GetStatuses(string searchText);

        StatusViewModel GetStatusById(Int16? id);

        StatusViewModel CreateStatus(StatusViewModel model);

        StatusViewModel UpdateStatus(StatusViewModel model);

        StatusViewModel DeleteStatus(StatusViewModel model);
    }
}
