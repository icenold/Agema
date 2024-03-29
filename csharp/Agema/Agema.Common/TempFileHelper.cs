﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Common.Logging;

namespace Agema.Common
{
    /// <summary>
    ///     Helper methods for creating Temporary files and directories
    /// </summary>
    public static class TempFileHelper
    {
        /// <summary>
        ///     The Log (Common.Logging)
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        ///     Gets the assembly application data directory directory (e.g. C:\Users\{{user}}\AppData\Roaming\{{assemblyname}})
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyAppDataDirectory()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            var assemblyAppData = Path.Combine(appDataPath, assemblyName);

            if (!Directory.Exists(assemblyAppData))
            {
                Directory.CreateDirectory(assemblyAppData);

                Log.Info("Create directory: {assemblyAppData}");
            }

            return assemblyAppData;
        }

        public static string GetTemporaryDirectory()
        {
            var tempDir = Path.Combine(GetAssemblyAppDataDirectory(), @"tmp");

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);

                Log.Info("Create directory: {tempDir}");
            }

            return tempDir;
        }

        static char[] _invalids;


        /// <summary>Replaces characters in <c>text</c> that are not allowed in 
        /// file names with the specified replacement character.</summary>
        /// <param name="text">Text to make into a valid filename. The same string is returned if it is valid already.</param>
        /// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
        /// <param name="fancy">Whether to replace quotes and slashes with the non-ASCII characters ” and ⁄.</param>
        /// <returns>A string that can be used as a filename. If the output string would otherwise be empty, returns "_".</returns>
        /// <remarks>
        ///  Source: https://stackoverflow.com/a/25223884/386619
        /// </remarks>
        public static string MakeValidFileName(string text, char? replacement = '_', bool fancy = true)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            var invalids = _invalids ?? (_invalids = Path.GetInvalidFileNameChars());
            bool changed = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (invalids.Contains(c))
                {
                    changed = true;
                    var repl = replacement ?? '\0';
                    if (fancy)
                    {
                        if (c == '"') repl = '”'; // U+201D right double quotation mark
                        else if (c == '\'') repl = '’'; // U+2019 right single quotation mark
                        else if (c == '/') repl = '⁄'; // U+2044 fraction slash
                    }
                    if (repl != '\0')
                        sb.Append(repl);
                }
                else
                    sb.Append(c);
            }
            if (sb.Length == 0)
                return "_";
            return changed ? sb.ToString() : text;
        }
    }
}