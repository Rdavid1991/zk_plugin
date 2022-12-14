using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZKTECODeleteUsers
{
    class CommandSyncTime
    {

        string command = "";
        string[] clockList = { };
        public CommandSyncTime(string command)
        {
            this.command = command;
        }

        public string[] ExecuteCommand()
        {
            if (Regex.IsMatch(command, @"(?<=sync-time --ip)\s(\s{0,1},{0,1}\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})+\z"))
            {
                Match m = Regex.Match(command, @"(?<=--ip)\s(\s{0,1},{0,1}\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})+\z");

                clockList = m.Value.Split(",");
            }
            else
            {
                Match m = Regex.Match(command, @"(?<=sync-time).*?\z");
                
                Console.WriteLine(m.Value.Length > 0? "{0} No es una opcion para sync-time" : "Hace falta opciones o parametros", m.Value);

                Console.WriteLine("sync-time <option> <params>");
                Console.WriteLine("\t--ip....................Lista de ip separado por coma. ejemplo: --ip 1.1.1.1,2.2.2.2");
            }

            return clockList;
        }
    }
}
