﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MDM100
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // CaptureArgs
            if (!ValidaArgs(e))
            {
                Current.Shutdown();
                return;
            }
            // ----------------------------

            MainWindow wnd = new MainWindow()
            {
                ShowTitleBar = false,
                TitleCaps = false,
                GlowBrush = new SolidColorBrush(Colors.DodgerBlue)
            };
            wnd.Show();
        }

        #region CaptureArgs
        private bool ValidaArgs(StartupEventArgs e)
        {
            return e.Args.Length == 1 && e.Args[0] == @"Odin"
                ? true
                : false;
        }
        #endregion
    }
}
