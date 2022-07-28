using System;
using System.Collections.Generic;
using System.Text;

namespace ZKTECODeleteUsers
{
    public class DeviceState
    {
        public bool connected { get; set; }
        public zkemkeeper.CZKEM zk { get; set; }
        public int iMachineNumber { get; set; }

    }
}
