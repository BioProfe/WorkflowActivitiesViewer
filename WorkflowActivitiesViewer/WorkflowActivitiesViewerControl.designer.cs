namespace WorkflowActivitiesViewer
{
    partial class WorkflowActivitiesViewerControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lblSearchCodeActivity = new System.Windows.Forms.ToolStripLabel();
            this.txtSearchCodeActivity = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelAssemblies = new System.Windows.Forms.Panel();
            this.lblAssemblies = new System.Windows.Forms.Label();
            this.panelProcessInfo = new System.Windows.Forms.Panel();
            this.toolStripMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.tsbRefresh,
            this.toolStripSeparator1,
            this.lblSearchCodeActivity,
            this.txtSearchCodeActivity,
            this.toolStripSeparator2});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(559, 25);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(40, 22);
            this.tsbClose.Text = "Close";
            this.tsbClose.ToolTipText = "Close";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbRefresh
            // 
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(50, 22);
            this.tsbRefresh.Text = "Refresh";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // lblSearchCodeActivity
            // 
            this.lblSearchCodeActivity.Margin = new System.Windows.Forms.Padding(20, 1, 0, 2);
            this.lblSearchCodeActivity.Name = "lblSearchCodeActivity";
            this.lblSearchCodeActivity.Size = new System.Drawing.Size(85, 22);
            this.lblSearchCodeActivity.Text = "Search Activity";
            // 
            // txtSearchCodeActivity
            // 
            this.txtSearchCodeActivity.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearchCodeActivity.Name = "txtSearchCodeActivity";
            this.txtSearchCodeActivity.Size = new System.Drawing.Size(180, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelAssemblies);
            this.splitContainer1.Panel1.Controls.Add(this.lblAssemblies);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelProcessInfo);
            this.splitContainer1.Size = new System.Drawing.Size(559, 275);
            this.splitContainer1.SplitterDistance = 261;
            this.splitContainer1.TabIndex = 5;
            // 
            // panelAssemblies
            // 
            this.panelAssemblies.AutoScroll = true;
            this.panelAssemblies.AutoSize = true;
            this.panelAssemblies.BackColor = System.Drawing.SystemColors.Window;
            this.panelAssemblies.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelAssemblies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAssemblies.Location = new System.Drawing.Point(0, 38);
            this.panelAssemblies.Margin = new System.Windows.Forms.Padding(4, 2, 2, 4);
            this.panelAssemblies.Name = "panelAssemblies";
            this.panelAssemblies.Size = new System.Drawing.Size(261, 237);
            this.panelAssemblies.TabIndex = 13;
            // 
            // lblAssemblies
            // 
            this.lblAssemblies.BackColor = System.Drawing.SystemColors.Window;
            this.lblAssemblies.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAssemblies.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAssemblies.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAssemblies.Location = new System.Drawing.Point(0, 0);
            this.lblAssemblies.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.lblAssemblies.Name = "lblAssemblies";
            this.lblAssemblies.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblAssemblies.Size = new System.Drawing.Size(261, 38);
            this.lblAssemblies.TabIndex = 12;
            this.lblAssemblies.Text = "Assemblies";
            this.lblAssemblies.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelProcessInfo
            // 
            this.panelProcessInfo.AutoScroll = true;
            this.panelProcessInfo.BackColor = System.Drawing.SystemColors.Window;
            this.panelProcessInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelProcessInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProcessInfo.Location = new System.Drawing.Point(0, 0);
            this.panelProcessInfo.Margin = new System.Windows.Forms.Padding(2, 2, 4, 4);
            this.panelProcessInfo.Name = "panelProcessInfo";
            this.panelProcessInfo.Size = new System.Drawing.Size(294, 275);
            this.panelProcessInfo.TabIndex = 8;
            // 
            // WorkflowActivitiesViewerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "WorkflowActivitiesViewerControl";
            this.Size = new System.Drawing.Size(559, 300);
            this.Load += new System.EventHandler(this.WorkflowActivitiesViewerControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lblSearchCodeActivity;
        private System.Windows.Forms.ToolStripTextBox txtSearchCodeActivity;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Panel panelAssemblies;
        private System.Windows.Forms.Label lblAssemblies;
        private System.Windows.Forms.Panel panelProcessInfo;
    }
}
