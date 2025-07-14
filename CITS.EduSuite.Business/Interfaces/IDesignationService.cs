using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IDesignationService
    {

        List<DesignationViewModel> GetDesignations(string searchText);

        DesignationViewModel GetDesignationById(int? id);

        DesignationViewModel CreateDesignation(DesignationViewModel model);

        DesignationViewModel UpdateDesignation(DesignationViewModel model);

        DesignationViewModel DeleteDesignation(DesignationViewModel model);
        List<DesignationViewModel> GetDesignationChart();
        DesignationViewModel UpdateDesignationChart(List<DesignationViewModel> modelList);

        #region Permission
        DesignationViewModel GetDesignationPermissionsById(Int16 DesignationKey);
        DesignationViewModel UpdateDesignationPermission(DesignationViewModel model);
        #endregion
    }
}
