using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class RotationSimple : MonoBehaviour {
    public Transform rotatedObject;
    [Space]
    public bool OverallRotation = true;
    public Vector3 VectorSpeed;
    public float OverallSpeed = 1f;

    [Space]
    public bool RotationToPoint;
    public Quaternion start;
    public Quaternion end;

    private void Start() {
        if (rotatedObject == null)
            rotatedObject = GetComponent<Transform>();

        //start = Quaternion.identity;
        //end = Quaternion.Euler(0f, 90f, 0f);
    }


    private void Update() {
        if (RotationToPoint) {
            rotatedObject.rotation = Quaternion.LerpUnclamped(start, end, Time.time);
        }
        if (OverallRotation) {
            rotatedObject.Rotate(VectorSpeed, OverallSpeed * Time.deltaTime);
        }

    }



    //public RectTransform targetAnchor;
    //RectTransform selfAnchor;

    ////Set the rotating elements RectTransform
    //void Start() {
    //    selfAnchor = GetComponent<RectTransform>();
    //}

    ////Update the UI elements rotation
    //void Update() {
    //    float targetRotation = Mathf.Atan((targetAnchor.y - selfAnchor.y) / (targetAnchor.x - selfAnchor.x));
    //    selfAnchor.rotation = Quaternion.Euler(0, 0, targetRotation);
    //}
    //ArcTan only gives a result that lies between -90 and 90 degrees right? Just a heads up as well.In the above example you'd have to multiply targetRotation by Mathf.Rad2Deg as the Atan Method returns the angle in radians. – Bakewell Jan 6 '17 at 15:45
}