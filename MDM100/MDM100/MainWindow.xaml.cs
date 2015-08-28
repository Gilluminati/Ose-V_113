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
using MahApps.Metro.Controls.Dialogs;
using MDM100.Class;
using MDM100.View;
using OSEInterface;
using Color = System.Windows.Media.Brushes;

namespace MDM100
{
    public partial class MainWindow : MetroWindow
    {
        #region Declare
        public BackgroundWorker MySql = new BackgroundWorker();
        public OseFunctions.Pesquisar Pesquisar;
        public OseFunctions.Filtrar Filtrar;

        #endregion
        public static Interface Interface = new Interface();
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

                // Contar total de modulos,menus,aplicacao
                string modulos, menus, aplicacao;
                OseFunctions.ContarTotalModulos(out modulos, out menus, out aplicacao);

                LblModCad.Content = modulos;
                LblMenuTotal.Content = menus;
                LblAplicTotal.Content = aplicacao;

                CallFilterAndLike();
                InicializarListView();

            };
            MySql.RunWorkerAsync();
        }

        void CallFilterAndLike()
        {
            //  LIKE
            if (RadModulo.IsChecked != null && RadModulo.IsChecked.Value)
            {
                Pesquisar = OseFunctions.Pesquisar.Modulo;
            }
            else if (RadDescricao.IsChecked != null && RadDescricao.IsChecked.Value)
            {
                Pesquisar = OseFunctions.Pesquisar.Descricao;
            }
            else if (RadPrivilegio.IsChecked != null && RadPrivilegio.IsChecked.Value)
            {
                Pesquisar = OseFunctions.Pesquisar.Privilegio;
            }

            // FILTRAR MENU OR APLICACAO
            if (RadFilTodos.IsChecked != null && RadFilTodos.IsChecked.Value)
            {
                Filtrar = OseFunctions.Filtrar.Todos;
            }

            if (RadApenasAplic.IsChecked != null && RadApenasAplic.IsChecked.Value)
            {
                Filtrar = OseFunctions.Filtrar.Aplicacao;
            }

            if (RadApenasModulo.IsChecked != null && RadApenasModulo.IsChecked.Value)
            {
                Filtrar = OseFunctions.Filtrar.Modulo;
            }
            OseFunctions.LikeByText = TextBox.Text;
            OseFunctions.CarregarMenus(Filtrar, Pesquisar);

            //GetAllModules()
            string modules, menus, aplicacoes;
            OseFunctions.ContarTotalModulos(out modules, out menus, out aplicacoes);
            LblModCad.Content = modules;
            LblMenuTotal.Content = menus;
            LblAplicTotal.Content = aplicacoes;
        }

        private void InicializarListView()
        {
            if (OseFunctions.ArrayList.Count == 0) { return; }
            ListView.Items.Clear();
            foreach (var item in OseFunctions.ArrayList.Cast<UiMenu>())
            {
                ListView.Items.Add(new UiMenu()
                {
                    Tipo = item.Tipo,
                    Modulo = item.Modulo,
                    Descricao = item.Descricao,
                    Privilegio = item.Privilegio
                });
            }
            OseFunctions.ArrayList.Clear();
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
            Manutencao manutencao = new Manutencao
            {
                ShowTitleBar = false,
                GlowBrush = new SolidColorBrush(Colors.DodgerBlue),
                //UiMenu = { Novo = true }
            };
            Manutencao.UiMenu.Novo = true;
            manutencao.InicializarComboBox(true);
            manutencao.ShowDialog();

            // Atualizar - manutencao.close()
            CallFilterAndLike();
            InicializarListView();

        }

        private void ListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowManutencaoFromItem();
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {

        }

        void ShowManutencaoFromItem()
        {
            if (ListView.SelectedItem == null)
            {
                return;
            }
            Manutencao manutencao = new Manutencao
            {
                ShowTitleBar = false,
                GlowBrush = new SolidColorBrush(Colors.DodgerBlue),
                //UiMenu = {Novo = false}
            };
            Manutencao.UiMenu.Novo = false;

            var item = ListView.SelectedItem as UiMenu ?? new UiMenu();

            manutencao.InicializarComboBox(false,OseFunctions.GetNameFromModulo(item.Modulo));
            Manutencao.UiMenu.Tipo = item.Tipo == @"APLICACAO" ? ManutencaoUiMenu.IsType.Aplicacao : ManutencaoUiMenu.IsType.Menu;
            Manutencao.UiMenu.Modulo = item.Modulo;
            Manutencao.UiMenu.Descricao = item.Descricao;
            Manutencao.UiMenu.Privilegio = item.Privilegio;
            Manutencao.UiMenu.ModuloPai = OseFunctions.GetPaiFrom(item.Modulo);

            manutencao.ShowDialog();

            // Atualizar - manutencao.close()
            CallFilterAndLike();
            InicializarListView();
        }

        #region Button - RadionButton - Events
        
        private void RadFilTodos_Checked(object sender, RoutedEventArgs e)
        {
            CallFilterAndLike();
            InicializarListView();
        }

        private void RadApenasModulo_Checked(object sender, RoutedEventArgs e)
        {
            CallFilterAndLike();
            InicializarListView();
        }

        private void RadApenasAplic_Checked(object sender, RoutedEventArgs e)
        {
            CallFilterAndLike();
            InicializarListView();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CallFilterAndLike();
            InicializarListView();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            CallFilterAndLike();
            InicializarListView();
        }

        private void RadModulo_Checked(object sender, RoutedEventArgs e)
        {
            CallFilterAndLike();
            InicializarListView();
        }

        private void RadDescricao_Checked(object sender, RoutedEventArgs e)
        {
            CallFilterAndLike();
            InicializarListView();
        }

        private void RadPrivilegio_Checked(object sender, RoutedEventArgs e)
        {
            CallFilterAndLike();
            InicializarListView();
        }
        #endregion

        private void ItemEditar_OnClick(object sender, RoutedEventArgs e)
        {
            ShowManutencaoFromItem();
        }

        private async void ItemExcluir_OnClick(object sender, RoutedEventArgs e)
        {
            var item = ListView.SelectedItem as UiMenu ?? new UiMenu();
            if (item.Modulo != null)
            {
                var result =await this.ShowMessageAsync(@"Exluir", @"Deseja realmente exluir modulo :" +
                item.Modulo, MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    OseFunctions.DeleteModulo(item.Modulo);
                    await this.ShowMessageAsync(@"Sucesso",
                    @"Modulo excluido com sucesso . . .");
                    //this.Close();

                    // Atualizar - manutencao.close()
                    CallFilterAndLike();
                    InicializarListView();
                }
            }
        }

        private void ItemAdcionar_OnClick(object sender, RoutedEventArgs e)
        {
            CmdCadastra_OnClick(null, null);
        }
    }
}
