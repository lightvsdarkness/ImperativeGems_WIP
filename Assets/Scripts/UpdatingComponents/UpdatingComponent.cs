using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class UpdatingComponent : MonoBehaviour {

    public UpdatingComponent UIPrefab;

    public UpdatingComponentEvent onChange;

    protected UpdatingComponent _mirror;

    public virtual void MakeDirty() {
		onChange.Invoke(this);
	}

    public virtual void Copy(UpdatingComponent source) {
        System.Type type = source.GetType();

        if (this.GetType() != type) {
            Debug.LogError(name + ": Cannot copy a " + type + " onto a " + this.GetType());
            return;
        }

        System.Reflection.FieldInfo[] fields = type.GetFields();

        foreach (System.Reflection.FieldInfo field in fields) {
            if (field.FieldType.ToString().EndsWith("Event")) // for skipping events - fields with names ending "Event"
                continue;

            Debug.Log("Copying field " + field.Name + " (" + field.Attributes + "!!!" + field.FieldType + ")");
            field.SetValue(this, field.GetValue(source));
        }

        // Mark dirty now, before the mirroring is established, to set up UI elements if there are any
        MakeDirty();
        Debug.Log("Copied " + source.name + " onto " + name + " C" + type + ")");
    }
	
    /// <summary>
    /// Initialization: clone the other entity and setup a connection
    /// </summary>
    /// <param name="other"></param>
	public virtual void Mirror(UpdatingComponent other) {
		Copy(other);
        _mirror = other;
        onChange.AddListener(other.Copy);
    }

    public virtual void OnDestroy() {
        // unsign-up for updates
        if (_mirror != null) {
            _mirror.onChange.RemoveListener(Copy);
        }
    }
}
