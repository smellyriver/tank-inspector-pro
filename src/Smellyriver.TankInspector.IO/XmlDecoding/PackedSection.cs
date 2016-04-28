using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Smellyriver.TankInspector.IO.XmlDecoding
{
    public class PackedSection
    {
        public static readonly Int32 PackedHeader = 0x62a14e45;
        public static readonly char[] intToBase64 = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };
        public const int MAX_LENGTH = 256;

        public class DataDescriptor
        {
            public readonly int address;
            public readonly int end;
            public readonly int type;

            public DataDescriptor(int end, int type, int address)
            {
                this.end = end;
                this.type = type;
                this.address = address;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("[");
                sb.Append("0x");
                sb.Append(Convert.ToString(end, 16));
                sb.Append(", ");
                sb.Append("0x");
                sb.Append(Convert.ToString(type, 16));
                sb.Append("]@0x");
                sb.Append(Convert.ToString(address, 16));
                return sb.ToString();
            }
        }

        public class ElementDescriptor
        {
            public readonly int nameIndex;
            public readonly DataDescriptor dataDescriptor;

            public ElementDescriptor(int nameIndex, DataDescriptor dataDescriptor)
            {
                this.nameIndex = nameIndex;
                this.dataDescriptor = dataDescriptor;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("[");
                sb.Append("0x");
                sb.Append(Convert.ToString(nameIndex, 16));
                sb.Append(":");
                sb.Append(dataDescriptor);
                return sb.ToString();
            }
        }

        public string readStringTillZero(BinaryReader reader)
        {
            char[] work = new char[MAX_LENGTH];

            int i = 0;

            char c = reader.ReadChar();
            while (c != Convert.ToChar(0x00))
            {
                work[i++] = c;
                c = reader.ReadChar();
            }
            return new string(work, 0, i);

        }

        public List<string> readDictionary(BinaryReader reader)
        {
            List<string> dictionary = new List<string>();
            int counter = 0;
            string text = readStringTillZero(reader);

            while (!(text.Length == 0))
            {
                dictionary.Add(text);
                text = readStringTillZero(reader);
                counter++;
            }
            return dictionary;
        }

        public int readLittleEndianShort(BinaryReader reader)
        {
            int LittleEndianShort = reader.ReadInt16();
            return LittleEndianShort;
        }

        public int readLittleEndianInt(BinaryReader reader)
        {
            int LittleEndianInt = reader.ReadInt32();
            return LittleEndianInt;
        }

        public DataDescriptor readDataDescriptor(BinaryReader reader)
        {
            int selfEndAndType = readLittleEndianInt(reader);
            return new DataDescriptor(selfEndAndType & 0x0fffffff, selfEndAndType >> 28, (int)reader.BaseStream.Position);
        }

        public ElementDescriptor[] readElementDescriptors(BinaryReader reader, int number)
        {
            ElementDescriptor[] elements = new ElementDescriptor[number];
            for (int i = 0; i < number; i++)
            {
                int nameIndex = readLittleEndianShort(reader);
                DataDescriptor dataDescriptor = readDataDescriptor(reader);
                elements[i] = new ElementDescriptor(nameIndex, dataDescriptor);
            }
            return elements;
        }

        public string readString(BinaryReader reader, int lengthInBytes)
        {
            string rString = Encoding.UTF8.GetString(reader.ReadBytes(lengthInBytes));

            return rString;
        }

        public string readNumber(BinaryReader reader, int lengthInBytes)
        {
            string Number = "";
            switch (lengthInBytes)
            {
                case 1:
                    Number = Convert.ToString(reader.ReadSByte(), CultureInfo.InvariantCulture);
                    break;
                case 2:
                    Number = Convert.ToString(readLittleEndianShort(reader), CultureInfo.InvariantCulture);
                    break;
                case 4:
                    Number = Convert.ToString(readLittleEndianInt(reader), CultureInfo.InvariantCulture);
                    break;
                default:
                    Number = "0";
                    break;
            }
            return Number;

        }

        public float readLittleEndianFloat(BinaryReader reader)
        {
            float LittleEndianFloat = reader.ReadSingle();
            return LittleEndianFloat;
        }

        public string readFloats(BinaryReader reader, int lengthInBytes)
        {
            int n = lengthInBytes / 4;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {

                if (i != 0)
                {
                    sb.Append(" ");
                }
                float rFloat = readLittleEndianFloat(reader);
                sb.Append(rFloat.ToString("0.000000", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }


        public bool readBoolean(BinaryReader reader, int lengthInBytes)
        {
            bool @bool = lengthInBytes == 1;
            if (@bool)
            {
                if (reader.ReadSByte() != 1)
                {
                    throw new ArgumentException("Boolean error");
                }
            }

            return @bool;
        }

        private static string byteArrayToBase64(sbyte[] a)
        {
            int aLen = a.Length;
            int numFullGroups = aLen / 3;
            int numBytesInPartialGroup = aLen - 3 * numFullGroups;
            int resultLen = 4 * ((aLen + 2) / 3);
            StringBuilder result = new StringBuilder(resultLen);

            int inCursor = 0;
            for (int i = 0; i < numFullGroups; i++)
            {
                int byte0 = a[inCursor++] & 0xff;
                int byte1 = a[inCursor++] & 0xff;
                int byte2 = a[inCursor++] & 0xff;
                result.Append(intToBase64[byte0 >> 2]);
                result.Append(intToBase64[(byte0 << 4) & 0x3f | (byte1 >> 4)]);
                result.Append(intToBase64[(byte1 << 2) & 0x3f | (byte2 >> 6)]);
                result.Append(intToBase64[byte2 & 0x3f]);
            }

            if (numBytesInPartialGroup != 0)
            {
                int byte0 = a[inCursor++] & 0xff;
                result.Append(intToBase64[byte0 >> 2]);
                if (numBytesInPartialGroup == 1)
                {
                    result.Append(intToBase64[(byte0 << 4) & 0x3f]);
                    result.Append("==");
                }
                else
                {
                    int byte1 = a[inCursor++] & 0xff;
                    result.Append(intToBase64[(byte0 << 4) & 0x3f | (byte1 >> 4)]);
                    result.Append(intToBase64[(byte1 << 2) & 0x3f]);
                    result.Append('=');
                }
            }

            return result.ToString();
        }

        public string readBase64(BinaryReader reader, int lengthInBytes)
        {
            sbyte[] bytes = new sbyte[lengthInBytes];
            for (int i = 0; i < lengthInBytes; i++)
            {
                bytes[i] = reader.ReadSByte();
            }
            return PackedSection.byteArrayToBase64(bytes);
        }

        public string readAndToHex(BinaryReader reader, int lengthInBytes)
        {
            sbyte[] bytes = new sbyte[lengthInBytes];
            for (int i = 0; i < lengthInBytes; i++)
            {
                bytes[i] = reader.ReadSByte();
            }
            StringBuilder sb = new StringBuilder("[ ");
            foreach (byte b in bytes)
            {
                sb.Append(Convert.ToString((b & 0xff), 16));
                sb.Append(" ");
            }
            sb.Append("]L:");
            sb.Append(lengthInBytes);

            return sb.ToString();
        }

        public int readData(BinaryReader reader, List<string> dictionary, XmlNode element, XmlDocument xDoc, int offset, DataDescriptor dataDescriptor)
        {
            int lengthInBytes = dataDescriptor.end - offset;
            if (dataDescriptor.type == 0x0)
            {
                // Element                
                readElement(reader, element, xDoc, dictionary);
            }
            else if (dataDescriptor.type == 0x1)
            {
                // String
                element.InnerText = readString(reader, lengthInBytes);

            }
            else if (dataDescriptor.type == 0x2)
            {
                // Integer number
                element.InnerText = "\t" + readNumber(reader, lengthInBytes) + "\t";
            }
            else if (dataDescriptor.type == 0x3)
            {
                // Floats
                string str = readFloats(reader, lengthInBytes);

                string[] strData = str.Split(' ');
                if (strData.Length == 12)
                {
                    XmlNode row0 = xDoc.CreateElement("row0");
                    XmlNode row1 = xDoc.CreateElement("row1");
                    XmlNode row2 = xDoc.CreateElement("row2");
                    XmlNode row3 = xDoc.CreateElement("row3");
                    row0.InnerText = "\t" + strData[0] + " " + strData[1] + " " + strData[2] + "\t";
                    row1.InnerText = "\t" + strData[3] + " " + strData[4] + " " + strData[5] + "\t";
                    row2.InnerText = "\t" + strData[6] + " " + strData[7] + " " + strData[8] + "\t";
                    row3.InnerText = "\t" + strData[9] + " " + strData[10] + " " + strData[11] + "\t";
                    element.AppendChild(row0);
                    element.AppendChild(row1);
                    element.AppendChild(row2);
                    element.AppendChild(row3);
                }
                else
                {
                    element.InnerText = "\t" + str + "\t";
                }
            }
            else if (dataDescriptor.type == 0x4)
            {
                // Boolean

                if (readBoolean(reader, lengthInBytes))
                {
                    element.InnerText = "\ttrue\t";
                }
                else
                {
                    element.InnerText = "\tfalse\t";
                }

            }
            else if (dataDescriptor.type == 0x5)
            {
                // Base64
                element.InnerText = "\t" + readBase64(reader, lengthInBytes) + "\t";
            }
            else
            {
                throw new ArgumentException("Unknown type of \"" + element.Name + ": " + dataDescriptor.ToString() + " " + readAndToHex(reader, lengthInBytes));
            }

            return dataDescriptor.end;
        }

        public void readElement(BinaryReader reader, XmlNode element, XmlDocument xDoc, List<string> dictionary)
        {
            int childrenNmber = readLittleEndianShort(reader);
            DataDescriptor selfDataDescriptor = readDataDescriptor(reader);
            ElementDescriptor[] children = readElementDescriptors(reader, childrenNmber);

            int offset = readData(reader, dictionary, element, xDoc, 0, selfDataDescriptor);

            foreach (ElementDescriptor elementDescriptor in children)
            {
                XmlNode child = xDoc.CreateElement(dictionary[elementDescriptor.nameIndex]);
                offset = readData(reader, dictionary, child, xDoc, offset, elementDescriptor.dataDescriptor);
                element.AppendChild(child);
            }

        }
    }
}
