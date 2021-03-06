using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using iTextSharp;
using Visualis.Extractor;
using iTextSharp.text;
using System.Text.RegularExpressions;
using WpfExplorer.ViewModel;
using WpfExplorer.Modules;
using System.Diagnostics;
using WpfExplorer.View;
using System.Collections.ObjectModel;

namespace WpfExplorer
{
    public class fs
    {


        /** Prüft alle Dateien und erstellt diese, falls nicht vorhanden. Läuft im neuen Thread */
        public static void checkConfig()
        {
            string path = MainWindowViewModel.CONFIG_LOCATIONS;
            string[] files = { "config", "database", "usersettings" };

            Directory.CreateDirectory(path);
            for (int i = 0; i < files.Length; i++)
            {
                if (!File.Exists(path + files[i] + ".json"))
                {
                    File.WriteAllText(path + files[i] + ".json", "{}");

                    switch(path)
                    {
                        case "config": 
                            db.DBConf dBConf= new db.DBConf();
                            dBConf.Host = "***REMOVED***"; dBConf.Database = "wpf"; dBConf.Password = "***REMOVED***"; dBConf.Username = "wpf"; dBConf.Port = 3306;
                            db.setConf("config", dBConf); break;


                    }
                }
            }

            Directory.CreateDirectory(MainWindowViewModel.TEMP_LOCATION);

            fs.C_IZ data = db.getConf<fs.C_IZ>("database");
            if (data.AUTH_KEY == null || data.AUTH_KEY.Length == 0) { data.AUTH_KEY = main.RandomString(64); }
            MainWindowViewModel.AUTH_KEY = data.AUTH_KEY;
        }
        public enum Window
        {
            Dialog,
            LogViewer,
            MainWindow,
            UserSettings
        }

        public static void checkWindowColors(Window window)
        {
            if (!File.Exists(Path.Combine(MainWindowViewModel.CONFIG_LOCATIONS, "usersettings.json"))) return;

            var converter = new System.Windows.Media.BrushConverter();
            MainWindowViewModel.us = db.getConf<fs.C_UC>("usersettings");

            if (window == Window.MainWindow)
            {
                var MainWin1 = MainWindow.instance;
                var MainWin2 = MainWindowViewModel.instance;
                if (MainWindowViewModel.us.DarkMode)
                {

                    MainWin1.Background = (System.Windows.Media.Brush)converter.ConvertFromString("#252525");
                    MainWin2.Color_ExceptionLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin2.Color_FileExceptionList = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin2.Color_SuchFeldLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin2.Color_tb_Ping = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin2.Color_FoundFiles = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin2.Color_tb_AddExceptions = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin2.Color_tb_Search = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                }
                else
                {
                    MainWin1.Background = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin2.Color_ExceptionLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    MainWin2.Color_FileExceptionList = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    MainWin2.Color_SuchFeldLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    MainWin2.Color_tb_Ping = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    MainWin2.Color_FoundFiles = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    MainWin2.Color_tb_AddExceptions = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    MainWin2.Color_tb_Search = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                }

            }
            else if (window == Window.Dialog)
            {
                var MainWin1 = MessageDialog.instance;
                if (MainWindowViewModel.us.DarkMode)
                {

                    MainWin1.Color_Background = (System.Windows.Media.Brush)converter.ConvertFromString("#252525");
                    MainWin1.Color_Foreground = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                }
                else
                {
                    MainWin1.Color_Background = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin1.Color_Foreground = (System.Windows.Media.Brush)converter.ConvertFromString("#252525");
                }
            }

            else if (window == Window.LogViewer)
            {
                var MainWin1 = LogViewModel.instance;
                if (MainWindowViewModel.us.DarkMode)
                {

                    MainWin1.Color_Background = (System.Windows.Media.Brush)converter.ConvertFromString("#252525");
                    MainWin1.Color_Foreground = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                }
                else
                {
                    MainWin1.Color_Background = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    MainWin1.Color_Foreground = (System.Windows.Media.Brush)converter.ConvertFromString("#252525");
                }
            }
            else if (window == Window.UserSettings)
            {
                if (MainWindowViewModel.us.DarkMode)
                {
                    var win3 = UserSettingsViewModel.instance;
                    win3.Color_UserSettingsFore = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win3.Color_CheckBox1Fore = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win3.Color_CheckBox1Back = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win3.Color_MiddleBorder = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win3.Color_MiddleBorderBrush = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win3.Color_Window = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win3.DarkModeCheck = MainWindowViewModel.us.DarkMode;
                    win3.RecursiveCheck = MainWindowViewModel.us.Recursive;
                }
                else
                {
                    var win3 = UserSettingsViewModel.instance;
                    win3.Color_UserSettingsFore = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win3.Color_CheckBox1Fore = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win3.Color_CheckBox1Back = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win3.Color_MiddleBorder = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win3.Color_MiddleBorderBrush = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                    win3.Color_Window = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win3.DarkModeCheck = MainWindowViewModel.us.DarkMode;
                    win3.RecursiveCheck = MainWindowViewModel.us.Recursive;
                }
            }
        }

        public static void checkUserSettings(bool initSettingsWindow)
        {
            if(File.Exists(Path.Combine(MainWindowViewModel.CONFIG_LOCATIONS, "usersettings.json")))
            {
                var converter = new System.Windows.Media.BrushConverter();
                MainWindowViewModel.us = db.getConf<fs.C_UC>("usersettings");
                var win2 = MainWindowViewModel.instance;
                var win = MainWindow.instance;
                
                if (MainWindowViewModel.us.DarkMode)
                {
                    win.Background = (System.Windows.Media.Brush)converter.ConvertFromString("#252525");
                    win2.Color_ExceptionLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_FileExceptionList = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_SuchFeldLabel = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_tb_Ping = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_FoundFiles = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_tb_AddExceptions = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    win2.Color_tb_Search = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                    if (initSettingsWindow)
                    {
                        var win3 = UserSettingsViewModel.instance;
                        win3.Color_UserSettingsFore = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                        win3.Color_CheckBox1Fore = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                        win3.Color_CheckBox1Back = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                        win3.Color_MiddleBorder = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                        win3.Color_MiddleBorderBrush = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                        win3.Color_Window = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                        win3.DarkModeCheck = MainWindowViewModel.us.DarkMode;
                        win3.RecursiveCheck = MainWindowViewModel.us.Recursive;
                    }
                    
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

                    if (initSettingsWindow)
                    {
                        var win3 = UserSettingsViewModel.instance;
                        win3.Color_UserSettingsFore = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                        win3.Color_CheckBox1Fore = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                        win3.Color_CheckBox1Back = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                        win3.Color_MiddleBorder = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                        win3.Color_MiddleBorderBrush = (System.Windows.Media.Brush)converter.ConvertFromString("#000000");
                        win3.Color_Window = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");
                        win3.DarkModeCheck = MainWindowViewModel.us.DarkMode;
                        win3.RecursiveCheck = MainWindowViewModel.us.Recursive;
                    }
                    
                }

                }
        }

        public static string[] readDirSync(string path, bool fullpath = false, bool recursive = false, string[] _dirs = null)
        {
            if (path == null) return null;
            if (!recursive)
            {
                
                if (fullpath) return Directory.GetFiles(path).Select(p => Path.GetFullPath(p)).ToArray();
                return Directory.GetFiles(path).Select(p => Path.GetFileName(p)).ToArray();
            }
            if (fullpath) return Directory.GetFiles(path, "*", SearchOption.AllDirectories).Select(p => Path.GetFullPath(p)).ToArray();
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories).Select(p => Path.GetFileName(p)).ToArray();
        }

        public static string readFileSync(string path)
        {
            using (StreamReader r = new StreamReader(path, Encoding.UTF8)) {
                string _ = null;
                try 
                { 
                    _ = r.ReadToEnd(); 
                }

                catch (Exception e)
                {
                    main.ReportError(e, main.status.warning, "Datei konnte nicht gelesen werden");
                }
                finally { r.Close(); }
                return _;
            }
        }

        public static void writeFileSync(string path, string context, bool overwrite = false)
        {
            if (!overwrite && File.Exists(path)) return;
            File.WriteAllText(path, context);
        }

        public static bool exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }


        public static void import(string path)
        {
            if (!exists(path)) return;

            try
            {
                string file = fs.readFileSync(path);
                string content = file;





                //Check if File is encrypted
                if (!path.EndsWith(".wpfex") && !path.EndsWith(".enc.wpfex")) { MessageBox.Show("Die Datei ist keine Datenbank", "", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                if (!file.StartsWith("{"))
                {
                    DialogWindow dialogWindow = new DialogWindow();
                    dialogWindow.Title = "Entschlüsseln der Datenbank";
                    dialogWindow.txt_Info.Text = "Die Datei ist geschützt und benötigt ein Passwort";
                    dialogWindow.ShowDialog();
                    if (exportPassword == null) MessageBox.Show("Fehler");
                    content = StringCipher.Decrypt(content, exportPassword);
                }
                
                C_IZ data = JsonConvert.DeserializeObject<C_IZ>(content);
                db.setConf("database", data);
                main.AddLog("Die Datenbank wurde importiert", main.status.log);
                MessageBox.Show("Datei erfolgreich importiert");
            }
            catch (Exception e) { 
                
                if(e.GetType().ToString() == "")
                
                MessageBox.Show($"Konnte Datei '{path}' nicht importieren: \n\n{e.Message}"); return; }
        }


        public static string exportPassword;
        public static void export()
        {
            try
            {
                bool enc = false;
                string path = null;
                var res = MessageBox.Show("Möchten Sie Ihre Datenbank verschlüsseln?", "Datenbank-Verschlüsselung", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if(res == MessageBoxResult.Cancel) { MessageBox.Show("Import abgebrochen", "Datenbank-Verschlüsselung", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
                if ( res == MessageBoxResult.Yes)
                {
                    path = main.getExportDialog(null, true);
                    if(path == null) { MessageBox.Show("Import abgebrochen", "Datenbank-Verschlüsselung", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
                    DialogWindow dialogWindow = new DialogWindow();
                    dialogWindow.Title = "Verschlüsseln der Datenbank";
                    dialogWindow.ShowDialog();
                    if (exportPassword == null) { MessageBox.Show("Fehler"); return; }
                    fs.writeFileSync(path, StringCipher.Encrypt(JsonConvert.SerializeObject(db.getConf<C_IZ>("database")), exportPassword));

                }
                else
                {
                    path = main.getExportDialog(null, false);
                    if (path == null) { MessageBox.Show("Import abgebrochen", "Datenbank-Verschlüsselung", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
                    fs.writeFileSync(path, JsonConvert.SerializeObject(db.getConf<C_IZ>("database")));
                }
                main.AddLog("Die Datenbank wurde exportiert", main.status.log);
                if (MessageBox.Show($"Datenbank erfolgreich unter {path} exportiert. Möchten Sie den Pfad öffnen?", "Datenbank-Verschlüsselung", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
                Process.Start(path);

            }
            catch(Exception e) { throw e; }
        }


        /** Other section*/

        /** GIbt -1 zurück, sollte der Dateiname ungültige Zeichen beinhalten*/
        public static string[] getPath(string fileName)
        {
            /**Error handling. Ungültige Zeichen wie ", ' oder ` geben -1 zurück */
            if (fileName.Contains("'") || fileName.Contains('"') || fileName.Contains('`')) { main.ReportError(new Exception(), main.status.warning, "Ungültiger Dateiname. Die Datei enthält ungültige Zeichen wie \", ' oder `. Aus Sicherheit wird die Query abgebrochen"); return new string[] { "-1" }; }
            return db.myquery("SELECT * FROM test WHERE fileName = @val1", new string[] {fileName}).ToArray();
        }

        public static string[] getContent(string content)
        {
            if (content.Contains("'") || content.Contains('"') || content.Contains('`')) { main.ReportError(new Exception(), main.status.warning, "Ungültiger Dateiname. Die Datei enthält ungültige Zeichen wie \", ' oder `. Aus Sicherheit wird die Query abgebrochen"); return new string[] { "-1" };
        }
            return db.myquery("SELECT * FROM test WHERE content = @val1", new string[] {content}).ToArray();
        }


        /** Liest alle Dateien aus dem Index */
        public static C_IZ readIndexedFiles()
        {
            /** Hole die Config, welche Directories gescannt werden sollten*/
            return db.getConf<C_IZ>("database");

        }
        /** Sucht nach einer Datei nach ihrem Namen und gibt die Datei mit dem Pfad zurück */
        public static List<Model.FileStructure> searchFile(string Filename, bool SearchFileContent)
        {
            /** Es wird noch indiziert. Kann nicht gesucht werden*/
            int ___ = 0;
            while (main.isIndexerRunning) { System.Diagnostics.Debug.WriteLine("Still running" + ___); ___++; }
            C_IZ conf = db.getConf<C_IZ>("database");
            List<Model.FileStructure> FoundFiles = new List<Model.FileStructure>();
            Regex.IsMatch(Filename, WildCardToRegular(Filename));

            for (int i = 0; i < conf.Paths.Count; i++)
            {
                
                List<C_File> _ = new List<C_File>();
                if (Filename.Contains("*")) _ = conf.Paths[i].Files.Where(p => Regex.IsMatch(p.Name, WildCardToRegular(Filename))).ToList<C_File>();
                else _ = conf.Paths[i].Files.Where(p => p.Name.Contains(Filename)).ToList();
                if(SearchFileContent)
                {
                    try
                    {
                        if (Filename.Contains("*") && Filename != "*") _.AddRange(conf.Paths[i].Files.Where(p => Regex.IsMatch(p.Content ?? "", WildCardToRegular(Filename))).ToList<C_File>());
                        else _.AddRange(conf.Paths[i].Files.Where(p => String.IsNullOrEmpty(p.Content) ? false : p.Content.Contains(Filename)).ToList());
                    }
                    catch(Exception e)
                    {
                        if (e.GetType() != typeof(NullReferenceException)) main.ReportError(e, main.status.error, "Der Datenkontext der Datei konnte nicht gelesen werden");
                    }
                }
                for(int j = 0; j < _.Count(); j++)
                {
                    FoundFiles.Add(new Model.FileStructure() { Filename = _[j].Name, Path = _[j].FullPath, Size = _[j].Size });
                }
            }
            return FoundFiles;
        }
        /** Holt sich die IndexDatei und fügt einen Eintrag hinzu */
        public static int AddToIndex(string _file)
        {
            try
            {
                main.isIndexerRunning = true;
                string path = Path.GetDirectoryName(_file);
                C_File file = getFileInfo(_file);

                //Hole die DB-Datei
                C_IZ data = db.getConf<C_IZ>("database");

                //Ist Paths[i].Path schon vorhanden?
                bool found = false;
                for (int i = 0; i < data.Paths.Count; i++)
                {
                    if (data.Paths[i].Path == path)
                    {
                        //JA, füge Eintrag zu Paths[i].Files hinzu, wenn diese nicht schon vorhanden ist
                        if (data.Paths[i].Files.Where(p => p.FullPath == file.FullPath).Count() == 0) data.Paths[i].Files.Add(file);
                        else { main.isIndexerRunning = false; return -1; }
                        //if (!data.Paths[i].Files.Contains(file)) { data.Paths[i].Files.Add(file); } else { main.isIndexerRunning = false; return -1; }
                        found = true;
                        break;
                    }
                }
                //NEIN, lege einen neuen Path Eintrag an und füge die File hinzu
                if (!found) { data.Paths.Add(new C_Path { Path = path, Files = new List<C_File> { file } }); }

                //Speicher die DB-Datei
                db.setConf("database", data);
                db.dbdata = data;
                main.AddToGrid(file, path);
                main.isIndexerRunning = false;
                return 0;
            }
            catch (Exception e)
            {
                main.ReportError(e, main.status.warning, "Datei konnte nicht indiziert werden: "+e.Message); return -255;
            }
        }

        public static List<string> validFileTypes = new List<string> {
            ".txt",
            ".pdf",
            ".doc",
            ".docx"
        };

        public static string WildCardToRegular(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }

        public static void RemoveFromIndex(string _file)
        {
            try
            {
                main.isIndexerRunning = true;
                string path = Path.GetDirectoryName(_file);
                var file = getFileInfo(_file);

                //Hole die DB-Datei
                C_IZ data = db.getConf<C_IZ>("database");

                //Gibt es den Pfad zur Datei?
                bool found = false;
                for (int i = 0; i < data.Paths.Count; i++)
                {
                    if (data.Paths[i].Path == path)
                    {
                        //JA, entferne Eintrag von der DB
                        List<C_File> f = data.Paths[i].Files.Where(p => (p.Name == file.Name && p.FullPath == file.FullPath)).ToList();
                        if (f.Count() == 1)
                        {
                            var old = MainWindowViewModel.instance.FoundFiles;
                            string o = $"{f[0].Name}\n{Path.GetDirectoryName(f[0].FullPath)}";
                            data.Paths[i].Files.Remove(f.First());
                            found = true;
                            List<string> r = MainWindowViewModel.instance.FoundFiles.ToList();
                            r.RemoveAll(u => u.Contains(o));
                            MainWindowViewModel.instance.FoundFiles = new ObservableCollection<string>(r);
                            var _new = MainWindowViewModel.instance.FoundFiles;
                            string text = MainWindowViewModel.instance.tb_DatenbankFiles;
                            string[] stext = text.Split(' ');
                            int p = Convert.ToInt32(stext[0]);
                            p--;
                            MainWindowViewModel.instance.tb_DatenbankFiles = $"{p} Dateien in der Datenbank";
                            break;
                        }
                        //Der Directory ist nicht indiziert
                        else
                        {
                            string log = $"Die Datei ${_file} in {path} konnte nicht entfernt werden, da das Verzeichnis nicht indiziert wurde";
                            main.AddLog(log, main.status.warning);
                            MessageBox.Show(log);
                            main.isIndexerRunning = false;
                            return;
                        }
                    }
                }
                //NEIN, der Directory ist nicht im Index und die Datei wurde nicht gefunden
                if (!found) { string log = $"Die Datei ${_file} in {path} konnte nicht entfernt werden, da diese nicht indiziert wurde"; main.AddLog(log, main.status.warning); MessageBox.Show(log);  }

                //Speicher die DB-Datei
                db.setConf("database", data);
                main.AddToGrid(file, path);
                main.isIndexerRunning = false;
                return;
            }
            catch (Exception e)
            {
                main.ReportError(e, main.status.warning, "Die Datei konnte nicht indiziert werden");
            }
        }

        public static string ExtractText(C_File file)
        {
            FileAttributes attr = File.GetAttributes(file.FullPath);

            if (attr.HasFlag(FileAttributes.Directory)) return "";

            TextExtractorD extractor = new TextExtractorD();
            FileInfo sd = new FileInfo(file.FullPath);
            return extractor.Extract(sd.FullName);
        }

        public static C_File getFileInfo(string _f)
        {
            C_File file = new C_File();
            file.Name = Path.GetFileName(_f);
            file.FullPath = Path.GetFullPath(_f);
            file.Content = null;
            file.Size = Convert.ToUInt64(new FileInfo(file.FullPath).Length);
            if (fs.validFileTypes.Contains(Path.GetExtension(file.Name).ToLower()))  file.Content = ExtractText(file);
            return file;
        }

        interface index
        {
            string path { get; set; }
        }

        public class i
        {
            public i(string path)
            {
                this.path = path;
            }
            public string path;
        }


        public class C_UC
        {
            [JsonProperty("DarkMode")]
            public bool DarkMode { get; set; }

            [JsonProperty("Recursive")]
            public bool Recursive { get; set; }
        };
        public class C_IZ
        {
            [JsonProperty("Paths")]
            public List<C_Path> Paths { get; set; }

            [JsonProperty("AUTH_KEY")]
            public string AUTH_KEY {get; set;}

            [JsonProperty("last_sync")]
            public string last_sync {get; set;}
        }

        public class C_Path
        {
            [JsonProperty("Path")]
            public string Path { get; set; }

            [JsonProperty("Files")]
            public List<C_File> Files { get; set; }
        }

        public class C_File
        {
            public string Name { get; set; }

            public string FullPath { get; set; }

            //<summary>
            //Die Länge der Datei in Bytes
            //</summary>
            public ulong Size { get; set; }
            public string Date { get; set; }
            public string Content { get; set; }
        }
        public partial class C_Which
        {
            [JsonProperty("Paths")]
            public List<string> Paths { get; set; }
        }


        public static int getFileCount(string path)
        {
            return Directory.GetFiles(path).Length;
        }
    }
}
