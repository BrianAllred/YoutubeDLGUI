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
using Gtk;
using YoutubeDL;
using Key = Gdk.Key;

namespace YoutubeDLGui
{
	public partial class MainWindow : Window
	{
		private readonly List<char> _userPassword = new List<char>();
		private readonly List<char> _videoPassword = new List<char>();
		public YoutubeDLController Controller = YoutubeDLController.Instance();

		public MainWindow()
			: base(WindowType.Toplevel)
		{
			Build();
		}

		protected void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			Application.Quit();
			a.RetVal = true;
		}

		protected void OnProxyCheckButtonToggled(object sender, EventArgs e)
		{
			proxyUrlTextView.Sensitive = proxyUrlTextView.Visible = proxyCheckButton.Active;
		}

		protected void OnSocketTimeoutCheckButtonToggled(object sender, EventArgs e)
		{
			socketTimeoutTextView.Sensitive = socketTimeoutTextView.Visible = socketTimeoutCheckButton.Active;
		}

		protected void OnRateLimitCheckButtonToggled(object sender, EventArgs e)
		{
			rateTextView.Sensitive = rateTextView.Visible = rateLimitCheckButton.Active;
			rateUnitComboBox.Sensitive = rateUnitComboBox.Visible = rateLimitCheckButton.Active;
		}

		protected void OnRetriesCheckButtonToggled(object sender, EventArgs e)
		{
			retriesTextView.Sensitive = retriesTextView.Visible = retriesCheckButton.Active;
		}

		protected void OnUsernameTextViewKeyReleaseEvent(object o, KeyReleaseEventArgs args)
		{
			if (usernameTextView.Buffer.Text.Length == 0)
			{
				Controller.Username = string.Empty;
			}
			else
			{
				Controller.Username = usernameTextView.Buffer.Text;
			}
		}

		protected void OnPasswordTextViewKeyReleaseEvent(object o, KeyReleaseEventArgs args)
		{
			ProcessPasswordText(passwordTextView, _userPassword, args);
			Controller.Password = new string(_userPassword.ToArray());
		}

		protected void OnVideoPasswordTextViewKeyReleaseEvent(object o, KeyReleaseEventArgs args)
		{
			ProcessPasswordText(videoPasswordTextView, _videoPassword, args);
			Controller.VideoPassword = new string(_videoPassword.ToArray());
		}

		protected void OnUsernameTextViewFocusOutEvent(object o, FocusOutEventArgs args)
		{
			if (string.IsNullOrEmpty(usernameTextView.Buffer.Text))
			{
				usernameTextView.Buffer.Text = "Username";
			}
		}

		protected void OnPasswordTextViewFocusOutEvent(object o, FocusOutEventArgs args)
		{
			if (string.IsNullOrEmpty(passwordTextView.Buffer.Text))
			{
				passwordTextView.Buffer.Text = "Password";
			}
		}

		protected void OnPasswordCheckButtonToggled(object sender, EventArgs e)
		{
			if (_userPassword.Count > 0)
			{
				passwordTextView.Buffer.Text = passwordCheckButton.Active
                    ? new string(_userPassword.ToArray())
                    : new string('*', _userPassword.Count);
			}
		}

		protected void ProcessPasswordText(TextView textView, List<char> localPassword, KeyReleaseEventArgs args)
		{
			if (textView.Buffer.Text.Length == 0)
			{
				localPassword.Clear();
			}
			else
			{
				if (args.Event.KeyValue > 32 && args.Event.KeyValue < 123)
				{
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
		}

		protected void OnVideoPasswordCheckButtonToggled(object sender, EventArgs e)
		{
			if (_videoPassword.Count > 0)
			{
				videoPasswordTextView.Buffer.Text = videoPasswordCheckButton.Active
                    ? new string(_videoPassword.ToArray())
                    : new string('*', _videoPassword.Count);
			}
		}

		protected void OnAudioQualityComboBoxChanged(object sender, EventArgs e)
		{
			this.audioQualityTextView.Sensitive = this.audioQualityTextView.Visible = this.audioQualityComboBox.Active == 10;
		}

		protected void OnExecCheckButtonToggled(object sender, EventArgs e)
		{
			this.execTextView.Sensitive = this.execTextView.Visible = this.execCheckButton.Active;
		}

		protected void OnFolderChooserButtonClicked(object sender, EventArgs e)
		{
			using (FolderChooserDialog folderChooserDialog = new FolderChooserDialog())
			{
				int returnValue = folderChooserDialog.Run();
				if (returnValue == (int)ResponseType.Ok)
				{
					this.destinationFolderTextView.Buffer.Text = folderChooserDialog.Folder + System.IO.Path.DirectorySeparatorChar + "%(uploader)s/%(title)s-%(id)s.%(ext)s";
				}
				folderChooserDialog.Destroy();
			}
		}

		protected void OnDownloadButtonClicked(object sender, EventArgs e)
		{
			var youtubeDlController = YoutubeDLController.Instance();

			youtubeDlController.VideoUrl = this.videoUrlTextView.Buffer.Text;
			youtubeDlController.Output = this.destinationFolderTextView.Buffer.Text;

			youtubeDlController.AbortOnError = this.abortCheckButton.Active;
			youtubeDlController.IgnoreConfig = this.ignoreConfigCheckButton.Active;

			youtubeDlController.ProxyUrl = this.proxyCheckButton.Active ? this.proxyUrlTextView.Buffer.Text : string.Empty;
			if (this.socketTimeoutCheckButton.Active)
			{
				int socket;
				youtubeDlController.SocketTimeout = int.TryParse(this.socketTimeoutTextView.Buffer.Text, out socket) ? socket : 0;
			}
			else
			{
				youtubeDlController.SocketTimeout = 0;
			}

			if (this.rateLimitCheckButton.Active)
			{
				double rateLimit;
				youtubeDlController.RateLimit = double.TryParse(this.rateTextView.Buffer.Text, out rateLimit) ? rateLimit : 0;
				youtubeDlController.RateLimitUnit = (YoutubeDLController.ByteUnit)Enum.Parse(typeof(YoutubeDLController.ByteUnit), this.rateUnitComboBox.ActiveText);
			}
			else
			{
				youtubeDlController.RateLimit = 0;
			}

			if (this.retriesCheckButton.Active)
			{
				int retries;
				youtubeDlController.Retries = int.TryParse(this.retriesTextView.Buffer.Text, out retries) ? retries : 0;
			}
			else
			{
				youtubeDlController.Retries = 10;
			}

			youtubeDlController.NoOverwrites = this.overwritesCheckButton.Active;

			if (this.defaultContinueRadioButton.Active)
			{
				youtubeDlController.Continue = false;
				youtubeDlController.NoContinue = false;
			}
			else if (this.continueRadioButton.Active)
			{
				youtubeDlController.Continue = true;
				youtubeDlController.NoContinue = false;
			}
			else if (this.noContinueRadioButton.Active)
			{
				youtubeDlController.Continue = false;
				youtubeDlController.NoContinue = true;
			}

			string username = this.usernameTextView.Buffer.Text.Trim();
			if (!username.Equals("Username") && !string.IsNullOrWhiteSpace(username))
			{
				youtubeDlController.Username = username;
				youtubeDlController.Password = this.passwordTextView.Buffer.Text;

				string twoFactor = this.twoFactorTextView.Buffer.Text.Trim();
				if (!twoFactor.Equals("Two-factor auth code") && !string.IsNullOrWhiteSpace(twoFactor))
				{
					youtubeDlController.TwoFactor = twoFactor;
				}
			}

			string videoPassword = this.videoPasswordTextView.Buffer.Text.Trim();
			if (!videoPassword.Equals("Video password") && !string.IsNullOrWhiteSpace(videoPassword))
			{
				youtubeDlController.VideoPassword = videoPassword;
			}

			youtubeDlController.ExtractAudio = this.extractAudioCheckButton.Active;

			youtubeDlController.AudioFormat = (YoutubeDLController.AudioFormatType)this.audioFormatComboBox.Active;

			youtubeDlController.AudioQuality = this.audioQualityComboBox.Active;

			int customAudioQuality;
			youtubeDlController.CustomAudioQuality = int.TryParse(this.audioQualityTextView.Buffer.Text, out customAudioQuality) ? customAudioQuality : 0;

			if (this.recodeVideoFormatComboBox.Active > 0)
			{
				youtubeDlController.RecodeVideo = true;
				youtubeDlController.RecodeVideoFormat = (YoutubeDLController.VideoFormatType)(Enum.Parse(typeof(YoutubeDLController.VideoFormatType), this.recodeVideoFormatComboBox.ActiveText.ToLower()));
			}

			youtubeDlController.KeepVideo = this.keepVideoCheckButton.Active;
			youtubeDlController.NoPostOverwrites = this.noPostOverwritesCheckButton.Active;
			youtubeDlController.EmbedSubs = this.embedSubsCheckButton.Active;
			youtubeDlController.EmbedThumbnail = this.embedThumbnailCheckButton.Active;
			youtubeDlController.AddMetadata = this.addMetadataCheckButton.Active;
			youtubeDlController.XAttrs = this.xattrsCheckButton.Active;
			youtubeDlController.Fixup = (YoutubeDLController.FixupPolicy)this.fixupComboBox.Active;
			youtubeDlController.Cmd = this.execCheckButton.Active ? this.execTextView.Buffer.Text : string.Empty;
		
			youtubeDlController.Download();
		}
	}
}