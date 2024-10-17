using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLibrary
{
    public class DBPropertyUtil
    {
        public static string GetConnectionString(string propertyFileName)
        {
            return "Data Source=AK\\SQLEXPRESS;Initial Catalog=CareerHub;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        }
    }

}
