using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ZKTECODeleteUsers
{
    public class AssignNameToUser
    {

        zkemkeeper.CZKEM zk;
        readonly string ip;
        readonly int port = 4370;
        readonly int iMachineNumber = 1;
        bool connected;


        List<Users> list;
        List<DbUser> listNames;
        string where;

        public AssignNameToUser(string ip)
        {
            this.ip = ip;
        }

        public void Init()
        {
            Console.WriteLine("Abre todo");
            this.zk = new zkemkeeper.CZKEM();
            this.zk.SetCommPassword(573757);
            this.connected = zk.Connect_Net(this.ip, this.port);
            this.zk.EnableDevice(this.iMachineNumber, false);
            ChangeNameUserIntoClock();
        }

        public void End()
        {
            Console.WriteLine("Cierra todo *********************");

            zk.EnableDevice(iMachineNumber, true);
            zk.Disconnect();
        }

        public void ChangeNameUserIntoClock()
        {
            bool hasUsers = GetUsersFromClock();

            if (hasUsers)
            {
                GetUserNameFromDataBase();
                PutNameIntoCock();
                SetNameToUser();
            }
        }

        public void SetNameToUser()
        {

            if (this.connected)
            {
                Console.WriteLine("Conecto reloj -  " + this.ip + "para agregar nombre");

                foreach (Users user in this.list)
                {
                    if (!Regex.IsMatch(user.name, @"^\d+$") && user.name.Length > 0)
                    {
                        bool si = zk.SSR_SetUserInfo(iMachineNumber, user.code, user.name, user.password, user.privilege, user.enable);
                        if (si)
                        {
                            Console.WriteLine("Se cambio nombre a " + user.code + " por " + user.name);
                        }
                        else
                        {
                            Console.WriteLine("No se pudo cambiar nombre a " + user.code + " por " + user.name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("El usuario " + user.code + " no tiene nombre");
                    }
                }
            }
            else
            {
                Console.WriteLine("No conecta");
            }

        }

        public void PutNameIntoCock()
        {
            string json = "";
            List<Users> userWithoutCode = new List<Users> { };

            foreach (Users u in this.list)
            {
                DbUser dbUser = this.listNames.Find(e => e.code.Equals(u.code));

                if (!(dbUser is null))
                {
                    if (Regex.IsMatch(u.name, @"^\d+$") || u.name.Length == 0)
                    {
                        u.name = dbUser.name;
                    }
                }
                else
                {
                    userWithoutCode.Add(new Users
                    {
                        code = u.code,
                        enable = u.enable,
                        name = u.name,
                        password = u.password,
                        privilege = u.privilege
                    });
                }
            }

            json = JsonSerializer.Serialize(this.list);
            File.WriteAllText("C:/Users/rcenteno/Desktop/json" + this.ip + ".txt", json);

            json = JsonSerializer.Serialize(userWithoutCode);
            File.WriteAllText("C:/Users/rcenteno/Desktop/json-sin-nombre" + this.ip + ".txt", json);
        }

        public void GetUserNameFromDataBase()
        {

            this.listNames = new List<DbUser> { };

            string cs = "server=172.17.100.100;user=root;password=mdb100ekato;database=zentrum_m";
            MySqlConnection con = new MySqlConnection(cs);
            con.Open();
            string sql = "SELECT code, firstname, lastname FROM (SELECT COD_MAR AS code, NOMB_ AS firstname , APELL_ AS lastname  FROM `dpersonal` WHERE resuelto <> 'NO LABORA') AS user";
            sql += this.where;
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader rdr = (MySqlDataReader)cmd.ExecuteReader();

            Console.WriteLine("Consulta terminada...\n");

            Console.WriteLine("Creando JSON...\n");

            while (rdr.Read())
            {
                string name = rdr[1].ToString().Split(" ")[0] + " " + rdr[2].ToString().Split(" ")[0];

                listNames.Add(new DbUser
                {
                    code = rdr[0].ToString(),
                    name = name
                });
            }
            rdr.Close();
        }

        private bool GetUsersFromClock()
        {
            this.where = "\nWHERE";
            this.list = new List<Users> { };
            if (this.connected)
            {
                this.zk.ReadAllUserID(this.iMachineNumber);
                while (this.zk.SSR_GetAllUserInfo(this.iMachineNumber, out string iEnrollNumber, out string iName, out string iPassword, out int iPrivilege, out bool iEnabled))
                {
                    where += "\ncode='" + iEnrollNumber + "' OR";

                    list.Add(new Users
                    {
                        name = iName,
                        code = iEnrollNumber,
                        password = iPassword,
                        privilege = iPrivilege,
                        enable = iEnabled,
                    });
                }

                where = where.Substring(0, where.Length - 2);

                return true;
            }
            return false;
        }
    }
}
