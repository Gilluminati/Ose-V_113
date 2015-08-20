using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using OSE_V110.View;

namespace OSE_V110.Class
{
    public class CoreMySql
    {
        public struct Me
        {
            public string Servidor { get; set; }
            public string Usuario { get; set; }
            public string Senha { get; set; }
            public string Banco { get; set; }
            public string Porta { get; set; }
            public string ConnectionString { get; set; }
            public bool IsOnline { get; set; }
        }

        public Me CoreMe = new Me();
        internal MySqlConnection Connection;
        internal string ConnectionString;

        #region Methods

        /// <summary>
        /// Funcao Verificar servico MySql Online
        /// </summary>
        /// <returns></returns>
        public bool IsConnectMySql()
        {
            if (Janela.CoreMySql.CoreMe.Servidor == string.Empty ||
                Janela.CoreMySql.CoreMe.Porta == string.Empty    ||
                Janela.CoreMySql.CoreMe.Usuario == string.Empty  ||
                Janela.CoreMySql.CoreMe.Senha == string.Empty    ||
                Janela.CoreMySql.CoreMe.Banco == string.Empty    )
            {
                return false;
            }
            Janela.CoreMySql.ConnectionString =
            "SERVER=" + Janela.CoreMySql.CoreMe.Servidor +
            ";PORT=" + Janela.CoreMySql.CoreMe.Porta +
            ";DATABASE=" + Janela.CoreMySql.CoreMe.Banco +
            ";UID=" + Janela.CoreMySql.CoreMe.Usuario +
            ";PASSWORD=" + Janela.CoreMySql.CoreMe.Senha;

            Janela.CoreMySql.Connection = new MySqlConnection(Janela.CoreMySql.ConnectionString);
            try
            {
                Janela.CoreMySql.Connection.Open();
                Janela.CoreMySql.CoreMe.IsOnline = true;
                return true;
            }
            catch (MySqlException e)
            {
                Janela.CoreMySql.CoreMe.IsOnline = false;
                return false;
            }
            finally
            {
                if (Janela.CoreMySql.Connection.State != ConnectionState.Closed)
                {
                    Janela.CoreMySql.Connection.Close();
                    Janela.CoreMySql.Connection.Dispose();
                }
            }
        }

        #endregion
    }
}
