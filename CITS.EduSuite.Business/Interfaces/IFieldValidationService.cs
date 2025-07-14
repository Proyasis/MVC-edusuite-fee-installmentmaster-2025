using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IFieldValidationService
    {
        FieldValidationViewModel GetFieldValidationById(short FieldType);
        FieldValidationViewModel UpdateFieldValidaion(FieldValidationViewModel model);
        List<FieldValidationViewModel> GetFieldValidation(string searchText);
    }
}
