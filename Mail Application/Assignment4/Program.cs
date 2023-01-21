using System;
using System.Collections.Generic;

namespace Assignment4
{
    class Program
    {
        public static List<User> users = new List<User>();

        static void Main(string[] args)
        {
            // Testing done here, you can experiment with more.

            
            User ke = new User("Kutluhan Erol", "ke");
            User ko = new User("Kaya Oğuz", "ko");
            User ce = new User("Cem Evrendilek", "ce");
            User tt = new User("Turhan Tunalı", "tt");
            User ad = new User("Alper Demir", "ad");

            User cg = new User("Cinar Gedizlioglu", "cg");
            User ha = new User("Hande Aka Uymaz", "ha");
            User eo = new User("Erdem Okur", "eo");
            User su = new User("Serhat Uzunbayır", "su");
            User bt = new User("Melek Büşra Temuçin", "bt");
            User cu = new User("Çağkan Uludağlı", "cu");
            User td = new User("Tugay Direk", "td");

            
            users.Add(ke);
            users.Add(ko);
            users.Add(ce);
            users.Add(tt);
            users.Add(ad);

            users.Add(cg);
            users.Add(ha);
            users.Add(eo);
            users.Add(su);
            users.Add(bt);
            users.Add(cu);
            users.Add(td);

            cg.ComposeMail();
            ke.ComposeMail();
            cg.ComposeMail();
            ke.ComposeMail();

            ke.ReadMailbox("I");
            ke.ReadMailbox("I");
            ke.ReadMailbox("O");
            ke.ReadMailbox("O");
            ke.ReadMailbox("T");
            ke.ReadMailbox("T");
            ke.ComposeMail();
            
            ke.ComposeMail();
            cg.ComposeMail();

            ke.ReadMailbox("I");
            eo.ReadMailbox("I");
            su.ReadMailbox("I");
            cu.ReadMailbox("I");

            cg.ReadMailbox("I");
        }
    }
}
