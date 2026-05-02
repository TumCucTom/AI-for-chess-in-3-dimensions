using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OfficialNotation
{
string moveNotation;

public void CreateNotation(Piece piece, Vector3Int atCoords, Vector3Int toCoords,
bool take, bool castle = false)
{
string pieceChar = piece.name[0].ToString();
Debug.Log(pieceChar);
string start = (atCoords.z.ToString() + (char)(65 + (atCoords.y)) +
atCoords.x.ToString());
string finish = (toCoords.z.ToString() + (char)(65 + (atCoords.y)) +
atCoords.x.ToString());
string combiation = take ? "x" : " ";
moveNotation = (pieceChar + start + combiation + finish);




410
if (castle) moveNotation = atCoords.z.ToString() + "O-O" +
toCoords.z.ToString();
}

public void DisplayNotation()
{
Debug.Log(moveNotation);
}
}



RANDOM HELPER – UNITY RANDOM FAILS DUE TO SEED ISSUES

The seed issues that arise form unity’s random mean AI makes same moves

TIME HELPER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerHelper : MonoBehaviour
{
[SerializeField] private ChessGameController chessGameController;

public float remainingTime;
public bool timerActive;

[SerializeField] public Text timerDisplay;

void Update()
{
if (timerActive)
{
if (remainingTime > 0)
{
remainingTime -= Time.deltaTime;
UpdateTimeRemaining(remainingTime);
}
else
{
chessGameController.ChangeActiveTeam();
chessGameController.EndGame();
remainingTime = 0;
timerActive = false;
}
}
}

public void UpdateTimeRemaining(float time)
{
time++;

float hours = Mathf.FloorToInt(time / 3600);
float mins = Mathf.FloorToInt((time-hours*3600)/60);
float secs = Mathf.FloorToInt(time % 60);

timerDisplay.text = string.Format("{0:00} : {1:00}: {2:00}", hours,mins,secs);
}
}




411
UI BUTTON
using System.Collections;
using System.Collections.Generic;
using UnityEngine;