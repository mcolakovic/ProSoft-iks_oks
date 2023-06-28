using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
using Domain;

namespace Server
{
    public class ClientHandler
    {
        public event EventHandler PrijavljenKorisnik;
        public event EventHandler OdjavljenKorisnik;
        CommunicationHelper helper;
        public CommunicationHelper Helper { get => helper; }
        Socket socket;
        List<ClientHandler> clients;

        public ClientHandler(Socket socket, List<ClientHandler> clients)
        {
            this.socket = socket;
            this.clients = clients;
            helper = new CommunicationHelper(socket);
        }

        internal void HandleRequests()
        {
            try
            {
                Request request;
                while ((request = helper.Receive<Request>()).Operations != Operations.EndCommunication)
                {
                    Response response;
                    try
                    {
                        CreateResponse(request);
                    }
                    catch (Exception ex)
                    {
                        response = new Response
                        {
                            IsSuccessful = false,
                            MessageText = ex.Message
                        };
                        helper.Send(response);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                Stop();
            }
        }

        private void Stop()
        {
            if(socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
                socket = null;
            }
            OdjavljenKorisnik?.Invoke(this, EventArgs.Empty);
        }

        private void CreateResponse(Request request)
        {
            switch (request.Operations)
            {
                case Operations.Login:
                    PrijavljenKorisnik?.Invoke(this, EventArgs.Empty);
                    break;
                case Operations.Igra:
                    foreach (ClientHandler client in clients)
                    {
                        if(client != this)
                        {
                            client.Helper.Send(request);
                        }
                    }
                    break;
            }
        }
    }
}
