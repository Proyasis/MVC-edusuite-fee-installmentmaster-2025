using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
   public interface IClassModeService
    {
        ClassModeViewModel GetClassModeById(int? id);
        ClassModeViewModel CreateClassMode(ClassModeViewModel model);
        ClassModeViewModel UpdateClassMode(ClassModeViewModel model);
        ClassModeViewModel DeleteClassMode(ClassModeViewModel model);
        List<ClassModeViewModel> GetClassMode(string searchText);
    }
}
