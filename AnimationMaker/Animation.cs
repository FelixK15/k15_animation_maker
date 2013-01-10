using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnimationMaker
{
    [XmlRoot("Animation")]
    [XmlInclude(typeof(Frame))]
    [XmlInclude(typeof(SpriteSource))]
    [XmlInclude(typeof(SingleSpriteSource))]
    [XmlInclude(typeof(SpriteSheetSource))]
    public class Animation
    {
        [XmlIgnore]
        public bool Loop { get; set; }

        [XmlAttribute("Loop",DataType="string")]
        public string LoopSerialize
        {
            get
            {
                return Loop ? "true" : "false";
            }

            set
            {
                Loop = Convert.ToBoolean(value);
            }
        }

        [XmlArray("SpriteSources")]
        [XmlArrayItem("SingleSpriteSource", typeof(SingleSpriteSource))]
        [XmlArrayItem("SpriteSheetSource", typeof(SpriteSheetSource))]
        public List<SpriteSource> SpriteSources { get; set; }

        [XmlArray("Frames")]
        [XmlArrayItem("Frame")]
        public List<Frame> Frames { get; set; }

        
        public Animation()
        {
            Frames = new List<Frame>();
            SpriteSources = new List<SpriteSource>();
        }
    }
}
