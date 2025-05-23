﻿using GitCommands;
using GitCommands.Git;
using GitExtensions.Extensibility;
using GitExtensions.Extensibility.Git;
using GitExtUtils.GitUI.Theming;
using GitUI.HelperDialogs;
using GitUI.Theming;
using GitUI.UserControls;
using GitUIPluginInterfaces;
using Microsoft;
using ResourceManager;

namespace GitUI.CommandsDialogs
{
    public partial class FormDiff : GitModuleForm
    {
        private string? _firstCommitDisplayStr;
        private string? _secondCommitDisplayStr;
        private GitRevision? _firstRevision;
        private GitRevision? _secondRevision;
        private readonly GitRevision? _mergeBase;
        private Lazy<ObjectId?> _currentHead = null;

        private readonly IGitRevisionTester _revisionTester;
        private readonly IFileStatusListContextMenuController _revisionDiffContextMenuController;
        private readonly IFullPathResolver _fullPathResolver;
        private readonly IFindFilePredicateProvider _findFilePredicateProvider;
        private readonly CancellationTokenSequence _populateDiffFilesSequence = new();
        private readonly CancellationTokenSequence _viewChangesSequence = new();

        private readonly ToolTip _toolTipControl = new();

        private readonly TranslationString _anotherBranchTooltip = new("Select another branch");
        private readonly TranslationString _anotherCommitTooltip = new("Select another commit");
        private readonly TranslationString _btnSwapTooltip = new("Swap BASE and Compare commits");
        private readonly TranslationString _ckCompareToMergeBase = new("Compare to merge &base");

        public FormDiff(
            IGitUICommands commands,
            ObjectId firstId,
            ObjectId secondId,
            string firstCommitDisplayStr, string secondCommitDisplayStr)
            : base(commands)
        {
            _firstCommitDisplayStr = firstCommitDisplayStr;
            _secondCommitDisplayStr = secondCommitDisplayStr;

            InitializeComponent();

            InitializeComplete();

            _toolTipControl.SetToolTip(btnAnotherFirstBranch, _anotherBranchTooltip.Text);
            _toolTipControl.SetToolTip(btnAnotherSecondBranch, _anotherBranchTooltip.Text);
            _toolTipControl.SetToolTip(btnAnotherFirstCommit, _anotherCommitTooltip.Text);
            _toolTipControl.SetToolTip(btnAnotherSecondCommit, _anotherCommitTooltip.Text);
            _toolTipControl.SetToolTip(btnSwap, _btnSwapTooltip.Text);

            _firstRevision = new GitRevision(firstId);
            _secondRevision = new GitRevision(secondId);

            // _mergeBase is not changed if first/second is changed
            // similar, _currentHead is not updated if changed in Browse
            _currentHead = new(() => Module.GetCurrentCheckout());
            ObjectId? firstMergeId = firstId.IsArtificial ? _currentHead.Value : firstId;
            ObjectId? secondMergeId = secondId.IsArtificial ? _currentHead.Value : secondId;
            if (firstMergeId is null || secondMergeId is null || firstMergeId == secondMergeId)
            {
                _mergeBase = null;
            }
            else
            {
                ObjectId mergeBase = Module.GetMergeBase(firstMergeId, secondMergeId);
                _mergeBase = mergeBase is not null ? new GitRevision(mergeBase) : null;
            }

            ckCompareToMergeBase.Text = $"{_ckCompareToMergeBase} ({_mergeBase?.ObjectId.ToShortString()})";
            ckCompareToMergeBase.Enabled = _mergeBase is not null;

            _fullPathResolver = new FullPathResolver(() => Module.WorkingDir);
            _findFilePredicateProvider = new FindFilePredicateProvider();
            _revisionTester = new GitRevisionTester(_fullPathResolver);
            _revisionDiffContextMenuController = new FileStatusListContextMenuController();

            lblFirstCommit.BackColor = AppColor.AnsiTerminalRedBackNormal.GetThemeColor();
            lblSecondCommit.BackColor = AppColor.AnsiTerminalGreenBackNormal.GetThemeColor();

            DiffFiles.SelectedIndexChanged += delegate { ShowSelectedFileDiff(); };
            DiffText.ExtraDiffArgumentsChanged += delegate { ShowSelectedFileDiff(); };
            DiffText.TopScrollReached += FileViewer_TopScrollReached;
            DiffText.BottomScrollReached += FileViewer_BottomScrollReached;
            Load += delegate { PopulateDiffFiles(); };
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _populateDiffFilesSequence.Dispose();
                _viewChangesSequence.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void FileViewer_TopScrollReached(object sender, EventArgs e)
        {
            DiffFiles.SelectPreviousVisibleItem();
            DiffText.ScrollToBottom();
        }

        private void FileViewer_BottomScrollReached(object sender, EventArgs e)
        {
            DiffFiles.SelectNextVisibleItem();
            DiffText.ScrollToTop();
        }

        private void PopulateDiffFiles()
        {
            lblFirstCommit.Text = _firstCommitDisplayStr;
            lblSecondCommit.Text = _secondCommitDisplayStr;

            // Bug in git-for-windows: Comparing working directory to any branch, fails, due to -R
            // I.e., git difftool --gui --no-prompt --dir-diff -R HEAD fails, but
            // git difftool --gui --no-prompt --dir-diff HEAD succeeds
            // Thus, we disable comparing "from" working directory.
            bool enableDifftoolDirDiff = _firstRevision?.ObjectId != ObjectId.WorkTreeId;
            btnCompareDirectoriesWithDiffTool.Enabled = enableDifftoolDirDiff;

            Validates.NotNull(_secondRevision);
            GitRevision[] revisions;
            if (ckCompareToMergeBase.Checked)
            {
                Validates.NotNull(_mergeBase);
                revisions = new[] { _secondRevision, _mergeBase };
            }
            else
            {
                Validates.NotNull(_firstRevision);
                revisions = new[] { _secondRevision, _firstRevision };
            }

            DiffFiles.InvokeAndForget(() => DiffFiles.SetDiffsAsync(revisions, _currentHead.Value, _populateDiffFilesSequence.Next()));
        }

        private void ShowSelectedFileDiff()
        {
            _ = DiffText.ViewChangesAsync(DiffFiles.SelectedItem,
                cancellationToken: _viewChangesSequence.Next());
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            GitRevision orgFirstRev = _firstRevision;
            _firstRevision = _secondRevision;
            _secondRevision = orgFirstRev;

            string orgFirstStr = _firstCommitDisplayStr;
            _firstCommitDisplayStr = _secondCommitDisplayStr;
            _secondCommitDisplayStr = orgFirstStr;
            PopulateDiffFiles();
        }

        private void ckCompareToMergeBase_CheckedChanged(object sender, EventArgs e)
        {
            PopulateDiffFiles();
        }

        private void btnCompareDirectoriesWithDiffTool_Clicked(object sender, EventArgs e)
        {
            GitRevision? firstRevision = ckCompareToMergeBase.Checked ? _mergeBase : _firstRevision;
            Validates.NotNull(firstRevision);
            Validates.NotNull(_secondRevision);
            Module.OpenWithDifftoolDirDiff(firstRevision.Guid, _secondRevision.Guid, customTool: null);
        }

        private void btnPickAnotherFirstBranch_Click(object sender, EventArgs e)
        {
            Validates.NotNull(_firstRevision);
            PickAnotherBranch(_firstRevision, ref _firstCommitDisplayStr, ref _firstRevision);
        }

        private void btnAnotherFirstCommit_Click(object sender, EventArgs e)
        {
            Validates.NotNull(_firstRevision);
            PickAnotherCommit(_firstRevision, ref _firstCommitDisplayStr, ref _firstRevision);
        }

        private void btnAnotherSecondBranch_Click(object sender, EventArgs e)
        {
            Validates.NotNull(_secondRevision);
            PickAnotherBranch(_secondRevision, ref _secondCommitDisplayStr, ref _secondRevision);
        }

        private void btnAnotherSecondCommit_Click(object sender, EventArgs e)
        {
            Validates.NotNull(_secondRevision);
            PickAnotherCommit(_secondRevision, ref _secondCommitDisplayStr, ref _secondRevision);
        }

        private ContextMenuDiffToolInfo GetContextMenuDiffToolInfo()
        {
            List<ObjectId> parentIds = DiffFiles.SelectedItems.FirstIds().ToList();
            bool firstIsParent = _revisionTester.AllFirstAreParentsToSelected(parentIds, _secondRevision);
            bool localExists = _revisionTester.AnyLocalFileExists(DiffFiles.SelectedItems.Select(i => i.Item));

            bool allAreNew = DiffFiles.SelectedItems.All(i => i.Item.IsNew);
            bool allAreDeleted = DiffFiles.SelectedItems.All(i => i.Item.IsDeleted);

            return new ContextMenuDiffToolInfo(
                _secondRevision,
                parentIds,
                allAreNew: allAreNew,
                allAreDeleted: allAreDeleted,
                firstIsParent: firstIsParent,
                localExists: localExists);
        }

        private void PickAnotherBranch(GitRevision preSelectCommit, ref string? displayStr, ref GitRevision? revision)
        {
            using FormCompareToBranch form = new(UICommands, preSelectCommit.ObjectId);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                displayStr = form.BranchName;
                ObjectId objectId = Module.RevParse(form.BranchName);
                revision = objectId is null ? null : new GitRevision(objectId);
                PopulateDiffFiles();
            }
        }

        private void PickAnotherCommit(GitRevision preSelect, ref string? displayStr, ref GitRevision? revision)
        {
            using FormChooseCommit form = new(UICommands, preselectCommit: preSelect.Guid, showArtificial: true);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                revision = form.SelectedRevision;
                displayStr = form.SelectedRevision?.Subject;
                PopulateDiffFiles();
            }
        }
    }
}
