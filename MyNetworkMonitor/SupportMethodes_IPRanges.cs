﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IpRanges
{
    public class IPRange
    {
        public IPRange()
        {

        }

        /// <summary>
        /// Allows CIDR Notation 192.168.178.1/24 
        /// or
        /// simple Range 12.15-16.1-30.10-255
        /// </summary>
        /// <param name="ipRange"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public IPRange(string ipRange)
        {
            if (ipRange == null)
                throw new ArgumentNullException();

            if (!TryParseCIDRNotation(ipRange) && !TryParseSimpleRange(ipRange))
                throw new ArgumentException();
        }

        public IPRange(string FirstIP, string LastIP)
        {
            beginIP = new byte[4];
            endIP = new byte[4];

            beginIP = IPAddress.Parse(FirstIP).GetAddressBytes();
            endIP = IPAddress.Parse(LastIP).GetAddressBytes();
        }

        public IEnumerable<IPAddress> GetAllIP()
        {
            int capacity = 1;
            for (int i = 0; i < 4; i++)
                capacity *= endIP[i] - beginIP[i] + 1;

            if (capacity < 0) return new List<IPAddress>(0);

            List<IPAddress> ips = new List<IPAddress>(capacity);
            for (int i0 = beginIP[0]; i0 <= endIP[0]; i0++)
            {
                for (int i1 = beginIP[1]; i1 <= endIP[1]; i1++)
                {
                    for (int i2 = beginIP[2]; i2 <= endIP[2]; i2++)
                    {
                        for (int i3 = beginIP[3]; i3 <= endIP[3]; i3++)
                        {
                            ips.Add(new IPAddress(new byte[] { (byte)i0, (byte)i1, (byte)i2, (byte)i3 }));
                        }
                    }
                }
            }

            return ips;
        }

        /// <summary>
        /// Parse IP-range string in CIDR notation.
        /// For example "12.15.0.0/16".
        /// </summary>
        /// <param name="ipRange"></param>
        /// <returns></returns>
        private bool TryParseCIDRNotation(string ipRange)
        {
            string[] x = ipRange.Split('/');

            if (x.Length != 2)
                return false;

            byte bits = byte.Parse(x[1]);
            uint ip = 0;
            String[] ipParts0 = x[0].Split('.');
            for (int i = 0; i < 4; i++)
            {
                ip = ip << 8;
                ip += uint.Parse(ipParts0[i]);
            }

            byte shiftBits = (byte)(32 - bits);
            uint ip1 = (ip >> shiftBits) << shiftBits;

            if (ip1 != ip) // Check correct subnet address
                return false;

            uint ip2 = ip1 >> shiftBits;
            for (int k = 0; k < shiftBits; k++)
            {
                ip2 = (ip2 << 1) + 1;
            }

            beginIP = new byte[4];
            endIP = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                beginIP[i] = (byte)((ip1 >> (3 - i) * 8) & 255);
                endIP[i] = (byte)((ip2 >> (3 - i) * 8) & 255);
            }

            return true;
        }

        /// <summary>
        /// Parse IP-range string "12.15-16.1-30.10-255"
        /// </summary>
        /// <param name="ipRange"></param>
        /// <returns></returns>
        private bool TryParseSimpleRange(string ipRange)
        {
            String[] ipParts = ipRange.Split('.');

            beginIP = new byte[4];
            endIP = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                string[] rangeParts = ipParts[i].Split('-');

                if (rangeParts.Length < 1 || rangeParts.Length > 2)
                    return false;

                beginIP[i] = byte.Parse(rangeParts[0]);
                endIP[i] = (rangeParts.Length == 1) ? beginIP[i] : byte.Parse(rangeParts[1]);
            }

            return true;
        }


        public double NumberOfIPsInRange(string StartIP, string EndIP, bool includeStartAndEndAddress = true)
        {
            int[] start = StartIP.Split('.').Select(Int32.Parse).ToArray();
            double _start = (double)start[0] * Math.Pow(256, 3);
            _start += (double)start[1] * Math.Pow(256, 2);
            _start += (double)start[2] * 256;
            _start += start[3];


            int[] end = EndIP.Split('.').Select(Int32.Parse).ToArray();

            double _end = (double)end[0] * Math.Pow(256, 3);
            _end += (double)end[1] * Math.Pow(256, 2);
            _end += (double)end[2] * 256;
            _end += end[3];

            double counted = _end - _start;
            return includeStartAndEndAddress ? counted + 1 : counted - 1;
        }


        private byte[] beginIP;
        private byte[] endIP;
    }
}
