using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSE_V110.Class
{
    public static class Erros
    {
        /// <summary>
        /// Simples erro
        /// </summary>
        /// <param name="err"></param>
        public static void Output(string err)
        {
            Console.Clear();
            Console.WriteLine(@"Hora:" + DateTime.Now.ToString("HH:mm"));
            Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - ");
            Console.WriteLine(err);
            Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - ");
        }
        /// <summary>
        /// Erro + StackTrace
        /// </summary>
        /// <param name="err"></param>
        /// <param name="from"></param>
        public static void Output(string err,
                                  string from)
        {
            Console.Clear();
            Console.WriteLine(@"Hora:" + DateTime.Now.ToString("HH:mm"));
            Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - ");
            Console.WriteLine(@"from:" + from);
            Console.WriteLine(err);
            Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - ");
        }
    }
}
