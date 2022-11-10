using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwoSpaceTree
{
  public abstract class AbstractCellRenderer<T>
  {
    public List<Action<int, int, int, Graphics, Cell<T>>> PreRenderers { get; init; } = new List<Action<int, int, int, Graphics, Cell<T>>>();
    public List<Action<int, int, int, Graphics, Cell<T>>> PostRenderers { get; init; } = new List<Action<int, int, int, Graphics, Cell<T>>>();
    public Func<Cell<T>, Color> ColorMapper { get; set; }

    protected abstract void RenderImpl(Graphics g, Cell<T> cell, int margin, int width, int height);
    protected abstract int getWidth(Cell<T> cell);
    protected abstract int getHeight(Cell<T> cell);

    public virtual MemoryStream Render(Cell<T> cell, ImageFormat format = null, int margin=50)
    {
      var width = getWidth(cell);
      var height = getHeight(cell);
      using (var bmp = new Bitmap(width, height))
      using (var g = Graphics.FromImage(bmp))
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        PreRenderers.ForEach(x => x?.Invoke(width, height, margin, g, cell));
        RenderImpl(g, cell, margin, width, height);
        PostRenderers.ForEach(x => x?.Invoke(width, height, margin, g, cell));
        var mem = new MemoryStream();
        bmp.Save(mem, format ?? ImageFormat.Png);
        return mem;
      }
    }
  }
}
