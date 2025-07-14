using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class BaseActivityLogSearchService:IBaseActivityLogSearchService
    {
        private EduSuiteDatabase dbContext;
     

        public BaseActivityLogSearchService(EduSuiteDatabase objdb)
        {
            this.dbContext = objdb;
        }

        public void FillDropDown(ActivityLogReportViewModel model)
        {

            FillUserID(model);
            FillMenus(model);

            

        }

        public void FillUserID(ActivityLogReportViewModel model)
        {
            model.UserIDs = dbContext.AppUsers.Select(x => new SelectListModel
            {
                RowKey = x.RowKey,
                Text = x.AppUserName
            }).ToList();
            

        }
        public void FillMenus(ActivityLogReportViewModel model)
        {
            model.Menus = dbContext.Menus.Select(x => new SelectListModel
            {
                Code = x.MenuCode,
                Text = x.MenuName
            }).ToList();
            

        }
        

    }
}
