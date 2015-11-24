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

using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using YoutubeDL;
using Key = Gdk.Key;

namespace YoutubeDLGui
{
    /// <summary>
    ///     Main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        ///     The user password.
        /// </summary>
        private readonly List<char> _userPassword = new List<char>();

        /// <summary>
        ///     The video password.
        /// </summary>
        private readonly List<char> _videoPassword = new List<char>();

        /// <summary>
        ///     The youtube-dl controller.
        /// </summary>
        public YoutubeDLController Controller = YoutubeDLController.Instance();

        /// <summary>
        ///     Initializes a new instance of the <see cref="YoutubeDLGui.MainWindow" /> class.
        /// </summary>
        public MainWindow()
            : base(WindowType.Toplevel)
        {
            Build();
        }

        /// <summary>
        ///     Raises the delete event event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="a">The alpha component.</param>
        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }

        /// <summary>
        ///     Raises the proxy check button toggled event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnProxyCheckButtonToggled(object sender, EventArgs e)
        {
            proxyUrlTextView.Sensitive = proxyUrlTextView.Visible = proxyCheckButton.Active;
        }

        /// <summary>
        ///     Raises the socket timeout check button toggled event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnSocketTimeoutCheckButtonToggled(object sender, EventArgs e)
        {
            socketTimeoutTextView.Sensitive = socketTimeoutTextView.Visible = socketTimeoutCheckButton.Active;
        }

        /// <summary>
        ///     Raises the rate limit check button toggled event.*/
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnRateLimitCheckButtonToggled(object sender, EventArgs e)
        {
            rateTextView.Sensitive = rateTextView.Visible = rateLimitCheckButton.Active;
            rateUnitComboBox.Sensitive = rateUnitComboBox.Visible = rateLimitCheckButton.Active;
        }

        /// <summary>
        ///     Raises the retries check button toggled event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnRetriesCheckButtonToggled(object sender, EventArgs e)
        {
            retriesTextView.Sensitive = retriesTextView.Visible = retriesCheckButton.Active;
        }

        /// <summary>
        ///     Raises the username text view key release event event.
        /// </summary>
        /// <param name="o">O.</param>
        /// <param name="args">Arguments.</param>
        protected void OnUsernameTextViewKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            Controller.Username = usernameTextView.Buffer.Text.Length == 0 ? string.Empty : usernameTextView.Buffer.Text;
        }

        /// <summary>
        ///     Raises the password text view key release event event.
        /// </summary>
        /// <param name="o">O.</param>
        /// <param name="args">Arguments.</param>
        protected void OnPasswordTextViewKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            ProcessPasswordText(passwordTextView, _userPassword, args);
            Controller.Password = new string(_userPassword.ToArray());
        }

        /// <summary>
        ///     Raises the video password text view key release event event.
        /// </summary>
        /// <param name="o">O.</param>
        /// <param name="args">Arguments.</param>
        protected void OnVideoPasswordTextViewKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            ProcessPasswordText(videoPasswordTextView, _videoPassword, args);
            Controller.VideoPassword = new string(_videoPassword.ToArray());
        }

        /// <summary>
        ///     Raises the username text view focus out event event.
        /// </summary>
        /// <param name="o">O.</param>
        /// <param name="args">Arguments.</param>
        protected void OnUsernameTextViewFocusOutEvent(object o, FocusOutEventArgs args)
        {
            if (string.IsNullOrEmpty(usernameTextView.Buffer.Text))
            {
                usernameTextView.Buffer.Text = "Username";
            }
        }

        /// <summary>
        ///     Raises the password text view focus out event event.
        /// </summary>
        /// <param name="o">O.</param>
        /// <param name="args">Arguments.</param>
        protected void OnPasswordTextViewFocusOutEvent(object o, FocusOutEventArgs args)
        {
            if (string.IsNullOrEmpty(passwordTextView.Buffer.Text))
            {
                passwordTextView.Buffer.Text = "Password";
            }
        }

        /// <summary>
        ///     Raises the password check button toggled event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnPasswordCheckButtonToggled(object sender, EventArgs e)
        {
            if (_userPassword.Count > 0)
            {
                passwordTextView.Buffer.Text = passwordCheckButton.Active
                    ? new string(_userPassword.ToArray())
                    : new string('*', _userPassword.Count);
            }
        }

        /// <summary>
        ///     Processes the password text.
        /// </summary>
        /// <param name="textView">Text view.</param>
        /// <param name="localPassword">Local password.</param>
        /// <param name="args">Arguments.</param>
        protected void ProcessPasswordText(TextView textView, List<char> localPassword, KeyReleaseEventArgs args)
        {
            if (textView.Buffer.Text.Length == 0)
            {
                localPassword.Clear();
            }
            else
            {
                if (args.Event.KeyValue <= 32 || args.Event.KeyValue >= 123)
                    return;
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

                textView.Buffer.Text = passwordCheckButton.Active
                    ? new string(localPassword.ToArray())
                    : new string('*', localPassword.Count);
            }
        }

        /// <summary>
        ///     Raises the video password check button toggled event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnVideoPasswordCheckButtonToggled(object sender, EventArgs e)
        {
            if (_videoPassword.Count > 0)
            {
                videoPasswordTextView.Buffer.Text = videoPasswordCheckButton.Active
                    ? new string(_videoPassword.ToArray())
                    : new string('*', _videoPassword.Count);
            }
        }

        /// <summary>
        ///     Raises the audio quality combo box changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnAudioQualityComboBoxChanged(object sender, EventArgs e)
        {
            audioQualityTextView.Sensitive = audioQualityTextView.Visible = audioQualityComboBox.Active == 10;
        }

        /// <summary>
        ///     Raises the exec check button toggled event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnExecCheckButtonToggled(object sender, EventArgs e)
        {
            execTextView.Sensitive = execTextView.Visible = execCheckButton.Active;
        }

        /// <summary>
        ///     Choose a folder.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnFolderChooserButtonClicked(object sender, EventArgs e)
        {
            using (var folderChooserDialog = new FolderChooserDialog())
            {
                var returnValue = folderChooserDialog.Run();
                if (returnValue == (int)ResponseType.Ok)
                {
                    destinationFolderTextView.Buffer.Text = folderChooserDialog.Folder +
                    System.IO.Path.DirectorySeparatorChar +
                    "%(uploader)s/%(title)s-%(id)s.%(ext)s";
                }
                folderChooserDialog.Destroy();
            }
        }

        /// <summary>
        ///     Begin download process.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnDownloadButtonClicked(object sender, EventArgs e)
        {
            var youtubeDlController = YoutubeDLController.Instance();

            youtubeDlController.VideoUrl = videoUrlTextView.Buffer.Text;
            youtubeDlController.Output = destinationFolderTextView.Buffer.Text;

            youtubeDlController.AbortOnError = abortCheckButton.Active;
            youtubeDlController.IgnoreConfig = ignoreConfigCheckButton.Active;

            youtubeDlController.ProxyUrl = proxyCheckButton.Active ? proxyUrlTextView.Buffer.Text : string.Empty;
            if (socketTimeoutCheckButton.Active)
            {
                int socket;
                youtubeDlController.SocketTimeout = int.TryParse(socketTimeoutTextView.Buffer.Text, out socket)
                    ? socket
                    : 0;
            }
            else
            {
                youtubeDlController.SocketTimeout = 0;
            }

            if (rateLimitCheckButton.Active)
            {
                double rateLimit;
                youtubeDlController.RateLimit = double.TryParse(rateTextView.Buffer.Text, out rateLimit) ? rateLimit : 0;
                youtubeDlController.RateLimitUnit =
                    (YoutubeDLController.ByteUnit)
                        Enum.Parse(typeof(YoutubeDLController.ByteUnit), rateUnitComboBox.ActiveText);
            }
            else
            {
                youtubeDlController.RateLimit = 0;
            }

            if (retriesCheckButton.Active)
            {
                int retries;
                youtubeDlController.Retries = int.TryParse(retriesTextView.Buffer.Text, out retries) ? retries : 0;
            }
            else
            {
                youtubeDlController.Retries = 10;
            }

            youtubeDlController.NoOverwrites = overwritesCheckButton.Active;

            if (defaultContinueRadioButton.Active)
            {
                youtubeDlController.Continue = false;
                youtubeDlController.NoContinue = false;
            }
            else if (continueRadioButton.Active)
            {
                youtubeDlController.Continue = true;
                youtubeDlController.NoContinue = false;
            }
            else if (noContinueRadioButton.Active)
            {
                youtubeDlController.Continue = false;
                youtubeDlController.NoContinue = true;
            }

            var username = usernameTextView.Buffer.Text.Trim();
            if (!username.Equals("Username") && !string.IsNullOrWhiteSpace(username))
            {
                youtubeDlController.Username = username;
                youtubeDlController.Password = passwordTextView.Buffer.Text;

                var twoFactor = twoFactorTextView.Buffer.Text.Trim();
                if (!twoFactor.Equals("Two-factor auth code") && !string.IsNullOrWhiteSpace(twoFactor))
                {
                    youtubeDlController.TwoFactor = twoFactor;
                }
            }

            var videoPassword = videoPasswordTextView.Buffer.Text.Trim();
            if (!videoPassword.Equals("Video password") && !string.IsNullOrWhiteSpace(videoPassword))
            {
                youtubeDlController.VideoPassword = videoPassword;
            }

            youtubeDlController.ExtractAudio = extractAudioCheckButton.Active;

            youtubeDlController.AudioFormat = (YoutubeDLController.AudioFormatType)audioFormatComboBox.Active;

            youtubeDlController.AudioQuality = audioQualityComboBox.Active;

            int customAudioQuality;
            youtubeDlController.CustomAudioQuality = int.TryParse(audioQualityTextView.Buffer.Text,
                out customAudioQuality)
                ? customAudioQuality
                : 0;

            if (recodeVideoFormatComboBox.Active > 0)
            {
                youtubeDlController.RecodeVideo = true;
                youtubeDlController.RecodeVideoFormat =
                    (YoutubeDLController.VideoFormatType)
                        (Enum.Parse(typeof(YoutubeDLController.VideoFormatType),
                    recodeVideoFormatComboBox.ActiveText.ToLower()));
            }

            youtubeDlController.KeepVideo = keepVideoCheckButton.Active;
            youtubeDlController.NoPostOverwrites = noPostOverwritesCheckButton.Active;
            youtubeDlController.EmbedSubs = embedSubsCheckButton.Active;
            youtubeDlController.EmbedThumbnail = embedThumbnailCheckButton.Active;
            youtubeDlController.AddMetadata = addMetadataCheckButton.Active;
            youtubeDlController.XAttrs = xattrsCheckButton.Active;
            youtubeDlController.Fixup = (YoutubeDLController.FixupPolicy)fixupComboBox.Active;
            youtubeDlController.Cmd = execCheckButton.Active ? execTextView.Buffer.Text : string.Empty;

            var progressDialog = new ProgressDialog(youtubeDlController);
            if ((ResponseType)progressDialog.Run() == ResponseType.Cancel)
            {
                try
                {
                    youtubeDlController.KillProcess();
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
        ///     Collapses all other expanders
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnExpanderActivated(object sender, EventArgs e)
        {
            var thisExpander = (Expander)sender;
            foreach (var expander in vbox1.Children.OfType<Expander>().Where(expander => expander != thisExpander))
            {
                expander.Expanded = false;
            }
        }

        /// <summary>
        /// Raises the exit action activated event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnExitActionActivated(object sender, EventArgs e)
        {
            Application.Quit();
        }

        /// <summary>
        /// Raises the use embedded binary toggled event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnUseEmbeddedYoutubeDlBinaryActionToggled(object sender, EventArgs e)
        {
            YoutubeDLController.Instance().UseEmbeddedBinary = ((Gtk.ToggleAction)sender).Active;
        }
    }
}