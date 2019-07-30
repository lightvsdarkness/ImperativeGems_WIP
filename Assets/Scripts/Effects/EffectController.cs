using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IG {
    public class EffectController : MonoBehaviour {

        public List<Transform> Effects = new List<Transform>();
        public Dictionary<Transform, IObjectMovingAlongPath> EffectsMoving =new Dictionary<Transform, IObjectMovingAlongPath>();
        //public List<IObjectMovingAlongPath> EffectsMoving = new List<IObjectMovingAlongPath>();

        private void Start() {
            if (Effects.Count == 0)
                Effects = GetComponentsInChildren<Transform>().ToList();
            foreach (var effect in Effects) {
                EffectsMoving.Add(effect, effect.GetComponent<IObjectMovingAlongPath>());
            }

            //if (EffectsMoving.Count == 0)
            //    EffectsMoving = GetComponentsInChildren<IObjectMovingAlongPath>().ToList();
        }

        //private void Update() {

        //}

        public void MoveEffects(Transform newParent) {
            transform.parent = newParent;
            transform.localPosition = Vector3.zero;

            foreach (var effectMoving in EffectsMoving) {
                if (effectMoving.Value != null) {
                    effectMoving.Value.TeleportObject();
                }
             else {
                 //effectMoving.Key.parent = newParent;
             }

            }
        }
    }
}