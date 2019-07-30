using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextParticleDissolver : MonoBehaviour
{
    public bool PlayOnStart;
    public bool UseSpriteRend = true;

    [Space]
    public Sprite SpriteToUse;

    [Space]
    public SpriteRenderer SpriteRend;
    public List<ParticleSystem> ParticleSystems = new List<ParticleSystem>();

    private Coroutine _particleDissolveRoutine = null;


    private void Start() {
        if (ParticleSystems.Count == 0)
            ParticleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
        if (SpriteRend == null)
            SpriteRend = GetComponentInChildren<SpriteRenderer>();

        if (SpriteToUse != null)
            SetTexture(SpriteToUse.texture);

        if (PlayOnStart)
            ParticleDissolve();
    }

    [ContextMenu("ParticleDissolve")]
    public void ParticleDissolve() {
        _particleDissolveRoutine = StartCoroutine(StartParticleDissolve());
    }

    private IEnumerator StartParticleDissolve() {
        if (UseSpriteRend)
        {
            SpriteRend.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
        }

        foreach (var ps in ParticleSystems)
        {
            ps.Stop(true);
        }
        ActualParticleDissolve();
    }


    public void ActualParticleDissolve() {
        if (UseSpriteRend)
            SpriteRend.gameObject.SetActive(false);
        foreach (var ps in ParticleSystems)
        {
            ps.Play(true);
        }

    }

    public void StopParticleDissolve() {
        if (_particleDissolveRoutine != null)
            StopCoroutine(_particleDissolveRoutine);
        _particleDissolveRoutine = null;
    }

    public void SetTexture(Texture2D texture) {
        if (UseSpriteRend) //Texture2DChangeBlackPixelsToTransparent(texture)
            SpriteRend.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        foreach (var ps in ParticleSystems)
        {
            var shape = ps.shape;
            shape.texture = texture;
        }
    }

    public Texture2D Texture2DChangeBlackPixelsToTransparent(Texture2D copiedTexture) {
        Texture2D texture = new Texture2D(copiedTexture.width, copiedTexture.height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        int y = 0;
        while (y < texture.height)
        {
            int x = 0;
            while (x < texture.width)
            {
                if (copiedTexture.GetPixel(x, y) == Color.black)
                {
                    Color color = new Color(0, 0, 0, 0);
                    texture.SetPixel(x, y, color);
                }
                else
                {
                    texture.SetPixel(x, y, copiedTexture.GetPixel(x, y));
                }
                ++x;
            }
            ++y;
        }
        texture.Apply();


        return texture;
    }
}
