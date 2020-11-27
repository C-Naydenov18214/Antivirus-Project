using DeveloperKit.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeveloperKit.PeReader.Structures;

namespace DeveloperKit.PeReader
{
    public class Searcher
    {

        public ImageSectionHeader[] FindHeaderWithChar(PeFileContext pe, params DataSectionFlags[] characteristic)
        {
            List<ImageSectionHeader> results = new List<ImageSectionHeader>();
            
            foreach (ImageSectionHeader header in pe.ImageSectionHeaders) 
            {
                bool all = false;
                foreach (DataSectionFlags flag in characteristic)
                {
                    if ((header.SectionHeader.Characteristics.HasFlag(flag)) == true)
                    {
                        all = true;
                    }
                    else 
                    {
                        all = false;
                        break;
                    }
                }
                if (all)
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
