using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Provider;
using CITS.EduSuite.Business.Models.Security;
using System.Web.Security;

namespace CITS.EduSuite.UI.Controllers
{
    public class PrintInvoiceController : BaseController
    {
        private IPrintInvoiceService printInvoiceService;

        public PrintInvoiceController(IPrintInvoiceService objPrintInvoiceService)
        {   
            this.printInvoiceService = objPrintInvoiceService;

        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.PartyPendingAccount, ActionCode = MenuConstants.Print)]
        [HttpGet]
        public string PrintReciept(string ReceiptNumber, short BranchKey, int PrintType)
        {

            var resultString = printInvoiceService.GetPrintInvoice(null, PrintType, BranchKey, ReceiptNumber);
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.PartyPendingAccount, MenuConstants.Print, DbConstants.LogType.Error, Id, model.ExceptionMessage);
            //}

            return resultString;
        }


        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.Approval, ActionCode = MenuConstants.Print)]
        [HttpGet]
        public string PrintSalesOrderForm(long Id, int PrintType)
        {

            var resultString = printInvoiceService.GetPrintInvoice(Id, PrintType, null, null);
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.Approval, MenuConstants.Print, DbConstants.LogType.Error, id, model.ExceptionMessage);
            //}
            return resultString;
        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.Quotation, ActionCode = MenuConstants.Print)]
        [HttpGet]
        public string PrintQuotationDealerForm(long Id, int PrintType)
        {

            var resultString = printInvoiceService.GetPrintInvoice(Id, PrintType, null, null);
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.Approval, MenuConstants.Print, DbConstants.LogType.Error, id, model.ExceptionMessage);
            //}
            return resultString;
        }

        //[ActionAuthenticationAttribute(MenuCode = MenuConstants.Approval, ActionCode = MenuConstants.Print)]
        [HttpGet]
        public string PrintOrderDealerForm(long Id, int PrintType)
        {

            var resultString = printInvoiceService.GetPrintInvoice(Id, PrintType, null, null);
            //if (model.ExceptionMessage != null && model.ExceptionMessage != "")
            //{
            //    ActivityLog.CreateActivityLog(GetUserKey(), MenuConstants.Approval, MenuConstants.Print, DbConstants.LogType.Error, id, model.ExceptionMessage);
            //}
            return resultString;
        }

    }
}