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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using OSEInterface;
using Color = System.Windows.Media.Brushes;

namespace MDM900.View
{

    public partial class Janela : MetroWindow
    {
        #region Declare
        public static Interface Interface = new Interface();
        internal BackgroundWorker MySql = new BackgroundWorker();
        #endregion
        public Janela()
        {
            InitializeComponent();
            #region ListView Lock Column
            ListView.AddHandler(Thumb.DragDeltaEvent,
                                new DragDeltaEventHandler((sender, args) =>
                                {
                                    Thumb thumb = args.OriginalSource as Thumb;
                                    if (thumb != null)
                                    {
                                        GridViewColumnHeader column = thumb.TemplatedParent as GridViewColumnHeader;
                                        if (column != null)
                                        {
                                            switch (column.Content.ToString())
                                            {
                                                case @"Nome":
                                                    column.Column.Width = 200;
                                                    break;
                                                case @"Usuario":
                                                    column.Column.Width = 300;
                                                    break;
                                                case @"Privilegio":
                                                    column.Column.Width = 200;
                                                    break;
                                                case @"Estado":
                                                    column.Column.Width = 251;
                                                    break;

                                            }
                                        }
                                    }
                                }),true);
            #endregion
        }

        void PrintMyScreen()
        {
            try
            {
                PrintDialog d = new PrintDialog();

                if (d.ShowDialog() != true)
                {
                    return;
                }
                d.PrintVisual(this, @"this");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Falha de Captura", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Janela_OnLoaded(object sender, RoutedEventArgs e)
        {
            #region Carregar - OseInterface + Servico MySql

            Interface.Carregar_Config();
            
            // Delegate Events - BackGroundWorker
            MySql.DoWork += (o, args) =>
            {
                Interface.IsConnectMySql();
            };
            MySql.RunWorkerCompleted += (o, args) =>
            {
                // Offline
                if (!Interface.SMySql.IsOnline)
                {
                    Label.Content = @"MDM900" + @" - Offline";
                    Label.Foreground = Color.Red;
                    return;
                }
                // Online
                Label.Content = @"MDM900" + @" - Online";
                Label.Foreground = Color.DodgerBlue;

                // Inicializar Solucao
            };
            MySql.RunWorkerAsync();
            #endregion
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

        private void ItemImprimirTela_OnClick(object sender, RoutedEventArgs e)
        {
            PrintMyScreen();
        }

    }
}
