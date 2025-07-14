using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.Security
{
    public class CITSEduSuitePrincipalData
    {
        public long UserKey { get; set; }
        public string Name { get; set; }
        public short? CompanyKey { get; set; }
        public string CompanyName { get; set; }
        public short? BranchKey { get; set; }
        public short RoleKey { get; set; }
        public string MACAddress { get; set; }
        public long? EmployeeKey { get; set; }
        public long? ApplicationKey { get; set; }
        public bool IsTeacher { get; set; }
        public string Photo { get; set; }
        public bool AllowAdmissionToAccoount { get; set; }
        public bool AllowSplitCostOfService { get; set; }
        public bool AllowCenterShare { get; set; }
        public bool AllowUniversityAccountHead { get; set; }
        public byte? EducationTypeKey { get; set; }
        public string CompanyLogo { get; set; }

    }
}
