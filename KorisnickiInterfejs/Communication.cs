using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KorisnickiInterfejs
{
    public class Communication
    {
        Socket socket;
        CommunicationHelper helper;
        private static Communication instance;
        private Communication()
        {
        }
        public static Communication Instance
        {
            get
            {
                if (instance == null) instance = new Communication();
                return instance;
            }
        }

        public void Connect()
        {
            try
            {
                if (socket == null || !socket.Connected)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect("127.0.0.1", 9999);
                    helper = new CommunicationHelper(socket);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CancelConnection()
        {
            try
            {
                Request request = new Request {Operations = Operations.EndCommunication };
                helper.Send(request);
                socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
                socket = null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SendMessage<T>(T message) where T : class
        {
            try
            {
                helper.Send<T>(message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T ReadMessage<T>() where T : class
        {
            try
            {
                return helper.Receive<T>();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
