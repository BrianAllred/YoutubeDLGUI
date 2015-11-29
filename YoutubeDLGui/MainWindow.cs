//
// MainWindow.cs
//
// Author:
//       Brian Allred <brian.d.allred@gmail.com>
//
// Copyright (c) 2015 Brian Allred
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace YoutubeDLGui
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Gtk;

    using log4net;

    using YoutubeDL;

    using Key = Gdk.Key;

    #endregion

    /// <summary>
    ///     Main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     The user password.
        /// </summary>
        private readonly List<char> userPassword = new List<char>();

        /// <summary>
        ///     The video password.
        /// </summary>
        private readonly List<char> videoPassword = new List<char>();

        /// <summary>
        ///     The youtube-dl controller.
        /// </summary>
        private readonly YoutubeDLController youtubeController = YoutubeDLController.Instance();

        /// <summary>
        ///     Initializes a new instance of the <see cref="YoutubeDLGui.MainWindow" /> class.
        /// </summary>
        public MainWindow()
            : base(WindowType.Toplevel)
        {
            this.log.Info("MainWindow initialized.");
            this.DeleteEvent += this.OnDeleteEvent;
            this.Shown += this.OnShown;
            this.Build();
            this.execTextView.Buffer.Changed += this.OnExecTextViewBufferChanged;
            this.videoUrlTextView.Buffer.Changed += this.OnVideoUrlTextViewBufferChanged;
            this.destinationFolderTextView.Buffer.Changed += this.OnDestFolderTextViewBufferChanged;
            this.proxyUrlTextView.Buffer.Changed += this.OnProxyUrlTextViewBufferChanged;
            this.socketTimeoutTextView.Buffer.Changed += this.OnSocketTextViewBufferChanged;
            this.rateTextView.Buffer.Changed += this.OnRateTextViewBufferChanged;
            this.twoFactorTextView.Buffer.Changed += this.OnTwoFactorTextViewBufferChanged;
            this.audioQualityTextView.Buffer.Changed += this.OnAudioQualityTextViewBufferChanged;

            this.youtubeController.Output = this.destinationFolderTextView.Buffer.Text;
            this.youtubeController.Retries = 10;
            this.youtubeController.Fixup = YoutubeDLController.FixupPolicy.detect_or_warn;
            this.youtubeController.AudioQuality = 5;
        }

        /// <summary>
        /// Raises the on button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnContinueRadioButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.Continue = true;
            this.youtubeController.NoContinue = false;
        }

        /// <summary>
        /// Raises the on button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnDefaultContinueRadioButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.Continue = false;
            this.youtubeController.NoContinue = false;
        }

        /// <summary>
        /// Raises the action toggled event.
        /// </summary>
        /// <param name="sender">The action.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnEnableVerboseOutputActionToggled(object sender, EventArgs e)
        {
            this.youtubeController.Verbose = ((ToggleAction)sender).Active;
            ConfigurationHelper.AddUpdateAppSettings(ConfigurationHelper.EnableVerboseOutput, ((ToggleAction)sender).Active.ToString());
        }

        /// <summary>
        /// Show help dialog.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnHelpAction1Activated(object sender, EventArgs e)
        {
            var helpDialog = new HelpDialog();
            helpDialog.Show();
        }

        /// <summary>
        /// Raises the on button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnNoContinueRadioButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.Continue = false;
            this.youtubeController.NoContinue = true;
        }

        /// <summary>
        /// Raises the combo box changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnRateUnitComboBoxChanged(object sender, EventArgs e)
        {
            double rateLimit;
            this.youtubeController.RateLimit = double.TryParse(this.rateTextView.Buffer.Text, out rateLimit) ? rateLimit : 0;
            this.youtubeController.RateLimitUnit = (YoutubeDLController.ByteUnit)Enum.Parse(typeof(YoutubeDLController.ByteUnit), this.rateUnitComboBox.ActiveText);
        }

        /// <summary>
        /// Raises the action activated event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnUpdateEmbeddedYoutubeDlActionActivated(object sender, EventArgs e)
        {
            this.youtubeController.UseEmbeddedBinary = true;
            this.youtubeController.Update = true;
            var progressDialog = new ProgressDialog();
            if ((ResponseType)progressDialog.Run() == ResponseType.Cancel)
            {
                try
                {
                    this.youtubeController.KillProcess();
                }
                catch (Exception ex)
                {
                    using (var exceptionDialog = new ExceptionDialog(ex.GetType().Name, ex.Message))
                    {
                        exceptionDialog.Run();
                    }
                }
            }
        }

        /// <summary>
        /// Raises the action activated event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnUpdateYoutubeDlActionActivated(object sender, EventArgs e)
        {
            this.youtubeController.Update = true;
            var progressDialog = new ProgressDialog();
            if ((ResponseType)progressDialog.Run() == ResponseType.Cancel)
            {
                try
                {
                    this.youtubeController.KillProcess();
                }
                catch (Exception ex)
                {
                    using (var exceptionDialog = new ExceptionDialog(ex.GetType().Name, ex.Message))
                    {
                        exceptionDialog.Run();
                    }
                }
            }
        }

        /// <summary>
        /// Raises the action activated event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnYoutubeDlDocumentationActionActivated(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/rg3/youtube-dl/blob/master/README.md#readme"));
        }

        /// <summary>
        /// Raises the action activated event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnYoutubeDLGuiDocumentationActionActivated(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/BrianAllred/YoutubeDLGUI"));
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnAbortCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.AbortOnError = this.abortCheckButton.Active;
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnAddMetadataCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.AddMetadata = this.addMetadataCheckButton.Active;
        }

        /// <summary>
        /// Raises the combo box changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnAudioFormatComboBoxChanged(object sender, EventArgs e)
        {
            this.youtubeController.AudioFormat = (YoutubeDLController.AudioFormatType)this.audioFormatComboBox.Active;
        }

        /// <summary>
        ///     Raises the audio quality combo box changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnAudioQualityComboBoxChanged(object sender, EventArgs e)
        {
            this.audioQualityTextView.Sensitive = this.audioQualityTextView.Visible = this.audioQualityComboBox.Active == 10;

            this.youtubeController.AudioQuality = this.audioQualityComboBox.Active;
        }

        /// <summary>
        /// Raises the text view changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnAudioQualityTextViewBufferChanged(object sender, EventArgs e)
        {
            int customAudioQuality;
            this.youtubeController.CustomAudioQuality = int.TryParse(this.audioQualityTextView.Buffer.Text, out customAudioQuality) ? customAudioQuality : 0;
        }

        /// <summary>
        ///     Raises the delete event event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="a">The arguments.</param>
        private void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }

        /// <summary>
        /// Raises the text buffer changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnDestFolderTextViewBufferChanged(object sender, EventArgs e)
        {
            this.youtubeController.Output = this.destinationFolderTextView.Buffer.Text;
            ConfigurationHelper.AddUpdateAppSettings(ConfigurationHelper.DestinationFolder, this.destinationFolderTextView.Buffer.Text);
        }

        /// <summary>
        ///     Begin download process.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnDownloadButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var progressDialog = new ProgressDialog();
                if (!progressDialog.Error && (ResponseType)progressDialog.Run() == ResponseType.Cancel)
                {
                    try
                    {
                        this.youtubeController.KillProcess();
                    }
                    catch (Exception ex)
                    {
                        progressDialog.Destroy();
                        using (var exceptionDialog = new ExceptionDialog(ex.GetType().Name, ex.Message))
                        {
                            exceptionDialog.Run();
                        }

                        this.log.Error($"Error stopping youtube-dl process: {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                using (var exceptionDialog = new ExceptionDialog(ex.GetType().Name, ex.Message))
                {
                    exceptionDialog.Run();
                }

                this.log.Error($"Error downloading: {ex}");
            }
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnEmbedSubsCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.EmbedSubs = this.embedSubsCheckButton.Active;
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnEmbedThumbnailCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.EmbedThumbnail = this.embedThumbnailCheckButton.Active;
        }

        /// <summary>
        /// Raises the use embedded binary toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnEnableEmbeddedYoutubeDlActionToggled(object sender, EventArgs e)
        {
            this.youtubeController.UseEmbeddedBinary = ((ToggleAction)sender).Active;

            ConfigurationHelper.AddUpdateAppSettings(ConfigurationHelper.EnableEmbeddedBinary, ((ToggleAction)sender).Active.ToString());
        }

        /// <summary>
        ///     Raises the exec check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnExecCheckButtonToggled(object sender, EventArgs e)
        {
            this.execTextView.Sensitive = this.execTextView.Visible = this.execCheckButton.Active;
        }

        /// <summary>
        /// Raises the text view changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnExecTextViewBufferChanged(object sender, EventArgs e)
        {
            this.youtubeController.Cmd = this.execCheckButton.Active ? this.execTextView.Buffer.Text : string.Empty;
        }

        /// <summary>
        /// Raises the exit action activated event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnExitActionActivated(object sender, EventArgs e)
        {
            Application.Quit();
        }

        /// <summary>
        ///     Collapses all other expanders
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnExpanderActivated(object sender, EventArgs e)
        {
            var thisExpander = (Expander)sender;
            foreach (var expander in this.vbox1.Children.OfType<Expander>().Where(expander => expander != thisExpander))
            {
                expander.Expanded = false;
            }
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnExtractAudioCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.ExtractAudio = this.extractAudioCheckButton.Active;
        }

        /// <summary>
        /// Raises the combo box changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnFixupComboBoxChanged(object sender, EventArgs e)
        {
            this.youtubeController.Fixup = (YoutubeDLController.FixupPolicy)this.fixupComboBox.Active;
        }

        /// <summary>
        ///     Choose a folder.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnFolderChooserButtonClicked(object sender, EventArgs e)
        {
            using (var folderChooserDialog = new FolderChooserDialog())
            {
                var returnValue = folderChooserDialog.Run();
                if (returnValue == (int)ResponseType.Ok)
                {
                    this.destinationFolderTextView.Buffer.Text = folderChooserDialog.Folder + System.IO.Path.DirectorySeparatorChar + "%(uploader)s/%(title)s-%(id)s.%(ext)s";
                }

                folderChooserDialog.Destroy();
            }
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnIgnoreConfigCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.IgnoreConfig = this.ignoreConfigCheckButton.Active;
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnKeepVideoCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.KeepVideo = this.keepVideoCheckButton.Active;
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnNoPostOverwritesCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.NoPostOverwrites = this.noPostOverwritesCheckButton.Active;
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnOverwritesCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.NoOverwrites = this.overwritesCheckButton.Active;
        }

        /// <summary>
        ///     Raises the password check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnPasswordCheckButtonToggled(object sender, EventArgs e)
        {
            if (this.userPassword.Count > 0)
            {
                this.passwordTextView.Buffer.Text = this.passwordCheckButton.Active ? new string(this.userPassword.ToArray()) : new string('*', this.userPassword.Count);
            }
        }

        /// <summary>
        ///     Raises the password text view focus out event event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnPasswordTextViewFocusOutEvent(object sender, FocusOutEventArgs e)
        {
            if (string.IsNullOrEmpty(this.passwordTextView.Buffer.Text))
            {
                this.passwordTextView.Buffer.Text = "Password";
                this.youtubeController.Password = string.Empty;
            }
        }

        /// <summary>
        ///     Raises the password text view key release event event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="args">The arguments.</param>
        private void OnPasswordTextViewKeyReleaseEvent(object sender, KeyReleaseEventArgs args)
        {
            this.ProcessPasswordText(this.passwordTextView, this.userPassword, args);
            this.youtubeController.Password = new string(this.userPassword.ToArray());
        }

        /// <summary>
        ///     Raises the proxy check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnProxyCheckButtonToggled(object sender, EventArgs e)
        {
            this.proxyUrlTextView.Sensitive = this.proxyUrlTextView.Visible = this.proxyCheckButton.Active;
        }

        /// <summary>
        /// Raises the text buffer changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="eventArgs">The parameter is not used.</param>
        private void OnProxyUrlTextViewBufferChanged(object sender, EventArgs eventArgs)
        {
            this.youtubeController.ProxyUrl = this.proxyCheckButton.Active ? this.proxyUrlTextView.Buffer.Text : string.Empty;
        }

        /// <summary>
        ///     Raises the rate limit check button toggled event.*/
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnRateLimitCheckButtonToggled(object sender, EventArgs e)
        {
            this.rateTextView.Sensitive = this.rateTextView.Visible = this.rateLimitCheckButton.Active;
            this.rateUnitComboBox.Sensitive = this.rateUnitComboBox.Visible = this.rateLimitCheckButton.Active;

            if (this.rateLimitCheckButton.Active)
            {
                double rateLimit;
                this.youtubeController.RateLimit = double.TryParse(this.rateTextView.Buffer.Text, out rateLimit) ? rateLimit : 0;
                this.youtubeController.RateLimitUnit = (YoutubeDLController.ByteUnit)Enum.Parse(typeof(YoutubeDLController.ByteUnit), this.rateUnitComboBox.ActiveText);
            }
            else
            {
                this.youtubeController.RateLimit = 0;
            }
        }

        /// <summary>
        /// Raises the text buffer changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnRateTextViewBufferChanged(object sender, EventArgs e)
        {
            double rateLimit;
            this.youtubeController.RateLimit = double.TryParse(this.rateTextView.Buffer.Text, out rateLimit) ? rateLimit : 0;
            this.youtubeController.RateLimitUnit = (YoutubeDLController.ByteUnit)Enum.Parse(typeof(YoutubeDLController.ByteUnit), this.rateUnitComboBox.ActiveText);
        }

        /// <summary>
        /// Raises the combo box changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnRecodeVideoFormatComboBoxChanged(object sender, EventArgs e)
        {
            if (this.recodeVideoFormatComboBox.Active > 0)
            {
                this.youtubeController.RecodeVideo = true;
                this.youtubeController.RecodeVideoFormat = (YoutubeDLController.VideoFormatType)Enum.Parse(typeof(YoutubeDLController.VideoFormatType), this.recodeVideoFormatComboBox.ActiveText.ToLower());
            }
        }

        /// <summary>
        ///     Raises the retries check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnRetriesCheckButtonToggled(object sender, EventArgs e)
        {
            this.retriesTextView.Sensitive = this.retriesTextView.Visible = this.retriesCheckButton.Active;
            if (this.retriesCheckButton.Active)
            {
                int retries;
                this.youtubeController.Retries = int.TryParse(this.retriesTextView.Buffer.Text, out retries) ? retries : 0;
            }
            else
            {
                this.youtubeController.Retries = 10;
            }
        }

        /// <summary>
        /// Raises the window shown event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnShown(object sender, EventArgs e)
        {
            try
            {
                this.EnableEmbeddedYoutubeDlAction.Active = bool.Parse(ConfigurationHelper.ReadSetting(ConfigurationHelper.EnableEmbeddedBinary));
                this.youtubeController.UseEmbeddedBinary = this.EnableEmbeddedYoutubeDlAction.Active;
                this.EnableVerboseOutputAction.Active = bool.Parse(ConfigurationHelper.ReadSetting(ConfigurationHelper.EnableVerboseOutput));
                this.youtubeController.Verbose = this.EnableVerboseOutputAction.Active;

                string output = ConfigurationHelper.ReadSetting(ConfigurationHelper.DestinationFolder);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    this.destinationFolderTextView.Buffer.Text = output;
                    this.youtubeController.Output = output;
                }

                if (bool.Parse(ConfigurationHelper.ReadSetting(ConfigurationHelper.FirstRun)))
                {
                    this.OnHelpAction1Activated(null, null);
                }
            }
            catch (Exception ex)
            {
                this.log.Error($"Error reading config values: {ex}");
            }
        }

        /// <summary>
        /// Raises the on buffer changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnSocketTextViewBufferChanged(object sender, EventArgs e)
        {
            int socket;
            this.youtubeController.SocketTimeout = int.TryParse(this.socketTimeoutTextView.Buffer.Text, out socket) ? socket : 0;
        }

        /// <summary>
        ///     Raises the socket timeout check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnSocketTimeoutCheckButtonToggled(object sender, EventArgs e)
        {
            this.socketTimeoutTextView.Sensitive = this.socketTimeoutTextView.Visible = this.socketTimeoutCheckButton.Active;
            if (this.socketTimeoutCheckButton.Active)
            {
                int socket;
                this.youtubeController.SocketTimeout = int.TryParse(this.socketTimeoutTextView.Buffer.Text, out socket) ? socket : 0;
            }
            else
            {
                this.youtubeController.SocketTimeout = 0;
            }
        }

        /// <summary>
        /// Raises the text buffer changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnTwoFactorTextViewBufferChanged(object sender, EventArgs e)
        {
            var twoFactor = this.twoFactorTextView.Buffer.Text.Trim();
            if (!twoFactor.Equals("Two-factor auth code") && !string.IsNullOrEmpty(twoFactor))
            {
                this.youtubeController.TwoFactor = twoFactor;
            }
        }

        /// <summary>
        ///     Raises the username text view focus out event event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnUsernameTextViewFocusOutEvent(object sender, FocusOutEventArgs e)
        {
            if (string.IsNullOrEmpty(this.usernameTextView.Buffer.Text))
            {
                this.usernameTextView.Buffer.Text = "Username";
                this.youtubeController.Username = string.Empty;
            }
        }

        /// <summary>
        ///     Raises the username text view key release event event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnUsernameTextViewKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
        {
            if (!this.usernameTextView.Buffer.Text.Trim().Equals("Username"))
            {
                this.youtubeController.Username = this.usernameTextView.Buffer.Text.Length == 0 ? string.Empty : this.usernameTextView.Buffer.Text;
            }
        }

        /// <summary>
        ///     Raises the video password check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnVideoPasswordCheckButtonToggled(object sender, EventArgs e)
        {
            if (this.videoPassword.Count > 0)
            {
                this.videoPasswordTextView.Buffer.Text = this.videoPasswordCheckButton.Active ? new string(this.videoPassword.ToArray()) : new string('*', this.videoPassword.Count);
            }
        }

        /// <summary>
        ///     Raises the video password text view key release event event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="args">The arguments.</param>
        private void OnVideoPasswordTextViewKeyReleaseEvent(object sender, KeyReleaseEventArgs args)
        {
            this.ProcessPasswordText(this.videoPasswordTextView, this.videoPassword, args);
            this.youtubeController.VideoPassword = new string(this.videoPassword.ToArray());
        }

        /// <summary>
        /// Raises the text view changed event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="eventArgs">The parameter is not used.</param>
        private void OnVideoUrlTextViewBufferChanged(object sender, EventArgs eventArgs)
        {
            this.youtubeController.VideoUrl = this.videoUrlTextView.Buffer.Text;
        }

        /// <summary>
        /// Raises the check button toggled event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnXattrsCheckButtonToggled(object sender, EventArgs e)
        {
            this.youtubeController.XAttrs = this.xattrsCheckButton.Active;
        }

        /// <summary>
        ///     Processes the password text.
        /// </summary>
        /// <param name="textView">Text view.</param>
        /// <param name="localPassword">Local password.</param>
        /// <param name="args">The arguments.</param>
        private void ProcessPasswordText(TextView textView, List<char> localPassword, KeyReleaseEventArgs args)
        {
            if (textView.Buffer.Text.Length == 0)
            {
                localPassword.Clear();
            }
            else
            {
                if (args.Event.KeyValue <= 32 || args.Event.KeyValue >= 123)
                {
                    return;
                }

                textView.Buffer.Text = string.Empty;
                if (args.Event.Key == Key.BackSpace)
                {
                    if (localPassword.Count > 0)
                    {
                        localPassword.RemoveAt(localPassword.Count - 1);
                    }
                }
                else
                {
                    localPassword.Add((char)args.Event.KeyValue);
                }

                textView.Buffer.Text = this.passwordCheckButton.Active ? new string(localPassword.ToArray()) : new string('*', localPassword.Count);
            }
        }
    }
}