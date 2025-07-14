using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;



namespace CITS.EduSuite.UI.Controllers
{
    public class LeadManagementController : ApiController
    {

        //private IEnquiryLeadService enquiryLeadService;
        //public LeadManagementController()
        //{
        //    this.enquiryLeadService = objEnquiryLeadService;
        //}



        // GET: LeadManagement
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get()
        {

            string JsonString = "{'email':'Proyasis@gmail.com','full_name':'Proyasis','phone_number':'+919288112244','city':'Kadakkal','Branch_Code':'PRO','Employee_Code':null,'Platform':'ig','AdId':'23846139962580450','AdName':'LGIAMT09','AdKey':29,'LeadKey':1062}";

            dynamic jsonObject = JsonConvert.DeserializeObject(JsonString);

            IEnquiryLeadService EnquiryLead = new EnquiryLeadService();

            List<EnquiryLeadViewModel> LeadModelList = new List<EnquiryLeadViewModel>();
            EnquiryLeadViewModel LeadModel = new EnquiryLeadViewModel();
            LeadModel.Name = jsonObject["full_name"];
            LeadModel.MobileNumber = jsonObject["phone_number"];
            LeadModel.MobileNumber = LeadModel.MobileNumber == null ? "" : Regex.Replace(LeadModel.MobileNumber, "[^0-9]", "").ToString();
            //LeadModel.BranchKey = jsonObject["branch_key"];
            //LeadModel.ServiceTypeKey = jsonObject["servicetype_key"];
            LeadModel.Location = jsonObject["city"];
            LeadModel.EmailAddress = jsonObject["email"];
            LeadModel.EmployeeCode = jsonObject["Employee_Code"];
            LeadModel.BranchCode = jsonObject["Branch_Code"];
            //LeadModel.ServiceTypeCode = jsonObject["ServiceType_Code"];
            LeadModel.LeadApiKey = jsonObject["LeadKey"];
            LeadModel.AdName = jsonObject["AdName"];
            LeadModel.platformName = jsonObject["Platform"];
            LeadModel.AdKey = jsonObject["AdKey"];
            EnquiryLead.GetLeadValues(LeadModel);
            if (LeadModel.EmployeeKey != null)
            {
                LeadModel.LeadDate = DateTimeUTC.Now.Date;
            }

            //LeadModel.TelephoneCodeKey = 54;

            if (LeadModel.platformName == DbConstants.FacebookPlatForm.FB)
            {
                LeadModel.NatureOfEnquiryKey = DbConstants.NatureOfEnquiry.Facebook;
            }
            else if (LeadModel.platformName == DbConstants.FacebookPlatForm.IG)
            {
                LeadModel.NatureOfEnquiryKey = DbConstants.NatureOfEnquiry.Instagram;
            }
            else
            {
                LeadModel.NatureOfEnquiryKey = DbConstants.NatureOfEnquiry.Facebook;
            }

            if (LeadModel.Name.Length > 100)
            {
                LeadModel.Name = LeadModel.Name.Substring(0, 100);
            }

            LeadModelList.Add(LeadModel);
            try
            {

                LeadModel = EnquiryLead.UpdateEnquiryLeads(LeadModelList);
                if (LeadModel.IsSuccessful)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }


           
        }

        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Post(dynamic jsonObject)
        {

            IEnquiryLeadService EnquiryLead = new EnquiryLeadService();
            List<EnquiryLeadViewModel> LeadModelList = new List<EnquiryLeadViewModel>();
            EnquiryLeadViewModel LeadModel = new EnquiryLeadViewModel();
            LeadModel.Name = jsonObject["full_name"];
            LeadModel.MobileNumber = jsonObject["phone_number"];
            LeadModel.MobileNumber = LeadModel.MobileNumber == null ? "":Regex.Replace(LeadModel.MobileNumber, "[^0-9]", "").ToString();  
            //LeadModel.BranchKey = jsonObject["branch_key"];
            //LeadModel.ServiceTypeKey = jsonObject["servicetype_key"];
            LeadModel.Location = jsonObject["city"]; 
            LeadModel.EmailAddress = jsonObject["email"];
            LeadModel.EmployeeCode = jsonObject["Employee_Code"];
            LeadModel.BranchCode = jsonObject["Branch_Code"];
            //LeadModel.ServiceTypeCode = jsonObject["ServiceType_Code"];
            LeadModel.LeadApiKey = jsonObject["LeadKey"];
            LeadModel.AdName = jsonObject["AdName"];
            LeadModel.platformName = jsonObject["Platform"];
            LeadModel.AdKey = jsonObject["AdKey"];
            EnquiryLead.GetLeadValues(LeadModel);
            if(LeadModel.EmployeeKey!=null)
            {
                LeadModel.LeadDate = DateTimeUTC.Now.Date;
            }

            //LeadModel.TelephoneCodeKey = 54;

            if (LeadModel.platformName == DbConstants.FacebookPlatForm.FB)
            {
                LeadModel.NatureOfEnquiryKey = DbConstants.NatureOfEnquiry.Facebook;
            }
            else if (LeadModel.platformName == DbConstants.FacebookPlatForm.IG)
            {
                LeadModel.NatureOfEnquiryKey = DbConstants.NatureOfEnquiry.Instagram;
            }
            else
            {
                LeadModel.NatureOfEnquiryKey = DbConstants.NatureOfEnquiry.Facebook;
            }

            if(LeadModel.Name.Length>100)
            {
                LeadModel.Name = LeadModel.Name.Substring(0,100);
            }

            LeadModelList.Add(LeadModel);
            try
            {
              
               LeadModel = EnquiryLead.UpdateEnquiryLeads(LeadModelList);
                if (LeadModel.IsSuccessful)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }
       
        }


    }
}