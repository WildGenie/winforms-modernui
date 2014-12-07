using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{
    public partial class MetroMenuStrip : MenuStrip, IMetroControl
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

        public MetroMenuStrip(IContainer Container)
        {
            if (Container != null)
            {
                Container.Add(this);
            }
        }

        public static Color NormalBorder(MetroThemeStyle theme)
        {
            if (theme == MetroThemeStyle.Dark)
                return Color.FromArgb(51, 51, 52);

            return Color.FromArgb(204, 206, 219);
        }

        public static Color HoverBorder(MetroThemeStyle theme)
        {
            if (theme == MetroThemeStyle.Dark)
                return Color.FromArgb(62, 62, 64);

            return Color.FromArgb(248, 249, 250);
        }

        private void settheme()
        {
            BackColor = MetroPaint.BackColor.Form(Theme);
            ForeColor = MetroPaint.ForeColor.Button.Normal(Theme);
            Renderer = new MetroCTXRenderer(Theme, Style);
        }

        private class MetroCTXRenderer : ToolStripProfessionalRenderer
        {
            private MetroThemeStyle theme;
            private MetroColorStyle style;

            public MetroCTXRenderer(MetroThemeStyle Theme, MetroColorStyle Style)
                : base(new contextcolors(Theme, Style))
            {
                theme = Theme;
                style = Style;
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                ToolStripItem tsmi = e.Item as ToolStripMenuItem;

                if (tsmi.Owner.Text != String.Empty && tsmi.Selected)
                {
                    if (!tsmi.Pressed)
                    {
                        Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                        Brush brush = new SolidBrush(MetroPaint.BackColor.MenuStrip.Hover(theme));
                        e.Graphics.FillRectangle(brush, bounds);
                    }
                    else
                    {
                        RectangleF bounds = new RectangleF(Point.Empty, new SizeF(e.Item.Size.Width - 0.5f, e.Item.Size.Height));
                        Brush brush = new SolidBrush(MetroPaint.BackColor.MenuStrip.Press(theme));
                        e.Graphics.FillRectangle(brush, bounds);

                        using(Pen p = new Pen(NormalBorder(theme))) 
                        {
                            PointF leftBottomCorner = new PointF(bounds.X, bounds.Y + bounds.Height);
                            PointF leftTopCorner = new PointF(bounds.X, bounds.Y);
                            PointF rightTopCorner = new PointF(bounds.X + bounds.Width - 0.5f, bounds.Y);
                            PointF rightBottomCorner = new PointF(bounds.X + bounds.Width - 0.5f, bounds.Y + bounds.Height);
                            PointF[] points = { leftBottomCorner, leftTopCorner, rightTopCorner, rightBottomCorner };
                            e.Graphics.DrawLines(p, points);
                        }
                    }
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                var tsmi = e.Item as ToolStripMenuItem;
                Rectangle rect = new Rectangle(tsmi.ContentRectangle.X + 25, tsmi.ContentRectangle.Y, tsmi.ContentRectangle.Width, tsmi.ContentRectangle.Height);
                
                if (tsmi.Owner.Text != String.Empty)
                {
                    if (tsmi.Selected)
                    {
                        TextRenderer.DrawText(e.Graphics, tsmi.Text, MetroFonts.Default(12), tsmi.ContentRectangle, MetroPaint.ForeColor.MenuStrip.Hover(theme),
                                              MetroPaint.GetTextFormatFlags(ContentAlignment.MiddleCenter));
                    }
                    else if (!tsmi.Selected)
                    {
                        TextRenderer.DrawText(e.Graphics, tsmi.Text, MetroFonts.Default(12), tsmi.ContentRectangle, MetroPaint.ForeColor.MenuStrip.Normal(theme),
                                    MetroPaint.GetTextFormatFlags(ContentAlignment.MiddleCenter));
                    }
                }
                else
                {
                    if (tsmi.Selected)
                    {
                        TextRenderer.DrawText(e.Graphics, tsmi.Text, MetroFonts.Default(12), rect, MetroPaint.ForeColor.MenuStrip.Hover(theme),
                                              MetroPaint.GetTextFormatFlags(ContentAlignment.MiddleLeft));
                    }
                    else if (!tsmi.Selected)
                    {
                        TextRenderer.DrawText(e.Graphics, tsmi.Text, MetroFonts.Default(12), rect, MetroPaint.ForeColor.MenuStrip.Normal(theme),
                                    MetroPaint.GetTextFormatFlags(ContentAlignment.MiddleLeft));
                    }
                }
                
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                base.OnRenderArrow(e);
            }
        }

        private class contextcolors : ProfessionalColorTable
        {
            MetroThemeStyle _theme = MetroThemeStyle.Light;
            MetroColorStyle _style = MetroColorStyle.Blue;

            public contextcolors(MetroThemeStyle Theme, MetroColorStyle Style)
            {
                _theme = Theme;
                _style = Style;
            }

            public override Color SeparatorLight
            {
                get { return NormalBorder(_theme); }
            }

            public override Color SeparatorDark
            {
                get { return NormalBorder(_theme); }
            }

            public override Color MenuItemSelected
            {
                get { return MetroPaint.BackColor.MenuStrip.Hover(_theme); }
            }

            public override Color MenuItemPressedGradientBegin 
            {
                get { return MetroPaint.BackColor.MenuStrip.Press(_theme); }
            }

            public override Color MenuItemPressedGradientMiddle
            {
                get { return MetroPaint.BackColor.MenuStrip.Press(_theme); }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get { return MetroPaint.BackColor.MenuStrip.Press(_theme); }
            }

            public override Color MenuBorder
            {
                get { return NormalBorder(_theme); }
            }

            public override Color MenuItemBorder
            {
                get { return MetroPaint.BackColor.MenuStrip.Press(_theme); }
            }

            public override Color ImageMarginGradientBegin
            {
                get { return MetroPaint.BackColor.MenuStrip.Press(_theme); }
            }

            public override Color ImageMarginGradientMiddle
            {
                get { return MetroPaint.BackColor.MenuStrip.Press(_theme); }
            }

            public override Color ImageMarginGradientEnd
            {
                get { return MetroPaint.BackColor.MenuStrip.Press(_theme); }
            }

            public override Color ToolStripDropDownBackground
            {
                get { return MetroPaint.BackColor.MenuStrip.Press(_theme); }
            }
        }

    }
}
