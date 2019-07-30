using System.Collections;
using System.Collections.Generic;
using IG.CGrid;
using UnityEngine;

public class GridVisuals : MonoBehaviour {
    public CellLogic CellPrefab;

    [Space]
    public bool BackgroundCreation;
    public Vector2 BackgroundSizeForOneRow = new Vector2(0.3f, 0.3f);

    //public SpriteRenderer Background;
    public Transform Background;

    [Space]
    public float HeightDiff = 1.4f;
    public float WidthDiff = 0f;
    public float Distance = 1f;

    // If not identical
    public Vector3 Distances;


    private void Start() {
        if (BackgroundCreation)
            Background.localScale = new Vector3(BackgroundSizeForOneRow.x * ManagerGrids.I.MatrixLength.y, BackgroundSizeForOneRow.y * ManagerGrids.I.MatrixLength.x, Background.localScale.z);
        //Background.localScale.x
    }


}
