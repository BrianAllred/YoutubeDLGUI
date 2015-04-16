//
// YoutubeDLController.cs
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

using System.Diagnostics;

// ReSharper disable InconsistentNaming
// due to following youtube-dl naming
// conventions

namespace YoutubeDL
{
    public class YoutubeDLController
    {
        /// <summary>
        ///     Audio Format Types
        /// </summary>
        public enum AudioFormatType
        {
            best,
            aas,
            vorbis,
            mp3,
            m4a,
            opus,
            wav
        }

        public enum FixupPolicy
        {
            nothing,
            warn,
            detect_or_warn
        }

        /// <summary>
        ///     Download rate units (K, M)
        /// </summary>
        public enum RateUnit
        {
            K,
            M
        }

        /// <summary>
        ///     Video Format Types
        /// </summary>
        public enum VideoFormatType
        {
            mp4,
            flv,
            ogg,
            webm,
            mkv
        }

        /// <summary>
        ///     Controller singleton
        /// </summary>
        private static readonly YoutubeDLController Controller = new YoutubeDLController();

        /// <summary>
        ///     Actual youtube-dl process
        /// </summary>
        private Process _process;

        private YoutubeDLController()
        {
        }

        public static YoutubeDLController Instance()
        {
            return Controller;
        }

        #region Properties

        #region General Options

        /// <summary>
        ///     Abort downloading of further videos (in the
        ///     playlist or the command line) if an error
        ///     occurs
        /// </summary>
        public bool AbortOnError { get; set; }

        /// <summary>
        ///     Do not read configuration files. When given
        ///     in the global configuration file /etc
        ///     /youtube-dl.conf: Do not read the user
        ///     configuration in ~/.config/youtube-
        ///     dl/config(%APPDATA%/youtube-dl/config.txt
        ///     on Windows)
        /// </summary>
        public bool IgnoreConfig { get; set; }

        /// <summary>
        ///     Do not extract the videos of a playlist,
        ///     only list them.
        /// </summary>
        public bool FlatPlaylist { get; set; }

        #endregion

        #region Network Options

        /// <summary>
        ///     Use the specified HTTP/HTTPS proxy. Pass in
        ///     an empty string for direct
        ///     connection
        /// </summary>
        public string ProxyUrl { get; set; }

        /// <summary>
        ///     Time to wait before giving up, in seconds
        /// </summary>
        public int SocketTimeout { get; set; }

        #region Experimental

        /// <summary>
        ///     Client-side IP address to bind to
        /// </summary>
        public string SourceAddress { get; set; }

        /// <summary>
        ///     Make all connections via IPv4
        /// </summary>
        public bool Ipv4 { get; set; }

        /// <summary>
        ///     Make all connections via IPv6
        /// </summary>
        public bool Ipv6 { get; set; }

        /// <summary>
        ///     Use this proxy to verify the IP address for
        ///     some Chinese sites.The default proxy
        ///     specified by ProxyUrl (or none, if the
        ///     option is not present) is used for the
        ///     actual downloading.
        /// </summary>
        public string CnVerificationProxy { get; set; }

        #endregion

        #endregion

        // TODO: video selection options

        #region Download Options

        /// <summary>
        ///     Maximum download rate in bytes per second
        ///     (e.g. 50K or 4.2M)
        /// </summary>
        public double RateLimit { get; set; }

        /// <summary>
        ///     Download rate units (none, K, or M)
        /// </summary>
        public RateUnit RateLimitUnit { get; set; }

        /// <summary>
        ///     Number of retries (default is 10), or
        ///     "infinite" (-1).
        /// </summary>
        public int Retries { get; set; }

        // TODO: finish download options

        #endregion

        #region Filesystem Options

        /// <summary>
        ///     File containing URLs to download
        /// </summary>
        public string BatchFile { get; set; }

        /// <summary>
        ///     Output filename template.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        ///     Restrict filenames to only ASCII
        ///     characters, and avoid "&" and spaces in
        ///     filenames
        /// </summary>
        public bool RestrictFilenames { get; set; }

        /// <summary>
        ///     Do not overwrite files
        /// </summary>
        public bool NoOverwrites { get; set; }

        /// <summary>
        ///     Force resume of partially downloaded files.
        /// </summary>
        public bool Continue { get; set; }

        /// <summary>
        ///     Do not resume partially downloaded files.
        /// </summary>
        public bool NoContinue { get; set; }

        /// TODO: Finish Filesystem Options
        /// <summary>
        ///     Login with this account ID
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Account password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Two-factor auth code
        /// </summary>
        public string TwoFactor { get; set; }

        /// <summary>
        ///     Use .netrc authentication data
        /// </summary>
        public bool NetRc { get; set; }

        /// <summary>
        ///     Video password
        /// </summary>
        public string VideoPassword { get; set; }

        #endregion

        #region Post-processing Options

        /// <summary>
        ///     Convert video files to audio-only files
        /// </summary>
        public bool ExtractAudio { get; set; }

        /// <summary>
        ///     Specify audio format
        /// </summary>
        public AudioFormatType AudioFormat { get; set; }

        /// <summary>
        ///     Specify audio quality between 0 and 9 inclusive
        ///     0 is better
        /// </summary>
        public int AudioQuality { get; set; }

        /// <summary>
        ///     Encode the video to another format
        /// </summary>
        public bool RecodeVideo { get; set; }

        /// <summary>
        ///     Format for video recoding
        /// </summary>
        public VideoFormatType RecodeVideoFormat { get; set; }

        /// <summary>
        ///     Keep the video file on disk after the post-processing
        /// </summary>
        public bool KeepVideo { get; set; }

        /// <summary>
        ///     Do not overwrite post-processed files
        /// </summary>
        public bool NoPostOverwrites { get; set; }

        /// <summary>
        ///     Embed subtitles in the video
        /// </summary>
        public bool EmbedSubs { get; set; }

        /// <summary>
        ///     Embed thumbnail in the audio as cover art
        /// </summary>
        public bool EmbedThumbnail { get; set; }

        /// <summary>
        ///     Write metadata to the video file
        /// </summary>
        public bool AddMetadata { get; set; }

        /// <summary>
        ///     Write metadata to the video file's xattrs
        /// </summary>
        public bool XAttrs { get; set; }

        /// <summary>
        ///     Automatically correct known faults of the file.
        /// </summary>
        public FixupPolicy Fixup { get; set; }

        #endregion

        #endregion
    }
}