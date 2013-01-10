using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace AnimationMaker
{
    [XmlRoot("Frame")]
    [XmlInclude(typeof(Sprite))]
    public class Frame
    {
        [XmlAttribute("Duration",DataType="int")]
        public int Duration { get; set; }

        [XmlArray("Sprites")]
        [XmlArrayItem("Sprite")]
        public List<Sprite> Sprites { get; private set; }
       
        public Frame()
        {
            Sprites = new List<Sprite>();
            Duration = 0;
        }
    }
}
