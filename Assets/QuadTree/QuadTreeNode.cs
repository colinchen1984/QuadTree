using System;
using System.Collections.Generic;

namespace QuadTree
{
	public enum Quadrant
	{
		First = 0,
		Second,
		Third,
		Forth,
		Unknow,
	}

	public struct Point
	{
		public float X;

		public float Y;

		public Point(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}
	}

	public interface IRectangle
	{
		Point Center { get; }

		float Width { get; }

		float High { get; }

		float HalfWidth { get; }

		float HalfHigh { get; }

		bool IsInRectangle(IRectangle rect);

		bool IsInRectangle(Point position);

		bool IsInRectangle(float x, float y);

		bool IsPartInRectangle(IRectangle irect);

		IRectangle GetSubRectangle(Quadrant quadrant);

		Quadrant GetPointQuadrant(Point point);
	}

	public struct Rectangle : IRectangle
	{
		public Rectangle(Point center, float width, float high)
			: this()
		{
			this.center = center;
			this.width = width;
			this.high = high;
			this.halfWidth = Width * 0.5f;
			this.halfHigh = high * 0.5f;
		}

		private Point center;
		public Point Center { get { return center; } }

		private readonly float width;
		public float Width { get { return width; } }

		private readonly float high;
		public float High { get { return high; } }

		private readonly float halfWidth;
		public float HalfWidth { get { return halfWidth; } }

		private readonly float halfHigh;
		public float HalfHigh { get { return halfHigh; } }


		private static int[] factorForPointChech =
		{
			1, 1, //右上角
			-1, 1, //左上角
			-1, -1, //左下角
			1, -1, //右下角,
		};

		public bool IsInRectangle(IRectangle irect)
		{
			Rectangle rect = (Rectangle)irect;
			var ret = true;
			for (int i = 0; i < factorForPointChech.Length; i += 2)
			{
				ret = IsInRectangle(rect.center.X + rect.halfWidth * factorForPointChech[i],
					rect.center.Y + rect.halfHigh * factorForPointChech[i + 1]);
				if (false == ret)
				{
					break;
				}
			}
			return ret;
		}

		public bool IsPartInRectangle(IRectangle irect)
		{
			Rectangle rect = (Rectangle)irect;
			var ret = false;
			for (int i = 0; i < factorForPointChech.Length; i += 2)
			{
				ret = rect.IsInRectangle(center.X + halfWidth * factorForPointChech[i],
						center.Y + halfHigh * factorForPointChech[i + 1]);
				if (ret)
				{
					break;
				}

				ret = IsInRectangle(rect.center.X + rect.halfWidth * factorForPointChech[i],
						rect.center.Y + rect.halfHigh * factorForPointChech[i + 1]);
				if (ret)
				{
					break;
				}
			}
			return ret;
		}

		public bool IsInRectangle(Point position)
		{
			return IsInRectangle(position.X, position.Y);
		}

		public bool IsInRectangle(float x, float y)
		{
			bool ret = Math.Abs(center.X - x) > HalfWidth ||
				Math.Abs(center.Y - y) > HalfHigh;
			return !ret;
		}

		public Quadrant GetPointQuadrant(Point point)
		{
			Quadrant ret = Quadrant.Unknow;
			if (false == IsInRectangle(point))
			{
				return ret;
			}

			if (point.X >= center.X)
			{
				ret = point.Y >= center.Y ? Quadrant.First : Quadrant.Forth;
			}
			else
			{
				ret = point.Y >= center.Y ? Quadrant.Second : Quadrant.Third;
			}
			return ret;
		}

		public IRectangle GetSubRectangle(Quadrant quadrant)
		{
			var subWidth = Width * 0.25f;
			var subHigh = High * 0.25f;
			var xFactor = Quadrant.First == quadrant || Quadrant.Forth == quadrant ? 1 : -1;
			var yFactor = Quadrant.First == quadrant || Quadrant.Second == quadrant ? 1 : -1;
			var newCenterX = Center.X + xFactor * subWidth;
			var newCenterY = Center.Y + yFactor * subHigh;
			return new Rectangle(new Point(newCenterX, newCenterY), HalfWidth, HalfHigh);
		}
	}

	public interface IQuadTreedObject
	{
		Point Position { get; }

		IRectangle Rect { get; }

		object thing { get; }


	}

	public struct QuadTreedObject : IQuadTreedObject
	{
		public QuadTreedObject(object thing, float x, float y, float width, float high)
			: this()
		{
			Rect = new Rectangle(new Point(x, y), width, high);
			this.thing = thing;
		}

		public Point Position { get { return Rect.Center; } }
		public IRectangle Rect { get; private set; }
		public object thing { get; private set; }

		public override int GetHashCode()
		{
			return thing.GetHashCode();
		}
	}

	public class QuadTreeNode
	{
		private static int MaxChildCount = 4;

		private QuadTreeNode[] childrenNode = new QuadTreeNode[MaxChildCount];

		private int currentChildrenCount = 0;

		//用来存储于本区域的物体,尚未引发子区域分割时使用
		private List<IQuadTreedObject> objectList = null;
		
		//不完全属于本区域的物体
		private List<IQuadTreedObject> partialInNodeObject = new List<IQuadTreedObject>(64); 

		public readonly IRectangle Rect;

		private readonly int level = 0;

		private readonly QuadTreeNode parent = null;

		public QuadTreeNode(QuadTreeNode parent, int level, float x, float y, float width, float hight)
			: this(parent, level, new Rectangle(new Point(x, y), width, hight))
		{
		}

		public QuadTreeNode(QuadTreeNode parent, int level, IRectangle rect)
		{
			this.level = level;
			Rect = rect;
			this.parent = parent;
			objectList = new List<IQuadTreedObject>((1 << level) - 1);
		}

		public bool AddObject(object thing, float x, float y, float width, float hight)
		{
			return AddObject(new QuadTreedObject(thing, x, y, width, hight));
		}

		public bool AddObject(IQuadTreedObject thing)
		{
			//如果目标的不在区域内,直接返回
			if (false == Rect.IsInRectangle(thing.Rect))
			{
				return false;
			}

			do
			{
				if (currentChildrenCount > 0)
				{
					if (false == FindSubRectangleAndInsertObject(thing))
					{
						foreach (var kid in childrenNode)
						{
							kid.ProcessPartialInObject(thing);
						}
					}
					break;
				}

				//当前还没有子节点,或者子节点不能添加该物体,尝试直接在本区域添加该物体
				if (objectList.Count < (1 << level) - 1)
				{
					objectList.Add(thing);
					break;
				}

				//本区域内能添加的物体已经达到上限,尝试创建子节点并添加该物体
				MakeSubRetangle();
				objectList.Add(thing);

				//将当前已经存储过的节点,按照其位置,拆分到各个子节点
				foreach (var quadTreedObject in objectList)
				{
					if (false == FindSubRectangleAndInsertObject(quadTreedObject))
					{
						foreach (var kid in childrenNode)
						{
							kid.ProcessPartialInObject(quadTreedObject);
						}
					}
				}
				objectList = null;
			} while (false);
			return true;
		}

		public void GetObjectFromNode(HashSet<IQuadTreedObject> container, IRectangle rect)
		{
			partialInNodeObject.ForEach(o => container.Add(o));
			if (null != objectList)
			{
				objectList.ForEach(o => container.Add(o));
			}
			else
			{
				foreach (var kid in childrenNode)
				{
					if (kid.Rect.IsPartInRectangle(rect))
					{
						kid.GetObjectFromNode(container, rect);
					}
				}
			}
		}

		public QuadTreeNode GetNodeContainRectangle(IRectangle rectangle)
		{
			QuadTreeNode ret = null;
			do
			{
				if (false == Rect.IsInRectangle(rectangle))
				{
					break;
				}
				var quadrant = Rect.GetPointQuadrant(rectangle.Center);
				var childNode = childrenNode[(int) quadrant];
				if (null != childNode)
				{
					ret = childNode.GetNodeContainRectangle(rectangle);
				}

				ret = ret ?? this;
			} while (false);
			return ret;
		}

		private void MakeSubRetangle()
		{
			for (int i = 0; i < (int) Quadrant.Unknow; ++i)
			{
				childrenNode[i] = new QuadTreeNode(this, level + 1, Rect.GetSubRectangle((Quadrant) i));
			}
			currentChildrenCount = 4;
		}

		private bool FindSubRectangleAndInsertObject(IQuadTreedObject thing)
		{
			var quadrant = Rect.GetPointQuadrant(thing.Position);
			return childrenNode[(int)quadrant].AddObject(thing);
		}

		private void ProcessPartialInObject(IQuadTreedObject thing)
		{
			if (false == Rect.IsPartInRectangle(thing.Rect))
			{
				return;
			}

			if (currentChildrenCount > 0)
			{
				foreach (var quadTreeNode in childrenNode)
				{
					if (null != quadTreeNode)
					{
						quadTreeNode.ProcessPartialInObject(thing);
					}
				}
			}
			else
			{
				partialInNodeObject.Add(thing);
			}
		}

		#region Debug Output

		//private static Color[] colors = new[]{Color.Black, Color.DarkBlue, Color.Aqua, Color.Red, Color.DarkCyan,
		//	Color.LightPink, Color.DarkOrange, Color.DarkGoldenrod, Color.DarkMagenta};
		//public void Draw(Graphics graphics)
		//{
		//	Pen myPen = new Pen(colors[level % colors.Length], 1.0F);
		//	graphics.DrawString(level.ToString(), new Font("Arial", 16), myPen.Brush, Rect.Center.X, Rect.Center.Y);
		//	if (null != objectList)
		//	{
		//		foreach (var quadTreedObject in objectList)
		//		{
		//			graphics.DrawRectangle(myPen, quadTreedObject.Rect.Center.X - quadTreedObject.Rect.HalfWidth,
		//				quadTreedObject.Rect.Center.Y - quadTreedObject.Rect.HalfHigh,
		//				quadTreedObject.Rect.Width, quadTreedObject.Rect.High);
		//		}
		//	}
		//	else
		//	{
		//		myPen.Width = 4.0f;
		//		graphics.DrawLine(myPen, new System.Drawing.Point((int)(Rect.Center.X - Rect.Width * 0.5), (int)Rect.Center.Y),
		//			new System.Drawing.Point((int)(Rect.Center.X + Rect.Width * 0.5), (int)Rect.Center.Y));
		//		graphics.DrawLine(myPen, new System.Drawing.Point((int)Rect.Center.X, (int)(Rect.Center.Y - Rect.High * 0.5)),
		//			new System.Drawing.Point((int)Rect.Center.X, (int)(Rect.Center.Y + Rect.High * 0.5)));

		//		foreach (var quadTreeNode in childrenNode)
		//		{
		//			quadTreeNode.Draw(graphics);
		//		}	
		//	}

		//	foreach (var quadTreedObject in partialInNodeObject)
		//	{
		//		graphics.DrawRectangle(myPen, quadTreedObject.Rect.Center.X - quadTreedObject.Rect.HalfWidth,
		//			quadTreedObject.Rect.Center.Y - quadTreedObject.Rect.HalfHigh,
		//			quadTreedObject.Rect.Width, quadTreedObject.Rect.High);
		//	}
		//}
		#endregion
	}
}