using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverSimple : MonoBehaviour {
    public bool isActive;
    
    [Space]
    public Transform MovedTransform;

    [Space]
    public Vector3 Position1;
    public Vector3 Position2;

    [Space]
    public float rate;
    public int TicksTotal;
    public int TicksElapsed;

    public float TimeLastTick;

    private void Start() {
        if (MovedTransform == null)
            MovedTransform = transform;
    }


    private void Update() {
        Tick();
    }

    public Vector3 CalculateRandomVector3(Vector3 vectorOne, Vector3 vectorTwo) {

        return new Vector3(Random.Range(vectorOne.x, vectorTwo.x), Random.Range(vectorOne.y, vectorTwo.y), Random.Range(vectorOne.z, vectorTwo.z));
    }

    public void Tick() {
        TimeLastTick += Time.deltaTime;

        if (isActive && TimeLastTick >= rate) {
            TimeLastTick = 0;
            TicksElapsed++;

            //callBack.Invoke();



            if (TicksTotal > 0 && TicksTotal == TicksElapsed) {
                isActive = false;

                
                // TODO: PoolManager to deactivate
                Destroy(this);

            }
        }
    }
}
