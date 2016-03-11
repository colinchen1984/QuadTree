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
	void Start ()
	{

		Width = Random.Range(10f, 50f);
		halfWidth = Width*0.5f;
		High = Random.Range(10f, 50f);
		halfHigh = High * 0.5f;
		transform.position = new Vector3(Random.Range(0f, 1024f), 0.0f, Random.Range(0f, 1024f));
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
