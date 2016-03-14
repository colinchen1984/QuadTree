using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class ObjectScriptForDebug : MonoBehaviour
{

	public float Width { get; private set; }

	public float High { get; private set; }

	private float halfWidth = 0.0f;
	private float halfHigh = 0.0f;

	// Use this for initialization
	void Awake ()
	{

		Width = Random.Range(10f, 30);
		halfWidth = Width*0.5f;
		High = Random.Range(10f, 30);
		halfHigh = High * 0.5f;
		transform.position = new Vector3(Random.Range(-512, 512), 0.0f, Random.Range(-512, 512));
	}
	
	// Update is called once per frame
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(new Vector3(transform.position.x + halfWidth, transform.position.y, transform.position.z + halfHigh),
			new Vector3(transform.position.x - halfWidth, transform.position.y, transform.position.z + halfHigh));
		Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, transform.position.y, transform.position.z + halfHigh),
			new Vector3(transform.position.x - halfWidth, transform.position.y, transform.position.z - halfHigh));

		Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, transform.position.y, transform.position.z - halfHigh),
			new Vector3(transform.position.x + halfWidth, transform.position.y, transform.position.z - halfHigh));
		Gizmos.DrawLine(new Vector3(transform.position.x + halfWidth, transform.position.y, transform.position.z - halfHigh),
	new Vector3(transform.position.x + halfWidth, transform.position.y, transform.position.z + halfHigh));
	}
}
