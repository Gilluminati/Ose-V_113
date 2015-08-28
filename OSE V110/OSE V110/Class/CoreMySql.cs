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

        internal string TempSqlcomand;

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
                Erros.Output(e.ToString());
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

        /// <summary>
        /// Entrada de usuario
        /// </summary>
        /// <param name="user">usuario</param>
        /// <param name="pw">senha</param>
        /// <returns></returns>
        public bool UserInput(string user,
                              string pw)
          {
            pw = HashEncryp.Codifica(pw);
            if (!IsConnectMySql())
            {
                return false;
            }
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                Connection.Dispose();
            }

            try
            {
                Connection.Open();

                using (MySqlCommand s = new MySqlCommand(@"V101_ENTRADA_USUARIO", Connection))
                {
                    s.CommandType = CommandType.StoredProcedure;

                    s.Parameters.AddWithValue(@"p_USUARIO", user);
                    s.Parameters["@p_USUARIO"].Direction = ParameterDirection.Input;

                    s.Parameters.Add(new MySqlParameter(@"o_USUARIO", MySqlDbType.VarChar));
                    s.Parameters[@"o_USUARIO"].Direction = ParameterDirection.Output;

                    s.Parameters.Add(new MySqlParameter(@"o_PASSWORD", MySqlDbType.VarChar));
                    s.Parameters[@"o_PASSWORD"].Direction = ParameterDirection.Output;

                    s.Parameters.Add(new MySqlParameter(@"o_NOME", MySqlDbType.VarChar));
                    s.Parameters[@"o_NOME"].Direction = ParameterDirection.Output;

                    s.Parameters.Add(new MySqlParameter(@"o_PRIVILEGIO", MySqlDbType.VarChar));
                    s.Parameters[@"o_PRIVILEGIO"].Direction = ParameterDirection.Output;

                    s.Parameters.Add(new MySqlParameter(@"o_ESTADO", MySqlDbType.VarChar));
                    s.Parameters[@"o_ESTADO"].Direction = ParameterDirection.Output;

                    s.ExecuteNonQuery();

                    if (s.Parameters[@"p_USUARIO"].Value.ToString() != string.Empty)
                    {
                        // Comparacao - Senha
                        if (s.Parameters[@"o_PASSWORD"].Value.ToString() != pw)
                        {
                            //isConnect = false;
                            //myUser = null;
                            return false;
                        }
                        // Verificar Usuario Bloqueado 
                        if (s.Parameters[@"o_ESTADO"].Value.ToString() != "1")
                        {
                            //isConnect = false;
                            //myUser = @"BLOQUEADO";
                            return false;
                        }

                        // Usuario- Validado com sucesso
                        //if (s.Parameters[@"p_USUARIO"].Value.ToString() == @"bloqueado")
                        //{
                        //    Janela.Usuario.MUsuario.Usuario = s.Parameters[@"p_USUARIO"].Value.ToString() +
                        //                                      user;
                        //}
                        //else
                        //{
                            Janela.Usuario.MUsuario.Usuario = s.Parameters[@"p_USUARIO"].Value.ToString();
                        //}
                        Janela.Usuario.MUsuario.Nome = s.Parameters[@"o_NOME"].Value.ToString();
                        Janela.Usuario.MUsuario.Privilegio = s.Parameters[@"o_PRIVILEGIO"].Value.ToString();
                        Janela.Usuario.MUsuario.Estado = s.Parameters[@"o_ESTADO"].Value.ToString();
                        return true;
                    }
                }

            }
            catch (MySqlException e)
            {
                Erros.Output(e.ToString(),
                    @"public bool UserInput");
            }

            finally
            {
                if (Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                    Connection.Dispose();
                }    
            }
            return false;
        }

        /// <summary>
        /// LoadMenu - Carrega menu
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="priv"></param>
        public void LoadMenu(string nome,
            string priv)
        {
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                Connection.Dispose();
            }
            Janela.ArrayList.Clear();

            switch (priv.Count())
            {
                case 1:
                    TempSqlcomand = @" LIKE CONCAT('%','" + priv.Substring(0, 1) + "','%')";
                    break;
                case 2:
                    TempSqlcomand = @" LIKE CONCAT('%','" + priv.Substring(0, 1) + "','%')" +
                                     " OR `MOD.PAI` ='" + nome + "'" +
                                     " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(1, 1) + "','%')";
                    break;
                case 3:
                    TempSqlcomand = @" LIKE CONCAT('%','" + priv.Substring(0, 1) + "','%')" +
                                    " OR `MOD.PAI` ='" + nome + "'" +
                                    " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(1, 1) + "','%')" +

                                    " OR `MOD.PAI` ='" + nome + "'" +
                                    " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(2, 1) + "','%')";
                    break;
                case 4:
                    TempSqlcomand = @" LIKE CONCAT('%','" + priv.Substring(0, 1) + "','%')" +
                                    " OR `MOD.PAI` ='" + nome + "'" +
                                    " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(1, 1) + "','%')" +

                                    " OR `MOD.PAI` ='" + nome + "'" +
                                    " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(2, 1) + "','%')" +

                                    " OR `MOD.PAI` ='" + nome + "'" +
                                    " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(3, 1) + "','%')";
                    break;
                case 5:
                    TempSqlcomand = @" LIKE CONCAT('%','" + priv.Substring(0, 1) + "','%')" +
                                     " OR `MOD.PAI` ='" + nome + "'" +
                                     " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(1, 1) + "','%')" +

                                     " OR `MOD.PAI` ='" + nome + "'" +
                                     " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(2, 1) + "','%')" +

                                     " OR `MOD.PAI` ='" + nome + "'" +
                                     " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(3, 1) + "','%')" +

                                     " OR `MOD.PAI` ='" + nome + "'" +
                                     " AND `MOD.PRIVILEGIO` LIKE CONCAT('%','" + priv.Substring(4, 1) + "','%')";
                    break;        
            }

            try
            {
                Connection.Open();
                var commandMySql = @" SELECT `MOD.TIPO`," +
                                   "        `MOD.NOME`," +
                                   "        `MOD.DESCRICAO`," +
                                   "        `MOD.PAI`" +
                                   " FROM   `v101_tab_modulo`" +
                                   " WHERE  `MOD.PAI` ='" + nome + "'" +
                                   " AND    `MOD.PRIVILEGIO` " + TempSqlcomand;

                using (MySqlCommand command = new MySqlCommand(commandMySql, Connection))
                {
                    MySqlDataReader sqlDataReader = command.ExecuteReader();

                    while (sqlDataReader.Read())
                    {
                        if (sqlDataReader.GetString(1) != "MNU000")
                        {
                            var tempCmenu = new Menu()
                            {
                                Tipo = sqlDataReader.GetString(0),
                                Modulo = sqlDataReader.GetString(1),
                                Descricao = sqlDataReader.GetString(2),
                                ModPai = sqlDataReader.GetString(3)
                            };
                            Janela.ArrayList.Add(tempCmenu);
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                Erros.Output(e.ToString(),
                    @"public void LoadMenu");
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                    Connection.Dispose();
                }    
            }
        }

        public string GetMenuFromLast()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                Connection.Dispose();
            }

            try
            {
                Connection.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand(@"v113_GET_MENU_PAI",Connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.AddWithValue(@"i_nome", Janela.Lastobj);
                    sqlCommand.Parameters[@"i_nome"].Direction = ParameterDirection.Input;

                    sqlCommand.Parameters.Add(new MySqlParameter(@"o_pai", MySqlDbType.VarChar));
                    sqlCommand.Parameters[@"o_pai"].Direction = ParameterDirection.Output;

                    sqlCommand.ExecuteNonQuery();

                    if (sqlCommand.Parameters[@"o_pai"].Value.ToString() != string.Empty)
                    {
                        return sqlCommand.Parameters[@"o_pai"].Value.ToString();
                    }
                    else
                    {
                        return Janela.MenuAnt;
                    }
                }
            }
            catch (MySqlException e)
            {
                Erros.Output(e.ToString(),
                    @"public string GetMenuFromLast");
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                    Connection.Dispose();
                }    
            }
            return string.Empty;

        }
        #endregion
    }
}
