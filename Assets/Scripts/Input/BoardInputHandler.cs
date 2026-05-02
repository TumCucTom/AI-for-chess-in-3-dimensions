using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoardInputHandler : MonoBehaviour, IInputHandler
{
private Board board;

private void Awake()
{
board = GetComponent<Board>();
}

public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action
onClick)
{
board.OnSquareSelected(inputPosition);
}
}

COLLIDER INPUT RECIEVER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderInputReciever : InputReciever
{
private ChessGameController gameController = new ChessGameController();

private Vector3 clickPosition;
void Update()
{
if (Input.GetMouseButtonDown(0))
{
RaycastHit hit;
Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
if (Physics.Raycast(ray, out hit))
{
clickPosition = hit.point;
OnInputRecieved();
}
}
}

public override void OnInputRecieved()
{
foreach (var handler in inputHandlers)
{
if(gameController.AIactive == false)
{
handler.ProcessInput(clickPosition, null, null);
}
else
{
handler.ProcessInput(new Vector3(1000,1000,1000), null, null);
}
}
}
}



DEBUG INPUT HANDLER – UNUSED DURING GAME
using System;




374
using System.Collections;