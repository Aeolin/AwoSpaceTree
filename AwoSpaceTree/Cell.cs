using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwoSpaceTree
{
  public class Cell<T>
  {
    public List<Cell<T>> Children { get; init; } = new List<Cell<T>>();

    public bool IsParent => Children.Count > 0;
    public bool IsGrandParent => Children.Count > 0 && Children.Any(x => x.IsParent);

    public Cell<T> Parent { get; init; }

    public int Depth { get; set; }

    public Cell<T> this[int index] => Children[index];

    public IEnumerable<Cell<T>> Decendants()
    {
      yield return this;
      foreach(var cell in Children)
        foreach(var descendant in cell.Decendants())
          yield return descendant;
    }

    public void Subdivide()
    {
      Children.Clear();
      var halfSize = Size / 2;
      Children.Add(new Cell<T>(this.Position, halfSize, Depth + 1, this));
      Children.Add(new Cell<T>(this.Position.AddX(halfSize), halfSize, Depth + 1, this));
      Children.Add(new Cell<T>(this.Position.AddY(halfSize), halfSize, Depth + 1, this));
      Children.Add(new Cell<T>(this.Position + halfSize, halfSize, Depth + 1, this));
    }

    public override string ToString() => $"{Position} - {Size}";

    public Cell (Vec2D pos, Vec2D size, int depth = 0, Cell<T> parent = null)
    {
      this.Position = pos;
      this.Size = size;
      this.Depth = depth;
      this.Parent=parent;
    }

    public Cell(double x, double y, double width, double height, int depth = 0, Cell<T> parent = null)
    {
      this.Position = new Vec2D(x, y);
      this.Size = new Vec2D(width, height);
      this.Depth = depth;
      this.Parent = parent;
    }

    public IEnumerable<Vec2D> Corners()
    {
      yield return this.Position;
      yield return this.Position.AddX(this.Size);
      yield return this.Position.AddY(this.Size);
      yield return this.End;
    }

    public T Value { get; set; }
    public Vec2D Position { get; set; }
    public Vec2D Size { get; set; }
    public Vec2D End => Position+Size;
  }
}
