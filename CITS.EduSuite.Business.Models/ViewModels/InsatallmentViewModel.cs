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
    public class InsatallmentViewModel
    {
        public InsatallmentViewModel()
        {

            IsActive = true;
            CourseTypeList = new List<SelectListModel>();
            CourseDurationTypeList = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            FeeTypes = new List<SelectListItem>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            ScheduleList = new List<InstallmentScheduleViewModel>();


        }

        public long RowKey { get; set; }

        public long SelectedFeeTypeKey { get; set; }

        public List<SelectListItem> FeeTypes { get; set; }
        public string CourseCode { get; set; }


        public string CourseName { get; set; }

     

        public bool IsActive { get; set; }

        public short BatchKey { get; set; }
        public string IsActiveText => IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No;


        public short CourseTypeKey { get; set; }
        public List<SelectListModel> Batches { get; set; }

        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        

        public int? CourseDuration { get; set; }

        public string CourseTypeName { get; set; }

public short SelectedBatchKey { get; set; }
        public long? InstallmentId { get; set; }

        public int SelectedAcademicTermKey { get; set; }
        public short SelectedCourseTypeKey { get; set; }
        public long SelectedCourseKey { get; set; }
        public long SelectedUniversityKey { get; set; }

        public decimal FeeAmount { get; set; }
        public decimal InitialPayment { get; set; }
        public decimal BalancePayment { get; set; }

      

        public List<SelectListModel> CourseTypeList { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public long? UniversityCourseKey { get; set; }

        public short? SearchCourseTypeKey { get; set; }

        public string SearchText { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SortBy { get; set; }

        public string SortOrder { get; set; }

      
        public int? DurationCount { get; set; }

        public List<SelectListModel> CourseDurationTypeList { get; set; }
        public List<InstallmentScheduleViewModel> ScheduleList { get; set; }
        public List<FeeDuration> FeeDurations { get; set; } = new List<FeeDuration>();
    }
    public class FeeDuration
    {
        public string YearLabel { get; set; }
        public decimal? RegistrationFee { get; set; }
        public decimal? AdmissionFee { get; set; }
        public decimal TuitionFee { get; set; }

        public string  FeeTypeKey { get; set; }
       

        public string BalancePayment { get; set; }

        public long SelectedFeeTypeKey { get; set; }
        public string IntialPayment { get; set; }
        public string FeeTypeName { get; set; }
    }

}