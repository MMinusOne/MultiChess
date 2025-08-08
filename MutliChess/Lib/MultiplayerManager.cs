using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

namespace MultiChess.Lib
{
    public class MultiplayerManager
    {
        public MultiplayerManager()
        {
            _instance = this;
            this.Connect();
        }

        static MultiplayerManager _instance;
        public static MultiplayerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MultiplayerManager();
                }
                return _instance;
            }
        }

        Socket _socket;

        public void Connect()
        {
            //_socket = IO.Socket("http://localhost:4501");

            //_socket.On(Socket.EVENT_CONNECT, () =>
            //{
            //    Console.WriteLine("hi");
            //});

            //_socket.On(Socket.EVENT_CONNECT_ERROR, (error) =>
            //{
            //    Console.WriteLine("Connection Error: " + error);
            //});

        }
    }
}
