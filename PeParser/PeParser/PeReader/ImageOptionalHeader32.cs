using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeveloperKit.PeReader.Structures;

namespace DeveloperKit.PeReader
{
    public class ImageOptionalHeader32
    {

        private readonly IMAGE_OPTIONAL_HEADER32 optionalHeader32;

        public ImageOptionalHeader32(IMAGE_OPTIONAL_HEADER32 optionalHeader32)
        {
            this.optionalHeader32 = optionalHeader32;
        }

        public IMAGE_OPTIONAL_HEADER32 OptionalHeader32
        {
            get
            {
                return optionalHeader32;
            }
        }

   
    }
}
