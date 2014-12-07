using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroFramework.SwdevIUI
{
    class CommandBarItem
    {
        public string title;
        public Bitmap icon;
        public ActionCommandBarItem action;

        public CommandBarItem(string _title, Bitmap _icon, ActionCommandBarItem _action)
        {
            title = _title;
            icon = _icon;
            action = _action;
        }
    }

    public delegate void ActionCommandBarItem(object sender, EventArgs e);
}
