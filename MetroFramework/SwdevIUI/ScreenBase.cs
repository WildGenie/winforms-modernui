using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Animation.Swdev;
using MetroFramework.Forms;
using System.Collections;

namespace MetroFramework.SwdevIUI
{
    public enum ScreenTypeEnum { LoadingScreen, HomeScreen, Others };
    public partial class ScreenBase : UserControl
    {
        public MetroForm MetroFormContainer;
        public ScreenTypeEnum ScreenType;
        
        private bool isHavingCommandScreen;
        public bool IsHavingCommandScreen
        {
            get { return isHavingCommandScreen;  }
            set 
            { 
                isHavingCommandScreen = value; 
            }
        }

        public class ScreenEventArgs : EventArgs
        {
            private ScreenBase screen;

            public ScreenEventArgs(ScreenBase screen)
            {
                Screen = screen;
            }
            public ScreenBase Screen
            {
                get { return screen; }
                set { screen = value; }
            }
        }

        public event EventHandler<ScreenEventArgs> Finished;
        protected virtual void OnFinished(ScreenEventArgs e)
        {
            if (Finished != null)
            {
                Finished(this, e);
            }
        }
        

        public ScreenBase()
        {
            InitializeComponent();
        }

        public virtual void ActivateScreen()
        {

        }

        internal void Dismiss()
        {
            //AnimationUtil.Animate(this, AnimationUtil.Effect.Center, 1000, 0);
        }

        public void DisplayCommandBar()
        {
            MetroFormContainer.DisplayCommandBar(hashCommandBarItem);   
        }

        
        Hashtable hashCommandBarItem = new Hashtable();
        public void AddCommandBarItem(string title, Bitmap icon, ActionCommandBarItem action)
        {
            hashCommandBarItem.Add(title, new CommandBarItem(title, icon, action));
        }
    }
}
