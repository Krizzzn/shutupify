﻿using Alchemy;
using Alchemy.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Shutupify.Jukeboxes.Drivers
{
    public class WebSocket : IDisposable
    {
        WebSocketServer _server;
        List<UserContext> _clients;
        
        public const string CHROME_EXTENTION_ORIGIN = "chrome-extension://mnkmaflojambglihddgpalgbfmogokfd";

        public int Port { get { return 9971; } }

        public WebSocket()
        {
            _clients = new List<UserContext>();
            _server = new WebSocketServer(this.Port, IPAddress.Loopback)
            {
                OnReceive = OnReceive,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0),
                Origin = CHROME_EXTENTION_ORIGIN
            };

            _server.Start();
        }

        private void OnReceive(UserContext context)
        {
            if (context.DataFrame.ToString() == "PING")
                context.Send("PONG");
        }

        private void OnDisconnect(UserContext context)
        {
            _clients.Remove(context);
        }

        private void OnConnect(Alchemy.Classes.UserContext context)
        {
            _clients.Add(context);
        }

        public void SendMessage(string message)
        {
            var client = GetCurrentPlayer();
            if (client == null)
                return;
            client.Send("shutupify:" + message);
        }

        private UserContext GetCurrentPlayer() {
            return _clients.FirstOrDefault();
        }

        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_server != null)
                    {
                        _server.Stop();
                        _server.Dispose();
                    }
                }

                disposed = true;
            }
        }

        public void Dispose() // Implement IDisposable
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WebSocket() // the finalizer
        {
            Dispose(false);
        }
        #endregion
    }
}