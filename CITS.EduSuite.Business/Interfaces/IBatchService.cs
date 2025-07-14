using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
   public interface IBatchService
    {
        BatchViewModel GetBatchById(short? id);
        BatchViewModel CreateBatch(BatchViewModel model);
        BatchViewModel UpdateBatch(BatchViewModel model);
        BatchViewModel DeleteBatch(BatchViewModel model);
        List<BatchViewModel> GetBatch(string searchText);
        BatchViewModel CheckBatchCodeExist(string BatchCode,short Rowkey);

    }
}
