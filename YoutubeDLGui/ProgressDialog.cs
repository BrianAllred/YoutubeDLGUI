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

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using YoutubeDL;
using System.Threading;

namespace YoutubeDLGui
{
    /// <summary>
    /// Progress dialog.
    /// </summary>
    public partial class ProgressDialog : Gtk.Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubeDLGui.ProgressDialog"/> class.
        /// </summary>
        private ProgressDialog()
        {
            this.Build();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubeDLGui.ProgressDialog"/> class.
        /// </summary>
        /// <param name="youtubeDLController">Youtube DL controller.</param>
        public ProgressDialog(YoutubeDLController youtubeDLController)
            : this()
        {
            // Redirect stdout and stderr
            youtubeDLController.StandardError += OnStandardError;
            youtubeDLController.StandardOutput += OnStandardOutput;

            var youtubeProcess = youtubeDLController.Download();

            // Context sensitive depending on whether process is still alive
            youtubeProcess.Exited += OnDownloadComplete;

            this.ProcessTextView.Buffer.Text += "Running the following command:\n" + youtubeDLController.RunCommand + "\n";
        }

        /// <summary>
        /// Sets button sensitivity
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void OnDownloadComplete(object sender, EventArgs e)
        {
            Gtk.Application.Invoke(delegate
                {
                    this.buttonOk.Sensitive = true;
                    this.buttonCancel.Sensitive = false;
                });
        }

        /// <summary>
        /// Processes the stdout.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="text">Text.</param>
        void OnStandardOutput(object sender, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this.ProcessText(text);
                double percent = this.GetPercent(text);
                if (!double.IsNaN(percent))
                {
                    this.ProcessProgressBar.Fraction = percent / 100.0;
                }
            }
        }

        /// <summary>
        /// Processes the stderr.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="text">Text.</param>
        void OnStandardError(object sender, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                this.ProcessText(text);
            }
        }

        /// <summary>
        /// Close on ok clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonOkClicked(object sender, System.EventArgs e)
        {
            this.Destroy();
        }

        /// <summary>
        /// Close on cancel clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonCancelClicked(object sender, System.EventArgs e)
        {
            this.Respond(Gtk.ResponseType.Cancel);
            this.buttonCancel.Sensitive = false;
            this.buttonOk.Sensitive = true;
            this.ProcessTextView.Buffer.Text += "\nProcess killed.\n";
        }

        /// <summary>
        /// Displays text and scrolls to end of buffer.
        /// </summary>
        /// <param name="text">Text.</param>
        private void ProcessText(string text)
        {
            Gtk.Application.Invoke(delegate
                {
                    this.ProcessTextView.Buffer.Text += "\n" + text;
                    this.ProcessTextView.ScrollToIter(this.ProcessTextView.Buffer.EndIter, 0, false, 0, 0);
                });
        }

        /// <summary>
        /// Gets the progress as a percent.
        /// </summary>
        /// <returns>The percent.</returns>
        /// <param name="text">Text.</param>
        private double GetPercent(string text)
        {
            string re1 = ".*?";   // Non-greedy match on filler
            string re2 = "([+-]?\\d*\\.\\d+)(?![-+0-9\\.])";  // Float 1
            string re3 = "(%)";   // Any Single Character 1

            Regex regex = new Regex(re1 + re2 + re3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match match = regex.Match(text);

            if (match.Success)
            {
                return double.Parse(match.Groups[1].ToString());
            }

            return double.NaN;
        }
    }
}

