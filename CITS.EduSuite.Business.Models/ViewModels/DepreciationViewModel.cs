using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class DepreciationViewModel:BaseModel
    {
        public DepreciationViewModel()
        {
            DepreciationList = new List<DepreciationDetailsViewModel>();
        }
        public long RowKey { get; set; }
        public long AssetDetailsKey { get; set; }
        public decimal Depreciation { get; set; }
        public int Period { get; set; }
        public byte? PeriodTypeKey { get; set; }
        public byte DepreciationMethodKey { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public string AssetName { get; set; }
        public decimal? AccumulatedDepreciation { get; set; }
        public decimal? BookValue { get; set; }

        [RequiredIf("DepreciationMethodKey", DbConstants.DepreciationMethod.Unitsofproduction, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ProductionUnit", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? ProductionUnit { get; set; }
        public decimal? ProductionLimit { get; set; }
        public decimal? OldProduction { get; set; }
        public string PeriodName { get; set; }
        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public List<DepreciationDetailsViewModel> DepreciationList { get; set; }
    } 

    public class DepreciationDetailsViewModel
    {
        public long RowKey { get; set; }
        public long AssetDetailsKey { get; set; }
        public decimal Depreciation { get; set; }
        public int Period { get; set; }
        public DateTime PostDate { get; set; }
        public decimal? AccumulatedDepreciation { get; set; }
        public decimal? BookValue { get; set; }
        public string PeriodName { get; set; }
    }
}
