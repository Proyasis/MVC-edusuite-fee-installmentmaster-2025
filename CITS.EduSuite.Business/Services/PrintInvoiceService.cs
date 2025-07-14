using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System.Data.SqlClient;

namespace CITS.EduSuite.Business.Services
{
    public class PrintInvoiceService : IPrintInvoiceService
    {
        private EduSuiteDatabase dbContext;
        public PrintInvoiceService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public string GetPrintInvoice(long? RowKey, int PrintType, short? BranchKey, string ReceiptNumber)
        {
            IEnumerable<string> results = dbContext.Database.SqlQuery<string>("exec spPrintInvoice @PrintType,@RowKey,@BranchKey,@ReceiptNumber",
                                                                                    new SqlParameter("PrintType", PrintType),
                                                                                    new SqlParameter("RowKey", RowKey ?? 0),
                                                                                    new SqlParameter("BranchKey", BranchKey ?? 0),
                                                                                    new SqlParameter("ReceiptNumber", ReceiptNumber ?? "")
                                                                                );


            return String.Join("", results);
        }
    }
}
