using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CITS.EduSuite.Business.Models.ViewModels;
using System.Net;
using System.IO;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Services;

namespace CITS.EduSuite.UI
{
    public static class SMSHelper
    {
        //public static void SendSMS(SMSViewModel model)
        //{
        //}
        public static string SendSMS(SMSViewModel model)
        {
            String Sendurl = string.Empty;
            try
            {
                ISMSHelperService notificationService = new SMSHelperService();
                //model.SMSReceiptants = "91" + model.SMSReceiptants;
                model = notificationService.GetSMSDetails(model);
                //***** commend for sms txt sms also sending  
                // model.URL = "http://dnd.saakshisoftware.com/api/mt/SendSMS?user={0}&password={1}&senderid={2}&channel=trans&DCS=0&flashsms=0&number={3}&text={4}&route={5}";
                //Sendurl = String.Format(strUrl, "Forcits", "654321", "RNITBP", model.SMSReceiptants, model.SMSContent);
                if (model != null)
                {
                    if (model.IsApi)
                    {
                        Sendurl = String.Format(model.URL, model.APICode, model.SenderID, model.SMSReceiptants, model.SMSContent, model.Root, model.SmsPEID, model.SMSTemplateID);

                    }
                    else
                    {
                        Sendurl = String.Format(model.URL, model.UserId, model.Password, model.SenderID, model.SMSReceiptants, model.SMSContent, model.Root);
                    }
                }

                return Gateway.SenSMS(Sendurl);
            }
            catch (Exception ex)
            {
                return "Send Failed";
            }

        }
    }
    public class Gateway
    {
        /// <summary>
        /// Send Sms
        /// </summary>
        /// <param name="Url">Url</param>
        /// <returns>Result</returns>
        public static string SenSMS(string Url)
        {

            try
            {

                HttpWebRequest Webreq = (HttpWebRequest)WebRequest.Create(Url);

                HttpWebResponse Response = (HttpWebResponse)Webreq.GetResponse();

                Stream Responsestream = Response.GetResponseStream();

                StreamReader streamReader = new StreamReader(Responsestream);

                string result = streamReader.ReadLine();

                Responsestream.Flush();

                Responsestream.Close();

                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}