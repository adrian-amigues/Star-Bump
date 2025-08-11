using System;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {
  [SerializeField] private SoundsSO SO;
  private static SoundManager instance = null;
  private AudioSource audioSource;

  private void Awake() {
    if (!instance) {
      instance = this;
      audioSource = GetComponent<AudioSource>();
    }
  }

  public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1) {
    SoundList soundList = instance.SO.sounds[(int)sound];
    AudioClip[] clips = soundList.sounds;
    AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

    if (source) {
      source.outputAudioMixerGroup = soundList.mixer;
      source.clip = randomClip;
      source.volume = volume * soundList.volume;
      source.Play();
    } else {
      instance.audioSource.outputAudioMixerGroup = soundList.mixer;
      instance.audioSource.PlayOneShot(randomClip, volume * soundList.volume);
    }
  }
}

[Serializable]
public struct SoundList {
  [HideInInspector] public string name;
  [Range(0, 1)] public float volume;
  public AudioMixerGroup mixer;
  public AudioClip[] sounds;
}

// using UnityEngine;
// using System;

// public enum SoundType {
//   MissileLaunch,
//   MissileHit,
//   MissileExplosion,
//   EnemyExplosion,
//   EnemyHit,
// }

// [RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
// public class SoundManager : MonoBehaviour {
//   [SerializeField] private SoundList[] soundList;

//   private static SoundManager instance;
//   private AudioSource audioSource;

//   private void Awake() {
//     instance = this;
//   }

//   private void Start() {
//     audioSource = GetComponent<AudioSource>();
//   }

//   public static void PlaySound(SoundType sound, float volume = 1f) {
//     // instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
//   }

// #if UNITY_EDITOR
//   private void OnEnable() {
//     string[] names = Enum.GetNames(typeof(SoundType));
//     Array.Resize(ref soundList, names.Length);
//     for (int i = 0; i < names.Length; i++) {
//       soundList[i].name = names[i];
//     }
//   }
// #endif
// }

// [Serializable]
// public struct SoundList {
//   [HideInInspector] public string name;
//   [SerializeField] private AudioClip[] sounds;
// }
