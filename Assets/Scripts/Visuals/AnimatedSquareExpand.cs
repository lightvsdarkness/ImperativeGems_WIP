using UnityEngine;
using System.Collections;

public class AnimatedSquareExpand : MonoBehaviour 
{
	float X=4,Y=4;
	float Alpha = 1.0f;

	void Start () 
	{
		Animate ();
	}

	void Animate()
	{
		gameObject.GetComponent<SpriteRenderer> ().color = new Color(1.0f, 1.0f, 1.0f, Alpha);
		transform.localScale = new Vector2 (X, Y);

		X += 0.4f;
		Y += 0.4f;
		Alpha -= 0.1f;

		if (transform.localScale.x <= 8.0f) 
		{
			Invoke ("Animate", 0.05f);
		} 
		else
			Destroy (gameObject);
	}
}
