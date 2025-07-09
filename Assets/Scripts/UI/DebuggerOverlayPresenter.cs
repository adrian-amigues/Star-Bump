using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DebuggerOverlayPresenter : MonoBehaviour {
  [SerializeField] private UIDocument uiDocument;

  private ScrollView debugOverlayContainer;
  private Dictionary<Key, Action> debugActions = new Dictionary<Key, Action>();

  private void Start() {
    var root = uiDocument.rootVisualElement;
    debugOverlayContainer = root.Q<ScrollView>();

    AddDebugActions();
  }

  void Update() {
    if (Keyboard.current.backquoteKey.wasPressedThisFrame) {
      ToggleDebugOverlay();
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

  private void AddDebugActions() {
    // Trigger player death action
    var triggerPlayerDeathButton = new Button();
    triggerPlayerDeathButton.text = "Trigger Player Death (q key)";
    LinkButtonToAction(
      triggerPlayerDeathButton,
      () => {
        PlayerController.Instance.GetComponent<Damageable>().TakeDamage(100);
        Debug.Log("Player death triggered");
      },
      Key.Q
    );
    debugOverlayContainer.Add(triggerPlayerDeathButton);
  }

  private void LinkButtonToAction(Button button, Action action, Key keyCode = Key.None) {
    button.clicked += action;
    if (keyCode != Key.None) {
      debugActions.Add(keyCode, action);
    }
  }
}
