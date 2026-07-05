using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pixeval.Network.Maho.Fragmentation;

namespace Mako.Tests;

[TestClass]
public sealed class TlsRecordFragmentedStreamTest
{
    [TestMethod]
    public async Task WriteAsync_FragmentedClientHello_WritesCompleteTlsRecords()
    {
        var clientHello = CreateClientHello("app-api.pixiv.net");
        await using var innerStream = new MemoryStream();
        await using var stream = new TlsRecordFragmentedStream(innerStream);

        await stream.WriteAsync(clientHello.AsMemory(0, 3));
        await stream.WriteAsync(clientHello.AsMemory(3, 11));
        await stream.WriteAsync(clientHello.AsMemory(14));

        var records = ParseTlsRecords(innerStream.ToArray());

        Assert.IsGreaterThan(1, records.Count);
        CollectionAssert.AreEqual(clientHello[5..], records.SelectMany(static t => t.Payload).ToArray());
    }

    private static byte[] CreateClientHello(string host)
    {
        var hostBytes = Encoding.ASCII.GetBytes(host);
        var serverNameListLength = 1 + 2 + hostBytes.Length;
        var serverNameExtensionContentLength = 2 + serverNameListLength;
        var extensionsLength = 4 + serverNameExtensionContentLength;
        var handshakeBodyLength =
            2 // client version
            + 32 // random
            + 1 // session id length
            + 2 + 2 // cipher suites length + cipher suite
            + 1 + 1 // compression methods length + null compression
            + 2 + extensionsLength;
        var recordPayloadLength = 4 + handshakeBodyLength;
        var packet = new byte[5 + recordPayloadLength];
        var index = 0;

        packet[index++] = 0x16;
        packet[index++] = 0x03;
        packet[index++] = 0x03;
        WriteUInt16(packet, ref index, recordPayloadLength);
        packet[index++] = 0x01;
        WriteUInt24(packet, ref index, handshakeBodyLength);
        packet[index++] = 0x03;
        packet[index++] = 0x03;
        index += 32;
        packet[index++] = 0;
        WriteUInt16(packet, ref index, 2);
        packet[index++] = 0x13;
        packet[index++] = 0x01;
        packet[index++] = 1;
        packet[index++] = 0;
        WriteUInt16(packet, ref index, extensionsLength);
        WriteUInt16(packet, ref index, 0);
        WriteUInt16(packet, ref index, serverNameExtensionContentLength);
        WriteUInt16(packet, ref index, serverNameListLength);
        packet[index++] = 0;
        WriteUInt16(packet, ref index, hostBytes.Length);
        hostBytes.CopyTo(packet, index);
        index += hostBytes.Length;

        Assert.AreEqual(packet.Length, index);
        return packet;
    }

    private static IReadOnlyList<(byte[] Header, byte[] Payload)> ParseTlsRecords(byte[] buffer)
    {
        var records = new List<(byte[], byte[])>();
        var offset = 0;
        while (offset < buffer.Length)
        {
            Assert.IsTrue(buffer.Length - offset >= 5);
            Assert.AreEqual(0x16, buffer[offset]);

            var payloadLength = buffer[offset + 3] << 8 | buffer[offset + 4];
            Assert.IsTrue(buffer.Length - offset >= 5 + payloadLength);

            records.Add((buffer[offset..(offset + 5)], buffer[(offset + 5)..(offset + 5 + payloadLength)]));
            offset += 5 + payloadLength;
        }

        return records;
    }

    private static void WriteUInt16(byte[] buffer, ref int index, int value)
    {
        buffer[index++] = (byte) (value >> 8);
        buffer[index++] = (byte) value;
    }

    private static void WriteUInt24(byte[] buffer, ref int index, int value)
    {
        buffer[index++] = (byte) (value >> 16);
        buffer[index++] = (byte) (value >> 8);
        buffer[index++] = (byte) value;
    }
}
