//
// ProgressDialog.cs
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
    using System.Text.RegularExpressions;

    using Gtk;

    using YoutubeDL;

    #endregion

    /// <summary>
    ///     Progress dialog.
    /// </summary>
    public partial class ProgressDialog : Dialog
    {
        /// <summary>
        ///     Instantiates a new <see cref="YoutubeDLGui.ProgressDialog" /> class.
        /// </summary>
        public ProgressDialog()
        {
            this.Build();

            var youtubeDLController = YoutubeDLController.Instance();

            // Redirect stdout and stderr
            youtubeDLController.StandardError += this.OnStandardError;
            youtubeDLController.StandardOutput += this.OnStandardOutput;

            var youtubeProcess = youtubeDLController.Download();

            // Context sensitive depending on whether process is still alive
            youtubeProcess.Exited += this.OnDownloadComplete;

            this.processTextView.Buffer.Text += "Running the following command:\n" + youtubeDLController.RunCommand + "\n";
        }

        /// <summary>
        ///     Gets the progress as a percent.
        /// </summary>
        /// <returns>The percent.</returns>
        /// <param name="text">Text.</param>
        private static double GetPercent(string text)
        {
            var regex = new Regex(".*?" + "([+-]?\\d*\\.\\d+)(?![-+0-9\\.])" + "(%)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regex.Match(text);

            return match.Success ? double.Parse(match.Groups[1].ToString()) : double.NaN;
        }

        /// <summary>
        ///     Close on cancel clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnButtonCancelClicked(object sender, EventArgs e)
        {
            this.Respond(ResponseType.Cancel);
            this.buttonCancel.Sensitive = false;
            this.buttonOk.Sensitive = true;
            this.processTextView.Buffer.Text += "\nProcess killed.\n";
        }

        /// <summary>
        ///     Close on ok clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnButtonOkClicked(object sender, EventArgs e)
        {
            this.Destroy();
        }

        /// <summary>
        ///     Sets button sensitivity
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnDownloadComplete(object sender, EventArgs e)
        {
            Application.Invoke(delegate
                {
                    this.buttonOk.Sensitive = true;
                    this.buttonCancel.Sensitive = false;
                });
        }

        /// <summary>
        ///     Processes the stderr.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="text">Text.</param>
        private void OnStandardError(object sender, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this.ProcessText(text);
            }
        }

        /// <summary>
        ///     Processes the stdout.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="text">Text.</param>
        private void OnStandardOutput(object sender, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            this.ProcessText(text);
            var percent = GetPercent(text);
            if (!double.IsNaN(percent))
            {
                this.processProgressBar.Fraction = percent / 100.0;
            }
        }

        /// <summary>
        ///     Displays text and scrolls to end of buffer.
        /// </summary>
        /// <param name="text">Text.</param>
        private void ProcessText(string text)
        {
            Application.Invoke(delegate
                {
                    this.processTextView.Buffer.Text += "\n" + text;
                    this.processTextView.ScrollToIter(this.processTextView.Buffer.EndIter, 0, false, 0, 0);
                });
        }
    }
}