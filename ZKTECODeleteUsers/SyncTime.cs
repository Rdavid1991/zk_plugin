using System;
using System.Collections.Generic;
using System.Text;

namespace ZKTECODeleteUsers
{
    class SyncTime
    {
        zkemkeeper.CZKEM zk;
        readonly string ip;
        readonly int iMachineNumber = 1;

        Device device = new Device();

        public SyncTime(string ip)
        {
            this.ip = ip;
        }

        public void Init()
        {
            DeviceState response = device.Connect(this.ip);
            if (response.connected)
            {
                this.zk = response.zk;
                SyncTimeInToWatch();
            }
        }

        public void End()
        {
            device.Disconnect(this.zk);           
        }

        private void SyncTimeInToWatch()
        {

            bool synchronized = false;
            synchronized = this.zk.SetDeviceTime(this.iMachineNumber);

            if (synchronized)
            {
                Console.WriteLine("Tiempo sincronizado en - {0} ", this.ip);
            }
            else
            {
                Console.WriteLine("Tiempo no se pudo sincronizar en - {0}", this.ip);
            }
        }
    }
}
