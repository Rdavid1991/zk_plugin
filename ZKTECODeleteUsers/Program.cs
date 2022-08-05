using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace ZKTECODeleteUsers
{
    class Program
    {

        static void Main(string[] args)
        {

            string[] arg = Environment.GetCommandLineArgs();
            string command = string.Join(" ", arg);

            string[] relojes = {
                //"10.102.1.191",   // cafeteria
                //"10.104.7.242",   // Principal 1
                //"10.102.1.190",   // Principal2
                //"10.102.1.187",   // ODSS
                //"10.104.7.7",     // Recursos humanos
                //"10.103.0.125",   // Administracion1
                //"10.103.0.126",   // Administracion2
                //"10.102.2.188",   // COAI
                //"10.104.13.109",  // chepo
                //"172.20.70.241",  // Informatica
                //"10.104.9.126",    // veraguas
                //"10.104.6.201" //Almacen
            };

            if (Regex.IsMatch(command, @"(?<=sync-time).*?\z"))
            {
                CommandSyncTime cst = new CommandSyncTime(command);
                relojes = cst.ExecuteCommand();
                for (int i = 0; i < relojes.Length; i++)
                {
                    SyncTimeIntoWatch(relojes[i].Trim());
                }
            }else if (Regex.IsMatch(command, @"(?<=delete-fired-user).*?\z"))
            {
                DeleteFiredUserCommand dfuc = new DeleteFiredUserCommand(command);
                relojes = dfuc.ExecuteCommand();
                for (int i = 0; i < relojes.Length; i++)
                {
                    DeleteFiredUsersIntoWatch(relojes[i]);
                }
            }else if(Regex.IsMatch(command, @"(?<=change-name-user).*?\z"))
            {
                AssignNameToUserCommand anuc = new AssignNameToUserCommand(command);
                relojes = anuc.ExecuteCommand();
                for (int i = 0; i < relojes.Length; i++)
                {
                    AssignNameToUserIntoWatch(relojes[i]);
                }
            }else
            {
                Console.WriteLine("#Options");
                Console.WriteLine("#\tsync-time\t\ta");
                Console.WriteLine("#\tdelete-fired-user\t\ta");
                Console.WriteLine("#\tchange-name-user\t\ta");
            }
        }

        public static void AssignNameToUserIntoWatch(string reloj)
        {
            AssignNameToUser assign = new AssignNameToUser(reloj);
            assign.Init();
            assign.End();
        }

        static void DeleteFiredUsersIntoWatch(string ip)
        {
            DeleteFiredUser df = new DeleteFiredUser(ip);
            df.Init();
            df.End();
        }

        static void SyncTimeIntoWatch(string ip)
        {
            SyncTime st = new SyncTime(ip);
            st.Init();
            st.End();
        }

        static void getFinger()
        {
            zkemkeeper.CZKEM zk = new zkemkeeper.CZKEM();
            //string[] ip = {  };
            string[] relojes = { 
                //"10.102.1.191", // cafeteria
                "10.104.7.242",
                //"10.102.1.190",
                //"10.102.1.187",
                //"10.104.7.7",
                //"10.103.0.125",
                //"10.103.0.126",
                //"10.102.2.188",
                //"172.20.70.241"
            };
            int port = 4370;



            for (int r = 0; r < relojes.Length; r++)
            {
                string json = "";
                int iMachineNumber = 1;
                string iEnrollNumber;
                int iFinger = 0;
                int iPassword = 0;
                int iPrivilege = 0;
                bool iEnabled;

                zk.SetCommPassword(573757);
                bool conecto = zk.Connect_Net(relojes[r], port);

                if (conecto)
                {
                    Console.WriteLine("conecto " + relojes[r]);

                    List<EnrollUser> list = new List<EnrollUser> { };

                    zk.EnableDevice(iMachineNumber, false);
                    zk.ReadAllUserID(iMachineNumber);//read all the user information to the memory

                    bool obtivo = zk.GetEnrollData(iMachineNumber, iMachineNumber, 9028, 9, ref iPrivilege, ref iFinger, ref iPassword);

                    list.Add(new EnrollUser
                    {
                        privilege = iPrivilege,
                        finger = iFinger,
                        password = iPassword
                    });

                    json = JsonSerializer.Serialize(list);


                    File.WriteAllTextAsync("C:/Users/rcenteno/Desktop/json_enroll" + relojes[r] + ".txt", json);
                    zk.EnableDevice(iMachineNumber, true);
                    zk.Disconnect();
                }
                else
                {
                    Console.WriteLine("No conecta");
                }
            }
        }
    }
}

public class Users
{
    public string code { get; set; }
    public string name { get; set; }
    public string password { get; set; }
    public int privilege { get; set; }
    public bool enable { get; set; }
}

public class EnrollUser
{
    public int privilege { get; set; }
    public int finger { get; set; }
    public int password { get; set; }
}

public class DbUser
{
    public string code { get; set; }
    public string name { get; set; }
}