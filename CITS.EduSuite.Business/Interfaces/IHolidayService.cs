using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IHolidayService
    {
        List<HolidayViewModel> GetHolidays(HolidayViewModel model);

        HolidayViewModel GetHolidayById(HolidayViewModel model);

        HolidayViewModel GetHolidayByDate(HolidayViewModel model);

        HolidayViewModel CreateHoliday(HolidayViewModel model);

        HolidayViewModel UpdateHoliday(HolidayViewModel model);

        HolidayViewModel DeleteHoliday(HolidayViewModel model);

        void FillBranches(HolidayViewModel model);

    }
}
