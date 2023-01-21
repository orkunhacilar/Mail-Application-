using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment4
{

    public class Trash : Mailbox
    {
        public List<InOutEnum> InitialLocations { get; set; }

        public Trash((String, String) owner) : base(owner, InOutEnum.Trash)
        {
            InitialLocations = new List<InOutEnum>();
        }
        public Trash() : base()
        {
            InitialLocations = new List<InOutEnum>();
        }

        public InOutEnum GetLocation(int index)
        {
            return InitialLocations[index];
        }

        public override void DeleteMail(int index)
        {
            base.DeleteMail(index);
            InitialLocations.RemoveAt(index);
        }

        public void AddMail(Mail deletedMail, InOutEnum prevLoc, List<String> deletedMailTags)
        {
            ReceiveMail(deletedMail, deletedMailTags);
            InitialLocations.Add(prevLoc);
        }

        public override void DisplayMailbox()
        {
            if (Mails.Count == 0)
                Console.WriteLine("No mails here!");
            else
            {
                for (int i = 0; i < Mails.Count; i++)
                {
                    Console.Write("[" + (InitialLocations[i] == InOutEnum.Inbox ? "Inbox" : "Outbox") + "] " + (i + 1) + ") ");
                    Mails[i].DisplayHeader();
                }
            }
        }

        public override void SortByPriority(int mode)
        {
            // mode: 0 for ascending, 1 for descending
            for (int i = 0; i < Mails.Count; i++)
            {
                int j = i;
                if (mode == 1)
                {
                    while (j > 0 && Mails[j - 1].Prio < Mails[j].Prio)
                    {
                        Mail tempMail = Mails[j - 1];
                        Mails[j - 1] = Mails[j];
                        Mails[j] = tempMail;
                        InOutEnum tempEnum = InitialLocations[j - 1];
                        InitialLocations[j - 1] = InitialLocations[j];
                        InitialLocations[j] = tempEnum;
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
                        InOutEnum tempEnum = InitialLocations[j - 1];
                        InitialLocations[j - 1] = InitialLocations[j];
                        InitialLocations[j] = tempEnum;
                        j--;
                    }
                }
            }
        }

        public override void SortByDate(int mode)
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
                        InOutEnum tempEnum = InitialLocations[j - 1];
                        InitialLocations[j - 1] = InitialLocations[j];
                        InitialLocations[j] = tempEnum;
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
                        InOutEnum tempEnum = InitialLocations[j - 1];
                        InitialLocations[j - 1] = InitialLocations[j];
                        InitialLocations[j] = tempEnum;
                        j--;
                    }
                }
            }
        }
    }
}
