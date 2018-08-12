using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tools
{
    public static class FileManagement
    {
        static XmlSerializer blockSerializer = new XmlSerializer(typeof(MapBlock));

        public static void Serialize(string path, MapBlock block)
        {
            using (FileStream fstream = new FileStream(path, FileMode.Create))
                blockSerializer.Serialize(fstream, block);
        }

        public static MapBlock Deserialize(string path)
        {
            using (FileStream fstream = File.Open(path, FileMode.Open))
                return (MapBlock)blockSerializer.Deserialize(fstream);
        }

        public static MapBlock Deserialize(TextReader reader)
        {
            return (MapBlock)blockSerializer.Deserialize(reader);
        }
    }
}
