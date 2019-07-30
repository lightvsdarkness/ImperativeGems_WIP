using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    [RequireComponent(typeof(AudioSource))]
    public class ManagerAuVisuals : SingletonManager<ManagerAuVisuals> {

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _startingPitch = 1f;
        [SerializeField] private float _currentPitch;
        [SerializeField] private float _maxPitch = 1.5f;

        private void Start() {
            if (_audioSource == null)
                _audioSource = gameObject.GetComponent<AudioSource>();
        }


        //private void Update() {

        //}

        public void PlayAudio() {
            if (_currentPitch + 0.1f >= _maxPitch)
                _currentPitch = _startingPitch;

            _currentPitch += Random.Range(0.1f, 1.0f);
            _audioSource.pitch = Mathf.Clamp(_currentPitch, _startingPitch, _maxPitch);
            _audioSource.Play();
        }

        /// <summary>
        /// TODO: All kinds of effects!
        /// </summary>
        public void UpdateAuVisualsEndOfTurn() {
            CellLogic cell = null;

            for (int i = 0; i < ManagerGrids.I.MatrixLength.x; i++) {
                for (int j = 0; j < ManagerGrids.I.MatrixLength.y; j++) {
                    cell = ManagerGrids.I.GridMain[i, j];

                    // NOTE: Messy! TODO: Improve something?
                    cell.AuVisuals.UpdateCellEndOfTurn(cell.Data);
                }
            }


        }

        //public void UpdateGridPalette() {
        //    foreach (var cellInPalette in ManagerGrids.I.GridPalette) {
        //        cellInPalette.AuVisuals.UpdateCellDataChanged(cellInPalette.Data);
        //    }
        //}
    }
}