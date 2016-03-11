using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace QuadTree
{
	public class QuadTree
	{
		public static int MaxNodeCount = 10;

		public readonly int SceneWidth = 0;

		public readonly int SceneHigh = 0;

		private readonly QuadTreeNode root = null;

		private Rectangle selectRectangle;

		//private HashSet<IQuadTreedObject> selectedObjects = new HashSet<IQuadTreedObject>();
 
		public QuadTree(int sceneWidth, int sceneHigh)
		{
			this.SceneWidth = sceneWidth;
			this.SceneHigh = sceneHigh;
			root = new QuadTreeNode(null, 0, sceneWidth >> 1, sceneHigh >> 1, sceneWidth, sceneHigh);
		}

		public bool AddObject(object thing, float x, float y, float width, float high)
		{
			return root.AddObject(thing, x, y,width, high);
		}

		public void LoadObjectsFromQuadTree(float x, float y, float width, float high, HashSet<IQuadTreedObject> objectsTable)
		{
			float xmin = (float)Math.Max(0, x - width * 0.5);
			float xmax = (float)Math.Min(SceneWidth, x + width * 0.5);
			float ymin = (float)Math.Max(0, y - high * 0.5);
			float ymax = (float)Math.Min(SceneHigh, y + high * 0.5);
			x = (xmin + xmax) * .5f;
			y = (ymin + ymax) * .5f;
			width = xmax - xmin;
			high = ymax - ymin;
			selectRectangle = new Rectangle(new Point(x, y), width, high);
			var node = root.GetNodeContainRectangle(selectRectangle);
			
			if (null != node)
			{
				node.GetObjectFromNode(objectsTable, selectRectangle);
			}
		}

		//public void SaveToImage()
		//{
		//	Bitmap bitmap = new Bitmap(SceneWidth, SceneHigh, PixelFormat.Format32bppArgb);
		//	Graphics g = Graphics.FromImage(bitmap);

		//	// Add drawing commands here
		//	g.Clear(Color.White);
		//	root.Draw(g);
		//	Pen myPen = new Pen(Color.Black, 1.0F);
		//	foreach (var selectedObject in selectedObjects)
		//	{
		//		g.DrawString(selectedObject.thing.ToString(), new Font("Arial", 16), 
		//			myPen.Brush, selectedObject.Rect.Center.X, selectedObject.Rect.Center.Y);
		//	}
		//	myPen.Width = 5;
		//	Action<IRectangle> action = (IRectangle rect) =>
		//	{
		//		var points = new System.Drawing.Point[]
		//		{
		//			new System.Drawing.Point((int) (rect.Center.X + rect.HalfWidth), (int) (rect.Center.Y + rect.HalfHigh)),
		//			new System.Drawing.Point((int) (rect.Center.X - rect.HalfWidth), (int) (rect.Center.Y + rect.HalfHigh)),
		//			new System.Drawing.Point((int) (rect.Center.X - rect.HalfWidth), (int) (rect.Center.Y - rect.HalfHigh)),
		//			new System.Drawing.Point((int) (rect.Center.X + rect.HalfWidth), (int) (rect.Center.Y - rect.HalfHigh)),
		//			new System.Drawing.Point((int) (rect.Center.X + rect.HalfWidth), (int) (rect.Center.Y + rect.HalfHigh)),
		//		};
		//		g.DrawLines(myPen, points);
		//	};
		//	action(selectRectangle);
		//	myPen.Color = Color.PaleGreen;
		//	foreach (var quadTreedObject in selectedObjects)
		//	{
		//		myPen.Color = quadTreedObject.Rect.IsPartInRectangle(selectRectangle)
		//			? Color.Black
		//			: Color.PaleGreen;
		//		action(quadTreedObject.Rect);
		//	}
		//	bitmap.Save(@"./test.png", ImageFormat.Png);
		//}
	}
}