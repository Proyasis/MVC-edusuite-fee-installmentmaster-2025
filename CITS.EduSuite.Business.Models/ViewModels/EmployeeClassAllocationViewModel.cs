using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.ComponentModel.DataAnnotations;
using System.Web;
namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeClassAllocationViewModel : BaseModel
    {
        public EmployeeClassAllocationViewModel()
        {
            //TeacherClassAllocationModel = new List<TeacherClassAllocationModel>();  

            ClassDetails = new List<SelectListModel>();
            SubjectDetails = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
        }

        //public List<TeacherClassAllocationModel> TeacherClassAllocationModel { get; set; }       

        //public long? ClassDetailsKey { get; set; }
        //public long? TeacherClassAllocationKey { get; set; }


        public long RowKey { get; set; }
        public long EmployeesKey { get; set; }

        [System.Web.Mvc.Remote("CheckClassCodeExists", "EmployeeClassAllocation", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsKeyExists")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
        public long ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }
        public bool InCharge { get; set; }
        public bool IsAttendance { get; set; }
        public bool IsActive { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectRequired")]
        public List<long> SubjectKeys { get; set; }

        public List<SelectListModel> SubjectDetails { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
        public short? BatchKey { get; set; }
        public string BatchName { get; set; }
        public List<SelectListModel> Batches { get; set; }

        public List<string> SubjectNames { get; set; }

    }
    //public class TeacherClassAllocationModel
    //{
    //    public TeacherClassAllocationModel()
    //    {

    //        ClassDetails = new List<SelectListModel>();
    //        SubjectDetails = new List<SelectListModel>();
    //        Batches = new List<SelectListModel>();
    //    }


    //    public long RowKey { get; set; }
    //    public long EmployeeKey { get; set; }

    //    [System.Web.Mvc.Remote("CheckClassCodeExists", "EmployeeClassAllocation", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsKeyExists")]
    //    [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
    //    public long ClassDetailsKey { get; set; }
    //    public string ClassDetailsName { get; set; }
    //    public bool InCharge { get; set; }
    //    public bool IsAttendance { get; set; }
    //    public bool IsActive { get; set; }

    //    //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectRequired")]
    //    public List<long> SubjectKeys { get; set; }

    //    public List<SelectListModel> SubjectDetails { get; set; }
    //    public List<SelectListModel> ClassDetails { get; set; }

    //     [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
    //    public short? BatchKey { get; set; }
    //    public List<SelectListModel> Batches { get; set; }

    //}



}
