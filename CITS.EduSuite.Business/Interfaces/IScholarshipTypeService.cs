using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
  public interface IScholarshipTypeService
    {
        ScholarshipTypeViewModel GetScholarshipTypeById(int? id);
        ScholarshipTypeViewModel CreateScholarshipType(ScholarshipTypeViewModel model);
        ScholarshipTypeViewModel UpdateScholarshipType(ScholarshipTypeViewModel model);
        ScholarshipTypeViewModel DeleteScholarshipType(ScholarshipTypeViewModel model);
        List<ScholarshipTypeViewModel> GetScholarshipType(string searchText);
        ScholarshipTypeViewModel CheckScholarshipTypeCodeExists(ScholarshipTypeViewModel model);
        ScholarshipTypeViewModel CheckScholarshipTypeNameExists(ScholarshipTypeViewModel model);
    }
}
