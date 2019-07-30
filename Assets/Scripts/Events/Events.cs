using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UpdatingComponentEvent : UnityEvent<UpdatingComponent> { };

[Serializable]
public class IntEvent : UnityEvent<int> {};

[Serializable]
public class StringEvent : UnityEvent<string> { }
[Serializable]
public class StringAsyncEvent : UnityEvent<string, AsyncOperation> { }