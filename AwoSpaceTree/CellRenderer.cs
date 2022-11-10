using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwoSpaceTree
{

  public class CellRenderer<T> : AbstractCellRenderer<T>
  {
    public int Width { get; set; }
    public int Height { get; set; }


    public CellRenderer(int width, int height)
    {
      this.Width = width;
      this.Height = height;
    }


    protected override void RenderImpl(Graphics g, Cell<T> cell, int margin, int width, int height)
    {
      var pen = new Pen(Color.Black, 3);
      var cellSpaceWidth = cell.Size.X;
      var cellSpaceHeight = cell.Size.Y;
      var cellOffsetX = cell.Position.X*-1;
      var cellOffsetY = cell.Position.Y*-1;

      float mapX(double value) => (float)((cellOffsetX+value)/cellSpaceWidth*(width - margin))+margin/2F;
      float mapY(double value) => (float)((cellSpaceHeight-(cellOffsetY+value))/cellSpaceHeight*(height-margin))+margin/2F;
      float mapW(double value) => (float)(value/cellSpaceWidth*(width-margin));
      float mapH(double value) => (float)(value/cellSpaceHeight*(height-margin));

      void render(Graphics g, Cell<T> cell)
      {
        var x = mapX(cell.Position.X);
        var y = mapY(cell.End.Y);
        var w = mapW(cell.Size.X);
        var h = mapH(cell.Size.Y);
        //Console.WriteLine($"{cell}: {x}, {y}, {w}, {h}");

        if (ColorMapper != null)
          g.FillRectangle(new SolidBrush(ColorMapper(cell)), x, y, w, h);

        g.DrawRectangle(pen, x, y, w, h);
        cell.Children.ForEach(x => render(g, x));
      }

      render(g, cell);
    }

    protected override int getWidth(Cell<T> _) => Width;
    protected override int getHeight(Cell<T> _) => Height;
  }
}
