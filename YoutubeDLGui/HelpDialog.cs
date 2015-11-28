//
// HelpDialog.cs
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

    using Gtk;

    #endregion

    /// <summary>
    /// The help dialog.
    /// </summary>
    public partial class HelpDialog : Dialog
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YoutubeDLGui.HelpDialog" /> class.
        /// </summary>
        public HelpDialog()
        {
            this.Build();
        }

        /// <summary>
        /// Raises the button click event.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            ConfigurationHelper.AddUpdateAppSettings(ConfigurationHelper.FirstRun, "false");
            this.Destroy();
        }
    }
}