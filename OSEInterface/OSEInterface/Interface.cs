using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace OSEInterface
{
    /// <summary>
    /// Essa classe
    /// Carregar arquivo Configucarao [Configuraca\\Mysql.xml]
    /// Testa Servico MySql Disponivel
    /// Retorna o servidor MySql
    /// </summary>
    public class Interface
    {
        //public MyConfig Config = new MyConfig();
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
        public Me SMySql = new Me();
        internal MySqlConnection Connection;
        internal string ConnectionString;

        //public void Carregar_Config()
        //{
        //    Config.Carregar_Config();
        //}

        public bool Carregar_Config()
        {
            var file = string.Concat(AppDomain.CurrentDomain.BaseDirectory,
                                    "..\\Configuracao\\Mysql.Xml");

            try
            {
                if (!File.Exists(file))
                {
                    return false;
                }
                var xml = new XmlDocument();
                xml.Load(file);

                var list = xml.SelectNodes("/MYSQL");
                if (list == null)
                {
                    Console.WriteLine(@"Arquivo Local :[Configuracao\\Mysql.Xml]" + Environment.NewLine + @"Cod :1012");
                    return false;
                }
                foreach (XmlNode n in list)
                {
                    var s = n.SelectSingleNode("SERVIDOR");
                    if (s != null)
                    {

                        SMySql.Servidor = HashEncryp.Decodifica(s.InnerText);
                    }
                    s = n.SelectSingleNode("PORTA");
                    if (s != null)
                    {
                        SMySql.Porta = HashEncryp.Decodifica(s.InnerText);
                    }
                    s = n.SelectSingleNode("USUARIO");
                    if (s != null)
                    {
                        SMySql.Usuario = HashEncryp.Decodifica(s.InnerText);
                    }
                    s = n.SelectSingleNode("SENHA");
                    if (s != null)
                    {
                        SMySql.Senha = HashEncryp.Decodifica(s.InnerText);
                    }
                    s = n.SelectSingleNode("BANCO");
                    if (s != null)
                    {
                        SMySql.Banco = HashEncryp.Decodifica(s.InnerText);
                    }
                }
                return true;
            }
            catch (MySqlException e)
            {
                return false;
            }
        }    

        public bool IsConnectMySql()
        {
            if (SMySql.Servidor == string.Empty ||
                SMySql.Porta == string.Empty ||
                SMySql.Usuario == string.Empty ||
                SMySql.Senha == string.Empty ||
                SMySql.Banco == string.Empty)
            {
                return false;
            }
            SMySql.ConnectionString =
                 "SERVER=" + SMySql.Servidor +
                 ";PORT=" + SMySql.Porta +
                 ";DATABASE=" + SMySql.Banco +
                 ";UID=" + SMySql.Usuario +
                 ";PASSWORD=" + SMySql.Senha;

            Connection = new MySqlConnection(SMySql.ConnectionString);
            try
            {
                Connection.Open();
                SMySql.IsOnline = true;
                return true;
            }
            catch (MySqlException e)
            {
                SMySql.IsOnline = false;
                return false;
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

        public string MyServer
        {
            get { return SMySql.Servidor; }
        }
    }

    /// <summary>
    /// Essa class representa configuracao
    /// </summary>
    //public class MyConfig
    //{
    //    public Interface Interface = new Interface();
    //    public bool Carregar_Config()
    //    {
    //        var file = string.Concat(AppDomain.CurrentDomain.BaseDirectory,
    //                                "Configuracao\\Mysql.Xml");

    //        try
    //        {
    //            if (!File.Exists(file))
    //            {
    //                return false;
    //            }
    //            var xml = new XmlDocument();
    //            xml.Load(file);

    //            var list = xml.SelectNodes("/MYSQL");
    //            if (list == null)
    //            {
    //                Console.WriteLine(@"Arquivo Local :[Configuracao\\Mysql.Xml]" + Environment.NewLine + @"Cod :1012");
    //                return false;
    //            }
    //            foreach (XmlNode n in list)
    //            {
    //                var s = n.SelectSingleNode("SERVIDOR");
    //                if (s != null)
    //                {
                        
    //                    Interface.SMySql.Servidor = HashEncryp.Decodifica(s.InnerText);
    //                }
    //                s = n.SelectSingleNode("PORTA");
    //                if (s != null)
    //                {
    //                    Interface.SMySql.Porta = HashEncryp.Decodifica(s.InnerText);
    //                }
    //                s = n.SelectSingleNode("USUARIO");
    //                if (s != null)
    //                {
    //                    Interface.SMySql.Usuario = HashEncryp.Decodifica(s.InnerText);
    //                }
    //                s = n.SelectSingleNode("SENHA");
    //                if (s != null)
    //                {
    //                    Interface.SMySql.Senha = HashEncryp.Decodifica(s.InnerText);
    //                }
    //                s = n.SelectSingleNode("BANCO");
    //                if (s != null)
    //                {
    //                    Interface.SMySql.Banco = HashEncryp.Decodifica(s.InnerText);
    //                }
    //            }
    //            return true;
    //        }
    //        catch (MySqlException e)
    //        {
    //            return false;
    //        }
    //    }    
    //}
}

/*
    carregar arquivo configuracao/mysql.xml
    fazer teste conexao servico mysql 
    retorna o servidor para conexao mysql

*/