using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{
    public partial class MetroTreeView : TreeView, IMetroControl
    {
        #region Interface

        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;
        protected virtual void OnCustomPaintBackground(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintBackground != null)
            {
                CustomPaintBackground(this, e);
            }

            PaintNodesBackground(e.Graphics, Nodes);
        }

        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaint;
        protected virtual void OnCustomPaint(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaint != null)
            {
                CustomPaint(this, e);
            }
        }

        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
        protected virtual void OnCustomPaintForeground(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintForeground != null)
            {
                CustomPaintForeground(this, e);
            }

            PaintNodesForeground(e.Graphics, Nodes);
        }

        private MetroColorStyle metroStyle = MetroColorStyle.Default;
        [Category("Metro Appearance")]
        [DefaultValue(MetroColorStyle.Default)]
        public MetroColorStyle Style
        {
            get
            {
                if (DesignMode || metroStyle != MetroColorStyle.Default)
                {
                    return metroStyle;
                }

                if (StyleManager != null && metroStyle == MetroColorStyle.Default)
                {
                    return StyleManager.Style;
                }
                if (StyleManager == null && metroStyle == MetroColorStyle.Default)
                {
                    return MetroColorStyle.Blue;
                }

                return metroStyle;
            }
            set { metroStyle = value; }
        }

        private MetroThemeStyle metroTheme = MetroThemeStyle.Default;
        [Category("Metro Appearance")]
        [DefaultValue(MetroThemeStyle.Default)]
        public MetroThemeStyle Theme
        {
            get
            {
                if (DesignMode || metroTheme != MetroThemeStyle.Default)
                {
                    return metroTheme;
                }

                if (StyleManager != null && metroTheme == MetroThemeStyle.Default)
                {
                    return StyleManager.Theme;
                }
                if (StyleManager == null && metroTheme == MetroThemeStyle.Default)
                {
                    return MetroThemeStyle.Light;
                }

                return metroTheme;
            }
            set { metroTheme = value; }
        }

        private MetroStyleManager metroStyleManager = null;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MetroStyleManager StyleManager
        {
            get { return metroStyleManager; }
            set
            {
                metroStyleManager = value;
                settheme();
            }
        }

        private bool useCustomBackColor = false;
        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseCustomBackColor
        {
            get { return useCustomBackColor; }
            set { useCustomBackColor = value; }
        }

        private bool useCustomForeColor = false;
        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseCustomForeColor
        {
            get { return useCustomForeColor; }
            set { useCustomForeColor = value; }
        }

        private bool useStyleColors = false;
        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get { return useStyleColors; }
            set { useStyleColors = value; }
        }

        [Browsable(false)]
        [Category("Metro Behaviour")]
        [DefaultValue(false)]
        public bool UseSelectable
        {
            get { return GetStyle(ControlStyles.Selectable); }
            set { SetStyle(ControlStyles.Selectable, value); }
        }

        #endregion

        public MetroTreeView(IContainer Container)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.ResizeRedraw |
              ControlStyles.UserPaint, true);
        }

        private void settheme()
        {
            this.BackColor = MetroPaint.BackColor.Form(Theme);
            this.ForeColor = MetroPaint.ForeColor.Button.Normal(Theme);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                Color backColor = BackColor;

                base.OnPaintBackground(e);

                OnCustomPaintBackground(new MetroPaintEventArgs(backColor, Color.Empty, e.Graphics));
            }
            catch
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (GetStyle(ControlStyles.AllPaintingInWmPaint))
                {
                    OnPaintBackground(e);
                }

                OnCustomPaint(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
                OnPaintForeground(e);
            }
            catch
            {
                Invalidate();
            }
        }

        protected virtual void OnPaintForeground(PaintEventArgs e)
        {
            try
            {
                OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, Color.White, e.Graphics));
            }
            catch
            {
                Invalidate();
            }
        }

        private void PaintNodesBackground(Graphics g, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.IsSelected)
                {
                    RectangleF selectedRect = new RectangleF(0, node.Bounds.Y, this.Width, node.Bounds.Height);
                    g.FillRectangle(Brushes.DeepSkyBlue, selectedRect);
                }
                else
                {
                    RectangleF selectedRect = new RectangleF(0, node.Bounds.Y, this.Width, node.Bounds.Height);
                    g.FillRectangle(new SolidBrush(BackColor), selectedRect);
                }

                if (node.Nodes.Count > 0)
                {
                    PaintNodesBackground(g, node.Nodes);
                }
            }
        }

        private void PaintNodesForeground(Graphics g, TreeNodeCollection nodes)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float side = this.ItemHeight / 2.5f;
            float h = (float)Math.Sqrt(3) / 2f * side;

            foreach (TreeNode node in nodes)
            {
                int marginForImage = 0;

                if (node.IsVisible && ImageList != null && node.ImageIndex > -1)
                {
                    Image image = ImageList.Images[node.ImageIndex];
                    g.DrawImage(image, node.Bounds.X - 20, node.Bounds.Y);
                    marginForImage = 25;
                }

                Color textColor;
                Brush fillBrush;
                Color borderColor;
                if (node.IsSelected)
                {
                    textColor = Color.White;
                    fillBrush = new SolidBrush(Color.White);
                    borderColor = Color.White;
                }
                else
                {
                    textColor = MetroPaint.ForeColor.TreeView.Normal(Theme);
                    fillBrush = new SolidBrush(MetroPaint.ForeColor.TreeView.Normal(Theme));
                    borderColor = MetroPaint.ForeColor.TreeView.Normal(Theme);
                }

                Rectangle rect = node.Bounds;
                Size size = TextRenderer.MeasureText(g, node.Text, Font);
                if (node.Bounds.Width < size.Width)
                {
                    int displacement = size.Width - node.Bounds.Width;
                    rect = Rectangle.Inflate(node.Bounds, displacement, 0);
                    rect.X += displacement;
                }
                TextRenderer.DrawText(g, node.Text, Font, rect, textColor, MetroPaint.GetTextFormatFlags(ContentAlignment.MiddleLeft));

                if (node.IsExpanded)
                {
                    using (Pen p = new Pen(borderColor, 1.5f))
                    {
                        PointF a = new PointF(node.Bounds.X - marginForImage - h, node.Bounds.Y + (ItemHeight / 2) + 2.6f);
                        PointF b = new PointF(node.Bounds.X - marginForImage - h, node.Bounds.Y + (ItemHeight / 2) - 2.6f);
                        PointF c = new PointF(node.Bounds.X - marginForImage - h - 5.2f, node.Bounds.Y + (node.Bounds.Height / 2) + 2.6f);

                        g.DrawPolygon(p, new PointF[] { a, b, c });
                        g.FillPolygon(fillBrush, new PointF[] { a, b, c });
                    }
                }
                else
                {
                    using (Pen p = new Pen(borderColor, 1.5f))
                    {
                        PointF a = new PointF(node.Bounds.X - marginForImage - 10, node.Bounds.Y + (ItemHeight / 2) + 3.6f);
                        PointF b = new PointF(node.Bounds.X - marginForImage - 10, node.Bounds.Y + (ItemHeight / 2) - 3.6f);
                        PointF c = new PointF(node.Bounds.X - marginForImage - h, node.Bounds.Y + node.Bounds.Height / 2);

                        g.DrawPolygon(p, new PointF[] { a, b, c });
                    }
                }

                if (node.Nodes.Count > 0)
                {
                    PaintNodesForeground(g, node.Nodes);
                }
            }
        }
    }
}
