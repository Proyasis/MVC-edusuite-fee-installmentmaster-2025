using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
  public  interface ICountryService
    {

        List<CountryViewModel> GetCountries(string searchText);

        CountryViewModel GetCountryById(Int16? id);

        CountryViewModel CreateCountry(CountryViewModel model);

        CountryViewModel UpdateCountry(CountryViewModel model);

        CountryViewModel DeleteCountry(CountryViewModel model);


    }
}
