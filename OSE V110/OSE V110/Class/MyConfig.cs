using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySql.Data.MySqlClient;
using OSE_V110.View;

namespace OSE_V110.Class
{
    /// <summary>
    /// Carrega configuracao do sistema
    /// </summary>
    public class MyConfig
    {
        public bool Carregar_Config()
        {
            var file = string.Concat(AppDomain.CurrentDomain.BaseDirectory,
                                    "Configuracao\\Mysql.Xml");

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
                        Janela.CoreMySql.CoreMe.Servidor = HashEncryp.Decodifica(s.InnerText);
                    }
                    s = n.SelectSingleNode("PORTA");
                    if (s != null)
                    {
                        Janela.CoreMySql.CoreMe.Porta = HashEncryp.Decodifica(s.InnerText);
                    }
                    s = n.SelectSingleNode("USUARIO");
                    if (s != null)
                    {
                        Janela.CoreMySql.CoreMe.Usuario = HashEncryp.Decodifica(s.InnerText);
                    }
                    s = n.SelectSingleNode("SENHA");
                    if (s != null)
                    {
                        Janela.CoreMySql.CoreMe.Senha = HashEncryp.Decodifica(s.InnerText);
                    }
                    s = n.SelectSingleNode("BANCO");
                    if (s != null)
                    {
                        Janela.CoreMySql.CoreMe.Banco = HashEncryp.Decodifica(s.InnerText);
                    }
                }
                return true;
            }
            catch (MySqlException e)
            {
                Erros.Output(e.ToString());
                return false;
            }
        }
    }
}
