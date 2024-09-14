using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Traffic.View
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private int[] _menuScenesIndex;
        [SerializeField] private AudioClip _menuMusic, _gameMusic;
        [SerializeField] private FloatReference _musicVolume;
        private AudioSource _audioSource;
        // Start is called before the first frame update
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += ChangedActiveScene;
            _audioSource = GetComponent<AudioSource>();
            _audioSource.volume = _musicVolume.value;
            _musicVolume.UseEvent();
            _musicVolume.ValueChanged += VolumeChanged;
        }

        private void VolumeChanged(object sender, EventArgs e)
        {
            _audioSource.volume = _musicVolume.value;
        }

        private void ChangedActiveScene(Scene current, Scene next)
        {
            bool isLoadingMenuScene = false;

            foreach (int sceneIndex in _menuScenesIndex)
            {
                if (sceneIndex == next.buildIndex)
                {
                    isLoadingMenuScene = true;
                    if(_audioSource.clip == _gameMusic)
                        SetMusic(_menuMusic);
                }
            }
            if (!isLoadingMenuScene && _audioSource.clip == _menuMusic)
                SetMusic(_gameMusic);
        }

        private void SetMusic(AudioClip music)
        {
            _audioSource.clip = music;
            _audioSource.Play();
        }

        public void StartMusic()
        {
            SetMusic(_gameMusic);
        }
    }
}
