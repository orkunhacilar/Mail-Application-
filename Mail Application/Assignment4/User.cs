using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Assignment4
{
    public class User
    {
        public String Name { get; set; }
        public String EmailAddress { get; set; }
        public Mailbox Inbox { get; set; }
        public Mailbox Outbox { get; set; }
        public Trash Trash { get; set; }
        public List<String> Tags { get; set; }

        public User()
        {
            Name = ""; EmailAddress = "";
            Inbox = new Mailbox();
            Outbox = new Mailbox();
            Trash = new Trash(("", ""));
            Tags = new List<String>();
        }

        public User(String name, String emailAddress)
        {
            Name = name; EmailAddress = emailAddress;
            Inbox = new Mailbox((name, emailAddress), InOutEnum.Inbox); 
            Outbox = new Mailbox((name, emailAddress), InOutEnum.Outbox); 
            Trash = new Trash((name, emailAddress));
            Tags = new List<String>();
        }

        public void ReadMailbox(String mailbox)
        {
            Mailbox selectedMailbox = SelectMailbox(mailbox);
            if (selectedMailbox is null)
            {
                Console.WriteLine("Incorrect input.");
                return;
            }

            selectedMailbox.DisplayMailbox();
            if (selectedMailbox.Mails.Count <= 0)
                return;
            Console.WriteLine("\n1) Read a mail\n2) Sort by priority (asc)\n3) Sort by priority (desc)\n4) Sort by date (asc)\n5) Sort by date (desc)\n6) Exit");
            Console.Write("\nChoose an action : ");
            int selection = Convert.ToInt32(Console.ReadLine());
            while (selection != 1 && selection != 6)
            {
                Console.Clear();
                selectedMailbox.Sort(selection);
                selectedMailbox.DisplayMailbox();
                Console.WriteLine("\n1) Read a mail\n2) Sort by priority (asc)\n3) Sort by priority (desc)\n4) Sort by date (asc)\n5) Sort by date (desc)\n6) Exit");
                Console.Write("\nChoose an action : ");
                selection = Convert.ToInt32(Console.ReadLine());
            }

            if (selection == 1)
                Read(mailbox, selectedMailbox);
        }

        private void Read(String mailbox, Mailbox selectedMailbox)
        {
            Console.Clear();
            selectedMailbox.DisplayMailbox();
            try
            {
                Console.Write("\nEnter the number for the mail you want to read: ");
                int mailSelection = Convert.ToInt32(Console.ReadLine());
                Console.Clear();
                Mail selectedMail = selectedMailbox.ReadMail(mailSelection - 1);

                Console.WriteLine("\n1) Forward\n2) Reply\n3) Reply To All\n4) Delete \n5) Add Tag" + (mailbox == "T" ? "\n6) Restore" : ""));
                Console.Write("\nChoose an action (-1 to exit): ");
                int selection = Convert.ToInt32(Console.ReadLine());

                if (selection == -1)
                {
                    Console.Clear();
                    return;
                }
                else if (selection == 1)
                    ForwardMail(selectedMail);
                else if (selection == 2)
                    ReplyToMail(selectedMail);
                else if (selection == 3)
                    ReplyToAll(selectedMail);
                else if (selection == 4)
                    DeleteMail(selectedMail, mailSelection, selectedMailbox);
                else if (selection == 5)
                    TagMail(mailSelection, selectedMailbox);
                else if (mailbox == "T" && selection == 6)
                    RestoreMail(selectedMail, mailSelection);
            }
            catch (InvalidEmailAddressException e)
            {
                e.PrintException();
            }
            catch (ExceededCapacityException e)
            {
                e.PrintException(false);
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        public void ComposeMail()
        {
            Console.Write(Name + " <" + EmailAddress + "> composing a mail to: ");
            String emailAddress = Console.ReadLine();
            try
            {
                User recipient = FindUser(emailAddress);
       
                Console.WriteLine("Enter recipient e-mail addresses for the CC list (separate each address with a comma):");
                String emailAddressesStr = Console.ReadLine().Replace(" ", "");
                List<(String, String)> ccList = new List<(String, String)>();
                List<User> ccUserList = new List<User>();

                List<String> emailAddressesList = emailAddressesStr == "" ? new List<String>() : emailAddressesStr.Split(',').ToList();
                foreach(String item in emailAddressesList)
                {
                    try
                    {
                        User ccRecipient = FindUser(item);
                        ccList.Add((ccRecipient.Name, ccRecipient.EmailAddress));
                        ccUserList.Add(ccRecipient);
                    }
                    catch (InvalidEmailAddressException e)
                    {
                        e.PrintCCException();
                    }
                }

                Console.WriteLine("Enter subject:");
                String subject = Console.ReadLine();
                Console.WriteLine("Enter body:");
                String body = Console.ReadLine();
                Console.Write("Enter priority (0 for low, 1 for normal, 2 for high): ");
                PriorityEnum prio = (PriorityEnum) Convert.ToInt32(Console.ReadLine());

                (String, String) sender = (Name, EmailAddress);
                (String, String) receiver = (recipient.Name, recipient.EmailAddress);

                Mail composedMail = new Mail(DateTime.Now, sender, receiver, ccList, subject, body, prio);

                Send(this, recipient, ccUserList, composedMail);

            }
            catch (InvalidEmailAddressException e)
            {
                e.PrintException();
            }
            catch (ExceededCapacityException e)
            {
                e.PrintException(false);
            }
        }
        
        public void ReplyToMail(Mail selectedMail)
        {
            // Am I the sender of the original mail?
            // Then I'm replying to the receiver. Otherwise, I'm replying to the sender.
            User recipient = selectedMail.Sender.Item2 == EmailAddress ? FindUser(selectedMail.Receiver.Item2) : FindUser(selectedMail.Sender.Item2);

            String subject = "Re: " + selectedMail.Subject;

            Console.WriteLine(Name + " <" + EmailAddress + "> replying to " + recipient.Name + " <" + recipient.EmailAddress + ">");

            Console.WriteLine("Subject: " + subject);
            Console.WriteLine("Enter body:");
            String body = Console.ReadLine() + "\n----------------------------------------------------------------------\n" + selectedMail.ToString();

            (String, String) sender = (Name, EmailAddress);
            (String, String) receiver = (recipient.Name, recipient.EmailAddress);

            Mail composedMail = new Mail(DateTime.Now, sender, receiver, new List<(String, String)>(), subject, body, selectedMail.Prio);

            Send(this, recipient, new List<User>(), composedMail);
        }

        public void ReplyToAll(Mail selectedMail)
        {
            // Am I the sender of the original mail?
            // Then I'm replying to the receiver. Otherwise, I'm replying to the sender.
            User recipient = selectedMail.Sender.Item2 == EmailAddress ? FindUser(selectedMail.Receiver.Item2) : FindUser(selectedMail.Sender.Item2);

            // Am I in the CC list? Then the CC list becomes:
            // The initial CC list - me + receiver
            List<(String, String)> ccList = selectedMail.CC;
            if (selectedMail.CC.Contains((Name, EmailAddress)))
            {
                ccList.Remove((Name, EmailAddress));
                ccList.Add(selectedMail.Receiver);
            }

            List<User> ccUserList = new List<User>();
            foreach ((String,String) ccItem in ccList)
            {
                try
                {
                    ccUserList.Add(FindUser(ccItem.Item2));
                }
                catch (InvalidEmailAddressException e)
                {
                    e.PrintCCException();
                }
            }

            String subject = "Re: " + selectedMail.Subject;
            Console.WriteLine(Name + " <" + EmailAddress + "> replying to " + recipient.Name + " <" + recipient.EmailAddress + ">");
            Console.Write(selectedMail.CC.Count > 0 ? "CC: " + selectedMail.CC[0].Item1 + " <" + selectedMail.CC[0].Item2 + ">" : "");
            for (int i = 1; i < selectedMail.CC.Count; i++)
                Console.Write(", " + selectedMail.CC[i].Item1 + " <" + selectedMail.CC[i].Item2 + ">");
            Console.WriteLine("\nSubject: " + subject);
            Console.WriteLine("Enter body:");
            String body = Console.ReadLine() + "\n----------------------------------------------------------------------\n" + selectedMail.ToString();

            (String, String) sender = (Name, EmailAddress);
            (String, String) receiver = (recipient.Name, recipient.EmailAddress);

            Mail composedMail = new Mail(DateTime.Now, sender, receiver, ccList, subject, body, selectedMail.Prio);

            Send(this, recipient, ccUserList, composedMail);
        }

        public void ForwardMail(Mail selectedMail)
        {
            Console.Write(Name + " <" + EmailAddress + "> forwarding mail to: ");
            String emailAddress = Console.ReadLine();
             
            User recipient = FindUser(emailAddress);

            Console.WriteLine("Enter recipient e-mail addresses for the CC list (separate each address with a comma):");
            String emailAddressesStr = Console.ReadLine().Replace(" ", "");
            List<(String, String)> ccList = new List<(String, String)>();
            List<User> ccUserList = new List<User>();

            List<String> emailAddressesList = emailAddressesStr == "" ? new List<String>() : emailAddressesStr.Split(',').ToList();
            foreach (String item in emailAddressesList)
            {
                try
                {
                    User ccRecipient = FindUser(item);
                    ccList.Add((ccRecipient.Name, ccRecipient.EmailAddress));
                    ccUserList.Add(ccRecipient);
                }
                catch (InvalidEmailAddressException e)
                {
                    e.PrintCCException();
                }
            }

            String subject = "Fwd: " + selectedMail.Subject;
            String body = "---------------- Forwarded message ----------------\n" + selectedMail.ToString() + "\n---------------------------------------------------";

            (String, String) sender = (Name, EmailAddress);
            (String, String) receiver = (recipient.Name, recipient.EmailAddress);

            Mail composedMail = new Mail(DateTime.Now, sender, receiver, ccList, subject, body, selectedMail.Prio);

            Send(this, recipient, ccUserList, composedMail);
        }

        public void DeleteMail(Mail selectedMail, int mailSelection, Mailbox selectedMailbox)
        {
            if (selectedMailbox.IOIndicator != InOutEnum.Trash)
                try
                {
                    Trash.AddMail(selectedMail, selectedMailbox.IOIndicator, selectedMailbox.Tags[mailSelection - 1]);
                }
                catch (ExceededCapacityException e)
                {
                    e.PrintException(false);
                }
            selectedMailbox.DeleteMail(mailSelection - 1);
            Console.WriteLine("Mail successfully deleted.");
        }

        public void RestoreMail(Mail selectedMail, int mailSelection)
        {
            InOutEnum prevLoc = Trash.GetLocation(mailSelection - 1);
            Mailbox prevMailbox = prevLoc == InOutEnum.Inbox ? Inbox : Outbox;
            try
            {
                prevMailbox.ReceiveMail(selectedMail, Trash.Tags[mailSelection - 1]);
                Trash.DeleteMail(mailSelection - 1);
                Console.WriteLine("Mail successfully restored.");
            }
            catch (ExceededCapacityException e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.WriteLine("Restore failed!");
                Console.ResetColor();
            }
        }

        public void TagMail(int mailSelection, Mailbox selectedMailbox)
        {
            Console.WriteLine("Choose a tag from your list of tags or create a new one:");
            int i;
            for (i = 0; i < Tags.Count; i++)
                Console.WriteLine((i + 1) + ") " + Tags[i]);
            Console.WriteLine((i + 1) + ") Create a new tag");
            Console.Write("Choose an option: ");
            int selection = Convert.ToInt32(Console.ReadLine());

            String tag;
            if (selection - 1 == Tags.Count)
            {
                Console.Write("Enter the new tag: ");
                tag = Console.ReadLine();
                Tags.Add(tag);
            }
            else
                tag = Tags[selection - 1];

            if (!selectedMailbox.Tags[mailSelection - 1].Contains(tag))
                selectedMailbox.Tags[mailSelection - 1].Add(tag);

            Console.Write("Mail tagged successfully! Press enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }

        private Mailbox SelectMailbox(String mailbox){
            if (mailbox == "I")
                return Inbox;
            else if (mailbox == "O")
                return Outbox;
            else if (mailbox == "T")
                return Trash;
            else
                return null;
        }

        // Makes use of the static list declared in the main class
        private User FindUser(String emailAddress)
        {
            foreach (User user in Program.users)
            {
                if (user.EmailAddress == emailAddress)
                    return user;
            }
            throw new InvalidEmailAddressException(emailAddress);
        }

        // Helper method to avoid code repetition
        // Mail delivery for sender, receiver & CC-list users, all have their own try-catch block.
        // This way, if one mailbox fails to receive the mail, others will still get it.
        private void Send(User fromUser, User toUser, List<User> ccUsers, Mail mailToSend)
        {
            List<User> failedInboxHolders = new List<User>();

            Console.Write("Placing e-mail into outbox... ");
            try
            {
                fromUser.Outbox.ReceiveMail(mailToSend, new List<String>());
            }
            catch (ExceededCapacityException e)
            {
                e.PrintException(false);
            }

            Console.Write(toUser.EmailAddress + " receiving e-mail... ");
            try
            {
                toUser.Inbox.ReceiveMail(mailToSend, new List<String>());
            }
            catch (ExceededCapacityException e)
            {
                e.PrintException(false);
                failedInboxHolders.Add(toUser);
            }

            foreach (User user in ccUsers)
            {
                Console.Write(user.EmailAddress + " (from CC list) receiving e-mail... ");
                try
                {
                    user.Inbox.ReceiveMail(mailToSend, new List<String>());
                }
                catch (ExceededCapacityException e)
                {
                    e.PrintException(true);
                    failedInboxHolders.Add(user);
                }
            }

            if (failedInboxHolders.Count > 0)
                SendAutomatedMail(AutomatedTypeEnum.Fail, fromUser, failedInboxHolders, mailToSend);

            Console.WriteLine("\nPress enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }

        private void SendAutomatedMail(AutomatedTypeEnum type, User senderUser, List<User> users, Mail mail)
        {
            switch (type){
                case AutomatedTypeEnum.Fail:
                    // Capacity exceeded e-mails sent here.
                    Console.Write("Sending automated e-mails...");
                    (String, String) receiver = mail.Sender;
                    String subject = "Failure: Users failed to receive e-mail";
                    String body = "The following addresses failed to receive the email sent on " + mail.Date.ToString() + ":";

                    foreach (User user in users) { 
                        body += "\n" + user.Name + " <" + user.EmailAddress + ">";
                    }

                    body += "\n\n---------------- Original message ----------------\n" + mail.ToString() + "\n---------------------------------------------------";

                    // Form new automated e-mail
                    AutomatedMail autoMail = new AutomatedMail(DateTime.Now, receiver, new List<(string, string)>(), subject, body, type);
                    try
                    {
                        senderUser.Inbox.ReceiveMail(autoMail, new List<String>());
                    }
                    catch (ExceededCapacityException e)
                    {
                        e.PrintException(true);
                    }
                    break;
            }
        }

        public void FilterMailByTag()
        {

        }
    }

    class InvalidEmailAddressException : Exception
    {
        public InvalidEmailAddressException(String invalidAddress) : base("No user found with an e-mail address of " + invalidAddress + "...") { }
        public void PrintException()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(Message);
            Console.ResetColor();
        }

        public void PrintCCException()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(Message + " Skipping.");
            Console.ResetColor();
        }
    }

    class ExceededCapacityException : Exception
    {
        // These two fields are redundant for this solution. They will be useful later!
        readonly InOutEnum MailboxIndicator;
        (String, String) User;

        public ExceededCapacityException((String, String) user, InOutEnum indicator) 
            : base("Capacity exceeded! " + indicator.ToString() + " for " + user.Item1 + " <" + user.Item2 + "> is full!") 
        {
            User = user; MailboxIndicator = indicator;
        }

        public void PrintException(bool cc)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(Message + (cc ? " Skipping." : ""));
            Console.ResetColor();
        }
    }
}