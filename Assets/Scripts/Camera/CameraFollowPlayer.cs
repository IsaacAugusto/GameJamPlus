using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {
  private Transform _player;
  private Vector3 _speed = Vector3.zero;
  private float _smooth = 0.2f;
  void Start() {
    _player = FindObjectOfType<Player>().transform;
  }

  void Update() {
    FollowPlayer();
  }

  private void FollowPlayer() {
    if (_player) {
      Vector3 pos = Vector3.SmoothDamp(transform.position, _player.position, ref _speed, _smooth);
      pos.z = -10;
      transform.position = pos;
    }
  }
}
