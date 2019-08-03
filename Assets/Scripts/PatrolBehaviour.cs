using System;
using UnityEngine;

public class PatrolBehaviour : MonoBehaviour {
  [SerializeField] private SpriteRenderer _sprite;
  [SerializeField] private Rigidbody2D _rigidbody;
  [SerializeField] private float _velocity = 5;
  [SerializeField] private float _checkGroundDistance = 5;
  [SerializeField] [Range(0, 360)] private float _checkGroundAngle;

  private Vector3 _checkGroundDir;

  void Update() {
    _checkGroundDir = Quaternion.AngleAxis(_checkGroundAngle, Vector3.back) * Vector3.right;
    if (_sprite.flipX)
      _checkGroundDir.x *= -1;

    if (!CheckGround())
      FlipDirection();

    Vector2 dir;
    if (_sprite.flipX)
      dir = Vector2.left;
    else dir = Vector2.right;
    _rigidbody.velocity = dir * _velocity;
  }

  private void FlipDirection() {
    _sprite.flipX = !_sprite.flipX;
  }

  private bool CheckGround() {
    return Physics2D.Raycast(transform.position, _checkGroundDir, _checkGroundDistance, LayerMask.GetMask("Ground")).collider;
  }

  private void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.position, transform.position + _checkGroundDir.normalized * _checkGroundDistance);
  }
}
