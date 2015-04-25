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

        public MainWindow() : base(WindowType.Toplevel)
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
            proxyUrlTextView.Sensitive = proxyCheckButton.Active;
        }

        protected void OnSocketTimeoutCheckButtonToggled(object sender, EventArgs e)
        {
            socketTimeoutTextView.Sensitive = socketTimeoutCheckButton.Active;
        }

        protected void OnRateLimitCheckButtonToggled(object sender, EventArgs e)
        {
            rateTextView.Sensitive = rateLimitCheckButton.Active;
            rateUnitComboBox.Sensitive = rateLimitCheckButton.Active;
        }

        protected void OnRetriesCheckButtonToggled(object sender, EventArgs e)
        {
            retriesTextView.Sensitive = retriesCheckButton.Active;
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
                        localPassword.Add((char) args.Event.KeyValue);
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
    }
}