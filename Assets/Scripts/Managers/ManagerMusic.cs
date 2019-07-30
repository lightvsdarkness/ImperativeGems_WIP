using System.Collections;
using System.Collections.Generic;
using IG.CGrid;
using UnityEngine;

namespace IG.General {
    public class ManagerMusic : SingletonManager<ManagerMusic> {
        [Space]
        public bool Playing = false;

        [Header("Settings")]
        public List<AudioClip> MusicClips = new List<AudioClip>();
        public bool RandomStartingSong = true;
        public bool PlayOnStart;

        [Space]
        public AudioSource MusicSource;

        public int Index = 0; // just for Debugging purposes
        public float MusicSourceTime = 0; // just for Debugging purposes


        protected Coroutine RoutineWaitMusic;

        //bool introMusicIsNoMore;
        //public AudioSource introMusic;
        //public AudioSource actionMusicLoop;
        //float musicTimer;
        //float progress;

        //void Awake() {
        //    progress = 0f;
        //    musicTimer = 0f;
        //    introMusicIsNoMore = false;
        //}

        protected override void Awake() {
            base.Awake();

            if (MusicSource == null)
                MusicSource = GetComponent<AudioSource>();

            if (RandomStartingSong)
                Index = Random.Range(0, MusicClips.Count);
            MusicSource.clip = MusicClips[Index];


            //if (PlayOnStart)
            //    PlayMusic();
        }

        private void Update() {
            //musicTimer = musicTimer + Time.deltaTime;

            //progress = Mathf.Clamp01(introMusic.time / introMusic.clip.length);
            //if (progress == 1f)
            //{
            //    if (!actionMusicLoop.isPlaying)
            //    {
            //        introMusicIsNoMore = true;
            //        PlayThatFunkyMusic();
            //        Destroy(introMusic.gameObject);
            //    }
            //}
        }

        public void PlayMusic() {
            if (Playing) return;

            if (Debugging) Debug.Log("Started Playing music", this);

            Playing = true;
            MusicSource.Play();

            StartWaitingAudioEnd();
        }

        private IEnumerator WaitAudio() {
            if (Debugging) Debug.Log("Waiting music, length: " + (MusicClips[Index].length - MusicSource.time), this);
            MusicSourceTime = MusicSource.time;
            yield return new WaitForSeconds(MusicClips[Index].length - MusicSource.time);

            NextClip();
            Playing = false;
            PlayMusic();
        }

        public void NextClip() {
            Index++;
            if (Index >= MusicClips.Count)
                Index = 0;

            //Debug.Log("Index: " + Index, this);
            MusicSource.clip = MusicClips[Index];
        }

        public void ObjectSwitch() {
            Playing = false;
            gameObject.SetActive(!gameObject.activeSelf); 

            if (gameObject.activeSelf)
                PlayMusic();
        }

        public void PlayMusicSwitch() {
            Playing = !Playing;
            if (Playing) {
                MusicSource.UnPause();
                StartWaitingAudioEnd();
            }
            else {
                MusicSource.Pause();
                if (RoutineWaitMusic != null)
                    StopCoroutine(RoutineWaitMusic);
            }
            //MusicSource.enabled = Playing;
        }

        public void OnGameUnPaused() {
            StartWaitingAudioEnd();

            if (Debugging) Debug.Log("Game Unpaused, AudioSource at: " + MusicSource.time, this);
        }

        private void StartWaitingAudioEnd() {
            if (RoutineWaitMusic != null)
                StopCoroutine(RoutineWaitMusic);
            RoutineWaitMusic = StartCoroutine(WaitAudio());
        }
    }
}