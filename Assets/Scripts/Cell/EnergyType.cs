using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    [CreateAssetMenu(fileName = "EnergyType", menuName = "Scriptable Objects/!Cell/EnergyType")]
    public class EnergyType : ScriptableObject {
        public string Name;
        public ColorData StateColor;


    }
}