using System.Collections.Generic;
using System;
using Myriadbits.MXF.EssenceParser;

namespace Myriadbits.MXF.EssenceParsers
{
    public class MXFEssenceParserFactory
    {
        private static readonly IReadOnlyDictionary<ByteArray, Type> dict = new Dictionary<ByteArray, Type>(new KeyPartialMatchComparer())
        { 
            {new PartialUL(0x06, 0x0e, 0x2b, 0x34, 0x01, 0x02, 0x01, 0x01, 0x0d, 0x01, 0x03, 0x01), typeof(MXFProResEssenceInfo) }
        };

        public static MXFEssenceInfoBase ParseEssence(MXFEssenceElement el)
        {
            if (dict.TryGetValue(el.Key, out Type foundType))
            {
                return (MXFEssenceInfoBase)Activator.CreateInstance(foundType, el);
            }
            return null;
        }
    };
}
