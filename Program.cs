using System;

namespace AndroidUSBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing Android");
            AdbAndroidDevice.GetAdbAndroidDevices();
        }
    }
}