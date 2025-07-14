using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class AccountHeadViewModel:BaseModel
    {
        public AccountHeadViewModel()
        {
            AccountHeadType = new List<SelectListModel>();
            AccountGroup = new List<SelectListModel>();
            PaymentConfigs = new List<SelectListModel>();
            SearchAccountHeadType = new List<SelectListModel>();
            IsActive = true;
            IsSystemAccount = false;
            FutureAccountHeadKeys = new List<long>();
            HideDaily = false;
            HideFuture = false;
        }
        public long RowKey { get; set; }
        public long ExtraUpdateKey { get; set; }
        public long? oldCashFlowTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadNameExpressionErrorMessage")]
        public string AccountHeadName { get; set; }
        public string AccountHeadCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadTypeRequired")]
        public short AccountHeadTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountGroupRequired")]
        public byte AccountGroupKey { get; set; }
        public string AccountGroupName { get; set; }

        public int DisplayOrder { get; set; }
        public byte CashFlowTypeKey { get; set; }
        public decimal TotalDebitAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsSystemAccount { get; set; }
        public decimal? OldAmount { get; set; }
        public string AccountHeadTypeName { get; set; }
        public List<SelectListModel> AccountHeadType { get; set; }
        public List<SelectListModel> AccountGroup { get; set; }
        public List<SelectListModel> AccountHeads { get; set; }

        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public bool HideDaily { get; set; }
        public bool HideFuture { get; set; }
        public string FutureAccountHeads { get; set; }
        public List<long> FutureAccountHeadKeys { get; set; }

        public byte? PaymentConfigTypeKey { get; set; }
        public List<SelectListModel> PaymentConfigs { get; set; }

        public byte? SearchAccountGroupKey { get; set; }
        public short? SearchAccountHeadTypeKey { get; set; }
        public List<SelectListModel> SearchAccountHeadType { get; set; }


    }
}
