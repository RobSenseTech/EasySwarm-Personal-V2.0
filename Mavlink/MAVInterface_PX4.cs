using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public partial class MAVLink
{
    public interface IBase
    {
        byte[] arm();
        byte[] disarm();
    }
    public class MAVInterface_PX4 : IMAVLink_API
    {
        private MAVLink.MavlinkParse mavlink = new MAVLink.MavlinkParse();
        // locking to prevent multiple reads on serial port
        private object readlock = new object();
        // our target sysid
        private byte sysid;
        // our target compid
        private byte compid;
        bool armed = false;
        public byte[] arm_disarm()
        {
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();//定义req为 MAVLink.mavlink_command_long_t类，填充常命令的数据包

            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM;//‘COMPONENT_ARM_DISARM’代表你要发送什么类的信息（此处为解锁信息），可自己选择（看类里有多少种）
                                                                       //req.command = (ushort)MAVLink.MAV_CMD
            req.param1 = armed ? 0 : 1;//此处发送解锁信息，只需用param1。其余param2—7注释掉
            armed = !armed;
            /*
            req.param2 = p2;//具体想要发送什么类型的数据,param如何设置，需要查看阿木社区中的MAV_CMD
            req.param3 = p3;//跟踪的可以参考里面的fellow
            req.param4 = p4;
            req.param5 = p5;
            req.param6 = p6;
            req.param7 = p7;
            */

            byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      //foreach (byte i in packet)
                                                                                                      //{
                                                                                                      //    Console.Write("{0:X} ", i);
                                                                                                      //}
            return packet;
        }
        public byte[] arm()
        {
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();//定义req为 MAVLink.mavlink_command_long_t类，填充常命令的数据包

            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM;//‘COMPONENT_ARM_DISARM’代表你要发送什么类的信息（此处为解锁信息），可自己选择（看类里有多少种）
                                                                       //req.command = (ushort)MAVLink.MAV_CMD
            req.param1 = 1;
            /*
            req.param2 = p2;//具体想要发送什么类型的数据,param如何设置，需要查看阿木社区中的MAV_CMD
            req.param3 = p3;//跟踪的可以参考里面的fellow
            req.param4 = p4;
            req.param5 = p5;
            req.param6 = p6;
            req.param7 = p7;
            */

            byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      //foreach (byte i in packet)
                                                                                                      //{
                                                                                                      //    Console.Write("{0:X} ", i);
                                                                                                      //}
            return packet;
        }
        public byte[] disarm()
        {
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();//定义req为 MAVLink.mavlink_command_long_t类，填充常命令的数据包

            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM;//‘COMPONENT_ARM_DISARM’代表你要发送什么类的信息（此处为解锁信息），可自己选择（看类里有多少种）
                                                                       //req.command = (ushort)MAVLink.MAV_CMD
            req.param1 = 0;
            /*
            req.param2 = p2;//具体想要发送什么类型的数据,param如何设置，需要查看阿木社区中的MAV_CMD
            req.param3 = p3;//跟踪的可以参考里面的fellow
            req.param4 = p4;
            req.param5 = p5;
            req.param6 = p6;
            req.param7 = p7;
            */

            byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      //foreach (byte i in packet)
                                                                                                      //{
                                                                                                      //    Console.Write("{0:X} ", i);
                                                                                                      //}
            return packet;
        }
        public byte[] takeoff(float altitude)
        {
            float temp = (float)(((float)((1e+300) * (1e+300))) * 0.0F);
            //tmp = (float)((tmp[0] << 24) && (tmp[1] << 16) && (tmp[2] << 8) && (tmp[3] << 0));
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();//定义req为 MAVLink.mavlink_command_long_t类，填充常命令的数据包
            req.target_system = 1;
            req.target_component = 1;
            req.command = (ushort)MAVLink.MAV_CMD.TAKEOFF;//‘COMPONENT_ARM_DISARM’代表你要发送什么类的信息（此处为解锁信息），可自己选择（看类里有多少种）
                                                          //req.command = (ushort)MAVLink.MAV_CMD
                                                          //req.param1 = armed ? 0 : 1;//此处发送解锁信息，只需用param1。其余param2—7注释掉
            req.param1 = -1;
            req.param2 = 0;
            req.param3 = 0;
            req.param4 = temp;
            req.param5 = temp;
            req.param6 = temp;
            req.param7 = altitude;
            byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      // byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）

            //   mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.SET_POSITION_TARGET_LOCAL_NED, req);
            //foreach (byte i in packet)
            //{
            //    Console.Write("{0:X} ", i);
            //}
            return packet;
        }
        public byte[] gotolocation(float longitude, float latitude, float altitude)
        {
            //Console.WriteLine(altitude);
            //Console.WriteLine();
            float NAN = (((float)((1e+300) * (1e+300))) * 0.0F);
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();//定义req为 MAVLink.mavlink_command_long_t类，填充常命令的数据包
            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.DO_REPOSITION;//‘COMPONENT_ARM_DISARM’代表你要发送什么类的信息（此处为解锁信息），可自己选择（看类里有多少种）
                                                                //req.command = (ushort)MAVLink.MAV_CMD
                                                                //req.param1 = armed ? 0 : 1;//此处发送解锁信息，只需用param1。其余param2—7注释掉
            req.param1 = -1.0f;
            req.param2 = 1;
            req.param3 = 0.0f;
            req.param4 = NAN;
            req.param5 = latitude;
            req.param6 = longitude;
            req.param7 = altitude;
            byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      // byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      //   mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.SET_POSITION_TARGET_LOCAL_NED, req);
                                                                                                      //foreach (byte i in packet)
                                                                                                      //{
                                                                                                      //    Console.Write("{0:X} ", i                                                                                               //}
            return packet;
        }
        public byte[] changealtitude(float altitude)
        {
            float NAN = (((float)((1e+300) * (1e+300))) * 0.0F);
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();//定义req为 MAVLink.mavlink_command_long_t类，填充常命令的数据包
            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.DO_REPOSITION;//‘COMPONENT_ARM_DISARM’代表你要发送什么类的信息（此处为解锁信息），可自己选择（看类里有多少种）
                                                                //req.command = (ushort)MAVLink.MAV_CMD
                                                                //req.param1 = armed ? 0 : 1;//此处发送解锁信息，只需用param1。其余param2—7注释掉
            req.param1 = -1.0f;
            req.param2 = 1;
            req.param3 = 0.0f;
            req.param4 = NAN;
            req.param5 = NAN;
            req.param6 = NAN;
            req.param7 = altitude;
            byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      // byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）                                                                               //}
            return packet;
        }
        public byte[] pause()
        {
            float NAN = (((float)((1e+300) * (1e+300))) * 0.0F);
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();//定义req为 MAVLink.mavlink_command_long_t类，填充常命令的数据包
            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.DO_REPOSITION;//‘COMPONENT_ARM_DISARM’代表你要发送什么类的信息（此处为解锁信息），可自己选择（看类里有多少种）
                                                                //req.command = (ushort)MAVLink.MAV_CMD
                                                                //req.param1 = armed ? 0 : 1;//此处发送解锁信息，只需用param1。其余param2—7注释掉
            req.param1 = -1.0f;
            req.param2 = 1;
            req.param3 = 0.0f;
            req.param4 = NAN;
            req.param5 = NAN;
            req.param6 = NAN;
            req.param7 = NAN;
            byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      // byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
                                                                                                      //   mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.SET_POSITION_TARGET_LOCAL_NED, req);
                                                                                                      //foreach (byte i in packet)
                                                                                                      //{
                                                                                                      //    Console.Write("{0:X} ", i);
                                                                                                      //}
            return packet;
        }
        public byte[] return2launch()
        {
            sysid = 1;
            compid = 1;
            // request streams at 2 hz
            byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.SET_MODE,//向飞控申请数据流
                new MAVLink.mavlink_set_mode_t()//飞控发送数据流的格式要求
                {
                    base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED | 0x50,
                    //在指导模式下custom_mode = 4 标示指导模式
                    custom_mode = 0x05040000,
                    target_system = 1
                });
            //foreach (byte i in packet)
            //{
            //    Console.Write("{0:X} ", i);
            //}
            return packet;

         }
        //public byte[] gth()
        //{
        //    MAVLink.mavlink_mission_item_t req = new mavlink_mission_item_t();
        //    req.target_system = 1;
        //    req.target_component = 1;

        //    req.command = (ushort)MAVLink.MAV_CMD.WAYPOINT;

        //    req.current = 0;
        //    req.autocontinue = 1;

        //    req.frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
        //    req.y = (float)10;
        //    req.x = (float)10;
        //    req.z = (float)10;

        //    req.param1 = 0;
        //    req.param2 = 0;
        //    req.param3 = 0;
        //    req.param4 = 0;

        //    req.seq = 0;
        //    byte[] packet = mavlink.GenerateMAVLinkPacket_PX4(MAVLink.MAVLINK_MSG_ID.MISSION_ITEM, req);
        //    return packet;
            
            
        //}
        public byte[] land()
        {
            byte[] data = { 0xFD, 0x06, 0x00, 0x00, 0x54, 0xFF, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x04, 0x06, 0x01, 0x9D, 0xFB, 0x4B };
            return data;
        }
        public byte[] guided()
        {
            return null;
        }
    }
}




