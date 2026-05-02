using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoardButton : MonoBehaviour
{
[SerializeField] MeshRenderer respectiveBoard;
[SerializeField] Material zero;
[SerializeField] Material max;
[SerializeField] GameObject button;

private bool current = true;
private Color minC= new Color(1,1,1,0.25f);
private Color MaxC = new Color(1, 1, 1, 0.75f);

public void ButtonChange()
{
if (current)
{
respectiveBoard.material = zero;
button.GetComponent<Image>().color = minC;
current = false;
}
else
{
respectiveBoard.material = max;
button.GetComponent<Image>().color = MaxC;
current = true;
}
}
}



CUSTOM EXCEPTION – UNUSED DURING GAMEPLAY



409
using System;

[Serializable]

public class NeuralNetworkInstantiationFailed : Exception
{
public NeuralNetworkInstantiationFailed(string reason) : base(reason)
{

}
}

MATERIAL SETTER
using System;
using System.Collections;
using System.Collections.Generic;