using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public string unbuttontext
        {
            set
            {
                this.button1.Text = value;

            }
            get
            {
                return this.button1.Text;
            }
        }

        public void unbuttonbackcolor()
        {
                this.button1.BackColor = System.Drawing.Color.Blue; 

        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {
            Button btn = sender as Button;
            System.Drawing.Drawing2D.GraphicsPath btnPath = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Rectangle newRectangle = btn.ClientRectangle;
            newRectangle.Inflate(-2, -1);
            e.Graphics.DrawEllipse(System.Drawing.Pens.BlanchedAlmond, newRectangle);
            newRectangle.Inflate(-2, -4);
            btnPath.AddEllipse(newRectangle);
            btn.Region = new System.Drawing.Region(btnPath);
        }

        private void button1_Resize(object sender, EventArgs e)
        {
            this.Width = 43;
            this.Height = 46;
        }


    }
}
