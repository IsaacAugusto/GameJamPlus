using System;
using UnityEngine;

public class EndGamePanel : MonoBehaviour {
  void Start() {
    transform.gameObject.SetActive(false);
    EndGamePoint.OnGameEnded += OnGameEnded;
  }

  private void OnGameEnded() {
    transform.gameObject.SetActive(true);
  }

  private void OnDestroy() {
    EndGamePoint.OnGameEnded -= OnGameEnded;
  }
}
