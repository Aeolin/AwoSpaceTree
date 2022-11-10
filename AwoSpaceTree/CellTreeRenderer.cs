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
  public class CellTreeRenderer<T> : AbstractCellRenderer<T>
  {
    private Pen _borderPen = new Pen(Color.Black, 3);
    const int LEAF_DIAMETER = 32;
    const int LEAF_RADIUS = LEAF_DIAMETER/2;
    const int INTER_LEAF_WIDTH = LEAF_RADIUS;
    const int INTER_KNOT_HEIGHT = 75;
    const int MARGIN = 10;

    public CellTreeRenderer() 
    { 
    }

    private void drawLeaf(Graphics g, Cell<T> cell, float x, float y)
    {
      if (cell.IsParent && cell.IsGrandParent == false)
      {
        g.DrawLine(_borderPen, x, y, x, y+INTER_KNOT_HEIGHT);
        drawLeaf(g, cell[2], x-LEAF_RADIUS, y+INTER_KNOT_HEIGHT+LEAF_RADIUS-LEAF_RADIUS);
        drawLeaf(g, cell[3], x+LEAF_RADIUS, y+INTER_KNOT_HEIGHT+LEAF_RADIUS-LEAF_RADIUS);
        drawLeaf(g, cell[0], x-LEAF_RADIUS, y+INTER_KNOT_HEIGHT+LEAF_RADIUS+LEAF_RADIUS);
        drawLeaf(g, cell[1], x+LEAF_RADIUS, y+INTER_KNOT_HEIGHT+LEAF_RADIUS+LEAF_RADIUS);
      }

      g.FillEllipse(new SolidBrush(ColorMapper(cell)), x-LEAF_RADIUS, y-LEAF_RADIUS, LEAF_DIAMETER, LEAF_DIAMETER);
      g.DrawEllipse(_borderPen, x-LEAF_RADIUS, y-LEAF_RADIUS, LEAF_DIAMETER, LEAF_DIAMETER);
    }


    protected override void RenderImpl(Graphics g, Cell<T> root, int margin, int width, int height)
    {
      var midX = width / 2;
      void render(Graphics g, Cell<T> cell, float x, float y, float xDistance)
      {
        if (cell.IsGrandParent)
        {
          g.DrawLine(_borderPen, x, y, x, y+INTER_KNOT_HEIGHT);
          g.DrawLine(_borderPen, x-xDistance, y+INTER_KNOT_HEIGHT, x+xDistance, y+INTER_KNOT_HEIGHT);
          var stepLen = (2*xDistance)/(cell.Children.Count-1);
          for (int i = 0; i < cell.Children.Count; i++)
            render(g, cell[i], (x-xDistance)+i*stepLen, y+INTER_KNOT_HEIGHT, xDistance/4);
        }

        drawLeaf(g, cell, x, y);

      }

      render(g, root, midX, MARGIN+LEAF_RADIUS, (width-MARGIN)/3);
    }

    protected override int getWidth(Cell<T> cell)
    {
      var all = cell.Decendants().ToArray();
      var maxDepth = all.Max(x => x.Depth);
      var maxLeafes = Math.Pow(4, maxDepth-1);
      return (int)((MARGIN + (maxLeafes*LEAF_DIAMETER*2)+(maxLeafes*INTER_LEAF_WIDTH))*1);
    }

    protected override int getHeight(Cell<T> cell)
    {
      var all = cell.Decendants().ToArray();
      var maxDepth = all.Max(x => x.Depth);
      return (int)(MARGIN + INTER_KNOT_HEIGHT*(maxDepth+1));
    }
  }
}
