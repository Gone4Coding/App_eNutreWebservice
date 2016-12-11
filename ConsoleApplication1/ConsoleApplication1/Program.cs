using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inser String: \n");
            string read = Console.ReadLine();

            List<string> fuckList = getValues(read);

            string name = fuckList[0];
            decimal value = Decimal.Parse(fuckList[1]);
            
            Console.WriteLine("NAME: " + name);
            Console.WriteLine("VALUE: " + value);

            Console.ReadLine();
        }

        private static List<string> getValues(string s)
        {
            List<string> stringList = new List<string>();
            
            string[] ss = s.Split('<', '>');

            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i].Trim().Equals("name"))
                {
                    int nIndex = i + 1;
                    stringList.Add(ss[nIndex]);
                }

                if (ss[i].Trim().Equals("value"))
                {
                    int nIndex = i + 1;
                    stringList.Add(ss[nIndex]);
                }
            }

            return stringList;
        }
    }
}
