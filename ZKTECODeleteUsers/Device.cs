using System;
using System.Collections.Generic;
using System.Text;

namespace ZKTECODeleteUsers
{
    class Device
    {
        bool connected;
        int iMachineNumber;
        string ip;
        public DeviceState Connect(string ip, int password = 573757, int port = 4370, int iMachineNumber = 1 )
        {
            this.ip = ip.Trim();
            this.iMachineNumber = iMachineNumber;
            zkemkeeper.CZKEM zk = new zkemkeeper.CZKEM();
            zk.SetCommPassword(password);
            this.connected = zk.Connect_Net(ip.Trim(), port);

            if (connected)
            {
                zk.EnableDevice(iMachineNumber, false);
                Console.WriteLine("Conexion satisfactoria con - {0}\n", ip);
                return new DeviceState { connected = true, zk = zk, iMachineNumber = this.iMachineNumber };
            }

            Console.WriteLine("Fallo conexión con - {0} \n", ip);
            return new DeviceState { connected = false, zk = zk,iMachineNumber=this.iMachineNumber };
        }

        public void Disconnect(zkemkeeper.CZKEM zk)
        {
            if (this.connected)
            {
                Console.WriteLine("\nProceso finalizado, se desconecto de - {0}\n", this.ip);
                zk.EnableDevice(this.iMachineNumber, true);
                zk.Disconnect();
            }
        }
    }
}
