using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AnimationMaker
{
    [XmlRoot("Sprite")]
    public class Sprite
    {        
        [XmlIgnore]
        public Image Image { get; set; }
        
        [XmlAttribute("X",DataType="double")]
        public double X { get; set; }

        [XmlAttribute("Y",DataType="double")]
        public double Y { get; set; }

        [XmlAttribute("ID",DataType="int")]
        public int ID { get; set; }

        public Sprite() : this("",null,0.0,0.0)
        {
        
        }

        public Sprite(Image Image) : this("",Image,0.0,0.0)
        {

        }

        public Sprite(String Path,Image Image, double X, double Y)
        {
            this.Image = Image;
            this.X = X;
            this.Y = Y;
        }
    }
}
