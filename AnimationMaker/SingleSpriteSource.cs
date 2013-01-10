using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationMaker
{
    public class SingleSpriteSource : SpriteSource
    {
        public SingleSpriteSource() : base()
        {

        }

        public SingleSpriteSource(String path) : base()
        {
            Path = path;
        }
    }
}
