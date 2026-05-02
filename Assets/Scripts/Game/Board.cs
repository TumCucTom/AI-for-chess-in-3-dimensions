using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity;
public class Board : MonoBehaviour

{

public const int BOARD_SIZE = 8;



[SerializeField] private Transform bottomLeftSquareTransform;



[SerializeField] private float squareSize;

[SerializeField] private float boardHeight;

[SerializeField] private ChessUIManager chessUIManager;

[SerializeField] private GameObject moveButton;

[SerializeField] private Transform gridContent;



public Piece[,,] grid;



private Piece selectedPiece;

private ChessGameController chessController;

private SquareSelectorCreator squareSelector;



public List<(Vector3Int, Vector3Int)> pastMovesWhite = new List<(Vector3Int, Vector3Int)>();

public List<(Vector3Int, Vector3Int)> pastMovesBlack = new List<(Vector3Int, Vector3Int)>();




341
private Dictionary<String, int> pieceNameToValueDict = new Dictionary<String, int>()

{

{"Pawn", 1 },

{"Bishop", 3 },

{"Knight", 3 },

{"Commoner", 3 },

{"Rook", 5 },

{"Queen", 9 },



{"Pawn(Clone)", 1 },

{"Bishop(Clone)", 3 },

{"Knight(Clone)", 3 },

{"Commoner(Clone)", 3 },

{"Rook(Clone)", 5 },

{"Queen(Clone)", 9 },

};



private List<GameObject> notationList = new List<GameObject>();



public int majorPiecesTaken;

public int majorPiecesMoved;

public int materialImbalance;



private void Awake()

{

squareSelector = GetComponent<SquareSelectorCreator>();

CreateGrid();

moveButton.SetActive(false);

}




342
public void SetDependencies(ChessGameController chessController)

{

this.chessController = chessController;

}



private void CreateGrid()

{

grid = new Piece[BOARD_SIZE, BOARD_SIZE, BOARD_SIZE];

}



public Vector3 CalculatePositionFromCoords(Vector3Int coords)

{

return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize,
(coords.z*boardHeight) + 0.1f, coords.y * squareSize);

}




private Vector3Int CalculateCoordsFromPosition(Vector3 inputPosition)

{

int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + BOARD_SIZE
/ 2;

int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + BOARD_SIZE
/ 2;

int z = (Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).y / boardHeight));

return new Vector3Int(x, y,z);

}



public void OnSquareSelected(Vector3 inputPosition)

{

Vector3Int coords = CalculateCoordsFromPosition(inputPosition);

Piece piece = GetPieceOnSquare(coords);




343
if (selectedPiece)

{

if (piece != null && selectedPiece == piece)

DeselectPiece();

else if (piece != null && selectedPiece != piece && chessController.IsTeamTurnActive(piece.team))

SelectPiece(piece);

else if (selectedPiece.CanMoveTo(coords))

OnSelectedPieceMoved(coords, selectedPiece);

}

else

{

if (piece != null && chessController.IsTeamTurnActive(piece.team))

SelectPiece(piece);

}

}



public Vector3Int getPieceCoords(string name, TeamColor team)

{

for (int i = 0; i < BOARD_SIZE; i++)

{

for (int j = 0; j < BOARD_SIZE; j++)

{

for (int k = 0; k < BOARD_SIZE; k++)

{

if (grid[i, j, k] != null)

{

if (grid[i, j, k].name == name && grid[i, j, k].team == team)

{

return new Vector3Int(i, j, k);

}




344
}




}

}

}

return new Vector3Int(-1, -1, -1);

}



public void AIMakeMove(Vector3Int currentPosition, Vector3Int goToPosition)

{

Piece piece = GetPieceOnSquare(currentPosition);

SelectPiece(piece);

OnSelectedPieceMoved(goToPosition, piece);

}




private void SelectPiece(Piece piece)

{

chessController.RemoveMovesEnablingAttackOnPieceOfType<King>(piece);

selectedPiece = piece;

List<Vector3Int> selection = selectedPiece.avaliableMoves;

ShowSelectionSquares(selection);

}



private void ShowSelectionSquares(List<Vector3Int> selection)

{

Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();

for (int i = 0; i < selection.Count; i++)




345
{

Vector3 position = CalculatePositionFromCoords(selection[i]);

bool isSquareFree = GetPieceOnSquare(selection[i]) == null;

squaresData.Add(position, isSquareFree);

}

squareSelector.ShowSelection(squaresData);

}



private void DeselectPiece()

{

selectedPiece = null;

squareSelector.ClearSelection();

}

private void OnSelectedPieceMoved(Vector3Int coords, Piece piece)

{

ChessPlayer activePlayer = chessController.GetActivePlayer();

if(activePlayer.team == TeamColor.White)

{

pastMovesWhite.Add((piece.occupiedSquare, coords));

}

else

{

pastMovesBlack.Add((piece.occupiedSquare, coords));

}

bool take = TryToTakeOppositePiece(coords);

DisplayNotation(piece, piece.occupiedSquare, coords, take);

UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);

if(piece.hasMoved == false)

{




346
if(!(piece.name == "Pawn" || piece.name == "Pawn(Clone)" || piece.name == "King" || piece.name ==
"King(Clone)"))

{

majorPiecesMoved++;

}

}

selectedPiece.MovePiece(coords, piece);

DeselectPiece();

EndTurn();

}



private void EndTurn()

{

chessController.EndTurn();

}



public void UpdateBoardOnPieceMove(Vector3Int newCoords, Vector3Int oldCoords, Piece newPiece,
Piece oldPiece)

{

grid[oldCoords.x, oldCoords.y, oldCoords.z] = oldPiece;

grid[newCoords.x, newCoords.y, newCoords.z] = newPiece;

}



public Piece GetPieceOnSquare(Vector3Int coords)

{

if (CheckIfCoordinatesAreOnBoard(coords))

return grid[coords.x, coords.y, coords.z];

return null;

}



public bool CheckIfCoordinatesAreOnBoard(Vector3Int coords)




347
{

if (coords.x < 0 || coords.y < 0 || coords.z < 0 || coords.x >= BOARD_SIZE || coords.y >=
BOARD_SIZE || coords.z >= BOARD_SIZE)

return false;

return true;

}



public bool HasPiece(Piece piece)

{

for (int i = 0; i < BOARD_SIZE; i++)

{

for (int j = 0; j < BOARD_SIZE; j++)

{

for (int k = 0; k < BOARD_SIZE; k++)

{

if (grid[i, j,k] == piece)

return true;

}

}

}

return false;

}



public void SetPieceOnBoard(Vector3Int coords, Piece piece)

{

if (CheckIfCoordinatesAreOnBoard(coords))

grid[coords.x, coords.y, coords.z] = piece;

}



private bool TryToTakeOppositePiece(Vector3Int coords)




348
{

Piece piece = GetPieceOnSquare(coords);

if (piece != null && !selectedPiece.IsFromSameTeam(piece))

{

chessUIManager.IncreaseTakenPiece(piece);

if (!(piece.name == "Pawn" || piece.name == "Pawn(Clone)" || piece.name == "King" || piece.name ==
"King(Clone)"))

{

majorPiecesTaken++;

}

materialImbalance += piece.team == TeamColor.White ? pieceNameToValueDict[piece.name] : -1 *
pieceNameToValueDict[piece.name];

TakePiece(piece);

return true;

}

return false;

}



private void TakePiece(Piece piece)

{

if (piece)

{

grid[piece.occupiedSquare.x, piece.occupiedSquare.y, piece.occupiedSquare.z] = null;

chessController.OnPieceRemoved(piece);

Destroy(piece.gameObject);

}

}



public int GetBoardSize()

{

return BOARD_SIZE;




349
}



public void PromotePiece(Piece piece)

{

TakePiece(piece);

chessController.CreatePieceAndInitialize(piece.occupiedSquare, piece.team, typeof(Queen));

}



internal void OnGameRestarted()

{

selectedPiece = null;

CreateGrid();

}



public void DisplayNotation(Piece piece, Vector3Int atCoords, Vector3Int toCoords, bool take, bool castle
= false, bool check=false)

{

string checkS = "";

string pieceChar = piece.name[0].ToString();

if (piece.name == "Knight" || piece.name == "Knight(Clone)")

{

pieceChar = "N";

}

string start = ((atCoords.z+1).ToString() + (char)(65 + (atCoords.x)) + (atCoords.y+1).ToString());

string finish = ((toCoords.z + 1).ToString() + (char)(65 + (toCoords.x)) + (toCoords.y + 1).ToString());

string combiation = take ? "x" : "|";

if (check) checkS = "+";

string notation = pieceChar + start + combiation + finish + checkS;

if (castle) notation = (atCoords.z.ToString() + "O-O" + toCoords.z.ToString() + checkS);

InstantiateDisplayNotation(notation);




350
}



public void InstantiateDisplayNotation(string notation)

{

GameObject newMove = Instantiate(moveButton);

newMove.GetComponentInChildren<Text>().text = notation;

newMove.SetActive(true);

newMove.transform.SetParent(gridContent);

notationList.Add(newMove);

}



public void DeleteDisplayNotation()

{

if(notationList.Count != 0)

{

foreach (GameObject item in notationList)

{

item.SetActive(false);

}

}



}



}

CHESS GAME CONTROLLER

using System;

using System.IO;

using System.Collections;

using System.Collections.Generic;

using System.Linq;




351
using UnityEngine;

using System.Threading.Tasks;



public enum PlayerType

{

Person, AI

}



[RequireComponent(typeof(PiecesCreator))]

[RequireComponent(typeof(AIManager))]