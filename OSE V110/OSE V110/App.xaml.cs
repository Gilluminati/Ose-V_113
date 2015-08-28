using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OSE_V110.Class;
using OSE_V110.View;

namespace OSE_V110
{
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var janela = new Janela()
            {
                Title = @"OSE ® Versao :" + CoreVersion.Versao, 
                ShowTitleBar = true,
                TitleCaps = false,
                GlowBrush = new SolidColorBrush(Colors.Black)
            };
            janela.Show();
            janela.ModoDesenvolvedor(e);
        }
    }
}
