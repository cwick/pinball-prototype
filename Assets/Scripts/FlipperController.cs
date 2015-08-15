using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlipperController : MonoBehaviour
{
    #region Editor Fields

    [Range(0, 800)]
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
        SetInitialPosition();
        rigidBody.angularVelocity = AngularVelocity;
    }

    #endregion


    #region Public Properties

    public float AngularVelocity
    {
        get { return speed * MirrorFlip; }
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
            rigidBody.rotation = ActualEndAngle;
            rigidBody.angularVelocity = 0;
        }
    }

    void Update()
    {
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
    }

    #endregion

    #region Private Methods

    void SetInitialPosition()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, ActualStartAngle));
    }

    bool ShouldFlipperStop {
        get {
            return Mathf.Abs(rigidBody.rotation + (Time.deltaTime * AngularVelocity)) > Mathf.Abs(ActualEndAngle);
        }
    }

    #endregion
}
