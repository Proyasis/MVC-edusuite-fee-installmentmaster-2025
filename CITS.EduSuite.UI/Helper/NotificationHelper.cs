using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Linq;


namespace CITS.EduSuite.UI
{
    public class NotificationHelper
    {
        private INotificationTemplateService notificationService = null;
        public NotificationHelper(INotificationTemplateService objNotificationService)
        {
            this.notificationService = objNotificationService;
        }

        public void SendNotificationInBackground(NotificationDataViewModel model)
        {

            Thread bgThread = new Thread(new ParameterizedThreadStart(SendNotification));

            bgThread.IsBackground = true;
            bgThread.Start(model);

        }
        public void SendMultipleNotificationInBackground(List<NotificationDataViewModel> model)
        {

            Thread bgThread = new Thread(new ParameterizedThreadStart(SendMultipleNotification));

            bgThread.IsBackground = true;
            bgThread.Start(model);

        }



        private void SendNotification(Object model)
        {
            try
            {
                hanldebarHelepers();
                List<NotificationDataViewModel> modelList = new List<NotificationDataViewModel>();
                NotificationDataViewModel objViewModel = (NotificationDataViewModel)model;

                bool IsSMS = objViewModel.AutoSMS ?? false;
                bool IsEmail = objViewModel.AutoEmail ?? false;
                string FilePath = objViewModel.EmailTemplateName;
               
                if (objViewModel.PushNotificationTemplateKey != 0)
                {
                    var objPushNotificationModel = notificationService.GetPushNotificationData(objViewModel);
                    if (objPushNotificationModel.PushNotificationContent != null && objPushNotificationModel.NotificationData != "" && objPushNotificationModel.NotificationData != null)
                    {
                        var jsonArray = JObject.Parse(objPushNotificationModel.NotificationData);
                        JsonSerializer jsonSerializer = new JsonSerializer();
                        ExpandoObject json = jsonSerializer.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));

                        if (objViewModel != null)
                        {
                            var template = Handlebars.Compile(objPushNotificationModel.PushNotificationContent);
                            var result = template(json);
                            byte[] utf8Bytes = Encoding.UTF8.GetBytes(result);
                            String EncodedResult = Encoding.UTF8.GetString(utf8Bytes);
                            objPushNotificationModel.PushNotificationContent = EncodedResult;
                           
                            NotificationHub.PushNotification(objPushNotificationModel);
                            objPushNotificationModel.NotificationTypeKey = DbConstants.NotificationType.Push;
                            modelList.Add(objPushNotificationModel);
                        }

                    }
                }
                if (IsEmail || IsSMS)
                {
                    objViewModel = notificationService.GetNotificationData(objViewModel);
                    var jsonArray = JObject.Parse(objViewModel.NotificationData);
                    JsonSerializer jsonSerializer = new JsonSerializer();
                    ExpandoObject json = jsonSerializer.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));

                    if (objViewModel != null)
                    {
                        FilePath = FilePath + objViewModel.EmailTemplateName;
                        if (System.IO.File.Exists(FilePath) && objViewModel.EmailAddess != null && IsEmail)
                        {
                            //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                            //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                            //var result = generator.Render(json);
                            var template = Handlebars.Compile(System.IO.File.ReadAllText(FilePath));
                            var result = template(json);
                            EmailViewModel emailModel = new EmailViewModel();
                            emailModel.EmailBody = result;
                            emailModel.EmailTo = objViewModel.EmailAddess;
                            emailModel.EmailCC = objViewModel.AdminEmailAddress;
                            emailModel.EmailSubject = objViewModel.EmailSubject;

                            EmailHelper.SendEmail(emailModel);


                            objViewModel.NotificationTypeKey = DbConstants.NotificationType.Email;
                            modelList.Add(objViewModel);
                        }
                        if (objViewModel.SMSTemplate != null && objViewModel.MobileNumber != null && IsSMS)
                        {
                            //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                            //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                            //var result = generator.Render(json);
                            var template = Handlebars.Compile(objViewModel.SMSTemplate);
                            var result = template(json);
                            SMSViewModel smsModel = new SMSViewModel();
                            smsModel.SMSContent = result;
                            smsModel.SMSReceiptants = objViewModel.MobileNumber;
                            smsModel.SMSTemplateID = objViewModel.SMSTemplateID;
                            SMSHelper.SendSMS(smsModel);
                            var objSMSViewModel = (NotificationDataViewModel)objViewModel;
                            objSMSViewModel.NotificationTypeKey = DbConstants.NotificationType.SMS;
                            objSMSViewModel.SMSTemplate = smsModel.SMSContent;
                            modelList.Add(objSMSViewModel);
                        }
                    }
                }
                notificationService.CreateNotification(modelList);
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog("NOTIF", "Notification", DbConstants.LogType.Error, DbConstants.User.UserKey, ex.GetBaseException().Message);
                Thread.CurrentThread.Abort();
            }
        }

        private void SendMultipleNotification(Object model)
        {
            try
            {
                List<NotificationDataViewModel> modelList = new List<NotificationDataViewModel>();
                List<NotificationDataViewModel> objViewModelList = (List<NotificationDataViewModel>)model;
                foreach (NotificationDataViewModel objViewModel in objViewModelList)
                {
                    bool IsSMS = objViewModel.AutoSMS ?? false;
                    bool IsEmail = objViewModel.AutoEmail ?? false;
                    string FilePath = objViewModel.EmailTemplateName;
                  
                    if (objViewModel.PushNotificationTemplateKey != 0)
                    {
                        var objPushNotificationModel = notificationService.GetPushNotificationData(objViewModel);
                        if (objPushNotificationModel.PushNotificationContent != null && objPushNotificationModel.NotificationData != null)
                        {
                            var jsonArray = JObject.Parse(objPushNotificationModel.NotificationData);
                            JsonSerializer jsonSerializer = new JsonSerializer();
                            ExpandoObject json = jsonSerializer.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));

                            if (objViewModel != null)
                            {
                                var template = Handlebars.Compile(objPushNotificationModel.PushNotificationContent);
                                var result = template(json);
                                byte[] utf8Bytes = Encoding.UTF8.GetBytes(result);
                                String EncodedResult = Encoding.UTF8.GetString(utf8Bytes);
                                objPushNotificationModel.PushNotificationContent = EncodedResult;
                              
                                NotificationHub.PushNotification(objPushNotificationModel);
                                objPushNotificationModel.NotificationTypeKey = DbConstants.NotificationType.Push;
                                modelList.Add(objPushNotificationModel);
                            }

                        }
                    }
                    if (IsEmail || IsSMS)
                    {
                        var objNotificationModel = notificationService.GetNotificationData(objViewModel);
                        var jsonArray = JObject.Parse(objViewModel.NotificationData);
                        JsonSerializer jsonSerializer = new JsonSerializer();
                        ExpandoObject json = jsonSerializer.Deserialize<ExpandoObject>(new JTokenReader(jsonArray));

                        if (objNotificationModel != null)
                        {
                            FilePath = FilePath + objNotificationModel.EmailTemplateName;
                            if (System.IO.File.Exists(FilePath) && objNotificationModel.EmailAddess != null && IsEmail)
                            {
                                //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                                //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                                //var result = generator.Render(json);
                                var template = Handlebars.Compile(System.IO.File.ReadAllText(FilePath));
                                var result = template(json);
                                EmailViewModel emailModel = new EmailViewModel();
                                emailModel.EmailBody = result;
                                emailModel.EmailTo = objNotificationModel.EmailAddess;
                                emailModel.EmailCC = objNotificationModel.AdminEmailAddress;
                                emailModel.EmailSubject = objNotificationModel.EmailSubject;

                                EmailHelper.SendEmail(emailModel);
                                objNotificationModel.NotificationTypeKey = DbConstants.NotificationType.Email;
                                modelList.Add(objNotificationModel);
                            }
                            if (objNotificationModel.SMSTemplate != null && objNotificationModel.MobileNumber != null && IsSMS)
                            {
                                //Mustache.FormatCompiler mCompiler = new Mustache.FormatCompiler();
                                //Mustache.Generator generator = mCompiler.Compile(System.IO.File.ReadAllText(FilePath));
                                //var result = generator.Render(json);
                                var template = Handlebars.Compile(objNotificationModel.SMSTemplate);
                                var result = template(json);
                                SMSViewModel smsModel = new SMSViewModel();
                                smsModel.SMSContent = result;
                                smsModel.SMSReceiptants = objNotificationModel.MobileNumber;
                                SMSHelper.SendSMS(smsModel);
                                var objSMSViewModel = (NotificationDataViewModel)objViewModel;
                                objSMSViewModel.NotificationTypeKey = DbConstants.NotificationType.SMS;
                                objSMSViewModel.SMSTemplate = smsModel.SMSContent;
                                modelList.Add(objSMSViewModel);
                            }
                        }
                    }
                }
                notificationService.CreateNotification(modelList);
            }
            catch (Exception ex)
            {
                Thread.CurrentThread.Abort();
            }
        }

        public void SendNotificationWithoutBackground(NotificationDataViewModel model)
        {
            SendNotification(model);

        }
        //private static dynamic _getExpandoFromXml(String file, XElement node = null)
        //{
        //    if (String.IsNullOrWhiteSpace(file) && node == null) return null;

        //    // If a file is not empty then load the xml and overwrite node with the
        //    // root element of the loaded document
        //    XmlDocument doc = new XmlDocument();
        //    node = !String.IsNullOrWhiteSpace(file) ? XDocument.Parse(file).Root : node;

        //    IDictionary<String, dynamic> result = new ExpandoObject();

        //    // implement fix as suggested by [ndinges]
        //    var pluralizationService = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(CultureInfo.CreateSpecificCulture("en-us"));

        //    // use parallel as we dont really care of the order of our properties
        //    node.Elements().AsParallel().ForAll(gn =>
        //    {
        //        // Determine if node is a collection container
        //        var isCollection = gn.HasElements &&
        //            (
        //            // if multiple child elements and all the node names are the same
        //                gn.Elements().Count() > 1 &&
        //                gn.Elements().All(
        //                    e => e.Name.LocalName.ToLower() == gn.Elements().First().Name.LocalName) ||

        //                // if there's only one child element then determine using the PluralizationService if
        //            // the pluralization of the child elements name matches the parent node. 
        //                gn.Name.LocalName.ToLower() == pluralizationService.Pluralize(
        //                    gn.Elements().First().Name.LocalName).ToLower()
        //            );

        //        // If the current node is a container node then we want to skip adding
        //        // the container node itself, but instead we load the children elements
        //        // of the current node. If the current node has child elements then load
        //        // those child elements recursively
        //        var items = isCollection ? gn.Elements().ToList() : new List<XElement>() { gn };

        //        var values = new List<dynamic>();

        //        // use parallel as we dont really care of the order of our properties
        //        // and it will help processing larger XMLs
        //        items.AsParallel().ForAll(i => values.Add((i.HasElements) ?
        //           _getExpandoFromXml(null, i) : i.Value.Trim()));

        //        // Add the object name + value or value collection to the dictionary
        //        result[gn.Name.LocalName] = isCollection ? values : values.FirstOrDefault();
        //    });
        //    return result;
        //}

        public void hanldebarHelepers()
        {
            Handlebars.RegisterHelper("dateformat", (output, context, arguments) =>
{
    string str = "";
    if (arguments[0] != null)
    {
        str = Convert.ToDateTime(arguments[0]).ToString("dd/MM/yyyy");
    }
    output.Write(str);
});


        }
    }

}