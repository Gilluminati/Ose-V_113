using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using MahApps.Metro.Controls.Dialogs;
using OSE_V110.Class;
using Color = System.Windows.Media.Brushes;
using Menu = OSE_V110.Class.Menu;

namespace OSE_V110.View
{
    public partial class Janela : MetroWindow
    {
        #region Declare
        public readonly DispatcherTimer Relogio = new DispatcherTimer();
        
        public static CoreMySql CoreMySql = new CoreMySql();
        public static MyConfig MyConfig = new MyConfig();
        public static Usuario Usuario = new Usuario();
        public static ArrayList ArrayList = new ArrayList();

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

        public static string Lastobj { get; set; }
        public static string MenuAnt { get; set; }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BoxMenu.Visibility = Visibility.Hidden;
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
                        CmdEntra.Visibility = Visibility.Collapsed;
                    }
                    else if (CoreMySql.CoreMe.IsOnline)
                    {
                        Erros.Output(@"Sucesso iniciar Servico MySql");
                        if (Usuario.MUsuario.Usuario == null)
                        {
                            CmdEntra.Visibility = Visibility.Visible;
                        }
                    }
                    
                    // Check - Usuario online - Servico Mysql Online
                    UpdateStatusBar(CoreMySql.CoreMe.IsOnline, CoreMySql.CoreMe.Servidor, Usuario.MUsuario.Usuario ?? null);
                    
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
        private async void CmdEntra_OnClick(object sender, RoutedEventArgs e)
        {
             var result = await _showLogin(@"Entrada De Usuario", @"Informe Usuario e Senha");
            if (result != null)
            {
                if (result.Username == string.Empty ||
                    result.Password == string.Empty)
                {
                    return;
                }
                if (!CoreMySql.UserInput(result.Username,
                    result.Password))
                {
                    await this.ShowMessageAsync(@"Falha", @"Usuario - Senha Inválidos");
                    return;
                }
                await this.ShowMessageAsync(@"Bem vindo", Usuario.MUsuario.Nome, MessageDialogStyle.Affirmative, null);
                CmdEntra.Visibility = Visibility.Collapsed;
                CmdSair.Visibility = Visibility.Visible;
                
                UpdateStatusBar(true,
                                CoreMySql.CoreMe.Servidor,
                                Usuario.MUsuario.Usuario);
                CoreMySql.LoadMenu(@"MNU000",Usuario.MUsuario.Privilegio);
                InicializarListView();
            }

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
            switch (e.Key)
            {
                case Key.Enter:
                Navige();    
                break;

                case Key.Back:
                Navige(true);    
                break;        
            }
        }

        private void ListViewMenu_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Navige();
        }

        /// <summary>
        /// Configucarao do sistema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdConfig_OnClick(object sender, RoutedEventArgs e)
        {
        }
        private async Task<LoginDialogData> _showLogin(string lTitle,
                                                      string lMessage)
        {
            var login = new LoginDialogSettings
            {
                ColorScheme = this.MetroDialogOptions.ColorScheme,
                UsernameWatermark = @"Usuario . . .",
                PasswordWatermark = @"Senha . . .",
                AffirmativeButtonText = @"Entrar",
                NegativeButtonVisibility = Visibility.Visible,
                NegativeButtonText = "Cancelar"
            };
            var result = await this.ShowLoginAsync(lTitle, lMessage, login);
            return result;
        }

        /// <summary>
        /// Inicializar ListViewMenu 
        /// </summary>
        internal void InicializarListView()
        {
            ListViewMenu.Visibility = Visibility.Visible;
            BoxMenu.Visibility = Visibility.Visible;
            Gridcolunas.AllowsColumnReorder = false;

            Gridcolunas.Columns[0].Width = _mrColunaTipo;
            Gridcolunas.Columns[1].Width = _mrColunaModulo;
            Gridcolunas.Columns[2].Width = _mrColunaDescricao;

            if (ArrayList.Count == 0) { return; }
            ListViewMenu.Items.Clear();
            foreach (var vitem in ArrayList.Cast<Menu>())
            {
                ListViewMenu.Items.Add(new Menu()
                {
                    Tipo = vitem.Tipo,
                    Modulo = vitem.Modulo,
                    Descricao = vitem.Descricao,
                    ModPai = vitem.ModPai
                });
            }
            /*ListViewMenu.Foreground = Color.DodgerBlue;*/
            ArrayList.Clear();
            ListViewMenu.Focus();    
        }

        /// <summary>
        /// Navige Menu Or isBack = true Retorna Menu
        /// </summary>
        /// <param name="isBack">If true NavigeBack</param>
        internal void Navige(bool isBack = false)
        {
            // NavigeToBack
            if (isBack)
            {

                if (Lastobj == @"MNU000")
                {
                    return;
                }
                CoreMySql.LoadMenu(CoreMySql.GetMenuFromLast(),
                                   Usuario.MUsuario.Privilegio);
                if (ListViewMenu.SelectedItem != null)
                {
                    //var item = ListViewMenu.SelectedItem as Menu ?? new Menu();
                    Lastobj = CoreMySql.GetMenuFromLast();
                    //MenuAnt = item.ModPai;                        
                }

                InicializarListView();
                return;
            }

            // Navige
            if (ListViewMenu.SelectedItem != null)
            {
                var item = ListViewMenu.SelectedItem as Menu ?? new Menu();

                // Checar Menu Or Prgrama
                if (item.Tipo == @"APLICACAO")
                {
                    // Chamar Aplicacao
                    CallAplicacaoExterna(item.Modulo);
                    return;
                }
                //if (item.Modulo != Lastobj)
                //{
                    CoreMySql.LoadMenu(item.Modulo, Usuario.MUsuario.Privilegio);
                    Lastobj = item.Modulo;
                    MenuAnt = item.ModPai;

                    InicializarListView();
                //}
            }
        }

        /// <summary>
        /// Chama aplicacao externa + parametro de inicializacao
        /// </summary>
        /// <param name="aplic"></param>
        private static void CallAplicacaoExterna(string aplic)
        {
            var program = AppDomain.CurrentDomain.BaseDirectory +
                          "\\Aplicacao\\" +
                          aplic + ".exe";
            if (!File.Exists(program)) { return; }
            var p = Process.Start(program, @"Odin");
            if (p != null) p.WaitForExit();
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (ListViewMenu.Visibility != Visibility.Visible)
            {
                return;
            }                                                               
            switch (e.Key)
            {
                //case Key.Enter:
                //    Navige();
                //    break;

                case Key.Back:
                    Navige(true);
                    break;

                case Key.Escape:

                    break;
            }
        }

        /// <summary>
        /// Logout usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdSair_OnClick(object sender, RoutedEventArgs e)
        {
            Usuario.Logout();
            ArrayList.Clear();

            CmdSair.Visibility = Visibility.Collapsed;
            ListViewMenu.Visibility = Visibility.Hidden;
            BoxMenu.Visibility = Visibility.Hidden;

            MySqlService.RunWorkerAsync();
        }

        /// <summary>
        /// ShowConsole - Mododebug - Desenvolvedor
        /// </summary>
        /// <param name="args"></param>
        internal void ModoDesenvolvedor(StartupEventArgs args)
        {
            var handle = GetConsoleWindow();
            bool i;
            var m = new Mutex(true,@"OSE",out i);
            ShowWindow(handle, i ? SwShow : SwHide);

            if (args.Args.Length == 1 && args.Args[0] == @"debug")
            {
                /*Console.Clear();*/
                ShowWindow(handle, SwShow);
                Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - -");
                Console.WriteLine(@"Modo Debug");
                Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - -");
            }
            else
            {
                ShowWindow(handle, SwHide);
            }
        }

        #region DllImport's

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SwHide = 0;
        const int SwShow = 5;

        #endregion
    }
}
