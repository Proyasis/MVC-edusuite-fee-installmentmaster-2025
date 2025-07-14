using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BaseSearchActivityLogViewModel 
    {
        public BaseSearchActivityLogViewModel()
        {
            UserIDs = new List<SelectListModel>();
            Menus = new List<SelectListModel>();
            Columns = new List<SelectListModel>();
        }
        public List<SelectListModel> UserIDs { get; set; }
        public List<SelectListModel> Menus { get; set; }
        public List<SelectListModel> Columns { get; set; }
        public string MenuKey { get; set; }

        public string UserIdKey { get; set; }
        public DateTime? DateAddedFrom { get; set; }
        public DateTime? DateAddedTo { get; set; }
        public DateTime? DateAdded { get; set; }
        public int? AddedBy { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
        public int? page { get; set; }
        public int? rows { get; set; }
        public int? draw { get; set; }
        //public int PageSize { get; set; }
        //public int PageIndex { get; set; }
        public long TotalRecords { get; set; }
        
        

    }
}
