using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlipperController : MonoBehaviour
{
	public float speed = 360;
	[Range(0, 360)]
	public float
		initialAngle = 0;
	public float maxAngle = 90;
	private Rigidbody2D rigidBody;

	public void Flip()
	{
		SetInitialPosition();
		rigidBody.angularVelocity = speed;
	}

	#region Messages

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		SetInitialPosition();
	}

	void FixedUpdate()
	{
		if (rigidBody.rotation > maxAngle) {
			rigidBody.rotation = maxAngle;
			rigidBody.angularVelocity = 0;
		}
	}

	void OnValidate()
	{
		if (!Application.isPlaying) {
			SetInitialPosition();
		}
	}

	#endregion

	void SetInitialPosition()
	{
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, initialAngle));
	}
}
