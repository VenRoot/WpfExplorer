using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Exceptions
{
    public class MySQLConnectionFailedException : Exception
    {
        public MySQLConnectionFailedException(string message): base(message)
        {
            if(message == null) message = "Die Verbindung zum MySQL-Server konnte nicht hergestellt werden. Bitte prüfen Sie die Einstellungen";
        }
    }
}
