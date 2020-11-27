using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeveloperKit.PeReader.Structures;

namespace DeveloperKit.PeReader
{
    public class ImageOptionalHeader64
    {
        private readonly IMAGE_OPTIONAL_HEADER64 optionalHeader64;

        public ImageOptionalHeader64(IMAGE_OPTIONAL_HEADER64 optionalHeader64)
        {
            this.optionalHeader64 = optionalHeader64;
        }

        public IMAGE_OPTIONAL_HEADER64 OptionalHeader64
        {
            get
            {
                return optionalHeader64;
            }
        }


    }
}
