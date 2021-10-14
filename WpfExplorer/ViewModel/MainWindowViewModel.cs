using CommandHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RelayCommand = CommandHelper.RelayCommand;
using Newtonsoft.Json;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using WpfExplorer.Model;
using WpfExplorer.View;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace WpfExplorer.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            fs.checkConfig();

            //fs.checkUserSettings();

            main.AddLog("initialized", main.status.log);
            
            tb_Ping_Text = "Connecting to Database...";
            ButtonCommand = new RelayCommand(o => MainModel.instance.Debug_Click());
            Index_Click = new RelayCommand(o => MainModel.instance.Indiziere());
            tb_Search_Command = new RelayCommand(o => MainModel.instance.tb_Search_TextChanged());
            MouseDoubleClick = new RelayCommand(o => MainModel.instance.OpenFileInExplorer(o));
            //MyCommand = new RelayCommand(o => My(o));
            instance = this;
            if (main.PingDB()) tb_Ping_Text = "Connected";
            else
            {
                tb_Ping_Text = "Connection failed...";
                main.ReportError(new Exception(), main.status.error, "Die Datenbank ist nicht erreichbar.Stellen Sie sicher, dass Sie den Port 3306 von ryukyun.de erreichen können");
                return;
            }

            MainModel.allDrives = DriveInfo.GetDrives();
        }

        public static MainWindowViewModel instance;

        
        

        public void sync()
        {
            
            db.pull();
            db.push();
        }

        public ICommand KeyInputCommand
        {
            get
            {
                if (_keyInputCommand == null) _keyInputCommand = new GalaSoft.MvvmLight.Command.RelayCommand(KeyDown);
                return _keyInputCommand;
            }
        }

        private ICommand _deleteFileException;

        public ICommand DeleteFileException
        {
            get
            {
                if (_deleteFileException == null) _deleteFileException = new RelayCommand(DeleteFromExceptionList);
                return _deleteFileException;
            }
        }

        public void DeleteFromExceptionList(object commandParamter)
        {
            FileExceptionList.Remove(commandParamter.ToString());
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(FileExceptionList)));
        }

        public List<string> getFileExceptionList() => FileExceptionList.ToList<string>();

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private void KeyDown()
        {
            // Hier Logik
        }

        private ICommand _keyInputCommand;

        private string _name;

        private RelayCommand enterKeyCommand;

        public ICommand EnterKeyCommand
        {
            get
            {
                if (enterKeyCommand == null)
                {
                    enterKeyCommand = new RelayCommand(MainModel.instance.EnterKey);
                }

                return enterKeyCommand;
            }
        }

        

        

        public void searchContext()
        {

        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, newValue)) return false;

            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        

        

        public ICommand ButtonCommand { get; set; }
        public ICommand tb_Search_Command { get; set; }
        public ICommand MouseDoubleClick { get; set; }

        public ICommand Index_Click { get; set; }

        //public ICommand MyCommand { get; set; }

        

        public string tb_AddExceptionsText { get; set; } = null;

        public string _tb_Search_Text = "";
        public string tb_Search_Text
        {
            get { return _tb_Search_Text; }
            set
            {
                if (!_tb_Search_Text.Equals(value))
                {
                    _tb_Search_Text = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("tb_Search_Text"));
                        MainModel.instance.tb_Search_TextChanged();
                    }
                }

            }
        }

        public string _tb_Ping_Text = "";
        public string tb_Ping_Text
        {
            get { return _tb_Ping_Text; }
            set
            {
                if (!_tb_Ping_Text.Equals(value))
                {
                    _tb_Ping_Text = value;
                    if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs("tb_Ping_Text"));
                }
            }
        }






        public ObservableCollection<string> FileExceptionList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> FoundFiles { get; set; } = new ObservableCollection<string>();
        private object selectedFileException;

        public object SelectedFileException { get => selectedFileException; set => SetProperty(ref selectedFileException, value); }

        public Model.FileStructure selectedFile;

        public Model.FileStructure SelectedFile
        {
            get { return selectedFile; }
            set
            {
                //if (value == selectedFile) return;
                selectedFile = value;
                PropertyChanged(this, new PropertyChangedEventArgs("selectedFile"));
            }
        }

        

        private RelayCommand _bt_Help1;

        public ICommand _bt_Help
        {
            get
            {
                if (_bt_Help1 == null)
                {
                    _bt_Help1 = new RelayCommand(MainModel.instance.Perform_bt_Help);
                }

                return _bt_Help1;
            }
        }

        


        public string _PATH = "";
        public List<string> ExcList = new List<string>();

        ICommand _addToExceptList;

        public ICommand AddToExceptionList
        {
            get
            {
                if (_addToExceptList == null) _addToExceptList = new RelayCommand(e => MainModel.instance.ToExceptionList());
                return _addToExceptList;
            }
        }

        private RelayCommand settings_Click;

        public ICommand Settings_Click
        {
            get
            {
                if (settings_Click == null)
                {
                    settings_Click = new RelayCommand(MainModel.instance.PerformSettings_Click);
                }

                return settings_Click;
            }
        }

        public ICommand Log_Click
        {
            get
            {
                if (settings_Click == null)
                {
                    settings_Click = new RelayCommand(MainModel.instance.PerformLogs_Click);
                }

                return settings_Click;
            }
        }


        

        private string fileProgress;

        public string FileProgress
        {
            get => fileProgress;
            set
            {
                if (value == fileProgress) return;
                fileProgress = value;
                this?.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FileProgress)));
            }
        }


        private System.Windows.Media.Brush color_ExceptionLabel;

        public System.Windows.Media.Brush Color_ExceptionLabel { get => color_ExceptionLabel; set
            {
                SetProperty(ref color_ExceptionLabel, value);
            }
        }

        private System.Windows.Media.Brush color_SuchFeldLabel;

        public System.Windows.Media.Brush Color_SuchFeldLabel { get => color_SuchFeldLabel; set => SetProperty(ref color_SuchFeldLabel, value); }

        private System.Windows.Media.Brush color_FileExceptionList;

        public System.Windows.Media.Brush Color_FileExceptionList { get => color_FileExceptionList; set => SetProperty(ref color_FileExceptionList, value); }

        private System.Windows.Media.Brush color_tb_Ping;

        public System.Windows.Media.Brush Color_tb_Ping { get => color_tb_Ping; set => SetProperty(ref color_tb_Ping, value); }

        private System.Windows.Media.Brush color_tb_Search;

        public System.Windows.Media.Brush Color_tb_Search { get => color_tb_Search; set => SetProperty(ref color_tb_Search, value); }

        private System.Windows.Media.Brush color_tb_AddExceptions;

        public System.Windows.Media.Brush Color_tb_AddExceptions { get => color_tb_AddExceptions; set => SetProperty(ref color_tb_AddExceptions, value); }

        private System.Windows.Media.Brush color_FoundFiles;

        public System.Windows.Media.Brush Color_FoundFiles { get => color_FoundFiles; set => SetProperty(ref color_FoundFiles, value); }

        private RelayCommand _bt_Log1;

        public ICommand _bt_Log
        {
            get
            {
                if (_bt_Log1 == null) _bt_Log1 = new RelayCommand(Perform_bt_Log);
                return _bt_Log1;
            }
        }

        private void Perform_bt_Log(object commandParameter)
        {
            LogViewer viewer = new LogViewer();
            viewer.ShowDialog();
        }

        private string tb_DatenbankFiles1;

        public string tb_DatenbankFiles { get => tb_DatenbankFiles1; set => SetProperty(ref tb_DatenbankFiles1, value); }

        private string tb_IndizierteFiles1;

        public string tb_IndizierteFiles { get => tb_IndizierteFiles1; set => SetProperty(ref tb_IndizierteFiles1, value); }

        private string tb_FoundFiles1;

        public string tb_FoundFiles { get => tb_FoundFiles1; set => SetProperty(ref tb_FoundFiles1, value); }

        private RelayCommand removeFromDB;
        public ICommand RemoveFromDB
        {
            get
            {
                if (removeFromDB == null) removeFromDB = new RelayCommand(Perform_removeFromDB);
                return removeFromDB;
            }
        }

        private void Perform_removeFromDB(object commandParameter)
        {
            string[] splitted = commandParameter.ToString().Split('\n') ?? new string[] {};
            fs.RemoveFromIndex(Path.Combine(splitted[1], splitted[0]));
        }

        private object pPbtn;

        public object PPbtn { get => pPbtn; set => SetProperty(ref pPbtn, value); }

        private double syncbtn_Rotate;

        public double Syncbtn_Rotate { get => syncbtn_Rotate; set => SetProperty(ref syncbtn_Rotate, value); }

        private RelayCommand rotate_button1;

        public void _rotate_button(object commandParameter)
        {
            MainWindow.instance.AnimationBoard.Begin();
            //MainWindow.instance.AnimationBoard.BeginAnimation()
            MainModel.instance.ready_Tick();
            //MainWindow.instance.AnimationBoard.Stop();
        }

        public ICommand rotate_button
        {
            get
            {
                if (rotate_button1 == null)
                {
                    rotate_button1 = new RelayCommand(_rotate_button);
                }

                return rotate_button1;
            }
        }

        private RelayCommand syncbtn_Sync;

        public ICommand Syncbtn_Sync
        {
            get
            {
                if (syncbtn_Sync == null)
                {
                    syncbtn_Sync = new RelayCommand(PerformSyncbtn_Sync);
                }

                return syncbtn_Sync;
            }
        }

        private void PerformSyncbtn_Sync(object commandParameter)
        {
            db.pull();
            db.push();
            MainModel.instance.ready_Tick();
        }
    }
}
