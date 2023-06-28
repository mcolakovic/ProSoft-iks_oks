using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Common;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace Server
{
    public class Server
    {
        Socket serverSocket;
        List<ClientHandler> clients = new List<ClientHandler>();
        public List<ClientHandler> Clients { get => clients; }
        int maxKorisnika = 2;
        bool isRunning = false;
        public void Start()
        {
            if (!isRunning)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));
                serverSocket.Listen(5);
                isRunning = true;
            }
        }
        public void Stop()
        {
            if (isRunning)
            {
                serverSocket.Dispose();
                serverSocket = null;
                isRunning = false;
            }
        }

        public void HandleClients()
        {
            try
            {
                while (true)
                {
                    Socket clientSocket = serverSocket.Accept();
                    ClientHandler handler = new ClientHandler(clientSocket, clients);
                    clients.Add(handler);
                    Thread klijentskaNit = new Thread(handler.HandleRequests);
                    handler.PrijavljenKorisnik += Handler_PrijavljenKorisnik;
                    handler.OdjavljenKorisnik += Handler_OdjavljenKorisnik;
                    klijentskaNit.IsBackground = true;
                    klijentskaNit.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Handler_OdjavljenKorisnik(object sender, EventArgs e)
        {
            clients.Remove((ClientHandler)sender);
            if(clients.Count == 0)
            {
                Start();
                Thread serverskaNit = new Thread(HandleClients);
                serverskaNit.IsBackground = true;
                serverskaNit.Start();
            }
        }

        private void Handler_PrijavljenKorisnik(object sender, EventArgs e)
        {
            if(clients.Count == maxKorisnika)
            {
                Stop();
                clients[0].Helper.Send(new Request { Operations = Operations.ZapocniIgru });
                clients[1].Helper.Send(new Request { Operations = Operations.Pratiigru });
            }
        }
    }
}
