using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment4
{
    public enum InOutEnum
    {
        Inbox,
        Outbox,
        Trash, 
        None
    }
    public class Mailbox
    {
        public List<Mail> Mails { get; set; }
        public List<List<String>> Tags { get; set; }
        public int Capacity { get; set; }
        public (String, String) Owner { get; set; }
        public InOutEnum IOIndicator { get; set; }

        public Mailbox((String, String) owner, InOutEnum indicator)
        {
            Mails = new List<Mail>();
            Tags = new List<List<String>>();
            Capacity = 5;
            Owner = owner;
            IOIndicator = indicator;
        }

        public Mailbox()
        {
            Mails = new List<Mail>();
            Tags = new List<List<String>>();
            Capacity = 5;
            Owner = ("", "");
            IOIndicator = InOutEnum.None;
        }

        public virtual void DisplayMailbox( )
        {

            Console.WriteLine(IOIndicator + " for " + Owner.Item1 + " <" + Owner.Item2 + ">:");
            if (Mails.Count == 0)
                Console.WriteLine("No mails here!\n");
            else { 
                for(int i = 0; i < Mails.Count; i++) {
                    Console.Write((i+1) + ") ");
                    Console.Write(Tags[i].Count > 0 ? "{" + Tags[i][0] + "} " : "");
                    for (int j = 1; j < Tags[i].Count; j++)
                        Console.Write("{" + Tags[i][j] + "} ");
                    Mails[i].DisplayHeader();
                }
            }
        }

        public Mail ReadMail(int selection) 
        {
            if (selection >= Mails.Count || selection < 0)
                throw new Exception("Incorrect mail selection!");
            if (Tags[selection].Count > 0)
                Console.WriteLine("Tags: " + String.Join(", ", Tags[selection]));
            Console.WriteLine(Mails[selection].ToString());
            return Mails[selection];
        }

        public virtual void DeleteMail(int selection)
        {
            Mails.RemoveAt(selection);
            Tags.RemoveAt(selection);
        }

        public void ReceiveMail(Mail newMail, List<String> newTags)
        {
            if (Mails.Count == Capacity)
                throw new ExceededCapacityException(Owner, IOIndicator);

            Mails.Add(newMail);
            Tags.Add(newTags);
            Console.WriteLine("Success!");
        }

        public virtual void SortByPriority(int mode)
        {
            // mode: 0 for ascending, 1 for descending
            for (int i = 0; i < Mails.Count; i++)
            {
                int j = i;
                if (mode == 1)
                {
                    while (j > 0 && Mails[j - 1].Prio < Mails[j].Prio)
                    {
                        Mail temp = Mails[j - 1];
                        Mails[j - 1] = Mails[j];
                        Mails[j] = temp;
                        j--;
                    }
                }
                else
                {
                    while (j > 0 && Mails[j - 1].Prio > Mails[j].Prio)
                    {
                        Mail temp = Mails[j - 1];
                        Mails[j - 1] = Mails[j];
                        Mails[j] = temp;
                        j--;
                    }
                }
            }
        }

        public virtual void SortByDate(int mode)
        {
            // mode: 0 for ascending, 1 for descending
            for (int i = 0; i < Mails.Count; i++)
            {
                int j = i;
                if (mode == 1)
                {
                    while (j > 0 && Mails[j - 1].Date < Mails[j].Date)
                    {
                        Mail temp = Mails[j - 1];
                        Mails[j - 1] = Mails[j];
                        Mails[j] = temp;
                        j--;
                    }
                }
                else
                {
                    while (j > 0 && Mails[j - 1].Date > Mails[j].Date)
                    {
                        Mail temp = Mails[j - 1];
                        Mails[j - 1] = Mails[j];
                        Mails[j] = temp;
                        j--;
                    }
                }
            }
        }

        public void Sort(int sortBy)
        {
            if (sortBy == 2)
                SortByPriority(0); // ascending
            else if (sortBy == 3)
                SortByPriority(1); // descending
            else if (sortBy == 4)
                SortByDate(0); // ascending
            else
                SortByDate(1); // descending
        }
    }
}
