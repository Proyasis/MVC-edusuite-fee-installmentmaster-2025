using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class SMSHelperService : ISMSHelperService
    {

        private EduSuiteDatabase dbContext;

        public SMSHelperService()
        {
            this.dbContext = new EduSuiteDatabase();
        }
        public SMSViewModel GetSMSDetails(SMSViewModel model)
        {
            return dbContext.SMSSettings.Where(x => x.IsActive == true).Select(row => new SMSViewModel
            {
                RowKey = row.SmsId,
                UserId = row.SmsUserName,
                SenderID = row.SmsSenderId,
                Root = row.SmsRoot,
                Password = row.SmsPassword,
                APICode = row.SmsApiCode,
                URL = row.SmsUrl,
                IsApi = row.IsApi,
                SMSContent = model.SMSContent,
                SMSReceiptants = model.SMSReceiptants,
                SmsPEID = row.SmsPEID,
                SMSTemplateID = model.SMSTemplateID
            }).FirstOrDefault();

        }
    }
}
