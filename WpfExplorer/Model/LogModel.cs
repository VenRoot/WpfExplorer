using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfExplorer.ViewModel;
namespace WpfExplorer.Model
{
    public class LogModel
    {
        public LogModel()
        {
            instance = this;
        }
        public static LogModel instance;

        public void fillLogs()
        {
            string[] files = fs.readDirSync(Path.Combine(MainModel.TEMP_LOCATION));
            foreach (string file in files)
            {

                DateTime now = DateTime.Parse(Path.GetFileNameWithoutExtension(file));
                LogViewModel.instance.LogList.Add(now.ToString("D"));
            }
        }

        public void PerformWipeLogs(object commandParameter)
        {
            string[] fileList = fs.readDirSync(Path.Combine(MainModel.TEMP_LOCATION), true);
            for (int i = 0; i < fileList.Length; i++) File.Delete(fileList[i]);
            LogViewModel.instance.LogText = "";
            LogViewModel.instance.LogList.Clear();
            fillLogs();
        }

        public void PerformExportLogs(object commandParameter)
        {
            string path = main.exportZipDialog();
            if (path == null) { MessageBox.Show("Vorgang wurde vom Nutzer abgebrochen", null, MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            ZipFile.CreateFromDirectory(MainModel.TEMP_LOCATION, path);
        }

        public void PerformDeleteLog(object commandParameter)
        {
            string command = commandParameter.ToString();

            DateTime time;
            string timeS = "";
            DateTime.TryParse(command, out time);
            if (time != null) timeS = time.ToString("yyyy-MM-dd") + ".log";
            try
            {
                File.Delete(Path.Combine(MainModel.TEMP_LOCATION, timeS));
            }
            catch (Exception e)
            {
                main.ReportError(e, main.status.error, $"Die LogDatei '{timeS}' konnte nicht gelesen werden. Prüfen Sie den Dateinamen");
            }
        }

        public void PerformSelectLog(object commandParameter)
        {
            if (commandParameter == null) return;
            string command = commandParameter.ToString();

            DateTime time;
            string timeS = "";
            DateTime.TryParse(command, out time);
            if (time != null) timeS = time.ToString("yyyy-MM-dd") + ".log";
            try
            {
                string content = fs.readFileSync(Path.Combine(MainModel.TEMP_LOCATION, timeS));
                LogViewModel.instance.LogText = content;
            }
            catch (Exception e)
            {
                main.ReportError(e, main.status.error, $"Die LogDatei '{timeS}' konnte nicht gelesen werden. Prüfen Sie den Dateinamen");
            }
            //PropertyChanged(this, new PropertyChangedEventArgs(nameof(LogList)));
        }

        public void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;

            e.Handled = true;
            if (e.Delta > 0) ++LogViewModel.instance.TBFontSize;
            else --LogViewModel.instance.TBFontSize;
        }
    }
}
