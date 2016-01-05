using System;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;

namespace PancakePrinter
{
    /// <summary>
    /// **** Controllers Class ****
    /// HID Controller devices - XBox controller
    ///   Data transfer helpers: message parsers, direction to motor value translatores, etc.
    /// </summary>
    public class Controllers
    {
        public static bool FoundLocalControlsWorking = false;

        private static XboxHidController controller;
        private static int lastControllerCount = 0;

        private static PancakeDrive pancakeDrive = new PancakeDrive();

        public static async void XboxJoystickInit()
        {
            string deviceSelector = HidDevice.GetDeviceSelector(0x01, 0x05);
            DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(deviceSelector);

            if (deviceInformationCollection.Count == 0)
            {
                Debug.WriteLine("No Xbox360 controller found!");
            }
            lastControllerCount = deviceInformationCollection.Count;

            foreach (DeviceInformation d in deviceInformationCollection)
            {
                Debug.WriteLine("Device ID: " + d.Id);

                HidDevice hidDevice = await HidDevice.FromIdAsync(d.Id, Windows.Storage.FileAccessMode.Read);

                if (hidDevice == null)
                {
                    try
                    {
                        var deviceAccessStatus = DeviceAccessInformation.CreateFromId(d.Id).CurrentStatus;

                        if (!deviceAccessStatus.Equals(DeviceAccessStatus.Allowed))
                        {
                            Debug.WriteLine("DeviceAccess: " + deviceAccessStatus.ToString());
                            FoundLocalControlsWorking = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Xbox init - " + e.Message);
                    }

                    Debug.WriteLine("Failed to connect to the controller!");
                }

                controller = new XboxHidController(hidDevice);
                controller.DirectionChanged += Controller_DirectionChanged;
            }
        }

        public static async void XboxJoystickCheck()
        {
            string deviceSelector = HidDevice.GetDeviceSelector(0x01, 0x05);
            DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(deviceSelector);
            if (deviceInformationCollection.Count != lastControllerCount)
            {
                lastControllerCount = deviceInformationCollection.Count;
                XboxJoystickInit();
            }
        }

        private static void Controller_DirectionChanged(ControllerVector sender)
        {
            FoundLocalControlsWorking = true;
            Debug.WriteLine("Direction: " + sender.Direction + ", Magnitude: " + sender.Magnitude);
            GamepadToPancakePrinter((sender.Magnitude < 2500) ? ControllerDirection.None : sender.Direction, sender.Magnitude);
        }

        static void GamepadToPancakePrinter(ControllerDirection dir, int magnitude)
        {
            switch (dir)
            {
                case ControllerDirection.Down:
                    pancakeDrive.SetY(magnitude * -1);
                    pancakeDrive.SetX(0);
                    break;
                case ControllerDirection.Up:
                    pancakeDrive.SetY(magnitude);
                    pancakeDrive.SetX(0);
                    break;
                case ControllerDirection.Left:
                    pancakeDrive.SetX(magnitude * -1);
                    pancakeDrive.SetY(0);
                    break;
                case ControllerDirection.Right:
                    pancakeDrive.SetX(magnitude);
                    pancakeDrive.SetY(0);
                    break;
                case ControllerDirection.DownLeft:
                    pancakeDrive.SetX(magnitude * -1);
                    pancakeDrive.SetY(magnitude * -1);
                    break;
                case ControllerDirection.DownRight:
                    pancakeDrive.SetX(magnitude);
                    pancakeDrive.SetY(magnitude * -1);
                    break;
                case ControllerDirection.UpLeft:
                    pancakeDrive.SetX(magnitude * -1);
                    pancakeDrive.SetY(magnitude);
                    break;
                case ControllerDirection.UpRight:
                    pancakeDrive.SetX(magnitude);
                    pancakeDrive.SetY(magnitude);
                    break;
                default:
                    pancakeDrive.SetX(0);
                    pancakeDrive.SetY(0);
                    break;
            }
        }
    }
}
