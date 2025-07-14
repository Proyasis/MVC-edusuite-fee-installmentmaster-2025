using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Provider;
using System.Web.Security;
using CITS.EduSuite.Business.Models.Security;

namespace CITS.EduSuite.UI.Controllers
{
    public class ActivityLogSearchController : BaseController
    {
        // GET: ActivityLogSearch
        IBaseActivityLogSearchService activityLogsearchService;


        public ActivityLogSearchController(IBaseActivityLogSearchService ObjActivityLogsearchService)
        {
            this.activityLogsearchService = ObjActivityLogsearchService;
        }

        public ActionResult Index()
        {
            ActivityLogReportViewModel model = new ActivityLogReportViewModel();
            
            activityLogsearchService.FillDropDown(model);
            return PartialView(model);
        }
      
    }
}