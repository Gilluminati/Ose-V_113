using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using OSE_V110.Class;
using Color = System.Windows.Media.Brushes;


namespace OSE_V110.View
{
    public partial class Janela : MetroWindow
    {
        #region Declare
        public readonly DispatcherTimer Relogio = new DispatcherTimer();
        
        public static CoreMySql CoreMySql = new CoreMySql();
        public static MyConfig MyConfig = new MyConfig();
 
        public readonly BackgroundWorker MySqlService = new BackgroundWorker();
        #endregion
        public Janela()
        {
            InitializeComponent();
            // Delegate listview event
            ListViewMenu.AddHandler(Thumb.DragDeltaEvent,
                                new DragDeltaEventHandler(Thumb_DragDelta),
                                true);
           
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Relogio
            UpdateRelogio();
            Relogio.Interval = new TimeSpan(0, 0, 59);
            Relogio.Tick += (send, args) => UpdateRelogio();

            CmdSair.Visibility = Visibility.Collapsed;

            // Load Config
            if (!MyConfig.Carregar_Config())
            {
                Console.Write(@"Falha carregar arquivo configuracao");    
            }
            else
            {
                MySqlService.DoWork += (o, args) =>
                {
                    CoreMySql.IsConnectMySql();
                };
                MySqlService.RunWorkerCompleted += (o, args) =>
                {
                    if (!CoreMySql.CoreMe.IsOnline)
                    {
                        //Console.Write(@"Falha iniciar servico MySql"); 
                        Erros.Output(@"Falha iniciar Servico MySql",
                                     @"MetroWindow_Loaded");
                    }
                    else
                    {
                        Erros.Output(@"Sucesso iniciar Servico MySql");
                    }
                    UpdateStatusBar(CoreMySql.CoreMe.IsOnline,
                            CoreMySql.CoreMe.Servidor,
                            null);
                };
                MySqlService.RunWorkerAsync();
                //if (!CoreMySql.IsConnectMySql())
                //{
                //    Console.Write(@"Falha iniciar servico MySql");    
                //}
            }
            
        }

        /// <summary>
        /// Event atualiza Title + Hora + Data + Etc
        /// </summary>
        void UpdateRelogio()
        {
            var title = @"OSE ® Versao :" + CoreVersion.Versao;
            var data = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            var time = DateTime.Now.ToString("HH:mm");
            var culture = new System.Globalization.CultureInfo("pt-BR");
            var day = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
            Title = title + "       Hora: " + time + "   Data : " + data + "   Hoje : " + day;
        }

        /// <summary>
        /// Atualiza os campos da Janela
        /// </summary>
        /// <param name="conexao">Estado da conexao ONLINE - OFFLINE</param>
        /// <param name="servidor">Servidor conectado</param>
        /// <param name="usuario">Usuario conectado</param>
        internal void UpdateStatusBar(bool conexao,
                            string servidor,
                            string usuario)
        {
            LblConexaoStatus.Content = conexao ? "Online" : "Offline";
            LblConexaoStatus.Foreground = conexao ? Color.DeepSkyBlue : Color.Red;

            LblServidorStatus.Content = conexao ? servidor : "Offline";
            LblServidorStatus.Foreground = conexao ? Color.DeepSkyBlue : Color.Red;

            LblUsuarioStatus.Content = usuario ?? @"Desconectado(a)";
            LblUsuarioStatus.Foreground = usuario != null ? Color.DeepSkyBlue : Color.Silver;

            var myIpHost = IPAddress.None;
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                myIpHost = ip;
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    break;
            }
            //Captura de IP
            LblStatusMyIp.Content = myIpHost != null ? @"IP :" + myIpHost : "Ip : Falha";
            LblStatusMyIp.Foreground = myIpHost != null ? Color.DeepSkyBlue : Color.Red;

        }

        #region ListView _ Lock DragDelta
        private int _mrColunaTipo = 150;
        private int _mrColunaModulo = 150;
        private int _mrColunaDescricao = 673;
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = e.OriginalSource as Thumb;
            if (thumb != null)
            {
                GridViewColumnHeader column = thumb.TemplatedParent as GridViewColumnHeader;
                if (column != null)
                {
                    switch (column.Content.ToString())
                    {
                        case @"Tipo":
                            column.Column.Width = _mrColunaTipo;
                            break;
                        case @"Modulo":
                            column.Column.Width = _mrColunaModulo;
                            break;
                        case @"Descricao":
                            column.Column.Width = _mrColunaDescricao;
                            break;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Entrada de usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdEntra_OnClick(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Reset MySql Service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonMysql_Click(object sender, RoutedEventArgs e)
        {
            if (CoreMySql.ConnectionString != null)
            {
                MySqlService.RunWorkerAsync();
            }
        }

        private void ListViewMenu_OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void ListViewMenu_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Configucarao do sistema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdConfig_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
