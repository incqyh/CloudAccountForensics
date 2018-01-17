using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    public class Utility
    {
        public static void CantBeNull(object argument, string name)
        {
            if(argument is null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void PrintDataTable(DataTable table)
        {
            foreach(DataRow row in table.Rows)
            {
                foreach(DataColumn column in table.Columns)
                {
                    // Console.Write(row[column].GetType());
                    Console.Write(row[column]);
                    string tmp = row[column].ToString();
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}
