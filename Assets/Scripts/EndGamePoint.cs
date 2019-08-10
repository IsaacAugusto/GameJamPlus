using System;
using UnityEngine;

public delegate void GameEvent();

public class EndGamePoint : MonoBehaviour {
  public static event GameEvent OnGameEnded;

  [SerializeField] private Collider2D _endGameTrigger;
  [SerializeField] private SpriteRenderer _endGameSprite;

  private void Start() {
    PatrolBehaviour.OnEnemyDied += OnEnemyDied;
    EnableEndGameTrigger(false);
  }

  private void OnEnemyDied(PatrolBehaviour enemy) {
    if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 1)
      EnableEndGameTrigger(true);
  }

  private void EnableEndGameTrigger(bool value) {
    _endGameTrigger.enabled = value;
    _endGameSprite.enabled = value;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.transform.tag == "Player") {
      EnableEndGameTrigger(false);
      OnGameEnded?.Invoke();
    }
  }

  private void OnDestroy() {
    PatrolBehaviour.OnEnemyDied -= OnEnemyDied;
  }
}
