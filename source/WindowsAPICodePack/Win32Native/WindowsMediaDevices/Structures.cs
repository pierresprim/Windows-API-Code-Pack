﻿using MS.WindowsAPICodePack.Win32Native.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsAPICodePack.Win32Native.MediaDevices.ClientInterfaces
{
    /// <summary>
    /// The WMDMID structure describes serial numbers and group IDs.
    /// </summary>
    public struct WMDMID
    {
        /// <summary>
        /// Size of the <see cref="WMDMID"/> structure, in bytes.
        /// </summary>
        public ushort cbSize;
        /// <summary>
        /// <see cref="uint"/> containing the registered ID number of the vendor. Contains zero if not in use.
        /// </summary>
        public uint dwVendorID;
        /// <summary>
        /// An array of bytes containing the serial number. The serial number is a string of byte values that have no standard format. Note that this is not a wide-character value. <see cref="Consts.WMDMID_LENGTH"/> is a constant value defined in mswmdm.h in the Windows SDK.
        /// </summary>
        public char[] pID;
        /// <summary>
        /// Actual length of the serial number returned, in bytes.
        /// </summary>
        public ushort SerialNumberLength;
    }

    /// <summary>
    /// The <see cref="WAVEFORMATEX"/> structure defines the format of waveform-audio data.
    /// </summary>
    public struct WAVEFORMATEX
    {
        /// <summary>
        /// Must be set to a format or formats supported by the device. Note that previous versions of the Windows Media Device Manager recommended using WMDM_WAVE_FORMAT_ALL to indicate support for all formats. However, this is no longer recommended, as different media players will interpret this in different ways, and few devices can truly play any file format. It is now recommended that you use the <see cref="PropValidValuesForm.Any"/> value of the <see cref="PropValidValuesForm"/> enumeration, or better yet specify a range of formats with the <see cref="WMDM_PROP_VALUES_RANGE"/> structure.
        /// </summary>
        public PropValidValuesForm wFormatTag;
        public ushort nChannels;
        public uint nSamplesPerSec;
        public uint nAvgBytesPerSec;
        public ushort nBlockAlign;
        public ushort wBitsPerSample;
        public ushort cbSize;
    }

    /// <summary>
    /// The <see cref="WMDM_PROP_VALUES_RANGE"/> structure describes a range of valid values for a particular property in a particular property configuration.
    /// </summary>
    /// <remarks>This structure is used in the <see cref="WMDM_PROP_DESC"/> structure to describe a range of valid values. A range of valid values is applicable when <see cref="PropValidValuesForm.Enum"/> is selected from the <see cref="PropValidValuesForm"/> enumeration.</remarks>
    /// <seealso cref="IWMDMDevice3.GetFormatCapability"/>
    /// <seealso cref="PropValidValuesForm"/>
    /// <seealso cref="WMDM_FORMAT_CAPABILITY"/>
    /// <seealso cref="WMDM_PROP_CONFIG"/>
    /// <seealso cref="WMDM_PROP_DESC"/>
    /// <seealso cref="WMDM_PROP_VALUES_ENUM"/>
    /// <seealso href="https://docs.microsoft.com/fr-fr/windows/win32/wmdm/structures"/>
    public struct WMDM_PROP_VALUES_RANGE
    {
        /// <summary>
        /// Minimum value in the range.
        /// </summary>
        public PropVariant rangeMin;
        /// <summary>
        /// Maximum value in the range.
        /// </summary>
        public PropVariant rangeMax;
        /// <summary>
        /// The step size in which valid values increment from the minimum value to the maximum value.This permits specifying discrete permissible values in a range.
        /// </summary>
        public PropVariant rangeStep;
    }

    /// <summary>
    /// The <see cref="WMDM_PROP_DESC"/> structure describes valid values of a property in a particular property configuration.
    /// </summary>
    /// <remarks><para>The <see cref="WMDM_PROP_DESC"/> structure contains a property description that consists of a property name and its valid values in a particular configuration.</para>
    /// <para>The caller is required to free the memory used by <see cref="WMDM_PROP_DESC_ValidValues.ValidValuesRange"/> or <see cref="WMDM_PROP_DESC_ValidValues.EnumeratedValidValues"/>. For an example of how to do this, see <see cref="WMDM_FORMAT_CAPABILITY"/>.</para></remarks>
    public struct WMDM_PROP_DESC
    {
        /// <summary>
        /// Name of the property. The application must free this memory when it is done using it.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszPropName;
        /// <summary>
        /// A <see cref="PropValidValuesForm"/> enumeration value describing the type of values, such as a range or list. The value of this enumeration determines which member variable is used.
        /// </summary>
        public PropValidValuesForm ValidValuesForm;
        /// <summary>
        /// See <see cref="WMDM_PROP_DESC_ValidValues"/>.
        /// </summary>
        public WMDM_PROP_DESC_ValidValues ValidValues;
    }

    /// <summary>
    /// Holds the valid values of the property in a particular property configuration. This member holds one of three items: the enumeration value <see cref="PropValidValuesForm.Any"/>; the member <see cref="ValidValuesRange"/>; or the member <see cref="EnumeratedValidValues"/>. The value or member is indicated by <see cref="WMDM_PROP_DESC.ValidValuesForm"/>.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct WMDM_PROP_DESC_ValidValues

    {

        /// <summary>
        /// A <see cref="WMDM_PROP_VALUES_RANGE"/> structure containing a range of valid values. This is present only when <see cref="WMDM_PROP_DESC.ValidValuesForm"/> is set to <see cref="PropValidValuesForm.Range"/>. See the Remarks section of the <see cref="WMDM_PROP_DESC"/> structure.
        /// </summary>
        [FieldOffset(0)]
        public WMDM_PROP_VALUES_RANGE ValidValuesRange;
        /// <summary>
        /// A <see cref="WMDM_PROP_VALUES_ENUM"/> structure containing an enumerated set of valid values. This is present only when <see cref="WMDM_PROP_DESC.ValidValuesForm"/> is set to <see cref="PropValidValuesForm.Enum"/>. See the Remarks section of the <see cref="WMDM_PROP_DESC"/> structure.
        /// </summary>
        [FieldOffset(0)]
        public WMDM_PROP_VALUES_ENUM EnumeratedValidValues;

    }
}