using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MDM100.Class
{
    public static class OseFunctions
    {
        #region Declare
        private static MySqlConnection _connection;
        public static readonly ArrayList ArrayList = new ArrayList();
        public static List<string> Temp = new List<string>();
 
        public enum Pesquisar
        {
            Modulo,
            Descricao,
            Privilegio    
        }
        public enum Filtrar
        {
            Todos,
            Aplicacao,
            Modulo    
        }
        #endregion

        /// <summary>
        /// Retora Total de modulos (StoredProcedure)
        /// </summary>
        /// <param name="modulos">Total de modulos</param>
        /// <param name="menus">Menus</param>
        /// <param name="aplicacao">Aplicacoes></param>
        public static void ContarTotalModulos(out string modulos,
            out string menus,
            out string aplicacao)
        {
            try
            {
                using (_connection = new MySqlConnection(MainWindow.Interface.ConnectionString))
                {
                    _connection.Open();
                    using (var sql = new MySqlCommand(@"V113_MDM100_CONTAR_MODULOS", _connection))
                    {
                        sql.CommandType = CommandType.StoredProcedure;

                        sql.Parameters.Add(@"modulos", MySqlDbType.VarChar);
                        sql.Parameters[@"modulos"].Direction = ParameterDirection.Output;

                        sql.Parameters.Add(@"menu", MySqlDbType.VarChar);
                        sql.Parameters[@"menu"].Direction = ParameterDirection.Output;

                        sql.Parameters.Add(@"aplicacao", MySqlDbType.VarChar);
                        sql.Parameters[@"aplicacao"].Direction = ParameterDirection.Output;

                        sql.ExecuteNonQuery();

                        if (sql.Parameters[@"modulos"].Value.ToString() != string.Empty ||
                            sql.Parameters[@"menu"].Value.ToString() != string.Empty ||
                            sql.Parameters[@"aplicacao"].Value.ToString() != string.Empty)
                        {
                            modulos = (sql.Parameters[@"modulos"].Value.ToString());
                            menus = (sql.Parameters[@"menu"].Value.ToString());
                            aplicacao = (sql.Parameters[@"aplicacao"].Value.ToString());
                            return;
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            modulos = string.Empty;
            menus = string.Empty;
            aplicacao = string.Empty;
        }
        public static string LikeByText;

        /// <summary>
        /// funcao - carrega os menus 
        /// </summary>
        /// <param name="filtrar"></param>
        /// <param name="pesquisar"></param>
        public static void CarregarMenus(Filtrar filtrar,
                                         Pesquisar pesquisar)
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            try
            {
                ArrayList.Clear();
                using (_connection = new MySqlConnection(MainWindow.Interface.ConnectionString))
                {
                
                    _connection.Open();
                    var comandosql = @"SELECT `MOD.TIPO`," +
                                     "`MOD.NOME`," +
                                     "`MOD.DESCRICAO`," +
                                     "`MOD.PRIVILEGIO`" +
                                     "FROM   `v101_tab_modulo`";
                    switch (filtrar)
                    {
                            
                        case Filtrar.Todos:
                            if (LikeByText != string.Empty)
                            {
                                switch (pesquisar)
                                {
                                    case Pesquisar.Modulo:
                                        comandosql = string.Concat(comandosql,
                                        " WHERE `MOD.NOME` LIKE '%" + LikeByText + "%'");
                                        break;
                                    case Pesquisar.Descricao:
                                        comandosql = string.Concat(comandosql,
                                        " WHERE `MOD.DESCRICAO` LIKE '%" + LikeByText + "%'");
                                        break;
                                    case Pesquisar.Privilegio:
                                        comandosql = string.Concat(comandosql,
                                        " WHERE `MOD.PRIVILEGIO` LIKE '%" + LikeByText + "%'");
                                        break;
                                }
                            }
                            break;
                        case Filtrar.Aplicacao:
                            comandosql = string.Concat(comandosql,
                            " WHERE `MOD.TIPO` = 'APLICACAO'");
                            break;
                        case Filtrar.Modulo:
                            comandosql = string.Concat(comandosql,
                            " WHERE `MOD.TIPO` = 'MENU'");
                            break;
                    }
                    switch (pesquisar)
                    {
                       case Pesquisar.Modulo:
                            if (LikeByText != string.Empty && filtrar != Filtrar.Todos)
                            {
                                comandosql = string.Concat(comandosql,
                                " AND `MOD.NOME` LIKE '%" + LikeByText + "%'");
                            }
                            break;
                       case Pesquisar.Descricao:
                            if (LikeByText != string.Empty && filtrar != Filtrar.Todos)
                            {
                                comandosql = string.Concat(comandosql,
                                " AND `MOD.DESCRICAO` LIKE '%" + LikeByText + "%'");
                            }
                            break;
                       case Pesquisar.Privilegio:
                            if (LikeByText != string.Empty && filtrar != Filtrar.Todos)
                            {
                                comandosql = string.Concat(comandosql,
                                " AND `MOD.PRIVILEGIO` LIKE '%" + LikeByText + "%'");
                            }
                            break;
                    }
                    using (MySqlCommand command = new MySqlCommand(comandosql,_connection))
                    {
                        MySqlDataReader sqlDataReader = command.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            if (sqlDataReader.GetString(1) != @"MNU000")
                            {
                                var t = new UiMenu()
                                {
                                    Tipo = sqlDataReader.GetString(0),
                                    Modulo = sqlDataReader.GetString(1),
                                    Descricao = sqlDataReader.GetString(2),
                                    Privilegio = sqlDataReader.GetString(3)
                                };
                                ArrayList.Add(t);
                            }
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                }    
            }
        }
        /// <summary>
        /// retorna todos os modulos em lista
        /// </summary>
        /// <returns></returns>
        public static List<string> GetModulos()
        {
            Temp.Clear();
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            try
            {
                _connection.Open();

                var comando = @" SELECT `MOD.NOME`," +
                              " `MOD.DESCRICAO`" +
                              " FROM   `v101_tab_modulo` " +
                              " WHERE  `MOD.TIPO` = 'MENU'";

                using (MySqlCommand c = new MySqlCommand(comando,_connection))
                {
                    MySqlDataReader reader = c.ExecuteReader();
                    while (reader.Read())
                    {
                        Temp.Add(reader.GetString(0) + "   -   " + reader.GetString(1));
                    }
                }
                return Temp;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            return new List<string>(){};
        }
        /// <summary>
        /// retorna MOD.NOME
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static string GetNameFromModulo(string mod)
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            try
            {
                _connection.Open();
                var comando = @" SELECT `MOD.NOME`," +
                              " `MOD.DESCRICAO`" +
                              " FROM   `v101_tab_modulo` " +
                              " WHERE  `MOD.NOME` = '" + mod + "'";
                using (MySqlCommand c = new MySqlCommand(comando,_connection))
                {
                    MySqlDataReader reader = c.ExecuteReader();
                    while (reader.Read())
                    {
                        return reader.GetString(0) + "   -   " + reader.GetString(1);
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// retorna MOD.PAI
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        internal static string GetPaiFrom(string mod)
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            try
            {
                _connection.Open();
                using (MySqlCommand s = new MySqlCommand(@"v113_GET_MENU_PAI", _connection))
                {
                    s.CommandType = CommandType.StoredProcedure;

                    s.Parameters.AddWithValue(@"i_nome", mod);
                    s.Parameters[@"i_nome"].Direction = ParameterDirection.Input;

                    s.Parameters.Add(new MySqlParameter(@"o_pai", MySqlDbType.VarChar));
                    s.Parameters[@"o_pai"].Direction = ParameterDirection.Output;

                    s.ExecuteNonQuery();

                    if (s.Parameters[@"o_pai"].Value.ToString() != string.Empty)
                    {
                        return s.Parameters[@"o_pai"].Value.ToString();
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                }   
            }
            return string.Empty;
        }
        /// <summary>
        /// adciona novo Modulo (MySql)
        /// </summary>
        /// <param name="modTipo">MENU or APLICACAO (ENUM)</param>
        /// <param name="modNome">Nome do modulo</param>
        /// <param name="modDesc">Descricao sobre o modulo</param>
        /// <param name="modPriv">Privilegio para usuarios</param>
        /// <param name="modPai">dentro de qual modulo se encontrara o modulo</param>
        /// <param name="sucesso">retorna true se inserido com sucesso</param>
        public static void InsertNewModulo(string modTipo,
                                           string modNome,
                                           string modDesc,
                                           string modPriv,
                                           string modPai,
                                           out List<string> sucesso)
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            try
            {
                _connection.Open();
                var comandosql = @" INSERT INTO `v101_tab_modulo`" +
                                 " (`MOD.TIPO`," +
                                 " `MOD.NOME`," +
                                 " `MOD.DESCRICAO`," +
                                 " `MOD.PRIVILEGIO`," +
                                 " `MOD.PAI`)" +
                                 " VALUES ('" + modTipo + "'," +
                                 "'" + modNome + "'," +
                                 "'" + modDesc + "'," +
                                 "'" + modPriv + "'," +
                                 "'" + modPai + "')";
                using (MySqlCommand command = new MySqlCommand(comandosql,_connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
                // Falha
                switch (e.Number)
                {
                    case 1062:
                        sucesso = new List<string>
                        {
                            @"Falha" +
                            @"Chave duplicada"
                        };
                        return;
                }
                if (e.Number != 1062)
                {
                    sucesso = new List<string>
                    {
                        @"Falha"
                    };
                    return;
                }
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }

            // Inserido com sucesso
            sucesso = new List<string>
            {
            @"Sucesso"    
            };
        }
        /// <summary>
        /// Atualiza modulo existente
        /// </summary>
        /// <param name="modTipo">Tipo</param>
        /// <param name="modNome">Nome</param>
        /// <param name="modDesc">Descricao</param>
        /// <param name="modPriv">Privilegio</param>
        /// <param name="modPai">Pai</param>
        /// <param name="modUpdateThisModulo">O Modulo a ser atualizado</param>
        /// <param name="err">retorno do erro</param>
        public static void UpdateExisteModulo(string modTipo,
                                              string modNome,
                                              string modDesc,
                                              string modPriv,
                                              string modPai,
                                              string modUpdateThisModulo,
                                              out int err)
        {
            if(_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            try
            {
                _connection.Open();
                using (var sqlcomando = new MySqlCommand(@"v113_UPDATE_MODULO", _connection))
                {
                    sqlcomando.CommandType = CommandType.StoredProcedure;

                    sqlcomando.Parameters.AddWithValue(@"tipo", modTipo);
                    sqlcomando.Parameters[@"tipo"].Direction = ParameterDirection.Input;

                    sqlcomando.Parameters.AddWithValue(@"nome", modNome);
                    sqlcomando.Parameters[@"nome"].Direction = ParameterDirection.Input;

                    sqlcomando.Parameters.AddWithValue(@"descricao", modDesc);
                    sqlcomando.Parameters[@"descricao"].Direction = ParameterDirection.Input;

                    sqlcomando.Parameters.AddWithValue(@"privilegio", modPriv);
                    sqlcomando.Parameters[@"privilegio"].Direction = ParameterDirection.Input;

                    sqlcomando.Parameters.AddWithValue(@"pai", modPai);
                    sqlcomando.Parameters[@"pai"].Direction = ParameterDirection.Input;

                    sqlcomando.Parameters.AddWithValue(@"iswhere", modUpdateThisModulo);
                    sqlcomando.Parameters[@"iswhere"].Direction = ParameterDirection.Input;

                    sqlcomando.ExecuteNonQuery();
                }
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 1062: // -> [Err] 1062 - Duplicate entry 'NEW000' for key 'PRIMARY' 
                        Console.WriteLine(@"Chave Duplicada -");
                        err = 1062;
                        return;
                }
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                }    
            }
            err = 0;
        }
        /// <summary>
        /// Delete modulo 
        /// </summary>
        /// <param name="modNome">nome do modulo</param>
        public static void DeleteModulo(string modNome)
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            try
            {
                _connection.Open();
                using (var sqlcomando = new MySqlCommand(@"v113_DELETE_MODULO", _connection))
                {
                    sqlcomando.CommandType = CommandType.StoredProcedure;

                    sqlcomando.Parameters.AddWithValue(@"nome", modNome);
                    sqlcomando.Parameters[@"nome"].Direction = ParameterDirection.Input;

                    sqlcomando.ExecuteNonQuery();
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
