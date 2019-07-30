using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IG.CGrid
{
    [CreateAssetMenu(fileName = "DataCellStates", menuName = "Scriptable Objects/!Cell/DataCellStates")]
    public class DataCellStates : ScriptableObject { //ISerializationCallbackReceiver

        public List<CellStateColor> CellStatesColor = new List<CellStateColor>();
        public List<CellStateFormT> CellStatesFormT = new List<CellStateFormT>();

        //public void GetStateByName() {

        //}



    }
}