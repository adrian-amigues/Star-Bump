using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DebuggerOverlayPresenter : MonoBehaviour {
  [SerializeField] private UIDocument uiDocument;
  [SerializeField] private bool defaultIsPlayerInvulnerable = false;

  private ScrollView debugOverlayContainer;
  private Dictionary<Key, Action> debugActions = new Dictionary<Key, Action>();

  private readonly Key[] numberKeys = {
    Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5,
    Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9, Key.Digit0
  };

  private void Start() {
    var root = uiDocument.rootVisualElement;
    debugOverlayContainer = root.Q<ScrollView>();

    AddDebugActions();
    AddDebugShortcuts();
    InitialDebugState();
  }

  void Update() {
    if (Keyboard.current.backquoteKey.wasPressedThisFrame) {
      ToggleDebugOverlay();
    }

    if (Keyboard.current.leftAltKey.isPressed) {
      for (int i = 0; i < numberKeys.Length; i++) {
        if (Keyboard.current[numberKeys[i]].wasPressedThisFrame) {
          LevelManager.Instance.LoadLevel(i + 1);
          return;
        }
      }
    }

    foreach (var (key, action) in debugActions) {
      if (Keyboard.current[key].wasPressedThisFrame) {
        action();
      }
    }

  }

  private void ToggleDebugOverlay() {
    debugOverlayContainer.style.display =
      debugOverlayContainer.style.display == DisplayStyle.None
        ? DisplayStyle.Flex
        : DisplayStyle.None;
  }

  private void AddDebugShortcuts() {
    debugActions.Add(Key.Digit1, () => {
      if (GameManager.Instance.TryGetPlayer(out var player)) {
        player.GetComponentInChildren<Damageable>().TakeDamage(100);
      }
    });
  }

  private void AddDebugActions() {
    // Trigger player death action
    var triggerPlayerDeathButton = new Button();
    triggerPlayerDeathButton.text = "Trigger Player Death (1 key)";
    LinkButtonToAction(
      triggerPlayerDeathButton,
      () => {
        if (GameManager.Instance.TryGetPlayer(out var player)) {
          player.GetComponentInChildren<Damageable>().TakeDamage(100);
          Debug.Log("Player death triggered");
        }
      },
      Key.Digit1
    );
    debugOverlayContainer.Add(triggerPlayerDeathButton);

    // Toggle player invulnerability action
    var togglePlayerInvulnerabilityButton = new Button();
    togglePlayerInvulnerabilityButton.text = "Toggle Player Invulnerability (2 key)";
    LinkButtonToAction(
      togglePlayerInvulnerabilityButton,
      () => {
        if (GameManager.Instance.TryGetPlayer(out var player)) {
          player.GetComponentInChildren<Damageable>().ToggleIsInvulnerable();
          Debug.Log("Player invulnerability toggled");
        }
      },
      Key.Digit2
    );
    debugOverlayContainer.Add(togglePlayerInvulnerabilityButton);

    // Kill all spawners
    var killSpawnersButton = new Button();
    killSpawnersButton.text = "Kill All Spawners (3 key)";
    LinkButtonToAction(
      killSpawnersButton,
      () => {
        var spawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners) {
          spawner.GetComponent<Damageable>().TakeDamage(100);
        }
      },
      Key.Digit3
    );
    debugOverlayContainer.Add(killSpawnersButton);
  }

  private void LinkButtonToAction(Button button, Action action, Key keyCode = Key.None) {
    button.clicked += action;
    if (keyCode != Key.None) {
      debugActions.Add(keyCode, action);
    }
  }

  private void InitialDebugState() {
    if (GameManager.Instance.TryGetPlayer(out var player)) {
      player.GetComponentInChildren<Damageable>().SetIsInvulnerable(defaultIsPlayerInvulnerable);
    }
  }
}
