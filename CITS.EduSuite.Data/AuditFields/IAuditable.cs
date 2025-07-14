using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Data
{
    public interface IAuditable
    {
        DateTime DateAdded { get; set; }
        long AddedBy { get; set; }
        Nullable<DateTime> DateModified { get; set; }
        Nullable<long> ModifiedBy { get; set; }
    }
}
