using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Common
{
    static public class CITSEduSuiteResourceManager
    {
        private static readonly ResourceManager ApplicationResourceManager;

        static CITSEduSuiteResourceManager()
        {
            ApplicationResourceManager = new ResourceManager("CITS.EduSuite.Business.Models.Resources.EduSuiteUIResources", Assembly.GetExecutingAssembly());
        }

        static public string GetApplicationString(string code)
        {
            return ApplicationResourceManager.GetString(code);
        }
    }
}
