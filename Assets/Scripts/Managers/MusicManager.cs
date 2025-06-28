using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance; // Singleton instance

    [System.Serializable]
    public class SFXClip
    {
        public string name;
        public AudioClip clip;
    }

    public List<SFXClip> sfxClips = new List<SFXClip>();
    private Dictionary<string, AudioClip> sfxDictionary;
    public AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        InitializeSFXDictionary();
    }

    private void InitializeSFXDictionary()
    {
        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (var sfx in sfxClips)
        {
            if (!sfxDictionary.ContainsKey(sfx.name))
            {
                sfxDictionary.Add(sfx.name, sfx.clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate SFX name detected: {sfx.name}. Only the first instance will be used.");
            }
        }
    }
    public void PlaySFX(string clipName)
    {
        if (audioSource == null || sfxDictionary == null || !sfxDictionary.ContainsKey(clipName))
        {
            Debug.Log("Cant play that");
            return;
        }

        audioSource.PlayOneShot(sfxDictionary[clipName]);
    }
}
