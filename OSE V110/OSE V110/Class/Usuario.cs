using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSE_V110.View;

namespace OSE_V110.Class
{
    public class Usuario
    {
        public Me MUsuario = new Me();

        public struct Me
        {
            public string Usuario { get; set; }
            public string Nome { get; set; }
            public string Privilegio { get; set; }
            public string Estado { get; set; }
        }

        /// <summary>
        /// Saida de usuario - update fields Class Usuario
        /// </summary>
        public void Logout()
        {
            MUsuario.Usuario = null;
            MUsuario.Nome = null;
            MUsuario.Privilegio = null;
            MUsuario.Estado = null;
        }
    }
}
