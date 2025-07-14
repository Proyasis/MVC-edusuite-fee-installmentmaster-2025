using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BulkEmailSmsViewModel:BaseModel
    {

        public BulkEmailSmsViewModel()
        {
            RowKeyList = new List<SelectListModel>();
            RoleKeyList = new List<SelectListModel>();
            EmailList = new List<SelectListModel>();
            SMSList = new List<SelectListModel>();

            //Searching  
            Branches = new List<SelectListModel>();
            Countries = new List<SelectListModel>();
            UserTypes = new List<SelectListModel>();
            InTakes = new List<SelectListModel>();
            ApplicationStatuses = new List<SelectListModel>();
            CounsellingEmployees = new List<SelectListModel>();

            AcademicTerms = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            UniversityMasters = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            CourseYears = new List<SelectListModel>();

            CourseTypeKeys = new List<long>();
            CourseKeys = new List<long>();
            UniversityMasterKeys = new List<long>();
            CourseYearsKeys = new List<long>();
            BatchKeys = new List<long>();
            BranchKeys = new List<long>();
        }



        public long RowKey { get; set; }
        public short RoleKey { get; set; }
        
        public int BranchKey { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public byte FilterTypeKey { get; set; }
        public bool IsSelected { get; set; }
        public long SelectedRowKey { get; set; }
        public long SelectedRoleKey { get; set; }
        public List<BulkEmailSmsViewModel> BulkEmailSMSList {get;set;}        

        public List<long>  RowKeys { get; set; }
        public List<short>   RoleKeys { get; set; }
        public List<string> Emails { get; set; }
        public List<string> SMS { get; set; }
        public List<SelectListModel> RowKeyList { get; set; }
        public List<SelectListModel> RoleKeyList { get; set; }
        public List<SelectListModel> EmailList { get; set; }
        public List<SelectListModel> SMSList { get; set; }


        //paging
        public string SortOrder { get; set; }
        public string SortBy { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        public long TotalRecords { get; set; }


        //Searching  
        public int SearchBranchKey{get;set;}
        public int SearchCountryKey{get;set;}
        public int SearchUserTypeKey{get;set;}
        public int SearchInTakeKey{get;set;}
        public int SearchApplicationStatusKey { get; set; }
        public int SearchCounsellingEmployeeKey { get; set; }    
        public string SearchText {get;set;}
    
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Countries { get; set; }
        public List<SelectListModel> UserTypes { get; set; }
        public List<SelectListModel> InTakes { get; set; }
        public List<SelectListModel> ApplicationStatuses { get; set; }
        public List<SelectListModel> CounsellingEmployees { get; set; }


        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> UniversityMasters { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> CourseYears { get; set; }
        public List<long> CourseTypeKeys { get; set; }
        public List<long> CourseKeys { get; set; }
        public List<long> UniversityMasterKeys { get; set; }
        public List<long> CourseYearsKeys { get; set; }
        public List<long> BatchKeys { get; set; }
        public List<long> BranchKeys { get; set; }
        public short? AcademicTermKey { get; set; }

        public string BatchName { get; set; }
        public string CourseName { get; set; }
        public string UniversityMasterName { get; set; }
        public string AcademicTermName { get; set; }
        public string CourseTypeName { get; set; }
        public string DesignationName { get; set; }
        public string CurrentYearText { get; set; }
    }



    public class SendBulkEmailViewModel : BaseModel
    {
         public long BulkEmailTrackKey { get; set; }
         [UIHint("tinymce_jquery_full"), AllowHtml]
         [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailBodyRequired")]        
         public string EmailContent { get; set; }
         [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailSubjectRequired")]        
         public string Subject { get; set; }
         public string EmailFileName { get; set; }
         public List<SendBulkEmailList> BulkEmailList { get; set; }
    }

    public class SendBulkEmailList
    {
        public short? RoleKey { get; set; }
        public long? RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressLengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressExpressionErrorMessage")]       
        public string EmailAddress { get; set; }         
    }



    public class SendBulkSMSViewModel : BaseModel
    {
        public long BulkSMSTrackKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SMSContentRequired")]
        public string SMSContent { get; set; }
        public List<SendBulkSMSList> BulkSMSList { get; set; }
    }


    public class SendBulkSMSList
    {
        public short? RoleKey { get; set; }
        public long? RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberRequired")]
        [RegularExpression(@"^[0-9]{12}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SMSMobileNumberLengthErrorMessage")]       
        public string MobileNumber { get; set; }
    }


}
