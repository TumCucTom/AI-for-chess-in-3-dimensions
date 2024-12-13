using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic; using UnityEngine;
public abstract class AIManager {
protected ChessPlayer currentAiPlayer {get; set;} protected ChessPlayer opposingPlayer { get; set; } protected Board board { get; set; }
protected Dictionary<String, int> pieceNameToValueDict = new Dictionary<String, int>() {
{"King", 1 }, //Point for putting king in check {"Pawn", 1 },
{"Bishop", 3 },
{"Knight", 3 },
 255
 
{"Commoner", 3 }, {"Rook", 5 }, {"Queen", 9 },
{"King(Clone)",1 }, {"Pawn(Clone)", 1 }, {"Bishop(Clone)", 3 }, {"Knight(Clone)", 3 }, {"Commoner(Clone)", 3 }, {"Rook(Clone)", 5 }, {"Queen(Clone)", 9 },
};
protected static int[] layers = new int[] {1,1,1,1 };
protected float[] inputState;
protected NNManager networkManager = new NNManager(layers,activationType.tanh, costType.tanhCustom);
protected RandomNumber rnd = new RandomNumber(); protected int movesMade;
protected void NewAiPlayers (ChessPlayer newAiPlayer, ChessPlayer opposingPlayer) {
this.currentAiPlayer = newAiPlayer;
this.opposingPlayer = opposingPlayer; }
public abstract void MoveMakerController(ChessPlayer aiPlayer, ChessPlayer opposingPlayer);
protected (Vector3Int, Vector3Int) GetMoveRandom(List<List<Vector3Int>> movesCurrentlyAvailable, List<Vector3Int> pieceCoords)
 256
 
{
int val = Guid.NewGuid().GetHashCode() % pieceCoords.Count;
int atIndex = val > 0 ? val : -val;
val = Guid.NewGuid().GetHashCode() % movesCurrentlyAvailable.ElementAt(atIndex).Count;
int toIndex = val > 0 ? val : -val;
return (pieceCoords.ElementAt(atIndex), movesCurrentlyAvailable.ElementAt(atIndex).ElementAt(toIndex));
}
private float BasicTheoryAddition(Vector3Int toMoveCoords, List<Vector3Int> attackCoords, bool whitePiece)
{
if((toMoveCoords.x ==4 || toMoveCoords.x ==3)&&(toMoveCoords.y == 4 || toMoveCoords.y == 3))
{
addition += 0.4f;
}
if (whitePiece) {
if(toMoveCoords.y != 0) {
addition += 0.1f; }
}
else
{
if(toMoveCoords.y != 7) {
addition += 0.1f; }
}
foreach(Vector3Int coords in attackCoords) {
if ((coords.x == 4 || coords.x == 3) && (coords.y == 4 || coords.y == 3)) // middle {
addition += 0.1f; }
}
return addition;
float addition = 0f;
 257
 
}
private float BasicOpeningTheoryAddition(Vector3Int coords, Vector3Int attackCoords) {
float addition = 0;
Piece piece = board.GetPieceOnSquare(attackCoords); if (piece!= null)
{
addition -= pieceNameToValueDict[piece.name] *0.05f;
if ((coords.x == 4 || coords.x == 3) && (coords.y == 4 || coords.y == 3)) // middle {
addition += 1f; }
piece = board.GetPieceOnSquare(coords);
if (piece!= null && coords.z == board.getPieceCoords("King", piece.team).z) {
addition += 0.1f; }
}
return addition; }
private float BasicMiddleGameTheoryAddition(Vector3Int toMoveCoords, Vector3Int attackCoords) {
float addition = 0;
Piece piece = board.GetPieceOnSquare(attackCoords); if (piece != null)
{
addition += pieceNameToValueDict[piece.name] *0.05f; }
return addition;
 258
 
}
private float BasicEndGameTheoryAddition(Vector3Int toMoveCoords, Vector3Int attackCoords) {
float addition = 0;
if (board.GetPieceOnSquare(toMoveCoords) != null) {
if (board.GetPieceOnSquare(toMoveCoords).name == "Pawn" || board.GetPieceOnSquare(toMoveCoords).name == "Pawn(Clone)")
{
addition += 0.5f * attackCoords.y;
}
else if (board.GetPieceOnSquare(toMoveCoords).name == "King" || board.GetPieceOnSquare(toMoveCoords).name == "King(Clone)")
{
int numPiecesProtecting = 0; for (int i = -1; i < 2; i += 2)
{
for (int j = -1; j < 2; j += 2) {
for (int k = -1; k < 2; k += 2) {
if (board.GetPieceOnSquare(new Vector3Int(attackCoords.x + i, attackCoords.y + j, attackCoords.z + k)) != null)
{
numPiecesProtecting++;
} }
} }
addition += 0.05f * numPiecesProtecting; }
 259
 
}
return addition; }
private float IntermediateTheoryAddition(Vector3Int toMoveCoords, Vector3Int attackCoords) {
float addition = 0;
Piece piece = board.GetPieceOnSquare(toMoveCoords); if (piece != null)
{
if (piece.name == "Knight" || piece.name == "Knight(Clone)") {
if (attackCoords.x == 0 || attackCoords.x == 7 || attackCoords.y == 0 || attackCoords.y == 7 || attackCoords.z == 0 || attackCoords.z == 7)
{
addition -= 0.3f;
} }
else if (piece.name == "Rook" || piece.name == "Rook(Clone)") {
if (attackCoords.x == 0 || attackCoords.x == 7 || attackCoords.y == 0 || attackCoords.y == 7 || attackCoords.z == 0 || attackCoords.z == 7)
{
addition -= 0.3f;
} }
}
return addition; }
 260
 
protected (Vector3Int, Vector3Int) GetMoveMostValueFrom1LookAhead(List<List<Vector3Int>> movesCurrentlyAvailable, List<Vector3Int> pieceCoords)
{
List<int[]> maxValuePositions = new List<int[]>();
int maxValue = 0;
int currentValue = 0;
for (int i = 0; i < movesCurrentlyAvailable.Count; i++) {
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? -1 :
pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
if (currentValue > maxValue) {
maxValuePositions.Clear();
maxValuePositions.Add(new int[] { i, j }); }
if(currentValue == maxValue) {
maxValuePositions.Add(new int[] { i, j }); }
} }
int numBestMoves = maxValuePositions.Count; if (numBestMoves > 0)
{
int indecies = rnd.Next(maxValuePositions.Count);
 261
 
return (pieceCoords.ElementAt(maxValuePositions.ElementAt(indecies)[0]), movesCurrentlyAvailable.ElementAt(maxValuePositions.ElementAt(indecies)[0]).ElementAt(maxValuePositions. ElementAt(indecies)[1]));
} else {
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords); }
}
private List<(float, int[])> MergeSort(List<(float, int[])> nums) {
int count = nums.Count; if (count <= 1)
{
return nums; }
else
{
// splitting
int middle = count / 2; // truncation - will be left mid if even number of array elements
List<(float, int[])> left = new List<(float, int[])>(); List<(float, int[])> right = new List<(float, int[])>(); List<(float, int[])> mergedList = new List<(float, int[])>();
for (int i = 0; i < middle; i++) {
left.Add(nums[i]); }
for (int i = middle; i < count; i++)
{
right.Add(nums[i]);
}
//recursively split and sort for smaller and smaller list sizes until base case met left = MergeSort(left);
right = MergeSort(right);
// rebuilding back up bigger and bigger lists until back to base function call while (left.Count > 0 || right.Count > 0)
{
if (right.Count > 0)
{
if (left.Count > 0) {
if (left[0].Item1 < right[0].Item1)
{
mergedList.Add(left[0]); // add left if smaller than right at smallest index left.RemoveAt(0);
}
 262
 
else {
mergedList.Add(right[0]); //add right if smaller or equal left at smallest index
right.RemoveAt(0); }
} else {
foreach ((float, int[]) item in right) // if no left remaining fill list with right
{
mergedList.Add(item);
}
return mergedList; }
} else
{
foreach ((float, int[]) item in left)// if no right fill list with left
{
mergedList.Add(item);
}
return mergedList; }
}
return new List<(float, int[])>() { (-1f, new int[0]) }; }
}
protected (Vector3Int, Vector3Int) GetMoveFrom1LookAheadWithAccuracy(List<List<Vector3Int>> movesCurrentlyAvailable, List<Vector3Int> pieceCoords,float accuracy)
{
List<(float,int[])> positions = new List<(float, int[])>(); float currentValue = 0;
for (int i = 0; i < movesCurrentlyAvailable.Count; i++) {
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++)
{
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) ==
null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
positions.Add((currentValue, new int[] { i, j })); }
}
positions = MergeSort(positions); // orders list by lowest evaluation int numMoves = positions.Count-1;
if (numMoves > 0)
{
if (accuracy > 0)
{
int indecies = rnd.Next(Mathf.FloorToInt(numMoves - (numMoves * accuracy)), numMoves); int i = positions.ElementAt(indecies).Item2[0];
int j = positions.ElementAt(indecies).Item2[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
int indecies = rnd.Next(0, Mathf.FloorToInt((numMoves)*(-1*accuracy))); int i = positions.ElementAt(indecies).Item2.[0];
 263
 
int j = positions.ElementAt(indecies).Item2.[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j)); }
} else
{
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords);
} }
protected (Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAhead(List<List<Vector3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords)
{
if(board.pastMovesBlack.Count > 3) {
if (currentAiPlayer.team == TeamColor.White) {
if (board.pastMovesWhite.ElementAt(board.pastMovesWhite.Count - 1) == board.pastMovesWhite.ElementAt(board.pastMovesWhite.Count - 3))
{
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords);
} }
else {
if (board.pastMovesBlack.ElementAt(board.pastMovesBlack.Count - 1) == board.pastMovesBlack.ElementAt(board.pastMovesBlack.Count - 3))
{
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords);
} }
}
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
 264
 
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>(); Piece oldPiece;
List<int[]> maxValuePositions = new List<int[]>();
int maxValue = 0;
int currentValue = 0;
for (int i = 0; i < movesCurrentlyAvailable.Count; i++) {
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i),pieces.ElementAt(i), null );
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt (l))).name];
if (currentValue > maxValue) {
maxValuePositions.Clear();
maxValuePositions.Add(new int[] { i, j }); }
if (currentValue == maxValue)
 265
 
{
maxValuePositions.Add(new int[] { i, j });
} }
}
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numBestMoves = maxValuePositions.Count; if (numBestMoves > 0)
{
int val = Guid.NewGuid().GetHashCode() %numBestMoves;
val = val > 0 ? val : -val;
return (pieceCoords.ElementAt(maxValuePositions.ElementAt(val)[0]), movesCurrentlyAvailable.ElementAt(maxValuePositions.ElementAt(val)[0]).ElementAt(maxValuePositions.Elem entAt(val)[1]));
} else {
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords); }
}
protected (Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAheadWithAccuracy(List<List<Vector3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords, float accuracyDefault, float accuracyRange = 0f)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
 266
 
Piece oldPiece;
SortedDictionary<float, int[]> positions = new SortedDictionary<float, int[]>(); float currentValue = 0;
float small = 0.00001f; // prevents same keys
for (int i = 0; i < movesCurrentlyAvailable.Count; i++)
{
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
currentValue += BasicTheoryAddition(pieceCoords.ElementAt(i),movesCurrentlyAvailable.ElementAt(i),board.GetPieceOnSqua re(pieceCoords.ElementAt(i)) == null? true: board.GetPieceOnSquare(pieceCoords.ElementAt(i)).team == TeamColor.White ? true:false);
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt (l))).name];
currentValue -= BasicTheoryAddition(opposingPieceCoords.ElementAt(k),opposingMovesCurrentlyAvailable.ElementAt(k), board.GetPieceOnSquare(pieceCoords.ElementAt(i)) == null ? true : board.GetPieceOnSquare(pieceCoords.ElementAt(i)).team == TeamColor.White ? true : false);
while (positions.ContainsKey(currentValue)){currentValue -= 0.00001f;}positions.Add(currentValue, new int[] { i, j });
 267
 
small += 0.00001f; }
}
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numMoves = positions.Count-1; if (numMoves > 0)
{
if (accuracyRange == 0) {
if (accuracyDefault > 0) {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)), positions.Count);
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
int indecies = rnd.Next(0, Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault))); int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} }
else {
if (accuracyDefault > 0)
 268
 
{
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyRange)), Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count * (-1 * accuracyRange)), Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} }
} else {
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords); }
}
protected (Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAheadWithNN(List<List<Vector3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
Piece oldPiece;
 269
 
Piece oldOPiece;
SortedDictionary<float, int[]> positions = new SortedDictionary<float, int[]>(); float currentValue = 0;
float small = 0.00001f; // prevents same keys
for (int i = 0; i < movesCurrentlyAvailable.Count; i++)
{
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
currentValue += GetNNEvalFromBoardState(board.grid); if (board.majorPiecesMoved < 50)
{
currentValue += BasicOpeningTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
else if (board.majorPiecesTaken < 50) {
currentValue += BasicMiddleGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
currentValue += BasicEndGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
 270
 
}
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
oldOPiece = board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l));
board.UpdateBoardOnPieceMove(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l), opposingPieceCoords.ElementAt(k), opposingPieces.ElementAt(k), null);
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt (l))).name];
currentValue += GetNNEvalFromBoardState(board.grid);
board.UpdateBoardOnPieceMove(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l), opposingPieceCoords.ElementAt(k), oldOPiece, opposingPieces.ElementAt(k));
if (board.majorPiecesMoved < 50) {
currentValue -= BasicOpeningTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare,
opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l)); }
else if (board.majorPiecesTaken < 50) {
currentValue -= BasicMiddleGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
} else {
currentValue -= BasicEndGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
}
 271
 
while (positions.ContainsKey(currentValue)) { currentValue -= 0.00001f; } positions.Add(currentValue, new int[] { i, j });
small += 0.00001f;
} }
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numMoves = positions.Count-1; if (numMoves > 0)
{
int i = positions.Last().Value[0];
int j = positions.Last().Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords); }
}
protected (Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAheadWithAccuracyWithAdditionalTheory(List<List<Vector3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords, float accuracyDefault, float accuracyRange = 0f)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
Piece oldPiece;
 272
 
SortedDictionary<float, int[]> positions = new SortedDictionary<float, int[]>(); float currentValue = 0;
float small = 0.00001f; // prevents same keys
for (int i = 0; i < movesCurrentlyAvailable.Count; i++)
{
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
(pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).nam e] + board.materialImbalance*0.05f);
currentValue += IntermediateTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
if (board.majorPiecesMoved < 50) {
currentValue += BasicOpeningTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
else if (board.majorPiecesTaken < 50) {
currentValue += BasicMiddleGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
currentValue += BasicEndGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
 273
 
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : (pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementA t(l))).name] +board.materialImbalance * 0.05f);
currentValue -= IntermediateTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
if (board.majorPiecesMoved < 50) {
currentValue -= BasicOpeningTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare,
opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l)); }
else if (board.majorPiecesTaken < 50) {
currentValue -= BasicMiddleGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
} else {
currentValue -= BasicEndGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
}
while (positions.ContainsKey(currentValue)){currentValue -= 0.00001f;}positions.Add(currentValue, new int[] { i, j });
small += 0.00001f; }
}
 274
 
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numMoves = positions.Count-1; if (numMoves > 0)
{
if (accuracyRange == 0) {
if (accuracyDefault > 0) {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)), positions.Count);
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
int indecies = rnd.Next(0, Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault))); int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} }
else {
if (accuracyDefault > 0) {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyRange)), Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)));
 275
 
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count * (-1 * accuracyRange)), Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} }
} else {
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords); }
}
protected (float,Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAheadWithNNAndReturnEval(List<List<Vector3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
Piece oldPiece;
Piece oldOPiece;
 276
 
SortedDictionary<float, int[]> positions = new SortedDictionary<float, int[]>(); float currentValue = 0;
float small = 0.00001f; // prevents same keys
for (int i = 0; i < movesCurrentlyAvailable.Count; i++)
{
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
currentValue += GetNNEvalFromBoardState(board.grid); if (board.majorPiecesMoved < 50)
{
currentValue += BasicOpeningTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
else if (board.majorPiecesTaken < 50) {
currentValue += BasicMiddleGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
currentValue += BasicEndGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++)
 277
 
{
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
oldOPiece = board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l));
board.UpdateBoardOnPieceMove(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l), opposingPieceCoords.ElementAt(k), opposingPieces.ElementAt(k), null);
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt (l))).name];
currentValue += GetNNEvalFromBoardState(board.grid);
board.UpdateBoardOnPieceMove(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l), opposingPieceCoords.ElementAt(k), oldOPiece, opposingPieces.ElementAt(k));
if (board.majorPiecesMoved < 50) {
currentValue -= BasicOpeningTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare,
opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l)); }
else if (board.majorPiecesTaken < 50) {
currentValue -= BasicMiddleGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
} else {
currentValue -= BasicEndGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
}
while (positions.ContainsKey(currentValue)){currentValue -= 0.00001f;}positions.Add(currentValue, new int[] { i, j });
 278
 
small += 0.00001f; }
}
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numMoves = positions.Count-1; if (numMoves > 0)
{
int i = positions.Last().Value[0];
int j = positions.Last().Value[1];
return (positions.Last().Key, pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
(Vector3Int, Vector3Int) tempHolder = GetMoveRandom(movesCurrentlyAvailable, pieceCoords);
return (-1f, tempHolder.Item1,tempHolder.Item2); }
}
protected (float, Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAheadWithAccuracyAndReturnEval(List<List<Vector3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords, float accuracyDefault, float accuracyRange = 0f)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
Piece oldPiece;
 279
 
SortedDictionary<float, int[]> positions = new SortedDictionary<float, int[]>(); float currentValue = 0;
float small = 0.00001f; // prevents same keys
for (int i = 0; i < movesCurrentlyAvailable.Count; i++)
{
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
currentValue += BasicTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i), board.GetPieceOnSquare(pieceCoords.ElementAt(i)) == null ? true : board.GetPieceOnSquare(pieceCoords.ElementAt(i)).team == TeamColor.White ? true : false);
if (board.majorPiecesMoved < 50) {
currentValue += BasicOpeningTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
else if (board.majorPiecesTaken < 50) {
currentValue += BasicMiddleGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
currentValue += BasicEndGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
 280
 
}
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt (l))).name];
currentValue -= BasicTheoryAddition(opposingPieceCoords.ElementAt(k), opposingMovesCurrentlyAvailable.ElementAt(k), board.GetPieceOnSquare(pieceCoords.ElementAt(i)) == null ? true : board.GetPieceOnSquare(pieceCoords.ElementAt(i)).team == TeamColor.White ? true : false);
if (board.majorPiecesMoved < 50) {
currentValue -= BasicOpeningTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare,
opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l)); }
else if (board.majorPiecesTaken < 50) {
currentValue -= BasicMiddleGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
} else {
currentValue -= BasicEndGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
}
while (positions.ContainsKey(currentValue)) { currentValue -= 0.00001f; } positions.Add(currentValue, new int[] { i, j });
small += 0.00001f;
} }
 281
 
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numMoves = positions.Count-1; if (numMoves > 0)
{
if (accuracyRange == 0) {
if (accuracyDefault > 0) {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)), positions.Count);
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (positions.ElementAt(indecies).Key, pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
int indecies = rnd.Next(0, Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (positions.ElementAt(indecies).Key, pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} }
else {
if (accuracyDefault > 0) {
 282
 
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyRange)), Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (positions.ElementAt(indecies).Key, pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count * (-1 * accuracyRange)), Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (positions.ElementAt(indecies).Key, pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} }
} else {
(Vector3Int, Vector3Int) tempHolder = GetMoveRandom(movesCurrentlyAvailable, pieceCoords);
return (-1f, tempHolder.Item1, tempHolder.Item2); }
}
protected (Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAheadWithAccuracyAndDeeperLineEvaluationWithAdditionalTheory(List<List <Vector3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords, float accuracyDefault, float accuracyRange = 0f)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
 283
 
Piece oldPiece;
SortedDictionary<float, int[]> positions = new SortedDictionary<float, int[]>(); List<(Vector3Int, Vector3Int)> furtherPositions = new List<(Vector3Int, Vector3Int)>(); float currentValue = 0;
float small = 0.00001f; // prevents same keys
for (int i = 0; i < movesCurrentlyAvailable.Count; i++) {
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
(pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).nam e] + board.materialImbalance * 0.05f);
currentValue += IntermediateTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
if(board.majorPiecesMoved < 50) {
currentValue += BasicOpeningTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
else if(board.majorPiecesTaken < 50) {
currentValue += BasicMiddleGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else
 284
 
{
currentValue += BasicEndGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : (pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementA t(l))).name] + board.materialImbalance * 0.05f);
currentValue -= IntermediateTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
if (board.majorPiecesMoved < 50) {
currentValue -= BasicOpeningTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare,
opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l)); }
else if (board.majorPiecesTaken < 50) {
currentValue -= BasicMiddleGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
} else {
currentValue -= BasicEndGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
}
while (positions.ContainsKey(currentValue)){currentValue -= 0.00001f;}positions.Add(currentValue, new int[] { i, j });
small += 0.00001f;
 285
 
} }
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numMoves = positions.Count-1; if (numMoves > 0)
{
if (accuracyRange == 0) {
if (accuracyDefault > 0) {
for (int a = 0; a < 3; a++) {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)), positions.Count);
int i = positions.ElementAt(indecies - 1).Value[0]; int j = positions.ElementAt(indecies - 1).Value[1]; furtherPositions.Add((pieceCoords.ElementAt(i),
movesCurrentlyAvailable.ElementAt(i).ElementAt(j))); }
} else {
for (int a = 0; a < 3; a++) {
int indecies = rnd.Next(0, Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
furtherPositions.Add((pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j)));
 286
 
}
} }
else {
if (accuracyDefault > 0) {
for (int a = 0; a < 3; a++) {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyRange)), Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0]; int j = positions.ElementAt(indecies - 1).Value[1]; furtherPositions.Add((pieceCoords.ElementAt(i),
movesCurrentlyAvailable.ElementAt(i).ElementAt(j))); }
} else {
for (int a = 0; a < 3; a++) {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count * (-1 * accuracyRange)), Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0]; int j = positions.ElementAt(indecies - 1).Value[1]; furtherPositions.Add((pieceCoords.ElementAt(i),
movesCurrentlyAvailable.ElementAt(i).ElementAt(j))); }
 287
 
} }
List<(float, Vector3Int, Vector3Int)> furtherPositionsWithEval = new List<(float, Vector3Int, Vector3Int)>();
for (int a = 0; a < 3; a++) {
List<List<Vector3Int>> furtherTempMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Vector3Int> miniFurtherTempMovesCurrentlylAvailable = new List<Vector3Int>(); List<Piece> furtherTempPieces = new List<Piece>();
List<Vector3Int> furtherTempPieceCoords = new List<Vector3Int>();
miniFurtherTempMovesCurrentlylAvailable.Add(furtherPositions.ElementAt(a).Item2); furtherTempMovesCurrentlyAvailable.Add(miniFurtherTempMovesCurrentlylAvailable); furtherTempPieces.Add(board.GetPieceOnSquare(furtherPositions.ElementAt(a).Item1)); furtherTempPieceCoords.Add(furtherPositions.ElementAt(a).Item1);
(Vector3Int, Vector3Int) halfWayMoveChosen = GetMoveMostValueFrom2LookAheadWithAccuracy(furtherTempMovesCurrentlyAvailable, furtherTempPieces, furtherTempPieceCoords, 0.15f);
miniFurtherTempMovesCurrentlylAvailable.Clear(); furtherTempMovesCurrentlyAvailable.Clear();
furtherTempPieces.Clear();
furtherTempPieceCoords.Clear(); miniFurtherTempMovesCurrentlylAvailable.Add(halfWayMoveChosen.Item2); furtherTempMovesCurrentlyAvailable.Add(miniFurtherTempMovesCurrentlylAvailable); furtherTempPieces.Add(board.GetPieceOnSquare(halfWayMoveChosen.Item1)); furtherTempPieceCoords.Add(halfWayMoveChosen.Item1);
furtherPositionsWithEval.Add(GetMoveMostValueFrom2LookAheadWithAccuracyAndReturnEval(furtherTem pMovesCurrentlyAvailable, furtherTempPieces, furtherTempPieceCoords, 0.15f));
 288
 
}
float move1 = furtherPositionsWithEval.ElementAt(0).Item1; float move2 = furtherPositionsWithEval.ElementAt(1).Item1; float move3 = furtherPositionsWithEval.ElementAt(2).Item1; if (move1 > move2)
{
if (move1 > move3) {
return (furtherPositionsWithEval.ElementAt(0).Item2, furtherPositionsWithEval.ElementAt(0).Item3);
} else {
return (furtherPositionsWithEval.ElementAt(2).Item2, furtherPositionsWithEval.ElementAt(2).Item3);
} }
else {
if (move2 > move3) {
return (furtherPositionsWithEval.ElementAt(1).Item2, furtherPositionsWithEval.ElementAt(1).Item3);
} else {
return (furtherPositionsWithEval.ElementAt(2).Item2, furtherPositionsWithEval.ElementAt(2).Item3);
} }
} else
 289
 
{
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords);
}
}
protected (Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAheadAndDeeperLineEvaluationWithPositionalNeuralNetwork(List<List<Vect or3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
Piece oldPiece;
Piece oldOPiece;
SortedDictionary<float, int[]> positions = new SortedDictionary<float, int[]>(); List<(Vector3Int, Vector3Int)> furtherPositions = new List<(Vector3Int, Vector3Int)>(); float currentValue = 0;
float small = 0.00001f; // prevents same keys
for (int i = 0; i < movesCurrentlyAvailable.Count; i++) {
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
 290
 
(pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).nam e] + board.materialImbalance * 0.05f);
currentValue += GetNNEvalFromBoardState(board.grid); if (board.majorPiecesMoved < 50)
{
currentValue += BasicOpeningTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
else if (board.majorPiecesTaken < 50) {
currentValue += BasicMiddleGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
currentValue += BasicEndGameTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
oldOPiece = board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l));
board.UpdateBoardOnPieceMove(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l), opposingPieceCoords.ElementAt(k), opposingPieces.ElementAt(k), null);
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt (l))).name];
currentValue += GetNNEvalFromBoardState(board.grid);
board.UpdateBoardOnPieceMove(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l), opposingPieceCoords.ElementAt(k), oldOPiece, opposingPieces.ElementAt(k));
 291
 
if (board.majorPiecesMoved < 50) {
currentValue -= BasicOpeningTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare,
opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l)); }
else if (board.majorPiecesTaken < 50) {
currentValue -= BasicMiddleGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
} else {
currentValue -= BasicEndGameTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves.ElementAt(l));
}
while (positions.ContainsKey(currentValue)) { currentValue -= 0.00001f; } positions.Add(currentValue, new int[] { i, j });
small += 0.00001f;
} }
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numMoves = positions.Count-1; if (numMoves > 0)
{
for (int a = 0; a < 5; a++) {
int i = positions.Last().Value[0];
 292
 
int j = positions.Last().Value[1];
furtherPositions.Add((pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j)));
}
List<(float, Vector3Int, Vector3Int)> furtherPositionsWithEval = new List<(float, Vector3Int, Vector3Int)>();
for (int a = 0; a < 5; a++) {
List<List<Vector3Int>> furtherTempMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Vector3Int> miniFurtherTempMovesCurrentlylAvailable = new List<Vector3Int>(); List<Piece> furtherTempPieces = new List<Piece>();
List<Vector3Int> furtherTempPieceCoords = new List<Vector3Int>();
miniFurtherTempMovesCurrentlylAvailable.Add(furtherPositions.ElementAt(a).Item2); furtherTempMovesCurrentlyAvailable.Add(miniFurtherTempMovesCurrentlylAvailable); furtherTempPieces.Add(board.GetPieceOnSquare(furtherPositions.ElementAt(a).Item1)); furtherTempPieceCoords.Add(furtherPositions.ElementAt(a).Item1);
(Vector3Int, Vector3Int) halfWayMoveChosen = GetMoveMostValueFrom2LookAheadWithNN(furtherTempMovesCurrentlyAvailable, furtherTempPieces, furtherTempPieceCoords);
miniFurtherTempMovesCurrentlylAvailable.Clear(); furtherTempMovesCurrentlyAvailable.Clear();
furtherTempPieces.Clear();
furtherTempPieceCoords.Clear(); miniFurtherTempMovesCurrentlylAvailable.Add(halfWayMoveChosen.Item2); furtherTempMovesCurrentlyAvailable.Add(miniFurtherTempMovesCurrentlylAvailable); furtherTempPieces.Add(board.GetPieceOnSquare(halfWayMoveChosen.Item1)); furtherTempPieceCoords.Add(halfWayMoveChosen.Item1);
 293
 
furtherPositionsWithEval.Add(GetMoveMostValueFrom2LookAheadWithNNAndReturnEval(furtherTempMov esCurrentlyAvailable, furtherTempPieces, furtherTempPieceCoords));
}
int maxIndex = 0;
float max = 0;
for (int i = 0; i < 5; i++) {
if(furtherPositionsWithEval.ElementAt(i).Item1 > max) {
maxIndex = i; }
}
return (furtherPositionsWithEval.ElementAt(maxIndex).Item2, furtherPositionsWithEval.ElementAt(maxIndex).Item3);
} else {
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords); }
}
private float GetNNEvalFromBoardState(Piece[,,] grid) {
float[] inputs = new float[512*7*2]; int counter = 0;
for (int i = 0; i < 8; i++)
{
for (int j = 0; j < 8; j++) {
 294
 
for (int k = 0; k < 8; k++) {
if (grid[i,j,k] != null) {
if(grid[i,j,k].team == TeamColor.White) {
if (grid[i, j, k].name == "Pawn" || grid[i, j, k].name == "Pawn(Clone)") {
inputs[counter] = 1; }
else if (grid[i, j, k].name == "Knight" || grid[i, j, k].name == "Knight(Clone)") {
inputs[counter+1] = 1; }
else if (grid[i, j, k].name == "Bishop" || grid[i, j, k].name == "Bishop(Clone)") {
inputs[counter+2] = 1; }
else if (grid[i,j,k].name == "Rook" || grid[i, j, k].name == "Rook(Clone)") {
inputs[counter+3] = 1; }
else if (grid[i,j,k].name == "Commoner" || grid[i, j, k].name == "Commoner(Clone)") {
inputs[counter+4] = 1; }
else if (grid[i, j, k].name == "King" || grid[i, j, k].name == "King(Clone)") {
inputs[counter+5] = 1; }
 295
 
else if (grid[i, j, k].name == "Queen" || grid[i, j, k].name == "Queen(Clone)") {
inputs[counter+6] = 1; }
counter += 14; }
else {
counter += 7;
if (grid[i, j, k].name == "Pawn" || grid[i, j, k].name == "Pawn(Clone)") {
inputs[counter] = 1; }
else if (grid[i, j, k].name == "Knight" || grid[i, j, k].name == "Knight(Clone)") {
inputs[counter + 1] = 1; }
else if (grid[i, j, k].name == "Bishop" || grid[i, j, k].name == "Bishop(Clone)") {
inputs[counter + 2] = 1; }
else if (grid[i, j, k].name == "Rook" || grid[i, j, k].name == "Rook(Clone)") {
inputs[counter + 3] = 1; }
else if (grid[i, j, k].name == "Commoner" || grid[i, j, k].name == "Commoner(Clone)") {
inputs[counter + 4] = 1; }
else if (grid[i, j, k].name == "King" || grid[i, j, k].name == "King(Clone)")
 296
 
{
inputs[counter + 5] = 1;
}
else if (grid[i, j, k].name == "Queen" || grid[i, j, k].name == "Queen(Clone)") {
inputs[counter + 6] = 1; }
counter += 7; }
} else {
counter += 14; }
} }
}
return networkManager.FeedForward(inputs)[0];//it is okay to do this as we know there is only one output state (the eval)
}
protected (Vector3Int, Vector3Int) GetMoveMostValueFrom2LookAheadWithAccuracyAndLargeMoveNegativityMitigation(List<List<Vector3Int> > movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords, float accuracyDefault, float accuracyRange = 0f)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
Piece oldPiece;
 297
 
SortedDictionary<float, int[]> positions = new SortedDictionary<float, int[]>(); float currentValue = 0;
float small = 0.00001f; // prevents same keys
for (int i = 0; i < movesCurrentlyAvailable.Count; i++)
{
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++) {
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
currentValue += BasicTheoryAddition(pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i), board.GetPieceOnSquare(pieceCoords.ElementAt(i)) == null ? true : board.GetPieceOnSquare(pieceCoords.ElementAt(i)).team == TeamColor.White ? true : false);
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt (l))).name];
currentValue -= BasicTheoryAddition(opposingPieces.ElementAt(k).occupiedSquare, opposingPieces.ElementAt(k).avaliableMoves, board.GetPieceOnSquare(opposingPieceCoords.ElementAt(i)) == null ? true : board.GetPieceOnSquare(opposingPieceCoords.ElementAt(i)).team == TeamColor.White ? false : true);
while (positions.ContainsKey(currentValue)){currentValue -= 0.00001f;}positions.Add(currentValue, new int[] { i, j });
small += 0.00001f; }
}
 298
 
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numMoves = positions.Count-1; if (numMoves > 0)
{
if (accuracyRange == 0) {
if (accuracyDefault > 0) {
if(positions.ElementAt(Mathf.FloorToInt(positions.Count - Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)))/2).Key < 0)
{
// no need to create an additional list to only add values in the accuracy range as the above takes care of this at the same time as the 50% negative implementation discussed in 4.a.ii.7 in objectives
// this is because we know that the first negative value must lie within this range SortedDictionary<float, int[]> positivePositions = new SortedDictionary<float, int[]>(); foreach(KeyValuePair<float, int[]> item in positions)
{
if(item.Key >= 0) {
positivePositions.Add(item.Key,item.Value); }
}
int indecies = rnd.Next(0,positivePositions.Count);
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
 299
 
else {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)), positions.Count);
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
}
} else {
int indecies = rnd.Next(0, Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault))); int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} }
else {
if (accuracyDefault > 0) {
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count - (positions.Count * accuracyRange)), Mathf.FloorToInt(positions.Count - (positions.Count * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} else {
 300
 
int indecies = rnd.Next(Mathf.FloorToInt(positions.Count * (-1 * accuracyRange)), Mathf.FloorToInt(positions.Count * (-1 * accuracyDefault)));
int i = positions.ElementAt(indecies - 1).Value[0];
int j = positions.ElementAt(indecies - 1).Value[1];
return (pieceCoords.ElementAt(i), movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
} }
} else {
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords); }
}
protected (Vector3Int, Vector3Int) GetMoveByAlphaBetaPruningWithMovePriorityOrdering(List<List<Vector3Int>> movesCurrentlyAvailable, List<Piece> pieces, List<Vector3Int> pieceCoords, int runNum=0)
{
List<List<Vector3Int>> opposingMovesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> opposingPieces = new List<Piece>();
List<Vector3Int> opposingPieceCoords = new List<Vector3Int>();
Piece oldPiece;
List<int[]> maxValuePositions = new List<int[]>(); int alpha = 0;
int beta = 9;
int currentValue = 0;
for (int i = 0; i < movesCurrentlyAvailable.Count; i++) {
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count; j++)
 301
 
{
oldPiece = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j));
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), pieces.ElementAt(i), null);
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = opposingPlayer.ReturnAllPossibleMoves();
(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords) = GiveMovesPriorityOrder(opposingMovesCurrentlyAvailable, opposingPieces, opposingPieceCoords, movesCurrentlyAvailable);
currentValue = board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) == null ? 0 :
pieceNameToValueDict[(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j))).name ];
string pieceName = pieces.ElementAt(i).name;
if (pieceName == "Pawn" || pieceName == "Pawn(Clone)") {
currentValue += 9; }
for (int k = 0; k < opposingMovesCurrentlyAvailable.Count; k++) {
for (int l = 0; l < opposingMovesCurrentlyAvailable.ElementAt(k).Count; l++) {
currentValue -= board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt(l)) == null ? 0 : pieceNameToValueDict[(board.GetPieceOnSquare(opposingMovesCurrentlyAvailable.ElementAt(k).ElementAt (l))).name];
if (pieceName == "Pawn" || pieceName == "Pawn(Clone)") {
currentValue -= 9; }
if (currentValue < beta) {
break; }
 302
 
if (currentValue > alpha) {
maxValuePositions.Clear();
maxValuePositions.Add(new int[] { i, j }); }
else if (currentValue == alpha) {
maxValuePositions.Add(new int[] { i, j }); }
}
if (currentValue < beta) {
break; }
}
board.UpdateBoardOnPieceMove(movesCurrentlyAvailable.ElementAt(i).ElementAt(j), pieceCoords.ElementAt(i), oldPiece, pieces.ElementAt(i));
} }
int numBestMoves = maxValuePositions.Count; if (numBestMoves > 0)
{
int val = Guid.NewGuid().GetHashCode() % numBestMoves;
val = val > 0 ? val : -val;
return (pieceCoords.ElementAt(maxValuePositions.ElementAt(val)[0]), movesCurrentlyAvailable.ElementAt(maxValuePositions.ElementAt(val)[0]).ElementAt(maxValuePositions.Elem entAt(val)[1]));
} else {
return GetMoveRandom(movesCurrentlyAvailable, pieceCoords);
 303
 
} }
protected (List<List<Vector3Int>>, List<Piece>, List<Vector3Int>) GiveMovesPriorityOrder(List<List<Vector3Int>> moves, List<Piece> pieces, List<Vector3Int> positions, List<List<Vector3Int>> opposingAttacks)
{
List<List<Vector3Int>> orderedMoves = new List<List<Vector3Int>>(); List<Vector3Int> orderedPositions = new List<Vector3Int>(); List<Piece> orderedPieces = new List<Piece>();
Piece capturedPiece;
string capturedPieceName;
string pieceName;
int estimatedMoveReward = 0;
Vector3Int moveToSquare;
Dictionary<(Vector3Int, Piece, Vector3Int), int> findOrderDict = new Dictionary<(Vector3Int, Piece, Vector3Int), int>();
int totalMoves = 0;
for (int i = 0; i < moves.Count; i++) {
for (int j = 0; j < moves.ElementAt(i).Count; j++) {
moveToSquare = moves.ElementAt(i).ElementAt(j);
pieceName = pieces.ElementAt(i).name;
capturedPiece = board.GetPieceOnSquare(moveToSquare); capturedPieceName = (capturedPiece == null ? "" : capturedPiece.name); estimatedMoveReward = 0;
if (capturedPieceName != "") {
 304
 
estimatedMoveReward += 10 * pieceNameToValueDict[capturedPieceName] - pieceNameToValueDict[pieceName];
}
if ((pieceName == "Pawn" || pieceName == "Pawn(Clone") && moveToSquare.y == 7) {
estimatedMoveReward += 9; }
for (int k = 0; k < opposingAttacks.Count; k++) {
for (int l = 0; l < opposingAttacks.ElementAt(k).Count; l++) {
if (moveToSquare == opposingAttacks.ElementAt(k).ElementAt(l)) {
estimatedMoveReward -= pieceNameToValueDict[pieceName]; }
} }
totalMoves++;
findOrderDict.Add((moveToSquare, pieces.ElementAt(i), positions.ElementAt(i)), estimatedMoveReward);
} }
for (int i = 18; i > -10; i--) {
var matchingKeys = findOrderDict.Where(kvp => kvp.Value == i).Select(kvp => kvp.Key); foreach((Vector3Int, Piece, Vector3Int) key in matchingKeys)
{
List<Vector3Int> move = new List<Vector3Int>() { key.Item1 }; orderedMoves.Add(move);
 305
 
orderedPieces.Add(key.Item2); orderedPositions.Add(key.Item3);
} }
return (orderedMoves, orderedPieces, orderedPositions);
}
protected float[] SetupState(List<List<Vector3Int>> movesCurrentlyAvailable, List<Piece> pieceNames, List<Vector3Int> pieceCoords)
{
throw new NotImplementedException();
}
protected (Vector3Int coordsOfPieceToMove, Vector3Int coordsToMoveTo) DecodeInputState(object indexOfMove)
{
throw new NotImplementedException();
}
public int GetNumMovesMade() {
return movesMade; }
public void IncreaseNumMovesMade() {
movesMade++; }
}