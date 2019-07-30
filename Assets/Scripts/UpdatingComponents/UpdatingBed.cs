using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatingBed : UpdatingComponent {

    public int _mode = 0;

    public int Mode {
        get { return _mode; }
        set {
            _mode = value;
            MakeDirty();
        }
    }

    public IntEvent modeSet;
	

	public override void MakeDirty() {
	    Debug.Log("Invoking indirect mode change event to " + modeSet.GetPersistentEventCount() + " listeners.");
	    modeSet.Invoke(_mode);
	    base.MakeDirty();
	}
}
