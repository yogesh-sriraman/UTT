using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace com.medcare360.utt
{
    public class ServerManager
    {
        protected IUdpController udpController;
        protected UdpClient udpServer;
        protected UIManager uiManager;
        public UdpClient ProcessServer(IServerSide server, TRACKER_TYPE type, bool needsUDPCheck)
        {
            return CreateListener(server, type, needsUDPCheck);
        }

        public UDPConfig GetConfigs(TRACKER_TYPE type)
        {
            throw new NotImplementedException();
            //return ProcessManager.Instance.GetConfig(type);
        }

        private UdpClient CreateListener(IServerSide server, TRACKER_TYPE type, bool needsUDPCheck)
        {
            if (!udpController.IsValidTracker(type))
            {
                return null;
            }

            UDPConfig config = udpController.GetConfig(type);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), config.serverPort);

            UdpClient udpServer = new UdpClient();
            udpServer.Client.ReceiveBufferSize = config.bufferSize;
            udpServer.Client.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
            udpServer.ExclusiveAddressUse = config.exclusiveAddressUse;
            udpServer.EnableBroadcast = config.enableBroadcast;
            udpServer.DontFragment = config.dontFragment;
            udpServer.Client.Bind(ipEndPoint);

            ThreadManager.Instance.AddToThread(
                () => WaitForUdpPacket(udpServer, ipEndPoint, server, needsUDPCheck), type.ToString() + "_Server");

            return udpServer;

        }

        private void WaitForUdpPacket(UdpClient client, IPEndPoint ipEndPoint, IServerSide server,
            bool needsUDPCheck)
        {
            while(true)
            {
                if(needsUDPCheck)
                {
                    client.Client.ReceiveTimeout = 3000;
                }
                try
                {
                    byte[] data = client.Receive(ref ipEndPoint);
                    if (data.Length > 0)
                    {
                        ThreadManager.Instance.AddToMainThread(
                            () => server.ProcessData(data));
                    }
                    else
                    {
                    }
                    Thread.Sleep(5);
                }
                catch
                {
                    if(needsUDPCheck)
                    {
                        ThreadManager.Instance.AddToMainThread(
                            () => uiManager.UpdateDataReception(success: false));
                        //WaitForUdpPacket(client, ipEndPoint, server, needsUDPCheck);
                        break;
                    }
                }
            }
        }

        public static byte[] EncodePacket(Skeleton packet)
        {
            var binaryWriter = new BinaryWriter(new MemoryStream());

            binaryWriter.Write(packet.version);

            var hostNameAsString = packet.hostname.PadRight(totalWidth: 64, paddingChar: ' ');
            var hostnameAsBytes = System.Text.Encoding.ASCII.GetBytes(hostNameAsString);
            foreach (var byteValue in hostnameAsBytes) binaryWriter.Write(byteValue);

            binaryWriter.Write(packet.frameNumber);
            binaryWriter.Write(packet.frameTime_sec);
            binaryWriter.Write(packet.frameTime_nsec);
            binaryWriter.Write(packet.irqSequenceNum);
            binaryWriter.Write(packet.irqTime_sec);
            binaryWriter.Write(packet.irqTime_nsec);

            for (var i = 0; i < packet.aMarkers.Length; i++)
            {
                binaryWriter.Write(packet.aMarkers[i].markerSeriesNumber);
                binaryWriter.Write(packet.aMarkers[i].markerWidth);
                binaryWriter.Write(packet.aMarkers[i].markerThickness);
                binaryWriter.Write(packet.aMarkers[i].brightness);
                binaryWriter.Write(packet.aMarkers[i].focusMetric);
                binaryWriter.Write(packet.aMarkers[i].blurRadius);

                binaryWriter.Write(packet.aMarkers[i].x);
                binaryWriter.Write(packet.aMarkers[i].y);
                binaryWriter.Write(packet.aMarkers[i].z);
                binaryWriter.Write(packet.aMarkers[i].qr);
                binaryWriter.Write(packet.aMarkers[i].qx);
                binaryWriter.Write(packet.aMarkers[i].qy);
                binaryWriter.Write(packet.aMarkers[i].qz);
                binaryWriter.Write(packet.aMarkers[i].flags);
            }

            foreach (var voltage in packet.aVoltages) binaryWriter.Write(voltage);

            var encodedBytes = ((MemoryStream)binaryWriter.BaseStream).ToArray();

            return encodedBytes;
        }

        public static Skeleton DecodeSuperPacket(byte[] rawData)
        {
            var binaryReader = new BinaryReader(new MemoryStream(rawData));
            return DecodeNextSuperPacket(binaryReader);
        }

        public static Skeleton DecodeNextSuperPacket(BinaryReader reader)
        {
            // Note: order matters, data is serialized

            // Decode frame information
            var version = reader.ReadInt32();
            var hostname = new string(reader.ReadChars(64)).Trim();
            var frameNumber = reader.ReadInt32();
            var frameTimeInSeconds = reader.ReadUInt32();
            var frameTimeInNanoSeconds = reader.ReadUInt32();
            var irqSequenceNumber = reader.ReadUInt32();
            var irqTimeInSeconds = reader.ReadUInt32();
            var irqTimeNanoSeconds = reader.ReadUInt32();

            // Decode markers
            var markers = new SkeletonMarker[256];

            for (var index = 0; index < markers.Length; index++)
            {
                markers[index] = DecodeNextMarkerPosition(index, reader);
            }

            // Decode system voltages
            var voltages = new double[16];

            for (var i = 0; i < voltages.Length; i++)
            {
                voltages[i] = reader.ReadDouble();
            }

            return new Skeleton(
                version,
                hostname,
                frameNumber,
                frameTimeInSeconds,
                frameTimeInNanoSeconds,
                irqSequenceNumber,
                irqTimeInSeconds,
                irqTimeNanoSeconds,
                markers,
                voltages);
        }

        private static SkeletonMarker DecodeNextMarkerPosition(int index, BinaryReader reader)
        {
            // Decode marker identification
            var markerSeriesNumber = reader.ReadInt32();
            var markerWidth = reader.ReadSingle();
            var markerThickness = reader.ReadSingle();
            var brightness = reader.ReadSingle();
            var focusMetric = reader.ReadSingle();
            var blurRadius = reader.ReadSingle();

            // Decode marker position
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var qr = reader.ReadSingle();
            var qx = reader.ReadSingle();
            var qy = reader.ReadSingle();
            var qz = reader.ReadSingle();

            // Decode marker flags
            var flags = reader.ReadUInt32();

            return new SkeletonMarker(
                index,
                markerSeriesNumber,
                markerWidth,
                markerThickness,
                brightness,
                focusMetric,
                blurRadius,
                x, y, z,
                qr, qx, qy, qz,
                flags);
        }
    }
}