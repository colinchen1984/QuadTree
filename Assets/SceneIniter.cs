using UnityEngine;

public class SceneIniter : MonoBehaviour
{
	private QuadTree.QuadTree tree = null;
	private void Start()
	{
		tree = new QuadTree.QuadTree(1024, 1024);
		
	}
}
