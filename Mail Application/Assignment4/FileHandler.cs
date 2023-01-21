using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Assignment4
{
    class XMLHandler
    {
        public void WriteUser(string filename, User user)
        {
            XmlSerializer x = new XmlSerializer(user.GetType());
            Stream fs = new FileStream(filename, FileMode.Create);
            XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
            x.Serialize(writer, user);
            writer.Close();
        }

        public User ReadUser(string filename)
        {
            Stream reader = new FileStream(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(User));

            User user = (User)serializer.Deserialize(reader);
            return user;
        }
        
    }
        
}
