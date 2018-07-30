using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// A high-level wrapper class intended to provide similar functionality to 
    /// that of C#'s Directory class. Directory does not handle certain filepaths 
    /// correctly (i.e. paths with extraneous periods in folder names).
    /// </summary>
    public class AbnormalDirectoryHandler
    {
        #region Methods
        /// <summary>
        /// Creates a directory at the given filepath if it does not exist. 
        /// Calls the standard Directory.CreateDirectory method if the path 
        /// is relative (i.e. starts with '/'). Replaces all instances of 
        /// '\\' with '/'. Does nothing else special, and calls the standard
        /// Directory.CreateDirectory after this change.
        /// </summary>
        /// 
        /// <param name="filepath">
        /// The filepath to generate a directory at.
        /// </param>
        public static void CreateDirectory(string filepath)
        {
            if (filepath.StartsWith("/"))
            {
                Directory.CreateDirectory(filepath);
            }
            else
            {
                filepath = filepath.Replace('\\', '/');
                string[] directories = filepath.Split('/');
                string directory = "";
                for (int i = 0; i < directories.Length; i++)
                {
                    if (directory.EndsWith(":"))
                    {
                        // Account for the beginning of Windows directories (C://)
                        directory = directory + Path.DirectorySeparatorChar + directories[i];
                    }
                    else
                    {
                        directory = Path.Combine(directory, directories[i]);
                    }
                }

                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// The same as CreateDirectory, though aimed for files rather than 
        /// folders.
        /// </summary>
        /// 
        /// <param name="filepath">
        /// The file filepath to generate a directory for.
        /// </param>
        public static void CreateDirectoryFromFile(string filepath)
        {
            filepath = filepath.Replace('\\', '/');
            string[] directories = filepath.Split('/');
            string directory = "";
            for (int i = 0; i < directories.Length - 1; i++)
            {
                if (directory.EndsWith(":"))
                {
                    // Account for the beginning of Windows directories (C://)
                    directory = directory + Path.DirectorySeparatorChar + directories[i];
                }
                else
                {
                    directory = Path.Combine(directory, directories[i]);
                }
            }

            Directory.CreateDirectory(directory);
        }
        #endregion
    }
}