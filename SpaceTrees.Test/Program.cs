using AwoSpaceTree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.SpaceTree
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var root = new Cell<CellType>(-1, -1, 2, 2);
      var renderer = new CellRenderer<CellType>(2048, 2048);
      renderer.PreRenderers.Add(DrawGrid);
      renderer.PostRenderers.Add(DrawCircleOverlay);
      renderer.ColorMapper = c => c.Value == CellType.Border && c.IsParent == false ? Color.CornflowerBlue : c.Value == CellType.Outer ? Color.Gray : Color.White; 


      var origin = new Vec2D(0, 0);

      void performSubdivisionsForCircle(Cell<CellType> cell, double radius = 1, int maxDepth = 3)
      {
        var distances = cell.Corners().Select(x => x.Distance(origin)).ToArray();
        if (cell.Depth == 0 || distances.Any(x => x <= radius) && distances.Any(x => x >= radius))
        {
          cell.Value = CellType.Border;
          if (cell.Depth < maxDepth)
          {
            cell.Subdivide();
            cell.Children.ForEach(x => performSubdivisionsForCircle(x, radius, maxDepth));
          }
        }
        else
        {
          cell.Value = distances.All(x => x >= radius) ? CellType.Outer : CellType.Inner;
        }
      }

      var rad = 1F;
      performSubdivisionsForCircle(root, rad, 3);
      var pen = new Pen(Color.Black, 3);
      using (var stream = renderer.Render(root, ImageFormat.Png, 200))
      {
        File.WriteAllBytes("grid.png", stream.ToArray());
      }

      var tree = new CellTreeRenderer<CellType>();
      tree.ColorMapper = (c) => c.Value == CellType.Border ? Color.CornflowerBlue : c.Value == CellType.Outer ? Color.Gray : Color.White;
      using(var stream = tree.Render(root))
      {
        File.WriteAllBytes("tree.png", stream.ToArray());
      }

      var serializer = new CellSerializer<CellType>(2, c => c == CellType.Border ? 1 : 0, i => (CellType)i);
      var serialized = serializer.Serialize(root);
      
    }

    static void DrawCircleOverlay<T>(int width, int height, int margin, Graphics g, Cell<T> cell)
    {
      g.DrawEllipse(new Pen(Color.Red, 10), margin/2+10, margin/2+10, width-margin-20, height-margin-20);
    }


    static void DrawGrid<T>(int width, int height, int margin, Graphics g, Cell<T> cell)
    {
      int COUNT_SUBDIVISIONS = 5;
      var font = new Font("Arial", 28);
      var fontBrush = new SolidBrush(Color.Black);

      if (margin == 0)
        return;

      var stepSizeX = (float)cell.Size.X / (COUNT_SUBDIVISIONS-1);
      var stepSizeY = (float)cell.Size.Y / (COUNT_SUBDIVISIONS-1);

      var stepWidth = ((width-margin)/(COUNT_SUBDIVISIONS-1))+0.1F;
      var stepHeight = ((height-margin)/(COUNT_SUBDIVISIONS-1))+0.1F;

      var gridLinePen = new Pen(Color.Gray, 1);
      var gridBorderPen = new Pen(Color.Black, 3);
      var gridBorderOffset = margin / 2;
      g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
      g.DrawRectangle(gridBorderPen, gridBorderOffset, gridBorderOffset, width-margin, height-margin);

      for (int x = 0; x < COUNT_SUBDIVISIONS; x++)
      {
        var xOff = gridBorderOffset+x*stepWidth;
        g.DrawLine(gridLinePen, xOff, margin, xOff, height-margin);
        g.DrawLine(gridBorderPen, xOff, height-gridBorderOffset, xOff, height-gridBorderOffset+20);
        var text = (cell.Position.X+x*stepSizeX).ToString("0.0");
        var measure = g.MeasureString(text, font);
        g.DrawString(text, font, fontBrush, xOff-measure.Width/2, height-gridBorderOffset+20);
      }

      for (int y = 0; y < COUNT_SUBDIVISIONS; y++)
      {
        var yOff = gridBorderOffset+y*stepHeight;
        g.DrawLine(gridLinePen, margin, yOff, width-margin, yOff);
        g.DrawLine(gridBorderPen, gridBorderOffset-20, yOff, gridBorderOffset, yOff);
        var text = (cell.End.Y-y*stepSizeY).ToString("0.0");
        var measure = g.MeasureString(text, font);
        g.DrawString(text, font, fontBrush, 0, yOff-(measure.Height / 2));
      }

    }

  }
}
