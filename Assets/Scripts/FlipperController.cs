using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlipperController : MonoBehaviour
{
	public float speed = 360;
	[Range(0, 360)]
	public float startAngle = 0;
	[Range(0, 360)]
	public float endAngle = 90;
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
	}

	void Update()
	{
		if (IsFlipperAtMaxAngle) {
			rigidBody.rotation = endAngle;
			rigidBody.angularVelocity = 0;
			transform.rotation = Quaternion.Euler(new Vector3(0, 0, endAngle));
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
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, startAngle));
	}

	bool IsFlipperAtMaxAngle {
		get { return rigidBody.rotation > endAngle; }
	}
}
