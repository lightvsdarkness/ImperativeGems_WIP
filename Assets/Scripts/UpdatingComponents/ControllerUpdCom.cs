using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerUpdCom : MonoBehaviour
{
    public List<UpdatingComponent> UpdatingComponents;

    protected GameObject WorldContainer;
    public Transform UIContainter;

    public void Start()
    {
		if (WorldContainer == null) { 
            Debug.LogError("Controller doesn't have container for UI elements", this);
            return;
        }
        UpdatingComponents = WorldContainer.GetComponentsInChildren<UpdatingComponent>().ToList();
        Debug.Log("Setting UI controls for " + WorldContainer.name + "UpdatingComponents are: " + UpdatingComponents, this);
        foreach (var updComponent in UpdatingComponents) {
            UpdatingComponent popupUpdComp = UpdatingComponent.Instantiate(updComponent.UIPrefab);
            popupUpdComp.transform.SetParent(UIContainter, false);
            popupUpdComp.Mirror(updComponent);
        }
    }
	
	public void Setup(GameObject worldContainer) {
	    foreach (Transform transform in UIContainter) {
	        Destroy(transform.gameObject);

	        this.WorldContainer = worldContainer;
	    }
	}

    protected bool ControlUpdate() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Destroy(gameObject);
            return false;
        }
        return true; //base.ControlUpdate();
    }

    protected  Transform OurTarget() {
        return WorldContainer.transform;
    }
}
