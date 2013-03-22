namespace frm
{
    partial class ShutupifyView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Whatsup = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Whatsup
            // 
            this.Whatsup.AutoSize = true;
            this.Whatsup.Font = new System.Drawing.Font("Source Code Pro Black", 12F, System.Drawing.FontStyle.Bold);
            this.Whatsup.Location = new System.Drawing.Point(22, 17);
            this.Whatsup.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Whatsup.Name = "Whatsup";
            this.Whatsup.Size = new System.Drawing.Size(99, 20);
            this.Whatsup.TabIndex = 0;
            this.Whatsup.Text = "{whatup}?";
            // 
            // ShutupifyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 53);
            this.ControlBox = false;
            this.Controls.Add(this.Whatsup);
            this.Font = new System.Drawing.Font("Source Code Pro Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShutupifyView";
            this.Opacity = 0.95D;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Shutupify";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Whatsup;
    }
}

