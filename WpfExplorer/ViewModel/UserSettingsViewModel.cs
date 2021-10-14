using CommandHelper;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Input;
using WpfExplorer.View;
using System.Drawing;
using WpfExplorer.Model;

namespace WpfExplorer.ViewModel
{
    public class UserSettingsViewModel : INotifyPropertyChanged
    {
        
        public string windowTitle = "UserSettingsViewModel";
        public string WindowTitle { get => windowTitle; set => SetProperty(ref windowTitle, value); }

        

        public string messageTitle = "MessageTitle";
        public string MessageTitle { get => messageTitle; set => SetProperty(ref messageTitle, value); }


        public event PropertyChangedEventHandler PropertyChanged;
        public UserSettingsViewModel()
        {
            instance = this;
            //windowTitle = title;
            this.CloseWindowCommand = new RelayCommand<Window>(this.CloseWindow);
        }
        public static UserSettingsViewModel instance;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        private CommandHelper.RelayCommand oKButton;

        public ICommand OKButton
        {
            get
            {
                if (oKButton == null) oKButton = new CommandHelper.RelayCommand(UserSettingsModel.instance.PerformOKButton);
                return oKButton;
            }
        }

        public RelayCommand<Window> CloseWindowCommand { get; private set; }

        private void CloseWindow(Window window)
        {
            if (window != null) window.Close();
        }


        private string passwordText;

        public string PasswordText { get => passwordText; set => SetProperty(ref passwordText, value); }

        private bool recursiveCheck1 = false;

        public bool RecursiveCheck { get => recursiveCheck1; set => SetProperty(ref recursiveCheck1, value); }

        public bool darkModeCheck;
        public bool DarkModeCheck { get => darkModeCheck; set
                {
                    SetProperty(ref darkModeCheck, value);
                UserSettingsModel.instance.DarkModeCheck();
            }
        }

        public void checkOwnDarkMode()
        {

        }

        

        private RelayCommand<Window> importButton;

        public ICommand ImportButton
        {
            get
            {
                if (importButton == null)
                {
                    importButton = new RelayCommand<Window>(UserSettingsModel.instance.PerformImportButton);
                }

                return importButton;
            }
        }

        

        private RelayCommand<Window> exportButton;

        public ICommand ExportButton
        {
            get
            {
                if (exportButton == null)
                {
                    exportButton = new RelayCommand<Window>(UserSettingsModel.instance.PerformExportButton);
                }

                return exportButton;
            }
        }

        

        private System.Windows.Media.Brush color_Window;

        public System.Windows.Media.Brush Color_Window { get => color_Window; set => SetProperty(ref color_Window, value); }

        private System.Windows.Media.Brush color_MiddleBorder;

        public System.Windows.Media.Brush Color_MiddleBorder { get => color_MiddleBorder; set => SetProperty(ref color_MiddleBorder, value); }

        private System.Windows.Media.Brush color_MiddleBorderBrush;

        public System.Windows.Media.Brush Color_MiddleBorderBrush { get => color_MiddleBorderBrush; set => SetProperty(ref color_MiddleBorderBrush, value); }

        private System.Windows.Media.Brush color_CheckBox1Fore;

        public System.Windows.Media.Brush Color_CheckBox1Fore { get => color_CheckBox1Fore; set => SetProperty(ref color_CheckBox1Fore, value); }

        private System.Windows.Media.Brush color_CheckBox1Back;

        public System.Windows.Media.Brush Color_CheckBox1Back { get => color_CheckBox1Back; set => SetProperty(ref color_CheckBox1Back, value); }

        private System.Windows.Media.Brush color_UserSettingsFore;

        public System.Windows.Media.Brush Color_UserSettingsFore { get => color_UserSettingsFore; set => SetProperty(ref color_UserSettingsFore, value); }

        private System.Windows.Media.Color color_CheckBox1BackRev;

        public System.Windows.Media.Color Color_CheckBox1BackRev { get => color_CheckBox1BackRev; set => SetProperty(ref color_CheckBox1BackRev, value); }
    }
}
