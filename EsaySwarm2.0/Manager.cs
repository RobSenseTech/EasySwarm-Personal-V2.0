using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lora;

namespace EasySwarm2._0
{
    class Manager
    {
        private MAVLink.IMAVLink_API control;
        private Swarmlink swarmlink;
        private SerialPort serialport;

        public delegate void delegateCheckNodeEvent(object sender, Lora.StringEvent e);
        public event delegateCheckNodeEvent delecheckNodeEvent;

        public delegate void delegateGetRssiEvent(object sender, Lora.IntEvent e);
        public event delegateGetRssiEvent deleGetRssiEvent;

        public  void OnlineNodeChanged(object sender, Lora.StringEvent e)
        {
            this.delecheckNodeEvent(sender, e);
        }

        public void GetRssi(object sender, Lora.IntEvent e)
        {
            node_dBm = e.node_value;
            gw_dBm = e.gw_value;
            receiveNum = e.receive_num;
            this.deleGetRssiEvent(sender, e);
        }

        public Manager(string firmware_type)
        {
            if (firmware_type == "APM")
            {
                control = new MAVLink.MAVInterface_APM();
            }
            else if (firmware_type == "PX4")
            {
                control = new MAVLink.MAVInterface_PX4();
            }
            receiveNum = -1;
        }
      
        /// <summary>
        /// 串口操作
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public bool connect(string com)
        {
            try
            {
                swarmlink = new Swarmlink();
                swarmlink.getRssiEvent += new Swarmlink.getRssiEventInvoke(GetRssi);
                swarmlink.delecheckNodeEvent += new Swarmlink.delegateCheckNodeEvent(OnlineNodeChanged);
                serialport = new SerialPort();
                serialport.Parity = Parity.None;         //Check None
                serialport.DataBits = 8;                    //Data length 8
                serialport.StopBits = StopBits.One;   //Stop bits 1
                serialport.BaudRate = 115200;
                serialport.PortName = com;
                serialport.DataReceived += sp_DataReceived;//new SerialDataReceivedEventHandler(sp_DataReceived);
                serialport.Open();
                serialport.ReadTimeout = 5000;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool disconnect()
        {
            try
            {
                serialport.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("sp_dataReceived");
            int count = serialport.BytesToRead;
            byte[] re_buf = new byte[count];//Creat receive array.   
            serialport.Read(re_buf, 0, count);//Read receive data. 
           
            foreach (byte i in re_buf)
            {
                swarmlink.decode(i);
            }
        }
        /// <summary>
        /// 无人机控制相关
        /// </summary>
        /// <returns></returns>
        public bool arm()
        {
            try
            {
                broadcast(control.arm());
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool arm(byte[] mac_address)
        {
            try
            {
                unicast(mac_address, control.arm());
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool land()
        {
            try
            {
                broadcast(control.land());
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool land(byte[] mac_address)
        {
            try
            {
                unicast(mac_address, control.land());
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool pause()
        {
            try
            {
                broadcast(control.pause());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool pause(byte[] mac_address)
        {
            try
            {
                unicast(mac_address, control.pause());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool disarm()
        {
            try
            {
                broadcast(control.disarm());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool disarm(byte[] mac_address)
        {
            try
            {
                unicast(mac_address, control.disarm());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool return2launch()
        {
            try
            {
                broadcast(control.return2launch());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool return2launch(byte[] mac_address)
        {
            try
            {
                unicast(mac_address, control.return2launch());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool takeoff(float altitude)
        {
            try
            {
                //APM起飞前要设置飞行器模式GUIDED，连发两次确保模式切换成功
                broadcast(control.guided());
                System.Threading.Thread.Sleep(100);
                broadcast(control.guided());
                System.Threading.Thread.Sleep(100);
                broadcast(control.takeoff(altitude));
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool takeoff(byte[] mac_address, float altitude)
        {
            try
            {
                //APM起飞前要设置飞行器模式GUIDED，连发两次确保模式切换成功
                unicast(mac_address, control.guided());
                System.Threading.Thread.Sleep(100);
                unicast(mac_address, control.guided());
                System.Threading.Thread.Sleep(100);
                unicast(mac_address, control.takeoff(altitude));
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool changealtitude(float altitude)
        {
            try
            {
                broadcast(control.changealtitude(altitude));
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool changealtitude(byte[] mac_address, float altitude)
        {
            try
            {
                unicast(mac_address, control.changealtitude(altitude));
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool gotolocation(float longitude, float latitude, float altitude)
        {
            try
            {
                broadcast(control.gotolocation(longitude, latitude, altitude));
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool gotolocation(byte[] mac_address, float longitude, float latitude, float altitude)
        {
            try
            {
                unicast(mac_address, control.gotolocation(longitude, latitude, altitude));
                return true;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// lora相关
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool broadcast(byte[] data)
        {
            try
            {
                byte[] buf = swarmlink.broadcast(data);
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool unicast(byte[] mac_address, byte[] data)
        {
            try
            {
                byte[] buf = swarmlink.unicast(mac_address, data);
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool multicast(byte[] list, byte[] data)
        {
            try
            {
                byte[] buf = swarmlink.multicast(list, data);
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool add_node(byte[] node)
        {
            try
            {
                byte[] buf = swarmlink.add_node(node);
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool delete_node(byte[] node)
        {
            try
            {
                byte[] buf = swarmlink.delete_node(node);
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool set_node_mac(byte[] old_mac, byte[] new_mac)
        {
            try
            {
                byte[] buf = swarmlink.set_node_mac(old_mac, new_mac);
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool refresh_ap()
        {
            try
            {
                byte[] buf = swarmlink.refresh_ap();
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string get_settings()
        {
            return swarmlink.get_settings();
        }
        public byte[] get_payload()
        {
            return swarmlink.get_payload();
        }
        public void clear_settings()
        {
            swarmlink.clear_settings();
        }


        public void CheckNode(string mac)
        {
            uint Get_Mac = uint.Parse(mac, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] node_mac = new byte[2];
            node_mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x000000ff);//Data convert
            node_mac[1] = Convert.ToByte(Get_Mac & 0x000000ff);//Data convert
            byte[] data = new byte[4] { 0x00, 0x01, 0x0A, 0x00 };
            byte[] get_data = swarmlink.unicast_cmd(node_mac, data);
            if (serialport.IsOpen)
                serialport.Write(get_data, 0, get_data.Length);//Send.
        }

        public int node_dBm { get; set;}
        public int gw_dBm { get; set; }
        public int receiveNum { get; set; }

        public bool get_rssi(byte[] node)
        {
            try
            {
                byte[] cmd = new byte[5];
                //cmd[0] = 0x00;
                //cmd[1] = 0x01;
                //cmd[2] = 0x0f;
                //cmd[3] = 0x00;
                cmd[0] = 0x05;
                cmd[1] = 0x00;
                cmd[2] = 0x01;
                cmd[3] = 0x2C;
                cmd[4] = 0x00;
                byte[] buf = swarmlink.frame_rssi(node, cmd, 0);
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool get_rssi(byte[] node, byte count)
        {
            try
            {

                byte[] cmd = new byte[5];
                //cmd[0] = 0x00;
                //cmd[1] = 0x01;
                //cmd[2] = 0x0f;
                //cmd[3] = 0x00;
                cmd[0] = 0x05;
                cmd[1] = 0x00;
                cmd[2] = 0x01;
                cmd[3] = 0x2C;
                cmd[4] = 0x00;
                byte[] buf = swarmlink.frame_rssi(node, cmd, count);
                serialport.Write(buf, 0, buf.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
