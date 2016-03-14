using System;
using System.Linq;
using System.Text;
using System.IO;

namespace D2oReader
{
    public class D2OReader : IDisposable
    {
        public int Pointer
        {
            get { return (int) reader.BaseStream.Position; }
            set { reader.BaseStream.Position = value; }
        }

        public UInt32 BytesAvailable
        {
            get { return (UInt32) (reader.BaseStream.Length - reader.BaseStream.Position); }
        }

        public UInt32 ReadUInt()
        {
            Byte[] uint32 = reader.ReadBytes(4);

            uint32 = uint32.Reverse().ToArray();

            return BitConverter.ToUInt32(uint32, 0);
        }

        internal byte[] ReadBytes(int v)
        {
            return reader.ReadBytes(v);
        }

        public Int32 ReadInt()
        {
            Byte[] int32 = reader.ReadBytes(4);

            int32 = int32.Reverse().ToArray();

            return BitConverter.ToInt32(int32, 0);
        }
        

        public short ReadShort()
        {
            Byte[] shortVar = reader.ReadBytes(2);

            shortVar = shortVar.Reverse().ToArray();

            return BitConverter.ToInt16(shortVar,0);
        }
        public ushort ReadUShort()
        {
            Byte[] ushortVar = reader.ReadBytes(2);

            ushortVar = ushortVar.Reverse().ToArray();

            return BitConverter.ToUInt16(ushortVar, 0);
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        BinaryReader reader;

        public int Length
        {
            get { return (int)reader.BaseStream.Length; }
        }

        public string ReadAscii(Int32 bytesAmount)
        {
            Byte[] buffer = reader.ReadBytes(bytesAmount);

            return Encoding.ASCII.GetString(buffer);
        }
        public string ReadUtf8()
        {
            Byte[] buffer;

            ushort len = ReadUShort();

            buffer = reader.ReadBytes(len);

            return Encoding.UTF8.GetString(buffer);
        }

        public D2OReader(Stream input)
        {
            reader = new BinaryReader(input);
        }

        public bool ReadBool()
        {
            //TODO: correcT?
            //Byte buffer = reader.ReadByte();
            //return buffer == 1 ? true : false;
            return reader.ReadBoolean();
        }

        
    }
}
