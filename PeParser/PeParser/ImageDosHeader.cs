using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeParser.Structures;

namespace PeParser
{
    public class ImageDosHeader
    {
        private readonly IMAGE_DOS_HEADER dosHeader;


        public ImageDosHeader(IMAGE_DOS_HEADER dosHeader)
        {
            this.dosHeader = dosHeader;
        }

        public IMAGE_DOS_HEADER DosHeader
        {
            get 
            {
                return dosHeader;
            }
        
        }

    }
}
