using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfExplorer.View;
using WpfExplorer.ViewModel;

namespace WpfExplorer.Model
{
    public class MainModel
    {
        public static string AUTH_KEY = "";
        public static string TEMP_LOCATION = Path.Combine(Path.GetTempPath(), "WpfExplorer");
        public static fs.C_UC us = new fs.C_UC();
        public static string DBEXTENSION = ".wpfex";
        public static string DB_ENC_EXTENSION = ".enc.wpfex";
        public static string CONFIG_LOCATIONS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WpfExplorer\\");
        public static DriveInfo[] allDrives;
        MainModel()
        {
            instance = this;
        }
        public static MainModel instance;

        public void OpenDialog()
        {
            DialogWindow window = new DialogWindow();
            window.Show();
        }

        public void ready_Tick()
        {
            var res = db.fetch();
            if (res == 1)
            {
                MainWindowViewModel.instance.PPbtn = "↓";
            }
            else if (res == 0)
            {
                MainWindowViewModel.instance.PPbtn = "↑";
            }
            else if (res == -2)
            {
                MainWindowViewModel.instance.PPbtn = "✓";
            }
        }

        public static void CheckExtKey()
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.wpfex\\OpenWithList";
            const string subkey_enc = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.enc.wpfex\\OpenWithList";
            const string keyName = userRoot + "\\" + subkey;
            Registry.SetValue(keyName, "a", "WpfExplorer.exe", RegistryValueKind.String);
            Registry.SetValue(keyName, "b", "a", RegistryValueKind.String);
            Registry.SetValue(keyName, "MRUList", "ab", RegistryValueKind.String);
        }

        public void SetPing(object sender, EventArgs e)
        {
            double PingTime = db.PingDB();
            MainWindowViewModel.instance.tb_Ping_Text = $"{PingTime}ms";
        }

        public string[] checkForExcpetionlist(string[] files)
        {
            /*
             * Wenn el[i][0] == *, el.Includes(el[i][el[i]-1]
             * 
             * prüfe ob Path.GetExtension(files[i]) != '' && ob in el, dann entferne
             */

            List<string> el = MainWindowViewModel.instance.ExcList;
            List<string> filesList = files.ToList();

            //Entferne alle Dateitypen in der Liste
            for (int i = 0; i < filesList.Count; i++)
            {
                string ext = Path.GetExtension(filesList[i]);
                if (ext.Length > 0 && el.Contains(ext)) filesList.RemoveAt(i);
            }

            string[] _el = el.ToArray();
            //Enterne alle Verzeichnisse 
            for (int i = 0; i < _el.Length; i++)
            {
                for (int o = 0; o < filesList.Count; o++)
                {
                    //Prüfe, ob der Eintrag ein "Verzeichnis/" ist und prüfe anschließend, ob die Datei in solch einem Verzeichnis ist
                    if (_el[i].EndsWith("/") && filesList[o].Replace("\\", "/").Contains(_el[i])) filesList.RemoveAt(o);
                }
            }

            return filesList.ToArray();
        }

        public void FileSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);
            MessageBox.Show(lbi.Content.ToString());
            MessageBox.Show(MainWindowViewModel.instance.selectedFile.Path);
            Process.Start(lbi.Content.ToString());
        }

        public void OpenFileInExplorer(object o)
        {
            if (o == null) return;
            string p = o.ToString();
            string[] pp = p.Split('\n');
            string path = Path.Combine(pp[1], pp[0]);

            string argument = "/select, \"" + path + "\"";
            Process.Start("explorer.exe", argument);
            //cmd.StartInfo.Arguments = "/c " + $"explorer.exe /select,\"{pp[1]}\\{pp[0]}\"";
            //cmd.EnableRaisingEvents = true;
            //cmd.Start();
        }

        public void Indiziere()
        {
            MainWindowViewModel.instance._PATH = main.getPathDialog();
            if (MainWindowViewModel.instance._PATH == "") return;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += backgroundWorker1_ProgressChanged;
            worker.DoWork += backgroundWorker1_DoWork;
            worker.RunWorkerAsync();
            return;
        }

        public bool IsExceptedFile(string file)
        {
            //List<string> ExceptionList = getFileExceptionList();
            var valid = false;
            List<string> ExceptionList = new List<string>
            {
                @"C:\Temp\"
            };
            for (int i = 0; i < ExceptionList.Count; i++)
            {
                if (ExceptionList[i].EndsWith("/"))
                {
                    List<string> split = ExceptionList[i].Split('/').ToList();
                    if (file.Contains(split[0] + "\\")) return true;
                }
                if (ExceptionList[i].EndsWith("\\"))
                {
                    if (Path.GetDirectoryName(file) + "\\" == ExceptionList[i]) return true;
                }
                if (Regex.IsMatch(file, fs.WildCardToRegular(ExceptionList[i]))) { return true; }
            }

            return valid;
        }

        public void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            BW_Files _progress = new BW_Files();

            string[] files = fs.readDirSync(MainWindowViewModel.instance._PATH, true, true);
            if (files == null) return;
            List<fs.C_File> _files = new List<fs.C_File>();

            for (int i = 0; i < files.Length; i++)
            {
                if (MainModel.instance.IsExceptedFile(files[i])) continue;
                _files.Add(fs.getFileInfo(files[i]));
                _progress.Message = "Dateien werden gesucht... " + i + " Dateien gefunden";
                worker.ReportProgress(0, JsonConvert.SerializeObject(_progress));
            }


            //Check if the file type or files are in the ExceptionList
            files = MainModel.instance.checkForExcpetionlist(files);

            int TotalFiles = files.Length;
            _progress.Total = TotalFiles;
            C_TFiles ProcessedFiles = new C_TFiles();
            for (int i = 0; i < TotalFiles; i++)
            {
                //fs.AddToIndex(files[i]);
                //Thread.Sleep(100);
                switch (fs.AddToIndex(files[i]))
                {

                    case -1: string msg = $"Die Datei {Path.GetFileName(files[i])} konnte nicht indiziert werden, da sie schon vorhanden ist"; SetIndexMessage(msg); main.AddLog(msg, main.status.warning); ProcessedFiles.FilesSkipped.Add(new C_Files { FileName = Path.GetFileName(files[i]), Path = files[i] }); break; //Datei schon vorhanden
                    case -255: ProcessedFiles.FilesErr.Add(new C_Files { FileName = Path.GetFileName(files[i]), Path = files[i] }); break; //Exception
                    case 0: ProcessedFiles.FilesOk.Add(new C_Files { FileName = Path.GetFileName(files[i]), Path = files[i] }); main.AddLog($"Die Datei {Path.GetFileName(files[i])} wurde zur Datenbank hinzugefügt", main.status.log); break;

                }
                _progress.Current.FileCount++;
                _progress.Current.FileName = _files[i].Name;
                worker.ReportProgress(0, JsonConvert.SerializeObject(_progress));
                //SetIndexProgress(_files[i], i, TotalFiles);
            }
            var temp_file = db.getConf<fs.C_IZ>("database");
            temp_file.last_sync = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            db.setConf("database", temp_file);
            string MsgText = "";
            if (ProcessedFiles.FilesSkipped.Count != 0) MsgText += $"{ProcessedFiles.FilesSkipped.Count} Dateien übersprungen\n";
            if (ProcessedFiles.FilesErr.Count != 0) MsgText += $"{ProcessedFiles.FilesErr.Count} Dateien fehlerhaft\n";
            if (ProcessedFiles.FilesOk.Count != 0) MsgText += $"{ProcessedFiles.FilesOk.Count} Dateien erfolgreich hinzugefügt\n";

            int total = ProcessedFiles.FilesOk.Count + ProcessedFiles.FilesSkipped.Count + ProcessedFiles.FilesOk.Count;
            MsgText += $"\n{total} von {TotalFiles} Dateien verarbeitet";
            main.AddLog(MsgText.Replace("\n", " | "), main.status.log);
            MessageBox.Show(MsgText);
        }

        public void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BW_Files files = JsonConvert.DeserializeObject<BW_Files>(e.UserState.ToString());
            if (files.Message != null) SetIndexMessage(files.Message);
            if (files.Current != null) SetIndexProgress(files.Current.FileName, files.Current.FileCount, files.Total);

        }


        public class BW_Files
        {
            public class File
            {
                public string FileName;
                public int FileCount;
            };
            public int Total;
            public uint InDB;
            public File Current;
            public string Message;

            public BW_Files()
            {
                this.InDB = db.CountFiles();
                this.Current = new File();
            }
        }



        public class C_TFiles
        {
            public List<C_Files> FilesOk = new List<C_Files>() { };
            public List<C_Files> FilesErr = new List<C_Files>() { };
            public List<C_Files> FilesSkipped = new List<C_Files>() { };
        }

        public class C_Files
        {
            public string FileName;
            public string Path;
        }

        public void ToExceptionList()
        {
            return;
        }
        public void SetIndexMessage(string message)
        {
            MainWindowViewModel.instance.tb_FoundFiles = message;
            //FileProgress = message;
        }

        public void tb_Search_TextChanged()
        {
            MainWindowViewModel.instance.FoundFiles.Clear();

            /** Nichts eingegeben = nichts anzeigen */
            if (MainWindowViewModel.instance.tb_Search_Text.Length == 0) return;
            /**Es sollten zuerst die Dateinamen und DANN erst Dateien mit dem Inhalt durchsucht werden */

            List<FileStructure> File = fs.searchFile(MainWindowViewModel.instance.tb_Search_Text, true);
            Task.Run(MainWindowViewModel.instance.searchContext);
            if (File.Count != 0)
            {

                foreach (var v in File)
                {
                    double size = 0;
                    string end = "b";
                    if (v.Size > 1000) { end = "kB"; size = Convert.ToDouble(v.Size / 1000); }
                    else if (v.Size > 1000000) { end = "MB"; size = v.Size / 1000000; }
                    else if (v.Size > 1000000000) { end = "GB"; size = v.Size / 1000000000; }

                    v.Path = Path.GetDirectoryName(v.Path);

                    string res = "";
                    res += v.Filename + "\n";
                    res += v.Path + "\n";
                    res += size + end + "\n\n";

                    MainWindowViewModel.instance.FoundFiles.Add(res);
                }
            }
        }


        public void SetIndexProgress(string FileName, int current, int total)
        {
            double p = 100 / Convert.ToDouble(total);
            double prozent = p * Convert.ToDouble(current);
            if (Double.IsInfinity(prozent)) prozent = 0;

            MainWindowViewModel.instance.tb_IndizierteFiles = $"{current} von {total} ({Math.Round(prozent, 2)}%) | {FileName}";
            MainWindowViewModel.instance.tb_DatenbankFiles = $"{current} Dateien in der Datenbank";

        }


        //In der Window sollte es Tickboxen geben, welche beim Anklick Variablen ändern.
        //Z.B. ob rekursiv gesucht werden sollte. Unten im Eck sollte es einen "Schließen" Button geben
        public void PerformSettings_Click(object commandParameter)
        {
            //Einstellungen wie rekursiv indizieren, Cache leeren
            UserSettingsWindow window = new UserSettingsWindow();
            window.Title = "Einstellungen - WpfExplorer";
            window.ShowDialog();

        }

        public void PerformLogs_Click(object commandParameter)
        {
            //Einstellungen wie rekursiv indizieren, Cache leeren
            LogViewer window = new LogViewer();
            window.Title = "LogViewer - WpfExplorer";
            window.ShowDialog();

        }

        public void Perform_bt_Help(object commandParameter)
        {
            MessageBox.Show("Bedienung:\n*.jpg => Filter alle JPG Dateien\nHa* => Filter alle Dateien, welche mit Ha beginnen\n*2021* => Filter alle Dateien, welche 2021 im Namen haben\n\nFiles/ ignoriert jedes Verzeichnis mit dem Namen Files\n*les/ ignoriert jedes Verzeichnis mit les am Ende\nC:\\Users\\ ignoriert NUR diesen einen Ordner\n\nRechtsklicken Sie auf einen Eintrag, um diesen zu entfernen");
        }

        public void MyCommand(object sender, SelectionChangedEventArgs e)
        {
            List<string> lul = (List<string>)e.AddedItems;
            MessageBox.Show(string.Join("\n", lul));
        }

        public void Debug_Click()
        {
            MessageBox.Show(string.Join("\n", MainWindowViewModel.instance.FileExceptionList));
        }

        public static List<string> GetExceptionList(MainWindowViewModel model)
        {
            return model.FileExceptionList.ToList();
        }

        public void showFileExceptions()
        {
            List<string> ex = new List<string>();

            for (int i = 0; i < MainWindowViewModel.instance.FileExceptionList.Count; i++) ex.Add(MainWindowViewModel.instance.FileExceptionList[i]);

            MessageBox.Show(ex.ToString());
        }

        public void EnterKey(object commandParameter)
        {
            MessageBox.Show(commandParameter.ToString());

            MainWindowViewModel.instance.FileExceptionList.Add(commandParameter.ToString());
            MainWindowViewModel.instance.tb_AddExceptionsText = "";
        }

    }

    
}
