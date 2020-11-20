using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeParser.Structures;

namespace PeParser
{
    public class ImageFileHeader
    {
        private readonly IMAGE_FILE_HEADER fileHeader;


        public ImageFileHeader(IMAGE_FILE_HEADER fileHeader)
        {
            this.fileHeader = fileHeader;
        }
        public IMAGE_FILE_HEADER FileHeader
        {
            get
            {
                return fileHeader;
            }
        
        }
    }
}
