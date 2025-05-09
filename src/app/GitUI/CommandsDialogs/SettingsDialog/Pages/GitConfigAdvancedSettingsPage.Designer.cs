﻿namespace GitUI.CommandsDialogs.SettingsDialog.Pages
{
    partial class GitConfigAdvancedSettingsPage
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
            if (disposing && (components is not null))
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
            checkBoxRebaseAutostash = new CheckBox();
            checkBoxFetchPrune = new CheckBox();
            checkBoxPullRebase = new CheckBox();
            checkBoxRebaseAutosquash = new CheckBox();
            checkBoxUpdateRefs = new CheckBox();
            checkboxMergeAutoStash = new CheckBox();
            SuspendLayout();
            // 
            // checkBoxRebaseAutostash
            // 
            checkBoxRebaseAutostash.AutoSize = true;
            checkBoxRebaseAutostash.Location = new Point(19, 89);
            checkBoxRebaseAutostash.Name = "checkBoxRebaseAutostash";
            checkBoxRebaseAutostash.Size = new Size(247, 19);
            checkBoxRebaseAutostash.TabIndex = 4;
            checkBoxRebaseAutostash.Text = "Automatically stash before doing a rebase";
            checkBoxRebaseAutostash.ThreeState = true;
            checkBoxRebaseAutostash.UseVisualStyleBackColor = true;
            // 
            // checkBoxFetchPrune
            // 
            checkBoxFetchPrune.AutoSize = true;
            checkBoxFetchPrune.Location = new Point(19, 39);
            checkBoxFetchPrune.Name = "checkBoxFetchPrune";
            checkBoxFetchPrune.Size = new Size(217, 19);
            checkBoxFetchPrune.TabIndex = 2;
            checkBoxFetchPrune.Text = "Prune remote branches during fetch";
            checkBoxFetchPrune.ThreeState = true;
            checkBoxFetchPrune.UseVisualStyleBackColor = true;
            // 
            // checkBoxPullRebase
            // 
            checkBoxPullRebase.AutoSize = true;
            checkBoxPullRebase.Location = new Point(19, 14);
            checkBoxPullRebase.Name = "checkBoxPullRebase";
            checkBoxPullRebase.Size = new Size(303, 19);
            checkBoxPullRebase.TabIndex = 1;
            checkBoxPullRebase.Text = "Rebase local branch when pulling (instead of merge)";
            checkBoxPullRebase.ThreeState = true;
            checkBoxPullRebase.UseVisualStyleBackColor = true;
            // 
            // checkBoxRebaseAutosquash
            // 
            checkBoxRebaseAutosquash.AutoSize = true;
            checkBoxRebaseAutosquash.Location = new Point(19, 114);
            checkBoxRebaseAutosquash.Name = "checkBoxRebaseAutosquash";
            checkBoxRebaseAutosquash.Size = new Size(367, 19);
            checkBoxRebaseAutosquash.TabIndex = 5;
            checkBoxRebaseAutosquash.Text = "Automatically squash commits when doing an interactive rebase";
            checkBoxRebaseAutosquash.ThreeState = true;
            checkBoxRebaseAutosquash.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdateRefs
            // 
            checkBoxUpdateRefs.AutoSize = true;
            checkBoxUpdateRefs.Location = new Point(19, 139);
            checkBoxUpdateRefs.Name = "checkBoxUpdateRefs";
            checkBoxUpdateRefs.Size = new Size(198, 19);
            checkBoxUpdateRefs.TabIndex = 6;
            checkBoxUpdateRefs.Text = "Rebase also dependent branches";
            checkBoxUpdateRefs.ThreeState = true;
            checkBoxUpdateRefs.UseVisualStyleBackColor = true;
            // 
            // checkboxMergeAutoStash
            // 
            checkboxMergeAutoStash.AutoSize = true;
            checkboxMergeAutoStash.Location = new Point(19, 64);
            checkboxMergeAutoStash.Name = "checkboxMergeAutoStash";
            checkboxMergeAutoStash.Size = new Size(247, 19);
            checkboxMergeAutoStash.TabIndex = 3;
            checkboxMergeAutoStash.Text = "Automatically stash before doing a merge";
            checkboxMergeAutoStash.ThreeState = true;
            checkboxMergeAutoStash.UseVisualStyleBackColor = true;
            // 
            // GitConfigAdvancedSettingsPage
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(checkboxMergeAutoStash);
            Controls.Add(checkBoxRebaseAutostash);
            Controls.Add(checkBoxFetchPrune);
            Controls.Add(checkBoxPullRebase);
            Controls.Add(checkBoxUpdateRefs);
            Controls.Add(checkBoxRebaseAutosquash);
            Name = "GitConfigAdvancedSettingsPage";
            Size = new Size(1439, 516);
            Text = "Advanced";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBoxRebaseAutostash;
        private CheckBox checkBoxFetchPrune;
        private CheckBox checkBoxPullRebase;
        private CheckBox checkBoxRebaseAutosquash;
        private CheckBox checkBoxUpdateRefs;
        private CheckBox checkboxMergeAutoStash;
    }
}
