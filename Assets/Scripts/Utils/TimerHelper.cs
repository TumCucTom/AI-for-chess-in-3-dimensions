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