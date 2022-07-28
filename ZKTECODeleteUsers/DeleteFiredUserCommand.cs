using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZKTECODeleteUsers
{
    class DeleteFiredUserCommand
    {
        string command = "";
        string[] clockList = { };
        public DeleteFiredUserCommand(string command)
        {
            this.command = command;
        }

        public string[] ExecuteCommand()
        {

            if (Regex.IsMatch(command, @"(?<=delete-fired-user --ip)\s(\s{0,1},{0,1}\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})+\z"))
            {
                Match m = Regex.Match(command, @"(?<=--ip)\s(\s{0,1},{0,1}\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})+\z");

                clockList = m.Value.Split(",");
            }
            else
            {
                Match m = Regex.Match(command, @"(?<=delete-fired-user).*?\z");

                Console.WriteLine(m.Value.Length > 0 ? "{0} No es una opcion para borrar usuarios que no trabajan" : "Hace falta opciones o parametros", m.Value);

                Console.WriteLine("delete-fired-user <option> <params>");
                Console.WriteLine("\t--ip....................Lista de ip separado por coma. ejemplo: --ip 1.1.1.1,2.2.2.2");
            }

            return clockList;
        }
    }
}
