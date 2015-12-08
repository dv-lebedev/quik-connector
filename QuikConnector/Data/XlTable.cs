/*
The MIT License (MIT)

Copyright (c) 2015 Denis Lebedev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using System;
using System.IO;
using System.Text;

namespace QuikConnector.Data
{
    public sealed class XlTable : IDisposable
    {
        public enum BlockType
        {
            Table = 0x10,

            Float = 0x01,
            String = 0x02,
            Bool = 0x03,
            Error = 0x04,
            Blank = 0x05,
            Int = 0x06,
            Skip = 0x07,

            Unknown = 0x10000,
            Bad = 0x10001
        }

        const int codepage = 1251;

        const int wsize = 2;
        const int fsize = 8;
        const int hsize = wsize * 2;

        byte[] data;

        MemoryStream ms;
        BinaryReader br;

        int blocksize;

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public BlockType ValueType { get; private set; }

        public double FloatValue { get; private set; }
        public string StringValue { get; private set; }
        public ushort WValue { get; private set; }

        public XlTable(byte[] data)
        {
            this.data = data;

            ms = new MemoryStream(data);
            br = new BinaryReader(ms, Encoding.ASCII);

            if (data.Length < wsize * 4 || (BlockType)br.ReadUInt16() != BlockType.Table)
                SetBadDataStatus();

            ms.Seek(wsize, SeekOrigin.Current);

            Rows = br.ReadUInt16();
            Columns = br.ReadUInt16();

            ValueType = BlockType.Unknown;
        }

        void SetBadDataStatus()
        {
            ValueType = BlockType.Bad;
            blocksize = 1;
        }

        public void ReadValue()
        {

            if (ValueType == BlockType.Unknown)
            {
                if (ms.Position + hsize > data.Length)
                    SetBadDataStatus();
                else
                {
                    ValueType = (BlockType)br.ReadUInt16();
                    blocksize = br.ReadUInt16();

                    if (ms.Position + blocksize > data.Length)
                        SetBadDataStatus();
                }
            }

            if (blocksize > 0)
                switch (ValueType)
                {
                    case BlockType.Float:
                        blocksize -= fsize;

                        if (blocksize >= 0)
                            FloatValue = br.ReadDouble();
                        else
                            SetBadDataStatus();

                        break;

                    case BlockType.String:
                        int strlen = ms.ReadByte();
                        blocksize -= strlen + 1;

                        if (blocksize >= 0)
                        {
                            StringValue = Encoding.GetEncoding(codepage).GetString(data, (int)ms.Position, strlen);
                            br.BaseStream.Seek(strlen, SeekOrigin.Current);
                        }
                        else
                            SetBadDataStatus();

                        break;

                    case BlockType.Bool:
                    case BlockType.Error:
                    case BlockType.Blank:
                    case BlockType.Int:
                    case BlockType.Skip:
                        blocksize -= wsize;

                        if (blocksize >= 0)
                            WValue = br.ReadUInt16();
                        else
                            SetBadDataStatus();
                        break;
                    default:
                        SetBadDataStatus();
                        break;
                }
            else
            {
                ValueType = BlockType.Unknown;
                ReadValue();
            }
        }

        public void Dispose()
        {
            if (br != null) br.Dispose();

            if (ms != null) ms.Dispose();
        }
    }
}
