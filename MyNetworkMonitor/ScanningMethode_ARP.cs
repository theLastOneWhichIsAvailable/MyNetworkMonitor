﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyNetworkMonitor
{
    internal class ScanningMethode_ARP
    {
        public ScanningMethode_ARP()
        {

        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        private uint macAddrLen = (uint)new byte[6].Length;

        public string? SendArpRequest(IPAddress ipAddress)
        {
            byte[] macAddr = new byte[6];

            try
            {
                _ = SendARP(BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen);
                if (MacAddresstoString(macAddr) != "00-00-00-00-00-00")
                {
                    return MacAddresstoString(macAddr);
                }
            }
            catch (Exception e)
            {
                //ConsoleExt.WriteLine(e.Message, ConsoleColor.Red);
            }
            return null;
        }

        public static string MacAddresstoString(byte[] macAdrr)
        {
            string macString = BitConverter.ToString(macAdrr);
            return macString.ToUpper();
        }
    }
}
