
﻿using CommandHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using WpfExplorer.ViewModel;
using System.Windows.Media.Imaging;
using WpfExplorer.Modules;

#pragma warning disable 0649

namespace WpfExplorer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Loaded += InitVM;
        }

        private void InitVM(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).ready_Tick();
            USBDetector.Detect_Click(sender, e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // JC Load Image to View
            string img_path = System.IO.Directory.GetCurrentDirectory();
            img_path = img_path.Replace(@"WpfExplorer\bin\Debug", @"img\play_new.png");
            play_btn.Source = new BitmapImage(new Uri(img_path));
            Detect_Click(sender, e);
        }

        private void lb_Exceptions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}