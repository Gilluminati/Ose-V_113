using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using OSEInterface;
using Color = System.Windows.Media.Brushes;

namespace MDM100
{
    public partial class MainWindow : MetroWindow
    {
        #region Declare
        public BackgroundWorker MySql = new BackgroundWorker();
        #endregion
        public Interface Interface = new Interface();
        public MainWindow()
        {
            InitializeComponent();
            ListView.AddHandler(Thumb.DragDeltaEvent,
                new DragDeltaEventHandler(Target),
                true);
        }

        #region ListView DragDeltaEvent
        private int ColunaTipo { get { return 150; } }
        private int ColunaModulo { get { return 150; } }
        private int ColunaDescricao { get { return 500; } }
        private int ColunaPrivilegio { get { return 151; } }
        private void Target(object sender, DragDeltaEventArgs e)
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
                            column.Column.Width = ColunaTipo;
                            break;
                        case @"Modulo":
                            column.Column.Width = ColunaModulo;
                            break;
                        case @"Descricao":
                            column.Column.Width = ColunaDescricao;
                            break;
                        case @"Privilegio":
                            column.Column.Width = ColunaPrivilegio;
                            break;
                    }
                }
            }
        }
        #endregion

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            Interface.Carregar_Config();
            MySql.DoWork += (o, args) =>
            {
                Interface.IsConnectMySql();
            };
            MySql.RunWorkerCompleted += (o, args) =>
            {
                if (!Interface.SMySql.IsOnline)
                {
                    // Offline
                    Label.Content = @"MDM100" + @" - Offline";
                    Label.Foreground = Color.Red;
                    return;
                }
                // Online
                Label.Content = @"MDM100" + @" - Online";
                Label.Foreground = Color.DodgerBlue;

            };
            MySql.RunWorkerAsync();
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
            }
        }

        private void CmdCadastra_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void ListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void RadFilTodos_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void RadApenasModulo_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void RadApenasAplic_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void RadModulo_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void RadDescricao_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void RadPrivilegio_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}
