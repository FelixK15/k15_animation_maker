using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnimationMaker
{
    [XmlRoot("SpriteSource")]
    public abstract class SpriteSource
    {
        [XmlAttribute("Path",DataType="string")]
        public String Path { get; set; }

        [XmlAttribute("ID",DataType="int")]
        public int ID { get; set; }

        [XmlIgnore]
        public static int GID { get; set; }

        protected SpriteSource()
        {
            ID = GID++;
        }
    }
}
