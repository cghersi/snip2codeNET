﻿//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NppPluginNet
{
    public class SelectableFlowLayoutPanel : FlowLayoutPanel
    {
        public SelectableFlowLayoutPanel()
        {
            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            this.Focus();
            base.OnMouseDown(e);
        }
        protected override bool IsInputKey(Keys keyData) {
            if (keyData == Keys.Up || keyData == Keys.Down) return true;
            if (keyData == Keys.Left || keyData == Keys.Right) return true;
            return base.IsInputKey(keyData);
        }
        protected override void OnEnter(EventArgs e) {
            this.Invalidate();
            base.OnEnter(e);
        }
        protected override void OnLeave(EventArgs e) {
            this.Invalidate();
            base.OnLeave(e);
        }
        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            if (this.Focused) {
                var rc = this.ClientRectangle;
                rc.Inflate(-2, -2);
                ControlPaint.DrawFocusRectangle(pe.Graphics, rc);
            }
        }
    }
}
