using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwoSpaceTree
{
  public class CellSerializer<T>
  {
    public Func<T, int> _serializeValue;
    public Func<int, T> _deserializeValue;
    public int BitsPerValue { get; private set; }

    public CellSerializer(int bitsPerValue, Func<T, int> serializeValue, Func<int, T> deserializeValue)
    {
      _serializeValue = serializeValue;
      _deserializeValue = deserializeValue;
      BitsPerValue = bitsPerValue;
    }

    public byte[] Serialize(Cell<T> cell)
    {
      using (var stream = new MemoryStream())
      {
        var bitStream = new BitStream(stream);
        SerializeCell(cell, bitStream);
        bitStream.Flush();
        return stream.ToArray();
      }
    }


    private void SerializeCell(Cell<T> cell, BitStream bitStream)
    {
      Queue<Cell<T>> queue = new Queue<Cell<T>>();
      queue.Enqueue(cell);
      while (queue.Count > 0)
      {
        var current = queue.Dequeue();
        if (current.IsParent == false)
        {
          var value = _serializeValue(current.Value);
          Console.Write(Convert.ToString(value, 2).PadLeft(BitsPerValue, '0')+" ");
          bitStream.WriteBits(value, BitsPerValue);
        }
        else
        {
          bitStream.WriteBit(true);
          Console.Write("1 ");
          current.Children.ForEach(x => queue.Enqueue(x));
        }
      }
    }


    //private Cell<T> DeserializeCell(BitStream bitStream)
    //{
    //  var isParent = bitStream.ReadBit();
    //  if (isParent)
    //  {
    //    var children = new List<Cell<T>>();
    //    for (int i = 0; i < 4; i++)
    //    {
    //      children.Add(DeserializeCell(bitStream));
    //    }
    //    return new Cell<T>(children);
    //  }
    //  else
    //  {
    //    return new Cell<T>(bitStream.ReadBit());
    //  }
    //}
  }
}
