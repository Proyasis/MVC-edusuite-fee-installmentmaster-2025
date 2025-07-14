using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IPrintInvoiceService
    {

        string GetPrintInvoice(long? RowKey, int PrintType, short? BranchKey, string ReceiptNumber);
    }
}
