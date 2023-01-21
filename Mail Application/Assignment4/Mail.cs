using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment4
{
    public enum PriorityEnum
    {
        Low, Normal, High
    }

    public class Mail
    {
        public DateTime Date { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }
        public PriorityEnum Prio { get; set; }
        public (String, String) Sender { get; set; }
        public (String, String) Receiver { get; set; }
        public List<(String, String)> CC { get; set; }

        public Mail(DateTime date, (String, String) sender, (String, String) receiver, List<(String, String)> cc, String subject, String body, PriorityEnum prio)
        {
            Date = date; Sender = sender; Receiver = receiver; CC = cc;  Subject = subject; Body = body; Prio = prio;
        }

        public Mail()
        {
            Date = DateTime.Now; Sender = ("", ""); Receiver = ("", ""); CC = new List<(string, string)>(); Subject = ""; Body = ""; Prio = PriorityEnum.Normal;
        }

        public override String ToString()
        {
            String finalString = Date.ToShortDateString() + ", " + Date.ToShortTimeString() + "\n";
            finalString += "From: " + Sender.Item1 + " <" + Sender.Item2 + ">\n";
            finalString += "To: " + Receiver.Item1 + " <" + Receiver.Item2 + ">\n";
            finalString += CC.Count > 0 ? "CC: " + CC[0].Item1 + " <" + CC[0].Item2 + ">" : "";
            for(int i = 1; i < CC.Count; i++)
                finalString += ", " + CC[i].Item1 + " <" + CC[i].Item2 + ">";
            finalString += Prio != PriorityEnum.Normal ? "\n\n[Priority: " + Prio + "] ": "\n\n";
            finalString += "Subject: " + Subject + "\n\n" + Body;
            return finalString; 
        }

        public virtual void DisplayHeader() 
        {
            Console.Write(Date.ToString() + " --- ");
            Console.Write(Prio != PriorityEnum.Normal ? "[Priority: " + Prio + "] " : "");
            Console.WriteLine("Subject: " + Subject); 
        }
    }
}
