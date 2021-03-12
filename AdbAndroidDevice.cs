using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AndroidUSBTest
{
    public class AdbAndroidDevice
    {
        private static Int32 Port = 6100;
        private static  IPAddress LocalAddr = IPAddress.Parse("192.168.0.15");
        private static TcpListener Server = new TcpListener(LocalAddr, Port);

        public static List<IAndroidDevice> GetAdbAndroidDevices()
        {
            // ToDo : nee to add a try/catch/finally
            try
            {   // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-5.0
                // Start listening for client requests.
                Server.Start();

                // Buffer for reading data
                byte[] bytes = new byte[256];
                string data = null;
                List<IAndroidDevice> devices = new List<IAndroidDevice>();
                Console.WriteLine($"Waiting for connection on Ip: {LocalAddr} and Port: {Port}");
                var client = Server.AcceptTcpClient();
                Console.WriteLine("Made connection, waiting for message");
                int iteration = 0;

                bool lookForDevices = true;
                // Enter the listening loop.
                while(lookForDevices)
                {
                    iteration++;
                    data = null;
                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();
                    int i;

                    // Loop to receive all the data sent by the client 
                    // this should use the send method
                    while((i = stream.Read(bytes, 0, bytes.Length)) != 0) 
                    {
                        // some logic here to verify connection
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);
                    }
                    bool isCorrect = false;
                    /*if(isCorrect)
                        devices.Add(new AdbAndroidDevice(client));*/
                    
                    Console.WriteLine("Did not receive anything. Iteration: " + iteration);
                    Thread.Sleep(2 * 1000);
                }
                return devices;
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                throw;
            }
            finally
            {
                // Stop listening for new clients.
                Server.Stop();
            }
        }

        private TcpClient Client;

        public AdbAndroidDevice(TcpClient client)
        {
            Client = client;
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
            
            Byte[] bytes = new Byte[256];
            // Get a stream object for reading and writing
            NetworkStream stream = Client.GetStream();
            int i;

            // Loop to receive all the data sent by the client.
            while((i = stream.Read(bytes, 0, bytes.Length))!=0)
            { 
                // meh not sure
            }

            return (bytes, 0);
        }

        public int Send(byte[] bytes, int timeout)
        {
            throw new NotImplementedException();
        }

        public List<string> HardwareIds { get; set; }
        public int ReadBufferSize { get; set; }
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
        int Send(byte[] bytes, int timeout);
        List<string> HardwareIds { get; set; }
        int ReadBufferSize { get; set; }
    }
}