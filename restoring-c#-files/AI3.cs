using
using
using
System.Collections; System.Collections.Generic; UnityEngine;
public class AI3 : AIManager {
 252
 
public AI3(ChessPlayer currentAiPlayer, ChessPlayer opposingPlayer, Board board) {
this.currentAiPlayer = currentAiPlayer; this.board = board;
this.opposingPlayer = opposingPlayer;
}
public override void MoveMakerController(ChessPlayer aiPlayer, ChessPlayer opposingPlayer)
{
NewAiPlayers(aiPlayer, opposingPlayer);
List<List<Vector3Int>> movesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> pieces = new List<Piece>();
List<Vector3Int> pieceCoords = new List<Vector3Int>(); (movesCurrentlyAvailable, pieces, pieceCoords) =
currentAiPlayer.ReturnAllPossibleMoves();
Vector3Int coordsOfPieceToMove = Vector3Int.zero; Vector3Int coordsToMoveTo = Vector3Int.zero;
int randomVal = rnd.Next(0, 100);
if (randomVal < 2)//2% {
(coordsOfPieceToMove, coordsToMoveTo) = GetMoveMostValueFrom2LookAheadWithAccuracy(movesCurrentlyAvailable, pieces, pieceCoords, -0.15f);
Debug.Log("Blunder"); }
else if (randomVal < 7)//2+5=7 {
(coordsOfPieceToMove, coordsToMoveTo) = GetMoveMostValueFrom2LookAheadWithAccuracy(movesCurrentlyAvailable, pieces, pieceCoords, -0.5f, -0.25f);
Debug.Log("Innacuracy"); }
else
{
if(rnd.Next(2) == 1) {
(coordsOfPieceToMove, coordsToMoveTo) = GetMoveMostValueFrom2LookAheadWithAccuracyWithAdditionalTheory(movesCurrentlyAvailable , pieces, pieceCoords, 0.15f);
}
else
{
(coordsOfPieceToMove, coordsToMoveTo) =
GetMoveMostValueFrom2LookAheadWithAccuracyAndDeeperLineEvaluationWithAdditionalTheory( movesCurrentlyAvailable, pieces, pieceCoords, 0.15f);
} }
board.AIMakeMove(coordsOfPieceToMove, coordsToMoveTo);
} }