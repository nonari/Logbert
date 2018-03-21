﻿namespace Com.Couchcoding.Logbert.Receiver.Log4NetUdpReceiver
{
  partial class CustomUdpReceiverSettings
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.lblNetworkInterface = new System.Windows.Forms.Label();
      this.cmbNetworkInterface = new System.Windows.Forms.ComboBox();
      this.lblPort = new System.Windows.Forms.Label();
      this.nudPort = new System.Windows.Forms.NumericUpDown();
      this.nfoPanel = new Com.Couchcoding.GuiLibrary.Controls.InfoPanel();
      this.btnRemoveColumnizer = new System.Windows.Forms.Button();
      this.btnAddColumnizer = new System.Windows.Forms.Button();
      this.btnEditColumnizer = new System.Windows.Forms.Button();
      this.cmbColumnizer = new System.Windows.Forms.ComboBox();
      this.lblColumnizer = new System.Windows.Forms.Label();
      this.txtMulticastIp = new System.Windows.Forms.TextBox();
      this.chkMulticastGroup = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
      this.SuspendLayout();
      // 
      // lblNetworkInterface
      // 
      this.lblNetworkInterface.AutoSize = true;
      this.lblNetworkInterface.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.lblNetworkInterface.Location = new System.Drawing.Point(0, 0);
      this.lblNetworkInterface.Name = "lblNetworkInterface";
      this.lblNetworkInterface.Size = new System.Drawing.Size(95, 13);
      this.lblNetworkInterface.TabIndex = 0;
      this.lblNetworkInterface.Text = "&Network Interface:";
      // 
      // cmbNetworkInterface
      // 
      this.cmbNetworkInterface.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbNetworkInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbNetworkInterface.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.cmbNetworkInterface.FormattingEnabled = true;
      this.cmbNetworkInterface.Location = new System.Drawing.Point(0, 16);
      this.cmbNetworkInterface.Name = "cmbNetworkInterface";
      this.cmbNetworkInterface.Size = new System.Drawing.Size(400, 21);
      this.cmbNetworkInterface.TabIndex = 1;
      this.cmbNetworkInterface.SelectedIndexChanged += new System.EventHandler(this.CmbNetworkInterfaceSelectedIndexChanged);
      // 
      // lblPort
      // 
      this.lblPort.AutoSize = true;
      this.lblPort.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.lblPort.Location = new System.Drawing.Point(0, 116);
      this.lblPort.Name = "lblPort";
      this.lblPort.Size = new System.Drawing.Size(29, 13);
      this.lblPort.TabIndex = 3;
      this.lblPort.Text = "&Port:";
      // 
      // nudPort
      // 
      this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.nudPort.Location = new System.Drawing.Point(0, 132);
      this.nudPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.nudPort.Name = "nudPort";
      this.nudPort.Size = new System.Drawing.Size(400, 20);
      this.nudPort.TabIndex = 4;
      this.nudPort.Value = new decimal(new int[] {
            7071,
            0,
            0,
            0});
      // 
      // nfoPanel
      // 
      this.nfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.nfoPanel.BackColor = System.Drawing.Color.WhiteSmoke;
      this.nfoPanel.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
      this.nfoPanel.InfoIcon = global::Com.Couchcoding.Logbert.Properties.Resources.StatusAnnotations_Information_16xLG;
      this.nfoPanel.Location = new System.Drawing.Point(0, 43);
      this.nfoPanel.Name = "nfoPanel";
      this.nfoPanel.ShowInfoIcon = true;
      this.nfoPanel.ShowTitle = false;
      this.nfoPanel.Size = new System.Drawing.Size(400, 64);
      this.nfoPanel.TabIndex = 2;
      this.nfoPanel.TextPadding = new System.Windows.Forms.Padding(0, 6, 0, 0);
      this.nfoPanel.Title = "";
      // 
      // btnRemoveColumnizer
      // 
      this.btnRemoveColumnizer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRemoveColumnizer.Image = global::Com.Couchcoding.Logbert.Properties.Resources.Remove_16xMD;
      this.btnRemoveColumnizer.Location = new System.Drawing.Point(348, 206);
      this.btnRemoveColumnizer.Name = "btnRemoveColumnizer";
      this.btnRemoveColumnizer.Size = new System.Drawing.Size(23, 22);
      this.btnRemoveColumnizer.TabIndex = 10;
      this.btnRemoveColumnizer.UseVisualStyleBackColor = true;
      this.btnRemoveColumnizer.Click += new System.EventHandler(this.BtnRemoveColumnizerClick);
      // 
      // btnAddColumnizer
      // 
      this.btnAddColumnizer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAddColumnizer.Image = global::Com.Couchcoding.Logbert.Properties.Resources.action_add_16xMD;
      this.btnAddColumnizer.Location = new System.Drawing.Point(319, 206);
      this.btnAddColumnizer.Name = "btnAddColumnizer";
      this.btnAddColumnizer.Size = new System.Drawing.Size(23, 22);
      this.btnAddColumnizer.TabIndex = 9;
      this.btnAddColumnizer.UseVisualStyleBackColor = true;
      this.btnAddColumnizer.Click += new System.EventHandler(this.BtnAddColumnizerClick);
      // 
      // btnEditColumnizer
      // 
      this.btnEditColumnizer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnEditColumnizer.Enabled = false;
      this.btnEditColumnizer.Image = global::Com.Couchcoding.Logbert.Properties.Resources.PencilAngled_16xMD_color;
      this.btnEditColumnizer.Location = new System.Drawing.Point(377, 206);
      this.btnEditColumnizer.Name = "btnEditColumnizer";
      this.btnEditColumnizer.Size = new System.Drawing.Size(23, 22);
      this.btnEditColumnizer.TabIndex = 11;
      this.btnEditColumnizer.UseVisualStyleBackColor = true;
      this.btnEditColumnizer.Click += new System.EventHandler(this.BtnEditColumnizerClick);
      // 
      // cmbColumnizer
      // 
      this.cmbColumnizer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbColumnizer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbColumnizer.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.cmbColumnizer.FormattingEnabled = true;
      this.cmbColumnizer.Location = new System.Drawing.Point(0, 207);
      this.cmbColumnizer.Name = "cmbColumnizer";
      this.cmbColumnizer.Size = new System.Drawing.Size(313, 21);
      this.cmbColumnizer.Sorted = true;
      this.cmbColumnizer.TabIndex = 8;
      // 
      // lblColumnizer
      // 
      this.lblColumnizer.AutoSize = true;
      this.lblColumnizer.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.lblColumnizer.Location = new System.Drawing.Point(0, 191);
      this.lblColumnizer.Name = "lblColumnizer";
      this.lblColumnizer.Size = new System.Drawing.Size(61, 13);
      this.lblColumnizer.TabIndex = 7;
      this.lblColumnizer.Text = "&Columnizer:";
      // 
      // txtMulticastIp
      // 
      this.txtMulticastIp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMulticastIp.Enabled = false;
      this.txtMulticastIp.Location = new System.Drawing.Point(134, 163);
      this.txtMulticastIp.Name = "txtMulticastIp";
      this.txtMulticastIp.Size = new System.Drawing.Size(266, 20);
      this.txtMulticastIp.TabIndex = 6;
      // 
      // chkMulticastGroup
      // 
      this.chkMulticastGroup.AutoSize = true;
      this.chkMulticastGroup.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.chkMulticastGroup.Location = new System.Drawing.Point(0, 164);
      this.chkMulticastGroup.Name = "chkMulticastGroup";
      this.chkMulticastGroup.Size = new System.Drawing.Size(131, 18);
      this.chkMulticastGroup.TabIndex = 5;
      this.chkMulticastGroup.Text = "Join &Multicast Group:";
      this.chkMulticastGroup.UseVisualStyleBackColor = true;
      this.chkMulticastGroup.CheckedChanged += new System.EventHandler(this.ChkMulticastGroupCheckedChanged);
      // 
      // CustomUdpReceiverSettings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.txtMulticastIp);
      this.Controls.Add(this.chkMulticastGroup);
      this.Controls.Add(this.btnRemoveColumnizer);
      this.Controls.Add(this.btnAddColumnizer);
      this.Controls.Add(this.btnEditColumnizer);
      this.Controls.Add(this.cmbColumnizer);
      this.Controls.Add(this.lblColumnizer);
      this.Controls.Add(this.nfoPanel);
      this.Controls.Add(this.nudPort);
      this.Controls.Add(this.lblPort);
      this.Controls.Add(this.cmbNetworkInterface);
      this.Controls.Add(this.lblNetworkInterface);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "CustomUdpReceiverSettings";
      this.Size = new System.Drawing.Size(400, 300);
      ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblNetworkInterface;
    private System.Windows.Forms.ComboBox cmbNetworkInterface;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.NumericUpDown nudPort;
    private GuiLibrary.Controls.InfoPanel nfoPanel;
    private System.Windows.Forms.Button btnRemoveColumnizer;
    private System.Windows.Forms.Button btnAddColumnizer;
    private System.Windows.Forms.Button btnEditColumnizer;
    private System.Windows.Forms.ComboBox cmbColumnizer;
    private System.Windows.Forms.Label lblColumnizer;
    private System.Windows.Forms.TextBox txtMulticastIp;
    private System.Windows.Forms.CheckBox chkMulticastGroup;
  }
}