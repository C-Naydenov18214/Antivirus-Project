using DeveloperKit.PeReader;
using System;
using System.Collections.Generic;
using static DeveloperKit.PeReader.Structures;

namespace DeveloperKit.Context
{
    public class PeFileContext
    {

        private readonly ImageDosHeader dosHeader;
        private readonly ImageFileHeader fileHeader;
        private readonly ImageOptionalHeader32 optionalHeader32;
        private readonly ImageOptionalHeader64 optionalHeader64;
        private readonly List<ImageSectionHeader> imageSectionHeaders;
        private string name;

        public PeFileContext(ImageDosHeader dosHeader, ImageFileHeader fileHeader, ImageOptionalHeader32 optionalHeader32, ImageOptionalHeader64 optionalHeader64, string name)
        {
            this.dosHeader = dosHeader;
            this.fileHeader = fileHeader;
            this.optionalHeader32 = optionalHeader32;
            this.optionalHeader64 = optionalHeader64;
            this.name = name;
            imageSectionHeaders = new List<ImageSectionHeader>();
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }

        }

        public void AddSection(IMAGE_SECTION_HEADER section, byte[] bytes)
        {

            imageSectionHeaders.Add(new ImageSectionHeader(section, bytes));
        }
        public ImageOptionalHeader64 OptionalHeader64
        {
            get
            {
                return optionalHeader64;
            }
        }

        public ImageOptionalHeader32 OptionalHeader32
        {
            get
            {
                return optionalHeader32;
            }
        }

        public ImageDosHeader DosHeader
        {
            get
            {
                return dosHeader;
            }
        }

        public ImageSectionHeader[] ImageSectionHeaders
        {
            get
            {
                return imageSectionHeaders.ToArray();
            }
        }

        public ImageFileHeader FileHeader
        {
            get
            {
                return fileHeader;
            }
        }

        public ImageSectionHeader GetSection(string name)
        {
            foreach (ImageSectionHeader section in imageSectionHeaders)
            {

                string secName = new string(section.SectionHeader.Name);
                if (secName.CompareTo(name) == 0)
                {
                    return section;
                }
            }
            throw new Exception("No such section");
        }
    }
}
