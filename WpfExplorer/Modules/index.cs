using System.Linq;
using System.Threading;
using System.Windows;
using WpfExplorer.ViewModel;

namespace WpfExplorer
{
    public class index
    {

        public string path;
        public int TotalFiles;
        public int indexedFiles = 0;

        public index(string path)
        {
            this.path = path;
        }
        public void start()
        {
            string[] files = fs.readDirSync(path, true, true);
            TotalFiles = files.Length;
            //MainWindow window2 = Application.Current.Windows.Cast<MainWindow>().First();
            //for (int i = 0; i < TotalFiles; i++)
            //{
            //    //Thread.Sleep(100);

            //    window2.SetIndexProgress(files[i], i, TotalFiles);
            //}

            for(int i = 0; i < TotalFiles; i++)
            {
                Thread.Sleep(100);
                //MainWindowViewModel.SetIndexProgress(files[i], i, TotalFiles);
            }

        }
    }
}
