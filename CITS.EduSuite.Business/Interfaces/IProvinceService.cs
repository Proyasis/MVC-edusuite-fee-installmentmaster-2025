using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CITS.EduSuite.Business.Interfaces
{
 public interface IProvinceService
    {
        List<ProvinceViewModel> GetProvinces(string searchText);

        ProvinceViewModel GetProvinceById(int? id);

        ProvinceViewModel CreateProvince(ProvinceViewModel model);

        ProvinceViewModel UpdateProvince(ProvinceViewModel model);

        ProvinceViewModel DeleteProvince(ProvinceViewModel model);

    }
}
