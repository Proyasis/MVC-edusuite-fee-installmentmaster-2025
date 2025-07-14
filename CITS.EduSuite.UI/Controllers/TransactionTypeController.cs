using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CITS.EduSuite.UI.Controllers
{
    public class TransactionTypeController : BaseController
    {
        private ITransactionTypeService transactionTypeService;

        public TransactionTypeController(ITransactionTypeService objTransactionTypeService)
        {
            this.transactionTypeService = objTransactionTypeService;
        }

        [HttpGet]
        public ActionResult TransactionTypeList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTransactionTypes(string searchText)
        {
            int page = 1, rows = 10;

            List<TransactionTypeViewModel> transactionTypeList = new List<TransactionTypeViewModel>();
            transactionTypeList = transactionTypeService.GetTransactionType(searchText);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = transactionTypeList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = transactionTypeList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditTransactionType(byte? id)
        {
            var transactionType = transactionTypeService.GetTransactionTypeById(id);
            if (transactionType == null)
            {
                transactionType = new TransactionTypeViewModel();
            }
            return View(transactionType);
        }

        public ActionResult AddEditTransactionType(TransactionTypeViewModel transactionType)
        {
            if (ModelState.IsValid)
            {
                if (transactionType.RowKey == 0)
                {
                    transactionType = transactionTypeService.CreateTransactionType(transactionType);
                }
                else
                {
                    transactionType = transactionTypeService.UpdateTransactionType(transactionType);
                }

                if (transactionType.Message != AppConstants.Common.SUCCESS)
                {
                    ModelState.AddModelError("error_msg", transactionType.Message);
                }
                else
                {
                    return RedirectToAction("TransactionTypeList");
                }

                transactionType.Message = "";
                return View(transactionType);
            }

            transactionType.Message = EduSuiteUIResources.Failed;
            return View(transactionType);
        }

        [HttpPost]
        public ActionResult DeleteTransactionType(byte id)
        {
            TransactionTypeViewModel objViewModel = new TransactionTypeViewModel();

            objViewModel.RowKey = id;
            try
            {
                objViewModel = transactionTypeService.DeleteTransactionType(objViewModel);
            }
            catch (Exception)
            {
                objViewModel.Message = EduSuiteUIResources.Failed;
            }
            return Json(objViewModel);
        }
    }
}
