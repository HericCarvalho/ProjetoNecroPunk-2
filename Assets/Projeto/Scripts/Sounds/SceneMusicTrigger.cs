using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class SceneMusicTrigger : MonoBehaviour
{
    [SerializeField] private List<AudioClip> scenePlaylist;

    void Start()
    {
        // Envia as músicas da cena atual para o gerenciador persistente
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ChangePlaylist(scenePlaylist);
        }
    }
}
