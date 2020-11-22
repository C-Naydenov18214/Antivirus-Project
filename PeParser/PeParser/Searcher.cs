using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeParser.Structures;

namespace PeParser
{
    public class Searcher
    {

        public ImageSectionHeader[] FindHeaderWithChar(PeFile pe, DataSectionFlags characteristic)
        {
            List<ImageSectionHeader> results = new List<ImageSectionHeader>();            
            foreach (ImageSectionHeader header in pe.ImageSectionHeaders) 
            {
                if ((header.SectionHeader.Characteristics.HasFlag(characteristic)) == true) 
                {
                    results.Add(header);
                }

            
            }
            if (results.Count > 0)
            {
                return results.ToArray();
            }
            return null;
        }

    }
}
