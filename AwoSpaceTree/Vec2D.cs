using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AwoSpaceTree
{
  public struct Vec2D
  {
    public Vec2D(double x, double y)
    {
      this.X = x;
      this.Y = y;
    }

    public Vec2D() : this(0, 0)
    {

    }

    public override string ToString() => $"[{X}/{Y}]";

    public double X { get; set; }
    public double Y { get; set; }

    public Vec2D DivideX(double value) => new Vec2D(this.X / value, this.Y);
    public Vec2D DivideY(double value) => new Vec2D(this.X, this.Y / value);
    public Vec2D AddX(double value) => new Vec2D(this.X + value, this.Y);
    public Vec2D AddX(Vec2D vec) => new Vec2D(this.X + vec.X, this.Y);
    public Vec2D AddY(double value) => new Vec2D(this.X, this.Y + value);
    public Vec2D AddY(Vec2D vec) => new Vec2D(this.X, this.Y + vec.Y);
    public double Min() => Math.Min(X, Y);
    public double Max() => Math.Max(X, Y);

    public static Vec2D operator +(Vec2D pos1, Vec2D pos2) => new Vec2D(pos1.X + pos2.X, pos1.Y + pos2.Y);

    public static Vec2D operator +(Vec2D pos, double value) => new Vec2D(pos.X + value, pos.Y + value);

    public static Vec2D operator -(Vec2D pos1, Vec2D pos2) => new Vec2D(pos1.X - pos2.X, pos1.Y - pos2.Y);

    public static Vec2D operator -(Vec2D pos, double value) => new Vec2D(pos.X - value, pos.Y - value);

    public static Vec2D operator /(Vec2D pos, double value) => new Vec2D(pos.X / value, pos.Y / value);

    public static Vec2D operator *(Vec2D pos, double value) => new Vec2D(pos.X * value, pos.Y * value);

    public static implicit operator PointF(Vec2D vec) => new PointF((float)vec.X, (float)vec.Y);

    public double Magnitude()
    {
      return Math.Sqrt(X * X + Y * Y);
    }

    public double Distance(Vec2D other) => (this-other).Magnitude();

  }
}
