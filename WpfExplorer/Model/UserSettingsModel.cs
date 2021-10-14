using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.ViewModel;

namespace WpfExplorer.Model
{
    public class UserSettingsModel
    {
        public UserSettingsModel()
        {
            instance = this;
        }
        public static UserSettingsModel instance;
        public void DarkModeCheck()
        {
            MainWindow win = main.getSession<MainWindow>();


            //UserSettingsWindow win2 = main.getSession<UserSettingsWindow>();
            if (win != null)
            {
                var converter = new System.Windows.Media.BrushConverter();
                var win2 = MainWindowViewModel.instance;
                //RecursiveCheck = MainWindowViewModel.us.Recursive;
                //DarkModeCheck = MainWindowViewModel.us.DarkMode;
                if (UserSettingsViewModel.instance.darkModeCheck)
                {
                    win.Background = (System.Windows.Media.Brush)converter.ConvertFromString("#252525");
                    win2.Color_ExceptionLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_FileExceptionList = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_SuchFeldLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_tb_Ping = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_FoundFiles = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_tb_AddExceptions = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_tb_Search = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");

                    UserSettingsViewModel.instance.Color_UserSettingsFore = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    UserSettingsViewModel.instance.Color_CheckBox1Fore = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    UserSettingsViewModel.instance.Color_CheckBox1Back = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    UserSettingsViewModel.instance.Color_MiddleBorder = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    UserSettingsViewModel.instance.Color_MiddleBorderBrush = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    UserSettingsViewModel.instance.Color_Window = (System.Windows.Media.Brush)converter.ConvertFromString("#252525");
                }
                else
                {
                    win.Background = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_ExceptionLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win2.Color_FileExceptionList = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win2.Color_SuchFeldLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win2.Color_tb_Ping = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win2.Color_FoundFiles = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win2.Color_tb_AddExceptions = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win2.Color_tb_Search = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");


                    UserSettingsViewModel.instance.Color_UserSettingsFore = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    UserSettingsViewModel.instance.Color_CheckBox1Fore = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    UserSettingsViewModel.instance.Color_CheckBox1Back = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    UserSettingsViewModel.instance.Color_MiddleBorder = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    UserSettingsViewModel.instance.Color_MiddleBorderBrush = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    UserSettingsViewModel.instance.Color_Window = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                }
            }
            fs.C_UC conf = db.getConf<fs.C_UC>("usersettings");
            conf.DarkMode = UserSettingsViewModel.instance.darkModeCheck;
            db.setConf("usersettings", conf);
        }

        public void PerformImportButton(object commandParameter)
        {
            var path = new OpenFileDialog();
            path.ShowDialog();
            fs.import(path.FileName);
        }
        public void PerformExportButton(object commandParameter)
        {
            fs.export();
        }

        public void PerformOKButton(object commandParameter)
        {
            fs.exportPassword = UserSettingsViewModel.instance.PasswordText;
            UserSettingsViewModel.instance.CloseWindowCommand.Execute(commandParameter);

        }
    }
}
