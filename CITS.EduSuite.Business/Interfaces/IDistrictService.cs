using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public  interface IDistrictService
    {
        List<DistrictViewModel> GetDistricts(string searchText);

        DistrictViewModel GetDistrictById(int? id);

        DistrictViewModel CreateDistrict(DistrictViewModel model);

        DistrictViewModel UpdateDistrict(DistrictViewModel model);

        DistrictViewModel DeleteDistrict(DistrictViewModel model);

        DistrictViewModel GetProvinceByCountry(short CountryKey);

    }
}
