﻿// COPYRIGHT 2013 by the Open Rails project.
// 
// This file is part of Open Rails.
// 
// Open Rails is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Open Rails is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Open Rails.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ORTS.Common
{
    /// <summary>
    /// Static class which provides version and build information about the whole game.
    /// </summary>
    public static class VersionInfo
    {
        static readonly string ApplicationPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public static readonly string Version = GetVersion("Version.txt");    // e.g. Release: "0.9.0.1648", experimental: "X.1649", local: ""
        public static readonly string Revision = GetRevision("Revision.txt"); // e.g. Release: "1648",       experimental: "1649",   local: ""
        public static readonly string Build = GetBuild("ORTS.Common.dll", "OpenRails.exe", "Menu.exe", "RunActivity.exe"); // e.g. "0.0.5223.24629 (2014-04-20 13:40:58Z)"
        public static readonly string VersionOrBuild = GetVersionOrBuild();   // Version, but if "", returns Build

        static string GetRevision(string fileName)
        {
            try
            {
                using (var f = new StreamReader(Path.Combine(ApplicationPath, fileName)))
                {
                    var revision = f.ReadLine().Trim();
                    if (revision.StartsWith("$Revision:") && revision.EndsWith("$") && !revision.Contains(" 000 "))
                        return revision.Substring(10, revision.Length - 11).Trim();
                }
            }
            catch
            {
            }
            return "";
        }

        static string GetVersion(string fileName)
        {
            try
            {
                using (var f = new StreamReader(Path.Combine(ApplicationPath, fileName)))
                {
                    var version = f.ReadLine();
                    if (!String.IsNullOrEmpty(Revision))
                        return version + Revision;
                }
            }
            catch
            {
            }
            return "";
        }

        static string GetBuild(params string[] fileNames)
        {
            var builds = new Dictionary<TimeSpan, string>();
            foreach (var fileName in fileNames)
            {
                try
                {
                    var version = FileVersionInfo.GetVersionInfo(Path.Combine(ApplicationPath, fileName));
                    builds.Add(new TimeSpan(version.ProductBuildPart, 0, 0, version.ProductPrivatePart * 2), version.ProductVersion);
                }
                catch
                {
                }
            }
            if (builds.Count > 0)
            {
                var datetime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var timespan = builds.Keys.OrderBy(ts => ts).Last();
                return String.Format("{0} ({1:u})", builds[timespan], datetime + timespan);
            }
            return "";
        }

        static string GetVersionOrBuild()
        {
            return Version.Length > 0 ? Version : Build;
        }
    }
}
