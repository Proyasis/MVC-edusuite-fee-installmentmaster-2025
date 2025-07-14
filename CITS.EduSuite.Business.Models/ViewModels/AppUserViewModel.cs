using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class AppUserViewModel
    {
        public long RowKey { get; set; }
        public string AppUserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }
        public string EmailAddress { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Image { get; set; }
        public string BloodGroup { get; set; }
        public byte[] Password { get; set; }
        public short RoleKey { get; set; }
        public bool IsActive { get; set; }
    }
}
