using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        #region LogUsuario
        /// <summary>
        /// Tipo de funcao Usuario requisitou
        /// </summary>
        public enum TipoFuncaoUsuario
        {
            Entrada,
            Saida,
            Falha,
            Bloqueado,
            Indefinido
        }
        /// <summary>
        /// Simples log de usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="servidor">Servidor</param>
        /// <param name="funcao">Funcao</param>
        public static void LogUsuario(string user,
                                      string servidor,
                                      TipoFuncaoUsuario funcao)
        {
            var path = @"%AppData%\Ose\Log\";
            path = Environment.ExpandEnvironmentVariables(path);
            if (!Directory.Exists(path))
            {
                // Criar a Pasta    
                Directory.CreateDirectory(path);
            }
            // Continua :
            var data = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            var time = DateTime.Now.ToString("HH:mm");
            string file = string.Concat(path,HashEncryp.Codifica(user));
            StreamWriter sw;
            StreamReader sr;
            if (!File.Exists(file + @".txt"))
            {
                using (sw = File.CreateText(file + @".txt"))
                {
                    sw.WriteLine(@"- - - - - - - - - - - - - - - - - - - -");
                    sw.WriteLine(@"Data :" + data);
                    sw.WriteLine(@"Hora :" + time);
                    sw.WriteLine(@"Usuario :" + user);
                    sw.WriteLine(@"Servidor :" + servidor);
                    sw.WriteLine(@"Funcao :" + funcao.ToString());
                    sw.WriteLine(@"- - - - - - - - - - - - - - - - - - - -");
                    sw.WriteLine();
                }
            }
            else
            {
                string s = @"- - - - - - - - - - - - - - - - - - - -" + Environment.NewLine +
                           @"Data :" + data + Environment.NewLine +
                           @"Hora :" + time + Environment.NewLine +
                           @"Usuario :" + user + Environment.NewLine +
                           @"Servidor :" + servidor + Environment.NewLine +
                           @"Funcao :" + funcao.ToString() + Environment.NewLine +
                           @"- - - - - - - - - - - - - - - - - - - -" + Environment.NewLine ; 
                using (StreamWriter tw = File.AppendText(file + @".txt"))
                {
                   tw.WriteLine(s);
                   tw.Close();
                }
            }
            Output(@"Sucesso - LogUsuario");
        } 
	    #endregion
        
    }
}
