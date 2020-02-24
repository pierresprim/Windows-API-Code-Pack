﻿// Copyright (c) Microsoft Corporation.  All rights reserved.

using Microsoft.WindowsAPICodePack.Win32Native;
using Microsoft.WindowsAPICodePack.Win32Native.Guids.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Shell.Resources;
using System;
using System.Diagnostics;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Creates the helper class for known folders.
    /// </summary>
    public static class KnownFolderHelper
    {
        /// <summary>
        /// Returns the native known folder (IKnownFolderNative) given a PID list
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        internal static IKnownFolderNative FromPIDL(IntPtr pidl)
        {            
            var knownFolderManager = new KnownFolderManagerClass();

            HResult hr = knownFolderManager.FindFolderFromIDList(pidl, out IKnownFolderNative knownFolder);

            return (hr == HResult.Ok) ? knownFolder : null;
        }

        /// <summary>
        /// Returns a known folder given a globally unique identifier.
        /// </summary>
        /// <param name="knownFolderId">A GUID for the requested known folder.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given Known Folder ID is invalid.</exception>
        public static IKnownFolder FromKnownFolderId(Guid knownFolderId)
        {
            var knownFolderManager = new KnownFolderManagerClass();

            HResult hr = knownFolderManager.GetFolder(knownFolderId, out IKnownFolderNative knownFolderNative);
            if (hr != HResult.Ok) throw new ShellException(hr); 

            IKnownFolder kf = GetKnownFolder(knownFolderNative);

            if (kf == null)
            
                throw new ArgumentException(LocalizedMessages.KnownFolderInvalidGuid, nameof(knownFolderId));
            
            return kf;
        }

        /// <summary>
        /// Returns a known folder given a globally unique identifier.
        /// </summary>
        /// <param name="knownFolderId">A GUID for the requested known folder.</param>
        /// <returns>A known folder representing the specified name. Returns null if Known Folder is not found or could not be created.</returns>
        internal static IKnownFolder FromKnownFolderIdInternal(Guid knownFolderId)
        {
            IKnownFolderManager knownFolderManager = new KnownFolderManagerClass();

            HResult hr = knownFolderManager.GetFolder(knownFolderId, out IKnownFolderNative knownFolderNative);

            return (hr == HResult.Ok) ? GetKnownFolder(knownFolderNative) : null;
        }

        /// <summary>
        /// Given a native KnownFolder (IKnownFolderNative), create the right type of
        /// IKnownFolder object (FileSystemKnownFolder or NonFileSystemKnownFolder)
        /// </summary>
        /// <param name="knownFolderNative">Native Known Folder</param>
        /// <returns></returns>
        private static IKnownFolder GetKnownFolder(IKnownFolderNative knownFolderNative)
        {
            Debug.Assert(knownFolderNative != null, "Native IKnownFolder should not be null.");

            // Get the native IShellItem2 from the native IKnownFolder
            var guid = new Guid(ShellIIDGuid.IShellItem2);
            HResult hr = knownFolderNative.GetShellItem(0, ref guid, out IShellItem2 shellItem);

            if (!CoreErrorHelper.Succeeded(hr)) return null; 

            bool isFileSystem = false;

            // If we have a valid IShellItem, try to get the FileSystem attribute.
            if (shellItem != null)
            {
                shellItem.GetAttributes(ShellNativeMethods.ShellFileGetAttributesOptions.FileSystem, out ShellNativeMethods.ShellFileGetAttributesOptions sfgao);

                // Is this item a FileSystem item?
                isFileSystem = (sfgao & ShellNativeMethods.ShellFileGetAttributesOptions.FileSystem) != 0;
            }

            // If it's FileSystem, create a FileSystemKnownFolder, else NonFileSystemKnownFolder
            return isFileSystem ? new FileSystemKnownFolder(knownFolderNative) : (IKnownFolder)new NonFileSystemKnownFolder(knownFolderNative);
        }

        /// <summary>
        /// Returns the known folder given its canonical name.
        /// </summary>
        /// <param name="canonicalName">A non-localized canonical name for the known folder, such as MyComputer.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given canonical name is invalid or if the KnownFolder could not be created.</exception>
        public static IKnownFolder FromCanonicalName(string canonicalName)
        {
            IKnownFolderManager knownFolderManager = new KnownFolderManagerClass();

            knownFolderManager.GetFolderByName(canonicalName, out IKnownFolderNative knownFolderNative);
            IKnownFolder kf = GetKnownFolder(knownFolderNative);

            if (kf == null)

                throw new ArgumentException(LocalizedMessages.ShellInvalidCanonicalName, nameof(canonicalName));

            return kf;
        }

        /// <summary>
        /// Returns a known folder given its shell path, such as <c>C:\users\public\documents</c> or 
        /// <c>::{645FF040-5081-101B-9F08-00AA002F954E}</c> for the Recycle Bin.
        /// </summary>
        /// <param name="path">The path for the requested known folder; either a physical path or a virtual path.</param>
        /// <returns>A known folder representing the specified name.</returns>
        public static IKnownFolder FromPath(string path) => FromParsingName(path);

        /// <summary>
        /// Returns a known folder given its shell namespace parsing name, such as 
        /// <c>::{645FF040-5081-101B-9F08-00AA002F954E}</c> for the Recycle Bin.
        /// </summary>
        /// <param name="parsingName">The parsing name (or path) for the requested known folder.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given parsing name is invalid.</exception>
        public static IKnownFolder FromParsingName(string parsingName)
        {
            if (parsingName == null)
            
                throw new ArgumentNullException(nameof(parsingName));
            
            IntPtr pidl = IntPtr.Zero;
            IntPtr pidl2 = IntPtr.Zero;

            try
            {
                pidl = ShellHelper.PidlFromParsingName(parsingName);

                if (pidl == IntPtr.Zero)
                
                    throw new ArgumentException(LocalizedMessages.KnownFolderParsingName, nameof(parsingName));
                
                // It's probably a special folder, try to get it                
                IKnownFolderNative knownFolderNative = FromPIDL(pidl);
                if (knownFolderNative != null)
                {
                    IKnownFolder kf = GetKnownFolder(knownFolderNative);
                    if (kf == null)
                    
                        throw new ArgumentException(LocalizedMessages.KnownFolderParsingName, nameof(parsingName));
                    
                    return kf;
                }

                // No physical storage was found for this known folder
                // We'll try again with a different name

                // try one more time with a trailing \0
                pidl2 = ShellHelper.PidlFromParsingName(parsingName.PadRight(1, '\0'));

                if (pidl2 == IntPtr.Zero)
                
                    throw new ArgumentException(LocalizedMessages.KnownFolderParsingName, nameof(parsingName));
                
                IKnownFolder kf2 = GetKnownFolder(FromPIDL(pidl));
                if (kf2 == null)
                
                    throw new ArgumentException(LocalizedMessages.KnownFolderParsingName, nameof(parsingName));
                
                return kf2;
            }
            finally
            {
                ShellNativeMethods.ILFree(pidl);
                ShellNativeMethods.ILFree(pidl2);
            }

        }
    }
}