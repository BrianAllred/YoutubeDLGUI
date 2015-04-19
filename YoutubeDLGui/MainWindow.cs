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
using Gdk;
using Gtk;
using YoutubeDL;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace YoutubeDLGui{
	public partial class MainWindow : Window
	{

	    public YoutubeDLController Controller = YoutubeDLController.Instance();

        private readonly List<char> _userPassword = new List<char>();
        private List<char> _videoPassword = new List<char>();

	    public MainWindow() : base(WindowType.Toplevel)
	    {
	        Build();
	    }

	    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	    {
	        Application.Quit();
	        a.RetVal = true;
	    }

		protected void OnProxyCheckButtonToggled (object sender, System.EventArgs e)
		{
			proxyUrlTextView.Sensitive = proxyCheckButton.Active;
		}

		protected void OnSocketTimeoutCheckButtonToggled (object sender, System.EventArgs e)
		{
			socketTimeoutTextView.Sensitive = socketTimeoutCheckButton.Active;
		}

		protected void OnRateLimitCheckButtonToggled (object sender, System.EventArgs e)
		{
			rateTextView.Sensitive = rateLimitCheckButton.Active;
			rateUnitComboBox.Sensitive = rateLimitCheckButton.Active;
		}

		protected void OnRetriesCheckButtonToggled (object sender, System.EventArgs e)
		{
			retriesTextView.Sensitive = retriesCheckButton.Active;
		}

		protected void OnUsernameTextViewKeyReleaseEvent (object o, KeyReleaseEventArgs args)
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

		protected void OnPasswordTextViewKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{

            if (passwordTextView.Buffer.Text.Length == 0)
            {
                _userPassword.Clear();
            }
            else
            {
                if (args.Event.KeyValue > 32 && args.Event.KeyValue < 123)
                {
                    passwordTextView.Buffer.Text = string.Empty;
                    if (args.Event.Key == Gdk.Key.BackSpace)
                    {
                        if (_userPassword.Count > 0)
                        {
                            _userPassword.RemoveAt(_userPassword.Count - 1);
                            Controller.Password = new string(_userPassword.ToArray());
                        }
                    }
                    else
                    {
                        _userPassword.Add((char) args.Event.KeyValue);
                        Controller.Password = new string(_userPassword.ToArray());
                    }

                    passwordTextView.Buffer.Text = passwordCheckButton.Active
                        ? new string(_userPassword.ToArray())
                        : new string('*', _userPassword.Count);
                }
            }
        }

		protected void OnVideoPasswordTextViewKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			throw new NotImplementedException ();
		}

		protected void OnUsernameTextViewFocusOutEvent (object o, FocusOutEventArgs args)
		{
            if (string.IsNullOrEmpty(usernameTextView.Buffer.Text))
            {
                usernameTextView.Buffer.Text = "Username";
            }
		}

		protected void OnPasswordTextViewFocusOutEvent (object o, FocusOutEventArgs args)
		{
			if (string.IsNullOrEmpty(passwordTextView.Buffer.Text))
			{
				passwordTextView.Buffer.Text = "Password";
			}
		}

		protected void OnPasswordCheckButtonToggled (object sender, EventArgs e)
		{
		    if (_userPassword.Count > 0)
		    {
		        passwordTextView.Buffer.Text = passwordCheckButton.Active
		            ? new string(_userPassword.ToArray())
		            : new string('*', _userPassword.Count);
		    }
		}
	}
}