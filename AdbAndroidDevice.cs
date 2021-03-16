using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidUSBTest
{
    public class AdbAndroidDevice : IAndroidDevice
    {
        private const int Port = 6000;
        private static readonly IPAddress LocalAddr = IPAddress.Parse("127.0.0.1");
        private static readonly TcpListener Server = new TcpListener(LocalAddr, Port);
        private NetworkStream Stream;

        public static List<IAndroidDevice> GetAdbAndroidDevices()
        {
            // ToDo: What if multiple Android devices
            // We have to tell the phone to use another port?

            try
            {
                Server.Start();
                Debug.WriteLine("Waiting for ADB connection");

                // Buffer for reading data
                
                List<IAndroidDevice> devices = new List<IAndroidDevice>();
                TcpClient client = Server.AcceptTcpClient();
                Debug.WriteLine("Made ADB connection");

                byte[] bytes = ReceiveBytes(client.GetStream());

                bool isCamoDevice = false;
                var data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Debug.WriteLine(data);
                
                string howDoWeKnow = "idk";
                if (data == howDoWeKnow || true)
                    isCamoDevice = true;
                if (isCamoDevice)
                    devices.Add(new AdbAndroidDevice(client, new List<string>{"A" , "D" , "B"}));

                return devices;
            }
            catch (SocketException e)
            {
                // ToDo: Do something here?
                throw;
            }
            finally
            {
                // Server.Stop(); // When should I do this?
            }
        }

        private static byte[] ReceiveBytes(NetworkStream stream)
        {
            byte[] bytes = new byte[256];
            stream.Read(bytes, 0, bytes.Length);
            
            return bytes;
        }

        private readonly TcpClient Client;

        private AdbAndroidDevice(TcpClient client, List<string> hardWareIds)
        {
            Client = client;
            HardwareIds = hardWareIds;
            Stream = client.GetStream();
        }
        public void Configure(IAndroidDevice device)
        {
            // Do I need to do anything here?
        }

        public bool IsDeviceConfigured()
        {
            return true;
        }

        public (byte[] bytes, int errorCode) Receive(int timeout)
        {
            var bytes = ReceiveBytes(Stream);
            return (bytes, 0);
        }

        public async Task<int> Send(byte[] bytes, int timeout)
        {
            Task writeTask = Stream.WriteAsync(bytes, 0, bytes.Length);
            await writeTask;
            return writeTask.IsCompletedSuccessfully ? 0 : 1;
        }

        public List<string> HardwareIds { get; set; }
        public int ReadBufferSize { get; set; }

        public static bool IsADBInstalled()
        {
            return true;
        }
    }
    
    public interface IAndroidDevice
    {
        /*string Name { get; }
        int ProductId { get; }
        int VendorId { get; }
        string Path { get; }
        bool SupportsDriver { get; }
        bool IsAndroidDevice { get; }
        bool IsInAccessoryMode { get; }
        bool CanOpen { get; }*/
        void Configure(IAndroidDevice device); // eg put in accessory mode
        bool IsDeviceConfigured();
        (byte[] bytes, int errorCode) Receive(int timeout);
        Task<int> Send(byte[] bytes, int timeout);
        List<string> HardwareIds { get; set; }
        int ReadBufferSize { get; set; }
    }
}