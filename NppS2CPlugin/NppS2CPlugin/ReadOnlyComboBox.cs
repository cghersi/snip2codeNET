//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NppPluginNet
{
    public class ComboBoxReadOnly : ComboBox
    {
        public ComboBoxReadOnly()
        {
            textBox = new TextBox();
            textBox.ReadOnly = true;
            textBox.Visible = false;
        }

        private TextBox textBox;

        private bool readOnly = false;

        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;

                if (readOnly)
                {
                    this.Visible = false;
                    textBox.BackColor = this.BackColor;
                    textBox.ForeColor = Utilities.s_lightGrayS2C;
                    textBox.Font = this.Font;
                    textBox.Text = this.Text;
                    textBox.Location = this.Location;
                    textBox.Size = this.Size;
                    textBox.Visible = true;

                    if (textBox.Parent == null)
                        this.Parent.Controls.Add(textBox);
                }
                else
                {
                    this.Visible = true;
                    this.textBox.Visible = false;
                }
            }
        }
    }
}
