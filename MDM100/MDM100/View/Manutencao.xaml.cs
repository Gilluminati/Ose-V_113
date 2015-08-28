using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MDM100.Class;

namespace MDM100.View
{
    
    public partial class Manutencao : MetroWindow
    {
        public static ManutencaoUiMenu UiMenu = new ManutencaoUiMenu();

        public Manutencao()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //EstiloTextBlock(true);
            // Novo cadastro de menu
            if (UiMenu.Novo)
            {
                CmdDeletar.IsEnabled = false;
                UiMenu.Tipo = ManutencaoUiMenu.IsType.Null;
                UiMenu.Modulo = string.Empty;
                UiMenu.Descricao = string.Empty;
                UiMenu.Privilegio = string.Empty;
                EstiloTextBlock(true);

            }
            // Manutencao menu existente
            else
            {
                switch (UiMenu.Tipo)
                {
                    case ManutencaoUiMenu.IsType.Aplicacao:
                        RadAplic.IsChecked = true;
                        break;
                    case ManutencaoUiMenu.IsType.Menu:
                        RadMenu.IsChecked = true;
                        break;
                    case ManutencaoUiMenu.IsType.Null:
                        break;
                }
                TextBox.Text = UiMenu.Modulo;
                TextBoxDescricao.Text = UiMenu.Descricao;
                TextBoxPrivilegio.Text = UiMenu.Privilegio;
                LabelModuloId.Content = UiMenu.Modulo;
                EstiloTextBlock(false);
            }

        }

        public void InicializarComboBox(bool isDefault,
                                        string isMod=null)
        {
            if (!isDefault)
            {
                ComboBox.Items.Clear();
                if (ComboBox.Items.Count == 0)
                {
                    var result = OseFunctions.GetModulos();
                    foreach (var i in result)
                    {
                        ComboBox.Items.Add(i);
                    }
                }
                if (isMod != null)
                {
                    var result = OseFunctions.GetPaiFrom(isMod.Substring(0,6));
                    foreach (var i in ComboBox.Items)
                    {
                        if (result == i.ToString().Substring(0,6))
                        {
                            ComboBox.SelectedItem = i;
                        }
                    }
                }
                return;
            }

            if (ComboBox.Items.Count == 0)
            {
                ComboBox.Items.Clear();
                var getmods = OseFunctions.GetModulos();
                foreach (var i in getmods)
                {
                    ComboBox.Items.Add(i);
                }
            }
        }

        private void Manutencao_OnKeyDown(object sender, KeyEventArgs e)
        {
        }
        string _campoembranco = string.Empty;
        private string Tipo { get; set; }
        private string UpdateTipo { get; set; }
        private async void CmdConfirma_OnClick(object sender, RoutedEventArgs e)
        {
            if (RadMenu.IsChecked != null && !RadMenu.IsChecked.Value && RadAplic.IsChecked != null && !RadAplic.IsChecked.Value)
            {
                _campoembranco = string.Concat(_campoembranco, "Tipo \n");
               
            }
            /*EstiloTextBlock(true);*/
            if (TextBox.Text.Length == 0 || TextBox.Text.Length != 6)
            {
                _campoembranco = string.Concat(_campoembranco, "Modulo \n");
                /*BlockObrigatorioModulo.Visibility = Visibility.Visible;*/
            }
            if (TextBoxDescricao.Text.Length == 0)
            {
                _campoembranco = string.Concat(_campoembranco, "Descricao \n");
                /*BlockObrigatorioDescricao.Visibility = Visibility.Visible;*/
            }
            if (TextBoxPrivilegio.Text.Length == 0)
            {
                _campoembranco = string.Concat(_campoembranco, "Privilegio \n");
                /*BlockObrigatorioPrivilegio.Visibility = Visibility.Visible;*/
            }

            if (ComboBox.Text == string.Empty)
            {
                _campoembranco = string.Concat(_campoembranco, "Modulo Pai \n");
                /*BlockObrigatorioModuloPai.Visibility = Visibility.Visible;*/
            }
            if (_campoembranco != string.Empty)
            {
                /*MessageBox.Show(_campoembranco);*/
               await this.ShowMessageAsync(@"Campo(s) Obrigatorio(s)", _campoembranco , MessageDialogStyle.Affirmative, null);
                _campoembranco = string.Empty;
                return;
            }

            if (_campoembranco == string.Empty)
            {
                // Insert
                if (UiMenu.Novo)
                {
                    if (RadMenu.IsChecked != null && RadMenu.IsChecked.Value)
                    {
                        Tipo = @"MENU";
                    }
                    else if (RadAplic.IsChecked != null && RadAplic.IsChecked.Value)
                    {
                        Tipo = @"APLICACAO";
                    }
                    var modP = ComboBox.Text.Substring(0, 6);
                    List<string> sucesso;
                    OseFunctions.InsertNewModulo(Tipo,
                                                 TextBox.Text, 
                                                 TextBoxDescricao.Text, 
                                                 TextBoxPrivilegio.Text, 
                                                 modP, 
                                                 out sucesso);
                    foreach (var i in sucesso)
                    {
                        if (i == @"Sucesso")
                        {
                            await this.ShowMessageAsync(@"Sucesso", @"Criado com sucesso . . .");
                            this.Close();
                        }
                        else if (i == @"Falha" + @"Chave duplicada")
                        {
                            await this.ShowMessageAsync(@"Falha", @"Modulo já cadastrado !!!");
                            TextBox.Focus();
                        }
                        else if(i == @"Falha")
                        {
                            await this.ShowMessageAsync(@"Falha", @"Erro cadastrar novo modulo !!!");
                            TextBox.Focus();
                        }
                    }
                }
                // Update
                else
                {
                    // Compara os dados
                    if (RadMenu.IsChecked != null && RadMenu.IsChecked.Value)
                    {
                        Tipo = @"Menu";
                    }
                    else if (RadAplic.IsChecked != null && RadAplic.IsChecked.Value)
                    {
                        Tipo = @"Aplicacao";
                    }
                    if (UiMenu.Tipo.ToString() != Tipo ||
                        UiMenu.Modulo != TextBox.Text ||
                        UiMenu.Descricao != TextBoxDescricao.Text ||
                        UiMenu.Privilegio != TextBoxPrivilegio.Text ||
                        UiMenu.ModuloPai != ComboBox.Text.Substring(0, 6))
                    {
                        
                        switch (Tipo)
                        {
                            case @"Menu":
                               UpdateTipo = @"MENU";
                               break;
                            case @"Aplicacao":
                               UpdateTipo = @"APLICACAO";
                               break;
                        }
                        int err;
                        OseFunctions.UpdateExisteModulo(UpdateTipo,
                                             TextBox.Text,
                                             TextBoxDescricao.Text,
                                             TextBoxPrivilegio.Text,
                                             ComboBox.Text.Substring(0, 6),
                                             UiMenu.Modulo,
                                             out err);
                        switch (err)
                        {
                            case 1062: // -> [Err] 1062 - Duplicate entry 'NEW000' for key 'PRIMARY' 
                                /*MessageBox.Show(@"Chave Duplicada -");*/
                                await this.ShowMessageAsync(@"Falha", @"Modulo já existente !!!");
                                break;
                            case 0:
                                /*MessageBox.Show(@"Update");*/
                                await this.ShowMessageAsync(@"Sucesso", @"Modulo Atualizado com sucesso ...");
                                this.Close();
                                break;        
                        }
                    }
                }
            }
            _campoembranco = string.Empty;
        }

        private async void CmdDeletar_OnClick(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == @"MNU00") { return;}
            var result = await this.ShowMessageAsync(@"Exluir", @"Deseja realmente exluir modulo :" + TextBox.Text, MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                OseFunctions.DeleteModulo(TextBox.Text);
                await this.ShowMessageAsync(@"Sucesso", @"Modulo excluido com sucesso . . .");
                this.Close();    
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Block.Visibility = Visibility.Hidden;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text.Length > 0) { return; }
            Block.Visibility = Visibility.Visible;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (UiMenu.Novo)
            {
                LabelModuloId.Content = TextBox.Text.Length == 0 ? @"- - -" : TextBox.Text;
            }
        }

        private void TextBoxDescricao_GotFocus(object sender, RoutedEventArgs e)
        {
            BlockDescricao.Visibility = Visibility.Hidden;
        }

        private void TextBoxPrivilegio_GotFocus(object sender, RoutedEventArgs e)
        {
            BlockPrivilegio.Visibility = Visibility.Hidden;
        }

        private void TextBoxPrivilegio_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxPrivilegio.Text.Length == 0)
            {
                BlockPrivilegio.Visibility = Visibility.Visible;
            }
        }

        private void TextBoxDescricao_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxDescricao.Text.Length == 0)
            {
                BlockDescricao.Visibility = Visibility.Visible;
            }
        }

        public void EstiloTextBlock(bool isVisible)
        {
            if (!isVisible)
            {
                Block.Visibility = Visibility.Collapsed;
                BlockDescricao.Visibility = Visibility.Collapsed;
                BlockPrivilegio.Visibility = Visibility.Collapsed;
                return;
            }
            Block.Visibility = Visibility.Visible;
            BlockDescricao.Visibility = Visibility.Visible;
            BlockPrivilegio.Visibility = Visibility.Visible;
        }
    }
}
