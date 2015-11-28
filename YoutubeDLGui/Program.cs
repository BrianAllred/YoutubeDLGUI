//
// Program.cs
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
    using System.Reflection;

    using Gtk;

    using log4net;
    using log4net.Config;

    #endregion

    /// <summary>
    /// The main program entry point.
    /// </summary>
    internal class MainClass
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main program entry method.
        /// </summary>
        /// <param name="args">The parameter is not used.</param>
        public static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure();
                Log.Info("Log initialized.");

                Application.Init();
                MainWindow win = new MainWindow();
                win.Show();
                Application.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
            }
        }
    }
}