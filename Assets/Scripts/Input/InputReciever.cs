using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class InputReciever : MonoBehaviour
{
protected IInputHandler[] inputHandlers;

public abstract void OnInputRecieved();

private void Awake()
{
inputHandlers = GetComponents<IInputHandler>();
}
}



UI INPUT HANDLER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputHandler : MonoBehaviour, IInputHandler
{




375
public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action
onClick)
{
onClick?.Invoke();
}
}



UI INPUT RECIEVER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInputReciever : InputReciever
{
[SerializeField] UnityEvent onClick;

public override void OnInputRecieved()
{
foreach (var handler in inputHandlers)
{
handler.ProcessInput(Input.mousePosition, gameObject, () =>
onClick.Invoke());
}
}
}

NON GAME UTILITY

TRAINING DATA

using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using System;

using System.IO;