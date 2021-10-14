using CommandHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfExplorer.Model;

namespace WpfExplorer.ViewModel
{
    public class LogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LogViewModel()
        {
            instance = this;
            
        }
        public static LogViewModel instance;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        private object logSelected;

        public object LogSelected { get => logSelected; set => SetProperty(ref logSelected, value); }

        private ObservableCollection<string> logList = new ObservableCollection<string>();
        public ObservableCollection<string> LogList { get => logList; set => SetProperty(ref logList, value); }

        //public ListBox LogList { get => logList; set => SetProperty(ref logList, value); }

        private string logText = "";

        public string LogText { get => logText; set => SetProperty(ref logText, value); }
        public int tBFontSize = 12;

        public int TBFontSize { get => tBFontSize; set => SetProperty(ref tBFontSize, value);  }

        private RelayCommand selectLog;

        public ICommand SelectLog
        {
            get
            {
                if (selectLog == null)
                {
                    selectLog = new RelayCommand(LogModel.instance.PerformSelectLog);
                }

                return selectLog;
            }
        }

        private RelayCommand deleteLog;

        public ICommand DeleteLog
        {
            get
            {
                if (deleteLog == null)
                {
                    deleteLog = new RelayCommand(LogModel.instance.PerformDeleteLog);
                }

                return deleteLog;
            }
        }

        private RelayCommand wipeLogs;

        public ICommand WipeLogs
        {
            get
            {
                if (wipeLogs == null)
                {
                    wipeLogs = new RelayCommand(LogModel.instance.PerformWipeLogs);
                }

                return wipeLogs;
            }
        }

        private RelayCommand exportLogs;

        public ICommand ExportLogs
        {
            get
            {
                if (exportLogs == null)
                {
                    exportLogs = new RelayCommand(LogModel.instance.PerformExportLogs);
                }

                return exportLogs;
            }
        }

        private System.Windows.Media.Brush color_Background;

        public System.Windows.Media.Brush Color_Background { get => color_Background; set => SetProperty(ref color_Background, value); }

        private System.Windows.Media.Brush color_Foreground;

        public System.Windows.Media.Brush Color_Foreground { get => color_Foreground; set => SetProperty(ref color_Foreground, value); }
    }
}
