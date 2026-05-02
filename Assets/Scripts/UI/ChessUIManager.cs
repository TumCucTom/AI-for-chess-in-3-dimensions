using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChessUIManager : MonoBehaviour
{

[SerializeField] private GameObject gameOverParent;
[SerializeField] private GameObject settingsButtonParent;
[SerializeField] private GameObject settingsMenuParent;
[SerializeField] private GameObject pvp;
[SerializeField] private GameObject pva;
[SerializeField] private GameObject ava;
[SerializeField] private GameObject timers;

[SerializeField] private Button restartButton;
[SerializeField] private Text finishText;
[SerializeField] private Text numMovesMade;

[SerializeField] private Text timeMain;
[SerializeField] private Text timeIncrem;
[SerializeField] private Text aiDiff;
public bool playerPlaysBlack = false;
public int difficulty;

[SerializeField] public TimerHelper whiteClock;
[SerializeField] public TimerHelper blackClock;

public float timeInc;

[Header("THIS MUST HAVE SPECIFIC ORDER"), Tooltip("WP | WB | WN | WC | WR | WQ
| BP | BB | BN | BC | BR | BQ ")]
[SerializeField] private List<Text> takenPieceNums = new List<Text>();

private int[] takenPieceValues = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0 };

private Dictionary<(TeamColor, string), int> teamAndPieceToIndexDict = new
Dictionary<(TeamColor, string), int>()
{
{(TeamColor.White, "P"), 0},
{(TeamColor.White, "B"), 1},
{(TeamColor.White, "K"), 2},
{(TeamColor.White, "C"), 3},
{(TeamColor.White, "R"), 4},
{(TeamColor.White, "Q"), 5},




406
{(TeamColor.Black, "P"), 6},
{(TeamColor.Black, "B"), 7},
{(TeamColor.Black, "K"), 8},
{(TeamColor.Black, "C"), 9},
{(TeamColor.Black, "R"), 10},
{(TeamColor.Black, "Q"), 11 }

};

public void ConfirmPress()
{
pva.SetActive(false);
pvp.SetActive(false);
ava.SetActive(false);
DifficultySelection();
}

public void WhiteTeamClick()
{
playerPlaysBlack = false;
}

public void BlackTeamClick()
{
playerPlaysBlack = true;
}

public void ActivatePvp()
{
pvp.SetActive(true);
}
public void ActivatePva()
{
pva.SetActive(true);
gameOverParent.SetActive(false);
}
public void ActivateAva()
{
ava.SetActive(true);
gameOverParent.SetActive(false);
}

public void AdjustForPlaying()
{
settingsMenuParent.SetActive(false);
gameOverParent.SetActive(false);
settingsButtonParent.SetActive(true);
}

internal void OnGameFinished(string winner)
{
gameOverParent.SetActive(true);
settingsMenuParent.SetActive(false);
settingsButtonParent.SetActive(true);
finishText.text = winner;
}

public void SettingButtonPress()
{
gameOverParent.SetActive(false);
settingsMenuParent.SetActive(true);
settingsButtonParent.SetActive(false);




407
pva.SetActive(false);
pvp.SetActive(false);
ava.SetActive(false);
}

public void SettingReturnPress()
{
gameOverParent.SetActive(false);
settingsMenuParent.SetActive(false);
settingsButtonParent.SetActive(true);
pva.SetActive(false);
pvp.SetActive(false);
ava.SetActive(false);
}

public void GoToMainMenu()
{
gameOverParent.SetActive(true);
settingsMenuParent.SetActive(false);
settingsButtonParent.SetActive(true);
timers.SetActive(false);
pva.SetActive(false);
pvp.SetActive(false);
ava.SetActive(false);
}

public void TimeSelection()
{
timers.SetActive(true);
whiteClock.remainingTime = Convert.ToInt32(timeMain.text)*60;
blackClock.remainingTime = Convert.ToInt32(timeMain.text)*60;
blackClock.timerActive = false;
timeInc = Convert.ToInt32(timeIncrem.text);
}

public void DifficultySelection()
{
try
{
if (Convert.ToInt32(aiDiff.text) < 6 &&
Convert.ToInt32(aiDiff.text) > 0)
{
difficulty = Convert.ToInt32(aiDiff.text);
}
else
{
difficulty = 1;
}
}
catch
{
difficulty = 1;
}
}

public int GiveDifficulty()
{
return difficulty;
}

public void IncreaseTakenPiece(Piece piece)
{




408
int index = teamAndPieceToIndexDict[(piece.team,
piece.name[0].ToString())];
takenPieceValues[index]++;
takenPieceNums.ElementAt(index).text =
(takenPieceValues[index]).ToString();
}

public void ResetAllNums()
{
for (int i = 0; i < takenPieceNums.Count; i++)
{
takenPieceNums.ElementAt(i).text = "0";
takenPieceValues[i] = 0;
numMovesMade.text = "0";
}
}

public void DisplayNumMovesMade(int moves)
{
numMovesMade.text = moves.ToString();
}
}

UTILS

BOARD BUTTON
using System.Collections;
using System.Collections.Generic;