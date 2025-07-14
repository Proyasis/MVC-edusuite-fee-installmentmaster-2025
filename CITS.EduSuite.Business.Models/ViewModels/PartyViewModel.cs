using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class PartyViewModel : BaseModel
    {
        public PartyViewModel()
        {
            PartyTypes = new List<SelectListModel>();
            Statuses = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            IsActive = true;
            Parties = new List<SelectListModel>();
            States = new List<SelectListModel>();
            CashFlowTypes = new List<SelectListModel>();
            IsUpdateOpeningBalance = true;
            OtherPartyTypeKeys = new List<byte>();
            OtherPartyTypes = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public long? PartyKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameLengthErrorMessage")]
        //[RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PartyNameRegularExpressionErrorMessage")]
        public string PartyName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberRequired")]
        [StringLength(20, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        public string MobileNumber1 { get; set; }

        [StringLength(20, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        public string MobileNumber2 { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressRequired")]
        [StringLength(250, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,():&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressExpressionErrorMessage")]
        public string Address { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PartyTypeKeyRequired")]
        public byte? PartyTypeKey { get; set; }
        public string PartyTypeName { get; set; }
        public bool IsUpdateOpeningBalance { get; set; }
        public string CompanyBranchName { get; set; }

        [RequiredIfTrue("IsOpeningBalance", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "OpeningBalanceRequired")]
        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DecimalRegularExpressionErrorMessage")]
        public decimal? CreditBalance { get; set; }

        [RequiredIfTrue("IsOpeningBalance", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowTypeKeyRequired")]
        public byte? CashFlowTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LocationRequired")]
        public string LocationName { get; set; }
        
        public bool IsActive { get; set; }
        public string StatusName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonRegularExpressionErrorMessage")]
        public string ContactPerson { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DesignationLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DesignationRegularExpressionErrorMessage")]
        public string Designation { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressLengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressExpressionErrorMessage")]
        public string EmailId { get; set; }
        public bool IsOpeningBalance { get; set; }
        public bool IsTax { get; set; }

        [RequiredIfTrue("IsTax", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GSTINNumberRequired")]
        [RegularExpression(@"^[0-9]{2}[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9A-Za-z]{1}[Zz1-9A-Ja-j]{1}[0-9a-zA-Z]{1}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TinNumberExpressionErrorMessage")]
        //[RegularExpression(@"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TinNumberExpressionErrorMessage")]
        //[System.Web.Mvc.Remote("CheckGSTINNumberExists", "Party", AdditionalFields = "PartyTypeKey,RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GSTINNumberExistsErrorMessage")] //GSTIN Validation used
        public string GSTINNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StateRequired")]
        public int StateKey { get; set; }

        public List<SelectListModel> PartyTypes { get; set; }
        public List<SelectListModel> OtherPartyTypes { get; set; }
        public List<SelectListModel> CashFlowTypes { get; set; }
        public List<SelectListModel> Statuses { get; set; }

        public List<SelectListModel> Parties { get; set; }
        public List<SelectListModel> States { get; set; }

        public string StateName { get; set; }
        public List<byte> OtherPartyTypeKeys { get; set; }
        public string OtherPartyTypeNames { get; set; }
        public string CashFlowTypeName { get; set; }
        public int TotalItems { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalPaidAmount { get; set; }
        public decimal? TotalBalanceAmount
        {
            get
            {
                return (TotalAmount ?? 0) - (TotalPaidAmount ?? 0);
            }
            set { }
        }

        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public bool isBalanceDue { get; set; }
        public decimal? OldAdvanceBalance { get; set; }
        

        #region GetBranch
        public short? BranchKey { get; set; }
        public List<SelectListModel> Branches { get; set; }
        #endregion
    }
}
