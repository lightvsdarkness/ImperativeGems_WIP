using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    public enum Ages {
        TansferPlus1,
        TransferMinus1,
            
    }

    public class ManagerAges : SingletonManager<ManagerAges> {

        // Change Age each 5 turns
        public int HowOftenToChangeAge = 10;


        protected Coroutine RoutineAgeChange;

        private void Start() {


        }



        public void StartAgeChange() {
            if (RoutineAgeChange == null)
                RoutineAgeChange = StartCoroutine(EnergyFlow());
            else
            {
                Debug.LogError("Starting second Coroutine while first still active", this);
            }
        }

        private IEnumerator EnergyFlow() {


            yield return WaitTimer;
        }
    }
}