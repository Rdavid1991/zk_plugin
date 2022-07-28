using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZKTECODeleteUsers
{
    class DeleteFiredUser
    {

        zkemkeeper.CZKEM zk;
        readonly string ip;
        int iMachineNumber;
        Device d = new Device();

        List<string> codes, watchCodes;
        string where;

        public DeleteFiredUser(string ip)
        {
            this.ip = ip;
        }

        public void Init()
        {
            DeviceState response = d.Connect(this.ip);
            if (response.connected)
            {
                this.iMachineNumber = response.iMachineNumber;
                zk = response.zk;
                SearchUser();
            }
        }

        public void End()
        {
            d.Disconnect(zk);
        }

        public void SearchUser()
        {
            var dateAndTime = DateTime.Now;
            var Date = dateAndTime.ToLongDateString();

            string archivo = "\nUsuarios borrados - " + Date + "\n\n";
            int borrados = 0;
            archivo += this.ip + "\n";
            string[] usuarios = { };

            GetUserCodeFromDataBase();
            GetUsersFromClock();

            foreach (string code in codes)
            {
                if (watchCodes.Find((e) => e == code) != null)
                {
                    bool deleted = zk.SSR_DeleteEnrollData(1, code, 12);
                    if (deleted)
                    {
                        Console.WriteLine("funciono " + code);
                        archivo += "usuario " + code + " borrado \n";
                        borrados++;
                    }
                }
            }

            Console.WriteLine("Se borraron " + borrados + "Usuarios");
            archivo += "Se borraron " + borrados + "Usuarios\n\n";

            File.AppendAllTextAsync("C:/Users/rcenteno/Desktop/Usuarios_borrados.txt", archivo);
        }

        public void GetUserCodeFromDataBase()
        {

            codes = new List<string> { };

            string cs = "server=172.17.100.100;user=root;password=mdb100ekato;database=zentrum_m";
            MySqlConnection con = new MySqlConnection(cs);
            con.Open();
            string sql = "SELECT code, firstname, lastname FROM (SELECT COD_MAR AS code, NOMB_ AS firstname , APELL_ AS lastname  FROM `dpersonal` WHERE resuelto = 'NO LABORA' and (`COD_MAR` <> '0' OR `COD_MAR` <> null)) AS user";
            sql += this.where;
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader rdr = (MySqlDataReader)cmd.ExecuteReader();

            Console.WriteLine("Consulta terminada...\n");

            Console.WriteLine("Creando JSON...\n");

            while (rdr.Read())
            {
                codes.Add(rdr[0].ToString());
            }
            rdr.Close();
        }

        private void GetUsersFromClock()
        {
            this.where = "\nWHERE";
            this.watchCodes = new List<string> { };

            this.zk.ReadAllUserID(this.iMachineNumber);
            while (this.zk.SSR_GetAllUserInfo(this.iMachineNumber, out string iEnrollNumber, out string iName, out string iPassword, out int iPrivilege, out bool iEnabled))
            {
                where += "\ncode='" + iEnrollNumber + "' OR";
                watchCodes.Add(iEnrollNumber);
            }

            where = where.Substring(0, where.Length - 2);
        }
    }


}
