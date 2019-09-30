namespace Kibus4
{
    partial class Epocas
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
            if (disposing && (components != null))
            {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Epocas));
            this.numEpks = new System.Windows.Forms.NumericUpDown();
            this.btnEnviar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numEpks)).BeginInit();
            this.SuspendLayout();
            // 
            // numEpks
            // 
            this.numEpks.Location = new System.Drawing.Point(13, 13);
            this.numEpks.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numEpks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numEpks.Name = "numEpks";
            this.numEpks.Size = new System.Drawing.Size(75, 20);
            this.numEpks.TabIndex = 1;
            this.numEpks.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numEpks.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.numEpks_KeyPress);
            // 
            // btnEnviar
            // 
            this.btnEnviar.Location = new System.Drawing.Point(13, 39);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(75, 23);
            this.btnEnviar.TabIndex = 2;
            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.UseVisualStyleBackColor = true;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);
            // 
            // Epocas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(104, 75);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.numEpks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Epocas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Epocas";
            this.Load += new System.EventHandler(this.Epocas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numEpks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numEpks;
        private System.Windows.Forms.Button btnEnviar;
    }
}