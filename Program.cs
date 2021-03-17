using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace AndroidUSBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int iteration = 0;
            while (true)
            {
                iteration++;
                Debug.WriteLine("Testing Android: " + iteration);
                var devices = AdbAndroidDevice.GetAdbAndroidDevices();
                if (devices.Any())
                {
                    Debug.WriteLine("Connected");
                    var device = devices.First();

                    int messageIteration = 0;

                    while (true)
                    {
                        messageIteration++;
                        byte[] bytes = Encoding.Unicode.GetBytes("Test message to Android device: " + messageIteration);
                        var result = device.Send(bytes, 2 * 1000);
                        Debug.WriteLine("Sent message " + messageIteration);
                        Thread.Sleep(2 * 1000);
                    }
                }
                Thread.Sleep(2 * 1000);
            }
        }
    }
}