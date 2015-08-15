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
        get { return speed * Multiplier; }
    }

    #endregion

    #region Private Properties and Fields

    private int Multiplier {
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
            rigidBody.rotation = endAngle;
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

    void OnDrawGizmosSelected()
    {
        var meshFilter = GetComponent<MeshFilter>();
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position, Quaternion.Euler(Vector3.forward * endAngle), transform.localScale);
    }

    #endregion

    #region Private Methods

    void SetInitialPosition()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, startAngle));
    }

    bool ShouldFlipperStop {
        get {
            return rigidBody.rotation + (Time.deltaTime * speed) > endAngle; 
        }
    }

    #endregion
}
