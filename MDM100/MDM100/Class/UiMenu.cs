using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM100.Class
{
    public class UiMenu
    {
        public string Tipo { get; set; }
        public string Modulo { get; set; }
        public string Descricao { get; set; }
        public string Privilegio { get; set; }
        public bool Novo { get; set; }
    }

    public class ManutencaoUiMenu
    {
        #region Manutencao
        public bool Novo { get; set; } // True - novo menu
        public string Modulo { get; set; }
        public IsType Tipo { get; set; }
        public string Descricao { get; set; }
        public string Privilegio { get; set; }
        public string ModuloPai { get; set; }


        public enum IsType
        {
            Menu,
            Aplicacao,
            Null
        }

        #endregion
    }
}
