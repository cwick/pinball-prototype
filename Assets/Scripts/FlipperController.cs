using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlipperController : MonoBehaviour
{
	public float speed = 360;

	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		var body = GetComponent<Rigidbody2D>();
		body.angularVelocity = speed;
	}
}
