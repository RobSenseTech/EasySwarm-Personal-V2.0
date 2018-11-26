using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public partial class MAVLink
{
    public class MAVInterface_APM : IMAVLink_API
    {
        private MAVLink.MavlinkParse mavlink = new MAVLink.MavlinkParse();
        // locking to prevent multiple reads on serial port
        private object readlock = new object();
        // our target sysid
        private byte sysid;
        // our target compid
        private byte compid;
        bool armed = false;
        public byte[] takeoff(float altitude)
        {
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();//定义req为 MAVLink.mavlink_command_long_t类，填充常命令的数据包
            req.target_system = 1;
            req.target_component = 1;
            req.command = (ushort)MAVLink.MAV_CMD.TAKEOFF;//‘COMPONENT_ARM_DISARM’代表你要发送什么类的信息（此处为解锁信息），可自己选择（看类里有多少种）
                                                          //req.command = (ushort)MAVLink.MAV_CMD
                                                          //req.param1 = armed ? 0 : 1;//此处发送解锁信息，只需用param1。其余param2—7注释掉
            req.param1 = 0;
            req.param2 = 0;
            req.param3 = 0;
            req.param4 = 0;
            req.param5 = 0;
            req.param6 = 0;
            req.param7 = altitude;
            byte[] packet = mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
            return packet;
        }
        public byte[] gotolocation(float longitude, float latitude, float altitude)
        {
            mavlink_mission_item_t req = new mavlink_mission_item_t();
            req.target_system = 1;
            req.target_component = 1;
            req.command = (ushort)MAVLink.MAV_CMD.WAYPOINT;
            req.current = 2;
            req.autocontinue = 1;
            req.frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
            req.y = (float)longitude;//lng
            req.x = (float)latitude;//lat
            req.z = (float)altitude;//alt
            req.param1 = 0;
            req.param2 = 0;
            req.param3 = 0;
            req.param4 = 0;
            req.seq = 0;
            byte[] packet = mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.MISSION_ITEM, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
            return packet;

        }
        public byte[] arm()
        {
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();

            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM;

            req.param1 = 1;
            /*
            req.param2 = p2;
            req.param3 = p3;
            req.param4 = p4;
            req.param5 = p5;
            req.param6 = p6;
            req.param7 = p7;
            */

            return mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);
        }
        public byte[] disarm()
        {
            MAVLink.mavlink_command_long_t req = new MAVLink.mavlink_command_long_t();

            req.target_system = 1;
            req.target_component = 1;

            req.command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM;

            req.param1 = 0;
            /*
            req.param2 = p2;
            req.param3 = p3;
            req.param4 = p4;
            req.param5 = p5;
            req.param6 = p6;
            req.param7 = p7;
            */

            return mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, req);
        }
        public byte[] arm_disarm()
        {
            return null;
        }
        public byte[] guided()
        {
            sysid = 1;
            compid = 1;
            // request streams at 2 hz
            byte[] packet = mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.SET_MODE,//向飞控申请数据流
                new MAVLink.mavlink_set_mode_t()//飞控发送数据流的格式要求
                {
                    base_mode = 1,
                    //在指导模式下custom_mode = 4 标示指导模式
                    custom_mode = 4,
                    target_system = 1
                });
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
            byte[] packet = mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.SET_MODE,//向飞控申请数据流
                new MAVLink.mavlink_set_mode_t()//飞控发送数据流的格式要求
                {
                    base_mode = 1,
                    //在指导模式下custom_mode = 4 标示指导模式
                    custom_mode = 6,
                    target_system = 1
                });
            //foreach (byte i in packet)
            //{
            //    Console.Write("{0:X} ", i);
            //}
            return packet;

        }
        public byte[] changealtitude(float altitude)
        {
            float NAN = (((float)((1e+300) * (1e+300))) * 0.0F);
            mavlink_mission_item_t req = new mavlink_mission_item_t();
            req.target_system = 1;
            req.target_component = 1;
            req.command = (ushort)MAVLink.MAV_CMD.WAYPOINT;
            req.current = 2;
            req.autocontinue = 1;
            req.frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
            req.y = NAN;//lng
            req.x = NAN;//lat
            req.z = (float)altitude;//alt
            req.param1 = 0;
            req.param2 = 0;
            req.param3 = 0;
            req.param4 = 0;
            req.seq = 0;
            byte[] packet = mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.MISSION_ITEM, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
            return packet;
        }
        public byte[] pause()
        {
            float NAN = (((float)((1e+300) * (1e+300))) * 0.0F);
            mavlink_mission_item_t req = new mavlink_mission_item_t();
            req.target_system = 1;
            req.target_component = 1;
            req.command = (ushort)MAVLink.MAV_CMD.WAYPOINT;
            req.current = 2;
            req.autocontinue = 1;
            req.frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
            req.y = NAN;//lng
            req.x = NAN;//lat
            req.z = NAN;//alt
            req.param1 = 0;
            req.param2 = 0;
            req.param3 = 0;
            req.param4 = 0;
            req.seq = 0;
            byte[] packet = mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.MISSION_ITEM, req);//把req数据包，按照mavlink协议打包成（定义了临时变量packet）
            return packet;
        }
        public byte[] land()
        {
            sysid = 1;
            compid = 1;
            // request streams at 2 hz
            byte[] packet = mavlink.GenerateMAVLinkPacket_APM(MAVLink.MAVLINK_MSG_ID.SET_MODE,//向飞控申请数据流
                new MAVLink.mavlink_set_mode_t()//飞控发送数据流的格式要求
                {
                    base_mode = 1,
                    //在指导模式下custom_mode = 4 标示指导模式
                    custom_mode = 9,
                    target_system = 1
                });
            //foreach (byte i in packet)
            //{
            //    Console.Write("{0:X} ", i);
            //}
            return packet;
        }
    }
}




