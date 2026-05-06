using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
public class ChessGameController : MonoBehaviour
{
private enum GameState
{
Init, Play, Finished
}
[SerializeField] private BoardLayout startingBoardLayout;
[SerializeField] private Board board;
[SerializeField] private ChessUIManager UIManager;
[SerializeField] private Camera mainCamera;
[SerializeField] private AudioSource moveSound;
[SerializeField] public TimerHelper whiteClock;
[SerializeField] public TimerHelper blackClock;
private PiecesCreator pieceCreator;
private ChessPlayer whitePlayer;
private ChessPlayer blackPlayer;
public ChessPlayer activePlayer;
private AIManager ai;
private GameState state;
public bool AIactive;
public int AINum = 0;
public int aiDifficulty;
private void Awake()
{
SetDependencies();
}
private void SetDependencies()
{
pieceCreator = GetComponent<PiecesCreator>();
}
public AIManager CreateAIOfDifficulty(ChessPlayer a, ChessPlayer o, Board board,int difficulty)
{
if (difficulty == 1)
{
return new AI1(a, o, board);
}
else if (difficulty == 2)
{
return new AI2(a, o, board);
}
else if (difficulty == 3)
{
return new AI3(a, o, board);
}
else if (difficulty == 4)
{
return new AI3(a, o, board);
}
else if (difficulty == 5)
{
return new AI3(a, o, board);
}
else
{
return new AI1(a, o, board);
}
}
private void CreatePlayers()
{
UIManager.DifficultySelection();
Debug.Log(UIManager.GiveDifficulty());
if (UIManager.playerPlaysBlack == true)
{
Debug.Log("black plays");
whitePlayer = new ChessPlayer(TeamColor.White, board, PlayerType.Person);
ai = CreateAIOfDifficulty(whitePlayer, blackPlayer, board, UIManager.GiveDifficulty()) ;
PlayerType pType;
pType = AINum > 0 ? PlayerType.AI : PlayerType.Person;
whitePlayer = new ChessPlayer(TeamColor.White, board, pType);
if (pType == PlayerType.AI)
{
AIactive = true;
}
pType = AINum > 1 ? PlayerType.AI : PlayerType.Person;
blackPlayer = new ChessPlayer(TeamColor.Black, board, pType);
}
else
{
whitePlayer = new ChessPlayer(TeamColor.White, board, PlayerType.Person);
ai = CreateAIOfDifficulty(whitePlayer, blackPlayer, board, UIManager.GiveDifficulty());
PlayerType pType;
pType = AINum > 1 ? PlayerType.AI : PlayerType.Person;
whitePlayer = new ChessPlayer(TeamColor.White, board, pType);
if (pType == PlayerType.AI)
{
AIactive = true;
}
pType = AINum > 0 ? PlayerType.AI : PlayerType.Person;
blackPlayer = new ChessPlayer(TeamColor.Black, board, pType);
}
}
private void Start()
{
StartNewGame();
}
public void StartNewGame()
{
AIactive = false;
CreatePlayers();
SetGameState(GameState.Init);
board.SetDependencies(this);
CreatePiecesFromLayout(startingBoardLayout);
activePlayer = whitePlayer;
GenerateAllPossiblePlayerMoves(activePlayer);
SetGameState(GameState.Play);
board.DeleteDisplayNotation();
board.pastMovesBlack.Clear();
board.pastMovesWhite.Clear();
UIManager.blackClock.timerDisplay.text = "Waiting...";
if(AIactive)
{
StartCoroutine(MakeAnAiMove());
}
}
private void SetGameState(GameState state)
{
this.state = state;
}
internal bool IsGameInProgress()
{
return state == GameState.Play;
}
private void CreatePiecesFromLayout(BoardLayout layout)
{
for (int i = 0; i < layout.GetPiecesCount(); i++)
{
Vector3Int squareCoords = layout.GetSquareCoordsAtIndex(i);
TeamColor team = layout.GetSquareTeamColorAtIndex(i);
string typeName = layout.GetSquarePieceNameAtIndex(i);
Type type = Type.GetType(typeName);
CreatePieceAndInitialize(squareCoords, team, type);
}
}
public void CreatePieceAndInitialize(Vector3Int squareCoords, TeamColor team, Type type)
{
Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
newPiece.SetData(squareCoords, team, board);
Material teamMaterial = pieceCreator.GetTeamMaterial(team, type);
newPiece.SetMaterial(teamMaterial);
if (newPiece.team == TeamColor.White) newPiece.transform.Rotate(0, 0, 180);
board.SetPieceOnBoard(squareCoords, newPiece);
ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
currentPlayer.AddPiece(newPiece);
}
private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
{
player.GenerateAllPossibleMoves();
}
public bool IsTeamTurnActive(TeamColor team)
{
return activePlayer.team == team;
}
public void EndTurn()
{
if(activePlayer.team == TeamColor.White)
{
whiteClock.remainingTime += UIManager.timeInc;
whiteClock.UpdateTimeRemaining(whiteClock.remainingTime);
whiteClock.timerActive = false;
blackClock.timerActive = true;
}
else
{
blackClock.remainingTime += UIManager.timeInc;
blackClock.UpdateTimeRemaining(blackClock.remainingTime);
blackClock.timerActive = false;
whiteClock.timerActive = true;
}
moveSound.Play();
ai.IncreaseNumMovesMade();
UIManager.DisplayNumMovesMade(ai.GetNumMovesMade());
GenerateAllPossiblePlayerMoves(activePlayer);
GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
int finNum = CheckIfGameIsFinished();
if (finNum == 1)
{
EndGame();
}
else if(finNum == 2)
{
SetGameState(GameState.Finished);
UIManager.OnGameFinished("Draw");
}
else
{
ChangeActiveTeam();
if (activePlayer.type == PlayerType.AI)
{
RemoveCheckingMoves();
AIactive = true;
StartCoroutine(MakeAnAiMove());
AIactive = false;
}
//RotateCameraInstant();
//RotateCamera();
}
}
private bool CheckStalemate()
{
if (activePlayer.CheckForMoves())
{
return true;
}
return false;
}
private void RotateCamera()
{
var smooth = 1f;
Vector3 velocity = Vector3.zero;
float smoothTime = 1f;
Vector3 targetPosition = activePlayer == whitePlayer ? new Vector3(0, 10.5f, -5.25f) : new Vector3(0,
10.5f, 5.25f);
Quaternion targetRotation = activePlayer == whitePlayer ? Quaternion.Euler(50, 0, 0) :
Quaternion.Euler(130, 0, 180);
mainCamera.transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity,
smoothTime);
mainCamera.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
Time.deltaTime * smooth);
mainCamera.transform.position = Vector3.SmoothDamp(transform.position, targetPosition*2, ref
velocity, smoothTime);
}
private void RotateCameraInstant()
{
Vector3 targetPosition = activePlayer == whitePlayer ? new Vector3(0, 10.5f, -10.5f) : new Vector3(0,
10.5f, 10.5f);
Quaternion targetRotation = activePlayer == whitePlayer ? Quaternion.Euler(50, 0, 0) :
Quaternion.Euler(130, 0, 180);
mainCamera.transform.rotation = targetRotation;
mainCamera.transform.position = targetPosition;
}
private int CheckIfGameIsFinished()
{
Piece[] kingAttackingPieces = activePlayer.GetPieceAtackingOppositePiceOfType<King>();
if (kingAttackingPieces.Length > 0)
{
ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
Piece[] attackedKings = oppositePlayer.GetPiecesOfType<King>();
foreach (var attackedKing in attackedKings)
{
oppositePlayer.RemoveMovesEnablingAttackOnPieceOfType<King>(activePlayer, attackedKing);
int avaliableKingMoves = attackedKing.avaliableMoves.Count;
if (avaliableKingMoves == 0)
{
bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
if (!canCoverKing)
return 1; //mate
}
}
}
else
{
ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
Piece[] attackedKings = oppositePlayer.GetPiecesOfType<King>();
foreach (var attackedKing in attackedKings)
{
oppositePlayer.RemoveMovesEnablingAttackOnPieceOfType<King>(activePlayer, attackedKing);
foreach (Piece piece in oppositePlayer.activePieces)
{
if(piece.avaliableMoves.Count != 0)
{
return 0; //carry on
}
}
return 2; //stale
}
}
return 0; // carry on
}
public void RemoveCheckingMoves()
{
foreach(var active in activePlayer.activePieces)
{
activePlayer.RemoveMovesEnablingAttackOnPieceOfType<King>(GetOpponentToPlayer(activePlayer), active);
}
}
public void EndGame()
{
string display = activePlayer.team.ToString() + " Won!";
SetGameState(GameState.Finished);
UIManager.OnGameFinished(display);
}
public void RestartGame()
{
UIManager.ResetAllNums();
UIManager.AdjustForPlaying();
DestroyPieces();
board.OnGameRestarted();
whitePlayer.OnGameRestarted();
blackPlayer.OnGameRestarted();
StartNewGame();
}
private void DestroyPieces()
{
whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
}
public void ChangeActiveTeam()
{
activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
}
private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
{
return player == whitePlayer ? blackPlayer : whitePlayer;
}
public ChessPlayer GetActivePlayer()
{
return activePlayer == whitePlayer ? whitePlayer : blackPlayer;
}
public void SaveState()
{
Debug.Log("Saved");
string filePath = "Assets/Save Games/Cheat.txt";
string allMoves = "";
Vector3Int atCoords;
Vector3Int goCoords;
for (int i = 0; i < board.pastMovesBlack.Count; i++)
{
(atCoords, goCoords) = board.pastMovesWhite.ElementAt(i);
allMoves+=(atCoords.x.ToString());
allMoves += (atCoords.y.ToString());
allMoves += (atCoords.z.ToString());
allMoves += (goCoords.x.ToString());
allMoves += (goCoords.y.ToString());
allMoves += (goCoords.z.ToString());
(atCoords, goCoords) = board.pastMovesBlack.ElementAt(i);
allMoves += (atCoords.x.ToString());
allMoves += (atCoords.y.ToString());
allMoves += (atCoords.z.ToString());
allMoves += (goCoords.x.ToString());
allMoves += (goCoords.y.ToString());
allMoves += (goCoords.z.ToString());
}
if( board.pastMovesWhite.Count > board.pastMovesBlack.Count)
{
(atCoords, goCoords) = board.pastMovesWhite.ElementAt(board.pastMovesWhite.Count-1);
allMoves += (atCoords.x.ToString());
allMoves += (atCoords.y.ToString());
allMoves += (atCoords.z.ToString());
allMoves += (goCoords.x.ToString());
allMoves += (goCoords.y.ToString());
allMoves += (goCoords.z.ToString());
}
File.WriteAllText(filePath, allMoves);
}
public void LoadState()
{
AIactive = true;
string filePath = "Assets/Save Games/Cheat.txt";
string allMoves = File.ReadAllText(filePath);
Vector3Int atCoords;
Vector3Int goCoords;
for (int i = 0; i < allMoves.Length/3; i+=2)
{
atCoords = new Vector3Int(Convert.ToInt32(allMoves.Substring(3 * i,1)),
Convert.ToInt32(allMoves.Substring(3 * i +1,1)), Convert.ToInt32(allMoves.Substring(3 * i+2,1)));
goCoords = new Vector3Int(Convert.ToInt32(allMoves.Substring(3 * i+3,1)),
Convert.ToInt32(allMoves.Substring(3 * i + 4,1)), Convert.ToInt32(allMoves.Substring(3 * i + 5,1)));
StartCoroutine(LoadPiecePositions(atCoords, goCoords));
}
AIactive = false;
}
internal void OnPieceRemoved(Piece piece)
{
ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
pieceOwner.RemovePiece(piece);
}
internal void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
{
activePlayer.RemoveMovesEnablingAttackOnPieceOfType<T>(GetOpponentToPlayer(activePlayer),
piece);
}
IEnumerator MakeAnAiMove()
{
yield return new WaitForSeconds(0.25f);
ai.MoveMakerController(GetActivePlayer(),GetOpponentToPlayer(GetActivePlayer()));
}
IEnumerator LoadPiecePositions(Vector3Int atCoords, Vector3Int goCoords)
{
yield return new WaitForSeconds(0.25f);
board.AIMakeMove(atCoords, goCoords);
}
IEnumerator Wait(float time)
{
yield return new WaitForSeconds(time);
}
public void addAI1()
{
AINum = 1;
}
public void addAI2()
{
AINum = 2;
}
public void addAI0()
{
AINum = 0;
}
}