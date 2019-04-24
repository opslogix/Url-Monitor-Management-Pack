using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OpsLogix.IMP.Url.Shared.Helpers
{
    public static class TcpHelper
    {
        public static readonly string[] SecureSchemes = { "aaas", "https", "msrps", "sips", "snews", "rtsps", "coaps", "ftps", "ipps", "ircs", "ldaps", "smtp" };
        public static readonly char[] Separators = { ';', ',', '|' };

        public static IPAddress[] ResolveHostName(string host, IPAddress[] dnsServers)
        {
            if (string.IsNullOrEmpty(host)) throw new ArgumentNullException(nameof(host));
            if (dnsServers == null) throw new ArgumentNullException(nameof(dnsServers));

            foreach (var dns in dnsServers)
            {
                try
                {
                    var draftResult = GetHostEntry(host, dns);
                    if (draftResult?.AddressList?.Length > 0)
                        return draftResult.AddressList;
                }
                catch
                {
                    continue;
                }
            }

            throw new SocketException(99);
        }

        public static IPHostEntry GetHostEntry(string host, IPAddress dnsAddr = null, int timeOut = 20000, int dnsPort = 53)
        {
            // (C) https://stackoverflow.com/users/3327485/dexiang
            if (string.IsNullOrEmpty(host)) throw new ArgumentNullException(nameof(host));

            IPHostEntry result = null;

            // if host is an IP address already, just parse it and return
            if (IPAddress.TryParse(host, out var address))
                return new IPHostEntry { AddressList = new IPAddress[] { address }, HostName = host };

            // id no cusom DNS specified, use standard .Net relover
            if (dnsAddr == null)
                return Dns.GetHostEntry(host);

            using (var ms = new MemoryStream())
            {
                // About the dns message: http://www.ietf.org/rfc/rfc1035.txt
                var rnd = new Random();

                // Write message header.
                ms.Write(new byte[] {
                (byte)rnd.Next(0, 0xFF),(byte)rnd.Next(0, 0xFF),
                0x01,0x00,
                0x00,0x01,
                0x00,0x00,
                0x00,0x00,
                0x00,0x00
                }, 0, 12);

                //Write the host to query.
                foreach (string block in host.Split('.'))
                {
                    byte[] data = Encoding.UTF8.GetBytes(block);
                    ms.WriteByte((byte)data.Length);
                    ms.Write(data, 0, data.Length);
                }

                ms.WriteByte(0); // The end of query, muest 0(null string)

                // Query type:A
                ms.WriteByte(0x00);
                ms.WriteByte(0x01);

                // Query class:IN
                ms.WriteByte(0x00);
                ms.WriteByte(0x01);

                using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.ReceiveTimeout = timeOut;
                    // send to dns server
                    byte[] buffer = ms.ToArray();
                    // while (socket.SendTo(buffer, 0, buffer.Length, SocketFlags.None, new IPEndPoint(dnsAddr, dnsPort)) < buffer.Length) ;
                    socket.SendTo(buffer, 0, buffer.Length, SocketFlags.None, new IPEndPoint(dnsAddr, dnsPort));
                    buffer = new byte[0x100];

                    var ep = socket.LocalEndPoint;
                    int num = socket.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref ep);

                    // The response message has the same header and question structure, so we move index to the answer part directly.
                    int index = (int)ms.Length;

                    // Parse response records.
                    void SkipName()
                    {
                        while (index < num)
                        {
                            int length = buffer[index++];
                            if (length == 0)
                            {
                                break;
                            }
                            else if (length > 191)
                            {
                                return;
                            }
                            index += length;
                        }
                    }

                    var addresses = new List<IPAddress>();

                    while (index < num)
                    {
                        SkipName(); // Seems the name of record is useless in this scense, so we just needs to get the next index after name.
                        byte type = buffer[index += 2];
                        index += 7; // Skip class and ttl

                        int length = buffer[index++] << 8 | buffer[index++]; // Get record data's length

                        if (type == 0x01) // A record
                        {
                            if (length == 4) // Parse record data to ip v4, this is what we need.
                            {
                                addresses.Add(new IPAddress(new byte[] { buffer[index], buffer[index + 1], buffer[index + 2], buffer[index + 3] }));
                            }
                        }

                        index += length;
                    }

                    result = new IPHostEntry { AddressList = addresses.ToArray(), HostName = host };
                }
            }

            return result;
        }

        public static void SetHeaders(HttpWebRequest request, NameValueCollection headers)
        {
            foreach (string valueKey in headers.Keys)
                switch (valueKey)
                {
                    // https://docs.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.headers?view=netframework-4.7.2
                    case "Accept": request.Accept = headers[valueKey]; break;
                    case "Connection": request.Connection = headers[valueKey]; break;
                    case "Content-Length": request.ContentLength = int.Parse(headers[valueKey]); break;
                    case "Content-Type": request.ContentType = headers[valueKey]; break;
                    case "Expect": request.Expect = headers[valueKey]; break;
                    case "Date": request.Date = DateTime.Parse(headers[valueKey]); break;
                    case "Host": request.Host = headers[valueKey]; break;
                    case "If-Modified-Since": request.IfModifiedSince = DateTime.Parse(headers[valueKey]); break;
                    case "Range": break; // not supported  request.AddRange= headers[valueKey]; break;
                    case "Referer": request.Referer = headers[valueKey]; break;
                    case "Transfer-Encoding": request.TransferEncoding = headers[valueKey]; break;
                    case "User-Agent": request.UserAgent = headers[valueKey]; break;
                    default: request.Headers.Add(valueKey, headers[valueKey]); break;
                }
        }
    }
}