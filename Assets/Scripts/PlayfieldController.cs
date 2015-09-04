using UnityEngine;
using System.Collections;

public class PlayfieldController : MonoBehaviour {
    [Range(0, MAX_INCLINATION)]
    [Tooltip("Playfield inclination, in degrees")]
    public float inclination = 6.5f;
    private const float GRAVITY_ACCELERATION = 376.22f;
    private const float MAX_INCLINATION = 90;

    void Start() {
        Physics2D.gravity = Vector2.down * (GRAVITY_ACCELERATION / MAX_INCLINATION) * inclination;
    }
}
