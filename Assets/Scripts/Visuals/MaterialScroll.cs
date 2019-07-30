using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialScroll : MonoBehaviour {

    public float scroll_Speed = 0.1f;
    [SerializeField] private MeshRenderer Renderer;

    [SerializeField] private Vector2 saved_Offset;


    private void Awake() {
        Renderer = GetComponent<MeshRenderer>();
        saved_Offset = Renderer.sharedMaterial.GetTextureOffset("_MainTex");
    }


    private void Update() {
        float x = Time.time * scroll_Speed;
        Vector2 offset = new Vector2(x, 0);
        Renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);

    }
}
