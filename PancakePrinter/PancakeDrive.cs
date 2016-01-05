using System.Threading;
using Windows.Devices.Gpio;

namespace PancakePrinter
{
    public class PancakeDrive
    {
        // GPIO Pins to motor controllers
        static GpioPin stepperXPulse;
        static GpioPin stepperXDir;
        static GpioPin stepperXEn;

        static GpioPin stepperYPulse;
        static GpioPin stepperYDir;
        static GpioPin stepperYEn;

        // GPIO Pin to servo motor
        static GpioPin servoPulse;

        // X-Y Movement timer
        static Timer tmrPulse;

        // X-Y Movement switch
        static bool xMove = false;
        static bool yMove = false;

        public PancakeDrive()
        {
            // Initialize GPIO Controller
            var gpio = GpioController.GetDefault();

            // Set X motor pins
            stepperXPulse = GpioController.GetDefault().OpenPin(23);
            stepperXPulse.SetDriveMode(GpioPinDriveMode.Output);
            stepperXPulse.Write(GpioPinValue.Low);

            stepperXDir = gpio.OpenPin(24);
            stepperXDir.SetDriveMode(GpioPinDriveMode.Output);
            stepperXDir.Write(GpioPinValue.Low);

            stepperXEn = GpioController.GetDefault().OpenPin(18);
            stepperXEn.SetDriveMode(GpioPinDriveMode.Output);
            stepperXEn.Write(GpioPinValue.Low);

            // Set Y motor pins
            stepperYPulse = GpioController.GetDefault().OpenPin(12);
            stepperYPulse.SetDriveMode(GpioPinDriveMode.Output);
            stepperYPulse.Write(GpioPinValue.Low);

            stepperYDir = gpio.OpenPin(16);
            stepperYDir.SetDriveMode(GpioPinDriveMode.Output);
            stepperYDir.Write(GpioPinValue.Low);

            stepperYEn = GpioController.GetDefault().OpenPin(1);
            stepperYEn.SetDriveMode(GpioPinDriveMode.Output);
            stepperYEn.Write(GpioPinValue.Low);

            // Set stepper motor pin
            servoPulse = GpioController.GetDefault().OpenPin(27);
            servoPulse.SetDriveMode(GpioPinDriveMode.Output);
            servoPulse.Write(GpioPinValue.Low);

            // Initialize X-Y pulse timer
            tmrPulse = new Timer(new TimerCallback(PulseTimerTick), null, 1000, 10);
        }

        public void SetX(int speed)
        {
            if (speed > 0)
            {
                stepperXDir.Write(GpioPinValue.High);
                stepperXEn.Write(GpioPinValue.Low);
                xMove = true;

            }
            else if (speed < 0)
            {
                stepperXDir.Write(GpioPinValue.Low);
                stepperXEn.Write(GpioPinValue.Low);
                xMove = true;
            }
            else
            {
                stepperXEn.Write(GpioPinValue.High);
                xMove = false;
            }
        }

        public void SetY(int speed)
        {
            if (speed > 0)
            {
                stepperYDir.Write(GpioPinValue.High);
                stepperYEn.Write(GpioPinValue.Low);
                yMove = true;
            }
            else if (speed < 0)
            {
                stepperYDir.Write(GpioPinValue.Low);
                stepperYEn.Write(GpioPinValue.Low);
                yMove = true;
            }
            else
            {
                stepperYEn.Write(GpioPinValue.High);
                yMove = false;
            }
        }

        private static void PulseTimerTick(object state)
        {
            if (xMove)
                stepperXPulse.Write((stepperXPulse.Read() == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High));

            if (yMove)
                stepperYPulse.Write((stepperYPulse.Read() == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High));
        }
    }
}
