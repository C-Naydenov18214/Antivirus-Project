using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PeParser.Structures;

namespace PeParser
{
    public class PeHeaderReader
    {


        public PeHeaderReader()
        {

        }

        public PeFile ReadPeFile(string filePath)
        {
            PeFile peFile;
            using (FileStream stream = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(stream);

                byte[] fileBytes = reader.ReadBytes(Convert.ToInt32(stream.Length));

                stream.Seek(0, SeekOrigin.Begin);

                ImageDosHeader dosHeader = new ImageDosHeader(FromBinaryReader<IMAGE_DOS_HEADER>(reader));

                stream.Seek(dosHeader.DosHeader.e_lfanew, SeekOrigin.Begin);

                UInt32 ntHeadersSignature = reader.ReadUInt32();
                ImageFileHeader fileHeader = new ImageFileHeader(FromBinaryReader<IMAGE_FILE_HEADER>(reader));
                if (this.Is32BitHeader(fileHeader.FileHeader))
                {
                    ImageOptionalHeader32 OptionalHeader32 = new ImageOptionalHeader32(FromBinaryReader<IMAGE_OPTIONAL_HEADER32>(reader));
                    peFile = new PeFile(dosHeader, fileHeader, OptionalHeader32, null, filePath);
                }
                else
                {
                    ImageOptionalHeader64 OptionalHeader64 = new ImageOptionalHeader64(FromBinaryReader<IMAGE_OPTIONAL_HEADER64>(reader));
                    peFile = new PeFile(dosHeader, fileHeader, null, OptionalHeader64, filePath);
                }
                int numberOfSections = fileHeader.FileHeader.NumberOfSections;

                for (int headerNo = 0; headerNo < numberOfSections; headerNo++)
                {
                    IMAGE_SECTION_HEADER header = FromBinaryReader<IMAGE_SECTION_HEADER>(reader);
                    byte[] bytes = GetSectionBytes(fileBytes, header);
                    peFile.AddSection(header, bytes);
                }
            }
            return peFile;
        }


        private T FromBinaryReader<T>(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return theStructure;
        }

        private bool Is32BitHeader(IMAGE_FILE_HEADER header)
        {
            UInt16 IMAGE_FILE_32BIT_MACHINE = 0x0100;
            return (IMAGE_FILE_32BIT_MACHINE & header.Characteristics) == IMAGE_FILE_32BIT_MACHINE;
        }



        private byte[] GetSectionBytes(byte[] fileBytes, IMAGE_SECTION_HEADER header)
        {
            byte[] bytes;
            bytes = new byte[header.SizeOfRawData];
            Array.Copy(fileBytes, header.PointerToRawData, bytes, 0, header.SizeOfRawData);

            return bytes;
        }


    }

}
