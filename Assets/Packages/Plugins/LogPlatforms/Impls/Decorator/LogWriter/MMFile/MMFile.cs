using System;
using System.IO;
using System.IO.MemoryMappedFiles;
namespace Cr7Sund.Logger
{
    internal struct MMFileHeader
    {
        public int position;
        public int length;

        public static readonly int PositionIndex = 0;
        public static readonly int LengthIndex = PositionIndex + sizeof(int);
        public static readonly int FileIndex = LengthIndex + sizeof(int);

        public static readonly int ContextIndex = FileIndex;
    }

    internal class MMFile : IDisposable
    {
        private readonly MemoryMappedViewAccessor _accessor;
        private readonly MemoryMappedFile _mmf;

        public MMFile(string path, int capacity = 2048)
        {
            try
            {
                if (!File.Exists(path))
                {
                    using var fileStream = File.Create(path);
                    fileStream.SetLength(capacity);
                    _mmf = MemoryMappedFile.CreateFromFile(fileStream, null, capacity, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);
                }
                else
                {
                    using var fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                    fileStream.SetLength(capacity);
                    _mmf = MemoryMappedFile.CreateFromFile(fileStream, null, capacity, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);
                }
                _accessor = _mmf.CreateViewAccessor(0, capacity, MemoryMappedFileAccess.ReadWrite);
                var header = ReadHeader();
                header.length = capacity;
                WriteHeader(header);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
        }

        private MMFileHeader ReadHeader()
        {
            if (_accessor == null)
                throw new Exception("The accessor has not been initialized!!!");

            var header = new MMFileHeader
            {
                position = _accessor.ReadInt32(0),
                length = _accessor.ReadInt32(4)
            };
            return header;
        }

        private void WriteHeader(MMFileHeader header)
        {
            if (_accessor == null)
                throw new Exception("The accessor has not been initialized!!!");

            _accessor.Write(MMFileHeader.PositionIndex, header.position);
            _accessor.Write(MMFileHeader.LengthIndex, header.length);
        }

        public bool IsWritable()
        {
            var header = ReadHeader();
            return header.position <= 0;
        }

        public byte[] ReadAll()
        {
            if (_accessor == null)
                throw new Exception("The accessor has not been initialized!!!");

            var header = ReadHeader();
            byte[] result = new byte[header.position];
            _accessor.ReadArray(MMFileHeader.ContextIndex, result, 0, header.position);
            return result;
        }

        public void Write(byte[] sources, int offset, int length)
        {
            if (_accessor == null)
                throw new Exception("The accessor has not been initialized");

            if (!_accessor.CanWrite)
                throw new Exception("The accessor is write-protected");

            var header = ReadHeader();
            int absolutePosition = header.position + MMFileHeader.ContextIndex;

            if (absolutePosition + length > header.length)
            {
                int overflow = absolutePosition + length - header.length;
                throw new MMFileOverflowException(overflow, "Write overflow!");
            }

            _accessor.WriteArray(absolutePosition, sources, offset, length);
            header.position += length;
            WriteHeader(header);
        }

        public void Reset()
        {
            var header = ReadHeader();
            header.position = 0;
            WriteHeader(header);
        }
    }
}
