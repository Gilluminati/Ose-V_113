using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSE_V110.Class
{
    /// <summary>
    /// Representa Versao do Core OSE
    /// </summary>
    public static class CoreVersion
    {
        /// <summary>
        /// Versao Core sistema
        /// </summary>
        public static string Versao
        {
            get { return @"113.D"; }
        }

        /// <summary>
        /// Chave validacao do sistema
        /// </summary>
        public static string Key { get; set; }
    }
}
