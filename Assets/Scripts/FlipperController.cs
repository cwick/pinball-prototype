using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlipperController : MonoBehaviour
{
    #region Editor Fields

    [Range(0, 800)]
    [Tooltip("Angular speed at which the flipper moves")]
    public float speed = 360;

    [Range(0, 360)]
    public float startAngle = 0;

    [Range(0, 360)]
    public float endAngle = 90;

    public bool mirror = false;

    #endregion

    #region Public Methods

    public void Flip()
    {
        rigidBody.angularVelocity = DesiredAngularVelocity;
    }

    public void Fall()
    {
        rigidBody.angularVelocity = -DesiredAngularVelocity;
    }

    #endregion


    #region Public Properties

    public float DesiredAngularVelocity
    {
        get { return speed * MirrorFlip; }
    }

    public float ActualAngularVelocity {
        get { return rigidBody.angularVelocity * MirrorFlip; }
    }

    public float ActualStartAngle {
        get { return startAngle * MirrorFlip; }
    }

    public float ActualEndAngle {
        get { return endAngle * MirrorFlip; }
    }

    #endregion

    #region Private Properties and Fields

    private int MirrorFlip {
        get { return mirror ? -1 : 1; }
    }

    private Rigidbody2D rigidBody;

    #endregion

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
        if (ShouldFlipperStop) {
            if (ActualAngularVelocity > 0) {
                rigidBody.rotation = ActualEndAngle;
            } else {
                rigidBody.rotation = ActualStartAngle;
            }
            rigidBody.angularVelocity = 0;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Flippers")) {
            Flip();
        } else if (Input.GetButtonUp("Flippers")) {
            Fall();
        }
    }

    void OnValidate()
    {
        if (startAngle > endAngle) {
            startAngle = endAngle;
        }
        SetInitialPosition();
    }

    void OnDrawGizmos()
    {
        var meshFilter = GetComponent<MeshFilter>();
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position, Quaternion.Euler(Vector3.forward * ActualEndAngle), transform.localScale);
        Gizmos.color = Color.green;
        Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position, Quaternion.Euler(Vector3.forward * ActualStartAngle), transform.localScale);
    }

    #endregion

    #region Private Methods

    void SetInitialPosition()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, ActualStartAngle));
    }

    bool ShouldFlipperStop {
        get {
            var expectedAngle = Mathf.Abs(rigidBody.rotation + (Time.deltaTime * rigidBody.angularVelocity));

            return expectedAngle > endAngle || expectedAngle < startAngle;
        }
    }

    #endregion
}
