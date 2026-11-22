/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-10-04: Refactored for .NET 5
//2017-09-17: Initial version

namespace Medo.Net;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Simple NTP client.
/// </summary>
/// <example>
/// <code>
/// using var client = new TrivialNtpClient("time.medo64.com");
/// var time = client.RetrieveTime();
/// </code>
/// </example>
/// <remarks>RFC 5905</remarks>
public class TrivialNtpClient : IDisposable {

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="hostName">NTP server host name.</param>
    /// <exception cref="ArgumentNullException">Host name cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Host name cannot be empty.</exception>
    public TrivialNtpClient(string hostName)
        : this(hostName, DefaultNtpPort) {
    }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="hostName">NTP server host name.</param>
    /// <param name="port">NTP server port.</param>
    /// <exception cref="ArgumentNullException">Host name cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Host name cannot be empty. -or- Port must be between 1 and 65535.</exception>
    public TrivialNtpClient(string hostName, int port) {
        if (hostName == null) { throw new ArgumentNullException(nameof(hostName), "Host name cannot be null."); }
        if (string.IsNullOrWhiteSpace(hostName)) { throw new ArgumentOutOfRangeException(nameof(hostName), "Host name cannot be empty."); }
        if (port is < 1 or > 65535) { throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535."); }

        HostName = hostName;
        Port = port;
    }


    private const int DefaultNtpPort = 123;
    private const int DefaultTimeout = 500;


    /// <summary>
    /// Gets NTP server host name.
    /// </summary>
    public string HostName { get; }

    /// <summary>
    /// Gets NTP server port.
    /// </summary>
    public int Port { get; }


    private int _timeout = DefaultTimeout;
    /// <summary>
    /// Gets/sets timeout in milliseconds.
    /// </summary>
    public int Timeout {
        get { return _timeout; }
        set {
            if (value <= 0) { throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be 0 or negative."); }
            _timeout = value;
        }
    }


    /// <summary>
    /// Returns the current time as received from NTP server.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot retrieve time from NTP server.</exception>
    public DateTime RetrieveTime() {
        var requestPacket = NtpPacket.GetClientPacket();
        var requestBytes = requestPacket.GetBytes();

        try {
            using var udp = new UdpClient();
            udp.Client.ReceiveTimeout = Timeout;
            udp.Client.SendTimeout = Timeout;

            udp.Send(requestBytes, requestBytes.Length, HostName, Port);

            IPEndPoint? remoteEP = null;
            var responseBytes = udp.Receive(ref remoteEP);

            var responsePacket = NtpPacket.ParsePacket(responseBytes);
            if (responsePacket.TransmitTimestamp.HasValue) {
                return responsePacket.TransmitTimestamp.Value;
            } else {
                throw new InvalidOperationException("Cannot retrieve time from NTP server.");
            }
        } catch (SocketException ex) {
            throw new InvalidOperationException("Cannot retrieve time from NTP server.", ex);
        }
    }

    /// <summary>
    /// Returns time retrieved from NTP server.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot retrieve time from NTP server.</exception>
    public async Task<DateTime> RetrieveTimeAsync() {
        var requestPacket = NtpPacket.GetClientPacket();
        var requestBytes = requestPacket.GetBytes();

        try {
            using var udp = new UdpClient();
            udp.Client.ReceiveTimeout = Timeout;
            udp.Client.SendTimeout = Timeout;

            await udp.SendAsync(requestBytes, requestBytes.Length, HostName, Port);

            var receiveTask = udp.ReceiveAsync();
            using (var timeoutCancellationToken = new CancellationTokenSource()) {
                var completedTask = await Task.WhenAny(receiveTask, Task.Delay(Timeout, timeoutCancellationToken.Token));
                if (completedTask == receiveTask) {
                    timeoutCancellationToken.Cancel();
                    await receiveTask;
                } else {
                    throw new InvalidOperationException("Cannot retrieve time from NTP server.", new TimeoutException());
                }
            }

            var responsePacket = NtpPacket.ParsePacket(receiveTask.Result.Buffer);
            if (responsePacket.TransmitTimestamp.HasValue) {
                return responsePacket.TransmitTimestamp.Value;
            } else {
                throw new InvalidOperationException("Cannot retrieve time from NTP server.");
            }
        } catch (SocketException ex) {
            throw new InvalidOperationException("Cannot retrieve time from NTP server.", ex);
        }
    }


    /// <summary>
    /// Returns if time retrieval has been successful with time given as output parameter.
    /// </summary>
    /// <param name="time">Time if successful.</param>
    public bool TryRetrieveTime(out DateTime time) {
        try {
            time = RetrieveTime();
            return true;
        } catch (InvalidOperationException) {
            time = DateTime.MinValue;
            return false;
        }
    }


    #region Static

    /// <summary>
    /// Returns the current time as received from NTP server.
    /// </summary>
    /// <param name="hostName">NTP server host name.</param>
    /// <exception cref="ArgumentNullException">Host name cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Host name cannot be empty.</exception>
    /// <exception cref="InvalidOperationException">Cannot retrieve time from NTP server.</exception>
    public static DateTime RetrieveTime(string hostName) {
        using var client = new TrivialNtpClient(hostName);
        return client.RetrieveTime();
    }

    /// <summary>
    /// Returns time retrieved from NTP server.
    /// </summary>
    /// <param name="hostName">NTP server host name.</param>
    /// <exception cref="ArgumentNullException">Host name cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Host name cannot be empty.</exception>
    /// <exception cref="InvalidOperationException">Cannot retrieve time from NTP server.</exception>
    public static async Task<DateTime> RetrieveTimeAsync(string hostName) {
        using var client = new TrivialNtpClient(hostName);
        return await client.RetrieveTimeAsync();
    }

    /// <summary>
    /// Returns if time retrieval has been successful with time given as output parameter.
    /// </summary>
    /// <param name="hostName">NTP server host name.</param>
    /// <param name="time">Time if successful.</param>
    /// <exception cref="ArgumentNullException">Host name cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Host name cannot be empty.</exception>
    public static bool TryRetrieveTime(string hostName, out DateTime time) {
        using var client = new TrivialNtpClient(hostName);
        return client.TryRetrieveTime(out time);
    }

    #endregion Static


    #region NtpPacket

    private class NtpPacket {
        private NtpPacket() { }


        /// <summary>
        ///  This is a warning of an impending leap second to be inserted in the NTP timescale. The bits are set before 23:59 on the day of insertion and reset after 00:00 on the following day. This causes the number of seconds (rollover interval) in the day of insertion to be increased or decreased by one. In the case of primary servers the bits are set by operator intervention, while in the case of secondary servers the bits are set by the protocol.
        /// </summary>
        public NtpPacketLeapIndicator LeapIndicator { get; private set; }

        /// <summary>
        /// NTP version. Must be 4 (as per RFC 5905).
        /// </summary>
        public int VersionNumber { get; private set; }

        /// <summary>
        /// This is a number indicating the association mode.
        /// </summary>
        public NtpPacketMode Mode { get; private set; }

        /// <summary>
        /// This is a number indicating the stratum of the local clock.
        /// </summary>
        public NtpPacketStratum Stratum { get; private set; }

        /// <summary>
        /// This is a number indicating the minimum interval between transmitted messages, in seconds as a power of two. For instance, a value of six indicates a minimum interval of 64 seconds.
        /// </summary>
        public int PollAsLog2 { get; private set; }

        /// <summary>
        /// This is a number indicating the precision of the various clocks, in seconds to the nearest power of two. The value must be rounded to the next larger power of two; for instance, a 50-Hz (20 ms) or 60-Hz (16.67 ms) power-frequency clock would be assigned the value -5 (31.25 ms), while a 1000-Hz (1 ms) crystal-controlled clock would be assigned the value -9 (1.95 ms).
        /// </summary>
        public int PrecisionAsLog2 { get; private set; }

        /// <summary>
        /// This is a signed fixed-point number indicating the total roundtrip delay to the primary reference source at the root of the synchronization subnet, in seconds. Note that this variable can take on both positive and negative values, depending on clock precision and skew.
        /// </summary>
        public int RootDelay { get; private set; }

        /// <summary>
        /// This is a signed fixed-point number indicating the maximum error relative to the primary reference source at the root of the synchronization subnet, in seconds. Only positive values greater than zero are possible.
        /// </summary>
        public int RootDispersion { get; private set; }

        /// <summary>
        /// This is a 32-bit code identifying the particular reference clock. In the case of stratum 0 (unspecified) or stratum 1 (primary reference source), this is a four-octet, left-justified, zero-padded ASCII string. In the case of stratum 2 and greater (secondary reference) this is the four-octet Internet address of the primary reference host.
        /// </summary>
        public int ReferenceIdentifier { get; private set; }

        /// <summary>
        /// This field is the time the system clock was last set or corrected.
        /// </summary>
        public DateTime? ReferenceTimestamp { get; private set; }

        /// <summary>
        /// This is the time at which the request departed the client for the server.
        /// </summary>
        public DateTime? OriginTimestamp { get; private set; }

        /// <summary>
        /// This is the time at which the request arrived at the server or the reply arrived at the client.
        /// </summary>
        public DateTime? ReceiveTimestamp { get; private set; }

        /// <summary>
        /// This is the time at which the request departed the client or the reply departed the server.
        /// </summary>
        public DateTime? TransmitTimestamp { get; private set; }


        /// <summary>
        /// Returns packet bytes.
        /// </summary>
        public byte[] GetBytes() {
            var bytes = new List<byte>();

            var li = (int)LeapIndicator;
            var vn = (int)VersionNumber;
            var mode = (int)Mode;
            bytes.Add((byte)((li << 6) | (vn << 3) | (mode)));

            bytes.Add((byte)Stratum);
            bytes.Add((byte)PollAsLog2);
            bytes.Add((byte)PrecisionAsLog2);

            var rootDelay = BitConverter.GetBytes(RootDelay);
            bytes.AddRange(new byte[] { rootDelay[3], rootDelay[2], rootDelay[1], rootDelay[0] });

            var rootDispersion = BitConverter.GetBytes(RootDispersion);
            bytes.AddRange(new byte[] { rootDispersion[3], rootDispersion[2], rootDispersion[1], rootDispersion[0] });

            var referenceID = BitConverter.GetBytes(ReferenceIdentifier);
            bytes.AddRange(new byte[] { referenceID[3], referenceID[2], referenceID[1], referenceID[0] });

            bytes.AddRange(GetTimestamp(ReferenceTimestamp));
            bytes.AddRange(GetTimestamp(OriginTimestamp));
            bytes.AddRange(GetTimestamp(ReceiveTimestamp));
            bytes.AddRange(GetTimestamp(TransmitTimestamp));

            return bytes.ToArray();
        }


        #region Static

        public static NtpPacket GetClientPacket() {
            return new NtpPacket {
                VersionNumber = 4,
                Mode = NtpPacketMode.Client,
                OriginTimestamp = DateTime.UtcNow
            };
        }


        public static NtpPacket ParsePacket(byte[] bytes) {
            return new NtpPacket {
                LeapIndicator = (NtpPacketLeapIndicator)(bytes[0] >> 6),
                VersionNumber = (bytes[0] >> 3) & 0x07,
                Mode = (NtpPacketMode)(bytes[0] & 0x07),

                Stratum = (NtpPacketStratum)bytes[1],
                PollAsLog2 = bytes[2],
                PrecisionAsLog2 = bytes[3],

                RootDelay = BitConverter.ToInt32(new byte[] { bytes[7], bytes[6], bytes[5], bytes[4] }, 0),
                RootDispersion = BitConverter.ToInt32(new byte[] { bytes[11], bytes[10], bytes[9], bytes[8] }, 0),
                ReferenceIdentifier = BitConverter.ToInt32(new byte[] { bytes[15], bytes[14], bytes[13], bytes[12] }, 0),

                ReferenceTimestamp = GetTimestamp(bytes, 16),
                OriginTimestamp = GetTimestamp(bytes, 24),
                ReceiveTimestamp = GetTimestamp(bytes, 32),
                TransmitTimestamp = GetTimestamp(bytes, 40)
            };
        }

        #endregion Static


        private static DateTime? GetTimestamp(byte[] bytes, int offset) {
            long n1 = BitConverter.ToUInt32(new byte[] { bytes[offset + 3], bytes[offset + 2], bytes[offset + 1], bytes[offset + 0] }, 0);
            long n2 = BitConverter.ToUInt32(new byte[] { bytes[offset + 7], bytes[offset + 6], bytes[offset + 5], bytes[offset + 4] }, 0);

            if ((n1 == 0) && (n2 == 0)) { return null; }

            var time = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            time = time.AddTicks(n1 * 10000000);
            time = time.AddTicks((long)(n2 / 4294967296.0 * 10000000));

            return time;
        }

        private static byte[] GetTimestamp(DateTime? timestamp) {
            if (timestamp == null) { return new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }; }

            var time = timestamp.Value;
            if (time.Kind == DateTimeKind.Local) { time = time.ToUniversalTime(); }

            var eraTime = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var ts = time - eraTime;
            var n1 = (uint)ts.TotalSeconds;
            var buffer1LE = BitConverter.GetBytes(n1);

            var timeSec = eraTime.AddSeconds(n1);
            var n2 = (uint)System.Math.Min(System.Math.Max((time.Ticks - timeSec.Ticks) / 10000000.0 * 4294967296, 0), 4294967296);
            var buffer2LE = BitConverter.GetBytes(n2);

            return new byte[] { buffer1LE[3], buffer1LE[2], buffer1LE[1], buffer1LE[0], buffer2LE[3], buffer2LE[2], buffer2LE[1], buffer2LE[0] };
        }

    }


    /// <summary>
    /// This is a warning code of an impending leap second to be inserted/deleted in the last minute of the current day.
    /// </summary>
    private enum NtpPacketLeapIndicator {
        /// <summary>
        /// No warning.
        /// </summary>
        None = 0,
        /// <summary>
        /// Last minute has 61 seconds.
        /// </summary>
        LastMinuteHas61Seconds = 1,
        /// <summary>
        /// Last minute has 59 seconds.
        /// </summary>
        LastMinuteHas59Seconds = 2,
        /// <summary>
        /// Alarm condition (clock not synchronized).
        /// </summary>
        ClockNotSynchronized = 3
    }



    /// <summary>
    /// This is a number indicating the protocol mode.
    /// </summary>
    private enum NtpPacketMode {
        /// <summary>
        /// Unspecified.
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// Symmetric active.
        /// </summary>
        SymmetricActive = 1,
        /// <summary>
        /// Symmetric passive.
        /// </summary>
        SymmetricPassive = 2,
        /// <summary>
        /// Client.
        /// </summary>
        Client = 3,
        /// <summary>
        /// Server.
        /// </summary>
        Server = 4,
        /// <summary>
        /// Broadcast.
        /// </summary>
        Broadcast = 5,
        /// <summary>
        /// Reserved for NTP control messages.
        /// </summary>
        ReservedForNtpControlMessages = 6,
        /// <summary>
        /// Reserved for private use.
        /// </summary>
        ReservedForPrivateUse = 7
    }


    /// <summary>
    /// This is a number indicating the stratum.
    /// </summary>
    private enum NtpPacketStratum {
        /// <summary>
        /// Kiss-o'-death message.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Primary reference (e.g., calibrated atomic clock, radio clock).
        /// </summary>
        PrimaryReference = 1,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference1 = 2,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference2 = 3,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference3 = 4,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference4 = 5,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference5 = 6,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference6 = 7,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference7 = 8,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference8 = 9,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference9 = 10,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference10 = 11,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference11 = 12,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference12 = 13,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference13 = 14,
        /// <summary>
        /// Secondary reference (synchronized by NTP or SNTP).
        /// </summary>
        SecondaryReference14 = 15,
    }

    #endregion NtpPacket


    #region IDisposable

    /// <summary>
    /// Disposing used resources.
    /// </summary>
    /// <param name="disposing">True is disposing managed resources.</param>
    protected virtual void Dispose(bool disposing) {
    }

    /// <summary>
    /// Release used resources.
    /// </summary>
    /// <remarks>Not really necessary but enables use of using blocks.</remarks>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

}
