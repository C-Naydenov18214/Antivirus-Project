using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeParser.Structures;

namespace PeParser
{
    public class ImageSectionHeader
    {
        private readonly IMAGE_SECTION_HEADER imageSectionHeader;
        private byte[] sectionBytes;


        public ImageSectionHeader(IMAGE_SECTION_HEADER imageSectionHeader, byte[] sectionBytes)
        {
            this.imageSectionHeader = imageSectionHeader;
            this.sectionBytes = sectionBytes;
        }

        public IMAGE_SECTION_HEADER SectionHeader
        {
            get
            {
                return imageSectionHeader;
            }
        }
        public byte[] SectionBytes
        {

            get
            {
                return sectionBytes;
            }
        }


    }
}
