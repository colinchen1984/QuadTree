using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
			root = new QuadTreeNode(null, 0, 0, 0, sceneWidth, sceneHigh);
		}

		public bool AddObject(object thing, float x, float y, float width, float high)
		{
			//float xmin = (float)Math.Max(0, x - width * 0.5);
			//float xmax = (float)Math.Min(SceneWidth, x + width * 0.5);
			//float ymin = (float)Math.Max(0, y - high * 0.5);
			//float ymax = (float)Math.Min(SceneHigh, y + high * 0.5);
			//x = (xmin + xmax) * .5f;
			//y = (ymin + ymax) * .5f;
			//width = xmax - xmin;
			//high = ymax - ymin;
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

		public void OnDrawGizmos()
		{
			if (null == root)
			{
				return;
			}
			// Add drawing commands here
			root.OnDrawGizmos();
			
			Action<IRectangle> action = (IRectangle rect) =>
			{
				Gizmos.DrawLine(new Vector3(rect.Center.X + rect.HalfWidth, rect.Center.Y + rect.HalfHigh), 
					new Vector3(rect.Center.X - rect.HalfWidth, rect.Center.Y + rect.HalfHigh));
				Gizmos.DrawLine(new Vector3(rect.Center.X - rect.HalfWidth, rect.Center.Y + rect.HalfHigh),
						new Vector3(rect.Center.X - rect.HalfWidth, rect.Center.Y - rect.HalfHigh));
				Gizmos.DrawLine(new Vector3(rect.Center.X - rect.HalfWidth, rect.Center.Y - rect.HalfHigh),
					new Vector3(rect.Center.X + rect.HalfWidth, rect.Center.Y - rect.HalfHigh));
				Gizmos.DrawLine(new Vector3(rect.Center.X - rect.HalfWidth, rect.Center.Y + rect.HalfHigh),
					new Vector3(rect.Center.X + rect.HalfWidth, rect.Center.Y + rect.HalfHigh));
			};
			action(selectRectangle);
			//foreach (var quadTreedObject in selectedObjects)
			//{
			//	myPen.Color = quadTreedObject.Rect.IsPartInRectangle(selectRectangle)
			//		? Color.Black
			//		: Color.PaleGreen;
			//	action(quadTreedObject.Rect);
			//}
			//bitmap.Save(@"./test.png", ImageFormat.Png);
		}
	}
}