using Myriadbits.MXF.KLV;
using System.ComponentModel;
using System.IO;

namespace Myriadbits.MXF.EssenceParser
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MXFEssenceInfoBase
    {
        protected BinaryReader reader;

        [Browsable(false)]
        public MXFEssenceElement EssenceElement { get; protected set; }

        public MXFEssenceInfoBase(MXFEssenceElement el)
        {
            SubStream ss = el.GetValueStream();
            reader = new KLVStreamReader(ss);
            EssenceElement = el;
        }
    }
}