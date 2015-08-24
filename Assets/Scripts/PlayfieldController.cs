using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectangleBuilder))]
public class PlayfieldController : MonoBehaviour {
    [Range(0, MAX_INCLINATION)]
    [Tooltip("Playfield inclination, in degrees")]
    public float inclination = 6.5f;
    [Tooltip("Width of the playfield, in inches")]
    public float width = 20.25f;
    [Tooltip("Height of the playfield, in inches")]
    public float height = 42f;
    private const float GRAVITY_ACCELERATION = 376.22f;
    private const float MAX_INCLINATION = 90;

    void Start() {
        Physics2D.gravity = Vector2.down * (GRAVITY_ACCELERATION / MAX_INCLINATION) * inclination;
    }

    void Reset() {
        OnValidate();
    }

    void OnValidate() {
        var builder = GetComponent<RectangleBuilder>();
        builder.width = width + builder.thickness * 2;
        builder.height = height + builder.thickness * 2;
        builder.OnValidate();
    }
}
