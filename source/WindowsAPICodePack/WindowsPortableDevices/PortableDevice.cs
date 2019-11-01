﻿using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native.Core;
using Microsoft.WindowsAPICodePack.Win32Native.PortableDevices.ClientInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAPICodePack.WindowsPortableDevices
{

    public class PortableDeviceManager : IPortableDeviceManager
    {

        internal Microsoft.WindowsAPICodePack.Win32Native.PortableDevices.ClientInterfaces.IPortableDeviceManager _manager = null;

        private List<PortableDevice> _portableDevices = null;

        public IReadOnlyCollection<PortableDevice> PortableDevices { get; }

        private List<PortableDevice> _privatePortableDevices = null;

        public IReadOnlyCollection<PortableDevice> PrivatePortableDevices { get; }

        public PortableDeviceManager()

        {

            _manager = new Microsoft.WindowsAPICodePack.Win32Native.PortableDevices.CoClasses.ClientClasses.PortableDeviceManager();

            _portableDevices = new List<PortableDevice>();

            PortableDevices = new ReadOnlyCollection<PortableDevice>(_portableDevices);

            _privatePortableDevices = new List<PortableDevice>();

            PrivatePortableDevices = new ReadOnlyCollection<PortableDevice>(_privatePortableDevices);

        }

        public void RefreshDeviceList() => Marshal.ThrowExceptionForHR((int)_manager.RefreshDeviceList());

        public void GetDevices()

        {

            uint count = 1;

            Marshal.ThrowExceptionForHR((int)_manager.GetDevices(null, ref count)); // We get the PortableDevices.

            if (count == 0)

            {

                _portableDevices.Clear(); // We found no devices anymore, so we clear the existing PortableDevices.

                return;

            }

            string[] deviceIDs = new string[count];

            Marshal.ThrowExceptionForHR((int)_manager.GetDevices(deviceIDs, ref count));

            if (count == 0)

            {

                _portableDevices.Clear(); // We found no devices anymore, so we clear the existing PortableDevices.

                return;

            }

            OnUpdatingPortableDevices(deviceIDs, false);

        }

        public void GetPrivateDevices()

        {

            uint count = 1;

            Marshal.ThrowExceptionForHR((int)_manager.GetPrivateDevices(null, ref count)); // We get the PortableDevices.

            if (count == 0)

            {

                _privatePortableDevices.Clear(); // We found no devices anymore, so we clear the existing PortableDevices.

                return;

            }

            string[] deviceIDs = new string[count];

            Marshal.ThrowExceptionForHR((int)_manager.GetPrivateDevices(deviceIDs, ref count));

            if (count == 0)

            {

                _privatePortableDevices.Clear(); // We found no devices anymore, so we clear the existing PortableDevices.

                return;

            }

            OnUpdatingPortableDevices(deviceIDs, true);

        }

        protected virtual void OnUpdatingPortableDevices(string[] deviceIDs, bool privateDevices)

        {

            List<PortableDevice> portableDevices = privateDevices ? _privatePortableDevices : _portableDevices;

            int i = 0;

            portableDevices.RemoveAll(d => !deviceIDs.Contains(d.DeviceId));

            while (deviceIDs.Length > 0)

            {

                if (portableDevices.Any(d => d.DeviceId == deviceIDs[i]))

                    continue;

                OnAddingPortableDevice(deviceIDs[i], privateDevices);

                deviceIDs[i] = null;

                i++;

            }

        }

        protected virtual void OnAddingPortableDevice(string deviceId, bool isPrivateDevice) => OnAddingPortableDevice(new PortableDevice(this, deviceId), isPrivateDevice);

        protected virtual void OnAddingPortableDevice(PortableDevice portableDevice, bool isPrivateDevice) => (isPrivateDevice ? _privatePortableDevices : _portableDevices).Add(portableDevice);

    }

    public interface IPortableDevice
    {

        PortableDeviceManager PortableDeviceManager { get; }

        string FriendlyName { get; }

        string DeviceDescription { get; }

        string Manufacturer { get; }

    }

    [DebuggerDisplay("{FriendlyName}, {DeviceDescription}, {Manufacturer}")]
    public class PortableDevice : IPortableDevice
    {

        public PortableDeviceManager PortableDeviceManager { get; internal set; }

        private Microsoft.WindowsAPICodePack.Win32Native.PortableDevices.ClientInterfaces.IPortableDevice _portableDevice = null;

        public string DeviceId { get; }

        public string FriendlyName { get; internal set; }

        public string DeviceDescription { get; internal set; }

        public string Manufacturer { get; internal set; }

        internal PortableDevice(PortableDeviceManager portableDeviceManager, string deviceId)

        {

            PortableDeviceManager = portableDeviceManager;

            DeviceId = deviceId;

            uint length = 0;

            Marshal.ThrowExceptionForHR((int)PortableDeviceManager._manager.GetDeviceFriendlyName(DeviceId, null, length));

            var stringBuilder = new StringBuilder((int)length);

            Marshal.ThrowExceptionForHR((int)PortableDeviceManager._manager.GetDeviceFriendlyName(DeviceId, stringBuilder, ref length));

            FriendlyName = stringBuilder.ToString();

            length = 0;

            Marshal.ThrowExceptionForHR((int)PortableDeviceManager._manager.GetDeviceDescription(DeviceId, null, length));

            stringBuilder = new StringBuilder((int)length);

            Marshal.ThrowExceptionForHR((int)PortableDeviceManager._manager.GetDeviceDescription(DeviceId, stringBuilder, ref length));

            FriendlyName = stringBuilder.ToString();

            length = 0;

            Marshal.ThrowExceptionForHR((int)PortableDeviceManager._manager.GetDeviceManufacturer(DeviceId, null, length));

            stringBuilder = new StringBuilder((int)length);

            Marshal.ThrowExceptionForHR((int)PortableDeviceManager._manager.GetDeviceManufacturer(DeviceId, stringBuilder, ref length));

            FriendlyName = stringBuilder.ToString();

        }

    }
}
