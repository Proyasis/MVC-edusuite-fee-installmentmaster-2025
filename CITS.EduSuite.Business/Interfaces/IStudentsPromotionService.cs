using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IStudentsPromotionService
    {
        StudentsPromotionViewModel GetPromotionById(StudentsPromotionViewModel model);
        StudentsPromotionViewModel UpdatePromotion(StudentsPromotionViewModel model);
        StudentsPromotionViewModel CreatePromotion(StudentsPromotionViewModel model);
        List<StudentsPromotionViewModel> GetPromotion(StudentsPromotionViewModel model);      
        StudentsPromotionViewModel FillBatch(StudentsPromotionViewModel model);
        StudentsPromotionViewModel FillClassDetails(StudentsPromotionViewModel model);
        StudentsPromotionViewModel FillSearchClassDetails(StudentsPromotionViewModel model);
        StudentsPromotionViewModel GetSearchDropdownList(StudentsPromotionViewModel model);
        StudentsPromotionViewModel FillSearchBatch(StudentsPromotionViewModel model);
        StudentsPromotionViewModel ResetPromotion(long Id);
        StudentsPromotionViewModel FillPromotionDetailsViewModel(StudentsPromotionViewModel model);
        StudentsPromotionViewModel FillCourseType(StudentsPromotionViewModel model);
        StudentsPromotionViewModel DeletePromotion(long Id);
    }
}
