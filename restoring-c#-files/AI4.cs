using System.Collections;
using System.Collections.Generic; using UnityEngine;
public class AI4 : AIManager
 253
 
{
public AI4 (ChessPlayer currentAiPlayer, ChessPlayer opposingPlayer, Board board) {
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
int randomVal = rnd.Next(0, 10000);
if (randomVal < 75)//0.75% {
(coordsOfPieceToMove, coordsToMoveTo) = GetMoveMostValueFrom2LookAheadWithAccuracy(movesCurrentlyAvailable, pieces, pieceCoords, -0.15f);
Debug.Log("Blunder"); }
else if (randomVal < 300)//3% {
(coordsOfPieceToMove, coordsToMoveTo) = GetMoveMostValueFrom2LookAheadWithAccuracy(movesCurrentlyAvailable, pieces, pieceCoords, -0.5f, -0.25f);
Debug.Log("Innacuracy"); }
else
{
(coordsOfPieceToMove, coordsToMoveTo) =
GetMoveMostValueFrom2LookAheadAndDeeperLineEvaluationWithPositionalNeuralNetwork(moves CurrentlyAvailable, pieces, pieceCoords);
}
board.AIMakeMove(coordsOfPieceToMove, coordsToMoveTo);
} }