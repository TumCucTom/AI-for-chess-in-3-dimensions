using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IInputHandler
{
void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action
onClick);
}



INPUT RECIEVER
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