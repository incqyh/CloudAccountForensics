using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    public class RunJSEventManager
    {
        public delegate void RunJSEventHandler(object sender, System.EventArgs e);
        public event RunJSEventHandler RunJSEvent = delegate { };

        public void RaiseEvent()
        {
            EventArgs e = new EventArgs();
            RunJSEvent?.Invoke(this, e);
        }
    }

    public class EventManager
    { 
        public static RunJSEventManager runJSEventManager = new RunJSEventManager();
    }
}
