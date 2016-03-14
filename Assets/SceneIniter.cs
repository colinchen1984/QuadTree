using System.ComponentModel.Design;
using UnityEngine;

public class SceneIniter : MonoBehaviour
{
	private QuadTree.QuadTree tree = null;

	public GameObject TemplateGameObject = null;
	private void Start()
	{
		gameObject.transform.position = new Vector3(0,0,0);
		tree = new QuadTree.QuadTree(1024, 1024);
		for (int i = 0; i < 20; ++i)
		{
			var t = GameObject.Instantiate(TemplateGameObject).GetComponent<ObjectScriptForDebug>();
			tree.AddObject(t, t.transform.position.x, t.transform.position.z, t.Width, t.High);
		}
	}

	private void OnDrawGizmos()
	{
		if (null == tree)
		{
			return;
		}

		tree.OnDrawGizmos();
	}
}
