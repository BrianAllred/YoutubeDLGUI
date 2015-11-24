//
// ExceptionDialog.cs
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

#region Usings

using System;
using Gtk;

#endregion

namespace YoutubeDLGui
{
    /// <summary>
    ///     Class to show exception dialogs.
    /// </summary>
    public partial class ExceptionDialog : Dialog
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YoutubeDLGui.ExceptionDialog" /> class.
        /// </summary>
        private ExceptionDialog()
        {
            Build();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YoutubeDLGui.ExceptionDialog" /> class.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="exceptionMessage">Exception message.</param>
        public ExceptionDialog(string title, string exceptionMessage)
            : this()
        {
            frame1.Label = title;
            textview2.Buffer.Text = exceptionMessage;
        }

        /// <summary>
        ///     Raises the button ok clicked event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            Destroy();
        }
    }
}