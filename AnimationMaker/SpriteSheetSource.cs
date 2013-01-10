using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnimationMaker
{
    [XmlRoot("SpriteSheetSource")]
    public class SpriteSheetSource : SpriteSource
    {
        [XmlAttribute("SingleSpriteWidth",DataType="int")]
        public int SpriteWidth { get; set; }

        [XmlAttribute("SingleSpriteHeight",DataType="int")]
        public int SpriteHeight { get; set; }

        [XmlAttribute("SpriteAmount",DataType="int")]
        public int SpriteAmount { get; set; }

        public SpriteSheetSource()
            : base()
        {

        }

        public SpriteSheetSource(String Path, int SpriteWidth, int SpriteHeight, int SpriteAmount) : base()
        {
            this.Path = Path;
            this.SpriteWidth = SpriteWidth;
            this.SpriteHeight = SpriteHeight;
            this.SpriteAmount = SpriteAmount;
        }
    }
}
