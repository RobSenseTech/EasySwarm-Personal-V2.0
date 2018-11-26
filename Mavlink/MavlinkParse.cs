﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


public partial class MAVLink
    {
        public class MavlinkParse
        {
            public int packetcount = 0;
            public int badCRC = 0;
            public int badLength = 0;
            public static void ReadWithTimeout(Stream BaseStream, byte[] buffer, int offset, int count)//从流中读取数据
            {
                int timeout = BaseStream.ReadTimeout;

                if (timeout == -1)
                    timeout = 60000;

                DateTime to = DateTime.Now.AddMilliseconds(timeout);

                int toread = count;
                int pos = offset;

                while (true)
                {
                    // read from stream
                    int read = BaseStream.Read(buffer, pos, toread);

                    // update counter
                    toread -= read;
                    pos += read;

                    // reset timeout if we get data
                    if (read > 0)
                        to = DateTime.Now.AddMilliseconds(timeout);

                    if (toread == 0)
                        break;

                    if (DateTime.Now > to)
                    {
                        throw new TimeoutException("Timeout waiting for data");
                    }
                    System.Threading.Thread.Sleep(1);
                }
            }
            public MAVLinkMessage ReadPacket(Stream BaseStream)
            {
                byte[] buffer = new byte[270];
                BaseStream.Read(buffer, 0, 2);
                Console.WriteLine(buffer[0]);
                int readcount = 0;

                while (readcount < 200)
                {
                    // read STX byte
                    ReadWithTimeout(BaseStream, buffer, 0, 1);

                    if (buffer[0] == MAVLink.MAVLINK_STX || buffer[0] == MAVLINK_STX_MAVLINK1)
                        break;

                    readcount++;
                }

                var headerlength = buffer[0] == MAVLINK_STX ? MAVLINK_CORE_HEADER_LEN : MAVLINK_CORE_HEADER_MAVLINK1_LEN;
                var headerlengthstx = headerlength + 1;

                // read header
                ReadWithTimeout(BaseStream, buffer, 1, headerlength);

                // packet length
                int lengthtoread = 0;
                if (buffer[0] == MAVLINK_STX)
                {
                    lengthtoread = buffer[1] + headerlengthstx + 2 - 2; // data + header + checksum - magic - length
                    if ((buffer[2] & MAVLINK_IFLAG_SIGNED) > 0)
                    {
                        lengthtoread += MAVLINK_SIGNATURE_BLOCK_LEN;
                    }
                }
                else
                {
                    lengthtoread = buffer[1] + headerlengthstx + 2 - 2; // data + header + checksum - U - length    
                }

                //read rest of packet
                if (buffer[0] == MAVLink.MAVLINK_STX)
                {
                    ReadWithTimeout(BaseStream, buffer, 10, lengthtoread - (headerlengthstx - 2));
                }
                else
                {
                    ReadWithTimeout(BaseStream, buffer, 6, lengthtoread - (headerlengthstx - 2));
                }
                MAVLinkMessage message = new MAVLinkMessage(buffer);
                Console.WriteLine("message.msgid......................................!!!!!!!!!! = " + message.msgid);
                if (message.msgid == 150)
                {
                    //
                    /*Console.WriteLine(" buffer[0] = " + buffer[0]);
                                    Console.WriteLine("buffer[1] =" + buffer[5]);
                                    Console.WriteLine("buffer[2] =" + buffer[5]);
                                    Console.WriteLine("buffer[3] =" + buffer[5]);
                                    Console.WriteLine("buffer[4] =" + buffer[5]);*/

                    //   Console.WriteLine("buffer[5] =" +  buffer[5]);
                    // Console.WriteLine("buffer[6] =" + buffer[6]);
                    Console.WriteLine("other laser ID " + buffer[11]);

                    Console.WriteLine("message.msgid......................................!!!!!!!!!! = " + message.msgid);
                }



                // resize the packet to the correct length
                Array.Resize<byte>(ref buffer, lengthtoread + 2);

                // calc crc
                ushort crc = MavlinkCRC.crc_calculate(buffer, buffer.Length - 2);

                // calc extra bit of crc for mavlink 1.0+
                if (message.header == MAVLINK_STX || message.header == MAVLINK_STX_MAVLINK1)
                {
                    crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[message.msgid], crc);
                }

                // check crc
                if ((message.crc16 >> 8) != (crc >> 8) ||
                          (message.crc16 & 0xff) != (crc & 0xff))
                {
                    badCRC++;
                    // crc fail
                    return null;
                }
                //Console.WriteLine("message.msgid...................................... = " + message.msgid);
                return message;
            }
            public byte[] GenerateMAVLinkPacket_PX4(MAVLINK_MSG_ID messageType, object indata)
            {
                byte[] data;

                data = MavlinkUtil.StructureToByteArray(indata);

                byte[] packet = new byte[data.Length + 10 + 2];

                packet[0] = 0xfd;
                packet[1] = (byte)data.Length;
                packet[2] = 0;
                packet[3] = 0;
                packet[4] = 0x75;
                packet[5] = 255; // this is always 255 - MYGCS
                packet[6] = 0;
                packet[7] = (byte)messageType;//messageType
                packet[8] = 0;//messageType extera
                packet[9] = 0;//messageType extera
                packetcount++;

                //packet[3] = 255; // this is always 255 - MYGCS
                //packet[4] = (byte)MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
                //packet[5] = (byte)messageType;


                int i = 10;
                foreach (byte b in data)
                {
                    packet[i] = b;
                    i++;
                }

                ushort checksum = MavlinkCRC.crc_calculate(packet, packet[1] + 10);

                checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[(byte)messageType], checksum);

                byte ck_a = (byte)(checksum & 0xFF); ///< High byte
                byte ck_b = (byte)(checksum >> 8); ///< Low byte

                packet[i] = ck_a;
                i += 1;
                packet[i] = ck_b;
                i += 1;

                return packet;
            }
            public byte[] GenerateMAVLinkPacket_APM(MAVLINK_MSG_ID messageType, object indata)
        {
            byte[] data;

            data = MavlinkUtil.StructureToByteArray(indata);

            byte[] packet = new byte[data.Length + 6 + 2];

            packet[0] = 0xfe;
            packet[1] = (byte)data.Length;
            packet[2] = (byte)packetcount;

            packetcount++;

            packet[3] = 255; // this is always 255 - MYGCS
            packet[4] = (byte)MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
            packet[5] = (byte)messageType;


            int i = 6;
            foreach (byte b in data)
            {
                packet[i] = b;
                i++;
            }

            ushort checksum = MavlinkCRC.crc_calculate(packet, packet[1] + 6);
            checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[(byte)messageType], checksum);
            byte ck_a = (byte)(checksum & 0xFF); ///< High byte
            byte ck_b = (byte)(checksum >> 8); ///< Low byte

            packet[i] = ck_a;
            i += 1;
            packet[i] = ck_b;
            i += 1;

            return packet;
        }
    }
    }

    

