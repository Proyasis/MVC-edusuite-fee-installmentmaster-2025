using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Common
{
    public static class Configurations
    {
        public static void GenerateReceipt(EduSuiteDatabase dbContext, ConfigurationViewModel model)
        {
            //var numberConfig = dbContext.GeneralConfigurations.FirstOrDefault();
            PaymentReceiptNumberConfiguration paymentReceiptConfig = new PaymentReceiptNumberConfiguration();

            if (model.OldConfigType != null)
            {
                PaymentReceiptNumberConfiguration OldPaymentReceiptConfig = new PaymentReceiptNumberConfiguration();
                if (dbContext.PaymentReceiptNumberConfigurations.Any(x => x.BranchKey == model.BranchKey && x.Type == model.ConfigType))
                {
                    OldPaymentReceiptConfig = dbContext.PaymentReceiptNumberConfigurations.SingleOrDefault(x => x.BranchKey == model.BranchKey && x.Type == model.OldConfigType);
                }
                else
                {
                    OldPaymentReceiptConfig = dbContext.PaymentReceiptNumberConfigurations.SingleOrDefault(x => x.Type == model.OldConfigType);
                }
                if (model.SerialNumber == OldPaymentReceiptConfig.MaxValue)
                {
                    OldPaymentReceiptConfig.MaxValue = OldPaymentReceiptConfig.MaxValue - 1;
                }
            }
            if (dbContext.PaymentReceiptNumberConfigurations.Any(x => x.BranchKey == model.BranchKey && x.Type == model.ConfigType))
            {
                paymentReceiptConfig = dbContext.PaymentReceiptNumberConfigurations.SingleOrDefault(x => x.BranchKey == model.BranchKey && x.Type == model.ConfigType);
            }
            else
            {
                paymentReceiptConfig = dbContext.PaymentReceiptNumberConfigurations.SingleOrDefault(x => x.Type == model.ConfigType);
            }
            if (model.IsDelete)
            {
                if (model.SerialNumber == paymentReceiptConfig.MaxValue)
                {
                    paymentReceiptConfig.MaxValue = paymentReceiptConfig.MaxValue - 1;
                }
            }
            else
            {
                model.SerialNumber = (paymentReceiptConfig.MaxValue ?? 0) + 1;
                paymentReceiptConfig.MaxValue = model.SerialNumber;
                var InvoiceNumber = ((paymentReceiptConfig.StartValue ?? 0) + model.SerialNumber);

                if (paymentReceiptConfig != null)
                {
                    model.ReceiptNumber = InvoiceNumber.ToString("D" + paymentReceiptConfig.LeadZeros.ToString());
                    model.ReceiptNumber = (paymentReceiptConfig.PrefixText ?? "") + model.ReceiptNumber + (paymentReceiptConfig.SufixText ?? "");
                }
            }
        }
    }
}
