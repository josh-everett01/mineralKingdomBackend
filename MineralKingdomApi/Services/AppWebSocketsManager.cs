using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MineralKingdomApi.Services
{
    public class AppWebSocketsManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task Handle(Guid id, WebSocket webSocket)
        {
            _sockets.TryAdd(id.ToString(), webSocket);

            while (webSocket.State == WebSocketState.Open)
            {
                var message = await ReceiveStringAsync(webSocket);
                if (message != null)
                {
                    // Handle the message, e.g., broadcast to other clients
                    await BroadcastMessageAsync(message);
                }
            }

            _sockets.TryRemove(id.ToString(), out var _);
        }

        private async Task<string> ReceiveStringAsync(WebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[4096]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }

                return null;
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            foreach (var pair in _sockets)
            {
                if (pair.Value.State == WebSocketState.Open)
                {
                    await SendStringAsync(pair.Value, message);
                }
            }
        }

        private async Task SendStringAsync(WebSocket socket, string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}