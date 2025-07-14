using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IBaseActivityLogSearchService
    {
        void FillDropDown(ActivityLogReportViewModel model);
        //ReportViewModel FillUserID(ReportViewModel model);
        //ReportViewModel FillMenus(ReportViewModel model);
    }
}
