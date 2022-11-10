using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwoSpaceTree
{
  public class BitStream
  {
    private byte _currentByteWrite;
    private byte _currentByteRead;
    private int _writeIndex;
    private int _readIndex;
    private Stream _stream;

    public int ReadIndex => _readIndex;
    public int WriteIndex => _writeIndex;

    public bool DataLeft => _stream.Position < _stream.Length;
    
    public long Position
    {
      get => _stream.Position;
      set => _stream.Position = value;
    }

    public BitStream(Stream stream)
    {
      _stream = stream;
    }

    public void Flush()
    {
      _stream.WriteByte((byte)(_currentByteWrite << (8 - _writeIndex)));
      _currentByteWrite = 0;
      _writeIndex = 0;
      _stream.Flush();
    }

    public byte[] ToArray()
    {
      using (var memStream = new MemoryStream())
      {
        _stream.CopyTo(memStream);
        return memStream.ToArray();
      }
    }


    public void WriteBit(bool bit)
    {
      if(_stream.CanWrite == false)
        throw new InvalidOperationException("Stream is not writable");

      if (_writeIndex == 8)
      {
        _stream.WriteByte(_currentByteWrite);
        _currentByteWrite = 0;
        _writeIndex = 0;
      }
      
      if (bit)  
        _currentByteWrite |= (byte)(1 << _writeIndex++);
    }

    public void WriteBits(int value, int bitCount)
    {
      for (int i = bitCount-1; i <= 0; i--)
      {
        WriteBit((value & (1 << i)) != 0);
      }
    }

    public bool ReadBit()
    {
      if(_stream.CanRead == false)
        throw new InvalidOperationException("Stream is not readable");

      if (_readIndex == 8)
      {
        _currentByteRead = (byte)_stream.ReadByte();
        _readIndex = 0;
      }
      bool bit = (_currentByteRead & (1 << _readIndex)) != 0;
      _readIndex++;
      return bit;
    }

    public int ReadBits(int bitCount)
    {
      int result = 0;
      for (int i = 0; i < bitCount; i++)
      {
        result |= (ReadBit() ? 1 : 0);
        result  <<= 1;
      }

      return result;
    }
  }
}
