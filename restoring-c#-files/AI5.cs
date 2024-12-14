using System.Collections;
using System.Collections.Generic; using UnityEngine;
public class AI5 : AIManager {
public AI5 (ChessPlayer currentAiPlayer, ChessPlayer opposingPlayer, Board board) {
this.currentAiPlayer = currentAiPlayer; this.board = board;
this.opposingPlayer = opposingPlayer;
}
 254
 
public override void MoveMakerController(ChessPlayer aiPlayer, ChessPlayer opposingPlayer)
{
NewAiPlayers(aiPlayer, opposingPlayer);
List<List<Vector3Int>> movesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> pieces = new List<Piece>();
List<Vector3Int> pieceCoords = new List<Vector3Int>(); (movesCurrentlyAvailable, pieces, pieceCoords) =
currentAiPlayer.ReturnAllPossibleMoves();
Vector3Int coordsOfPieceToMove = Vector3Int.zero; Vector3Int coordsToMoveTo = Vector3Int.zero; (coordsOfPieceToMove, coordsToMoveTo) =
GetMoveMostValueFrom2LookAheadAndDeeperLineEvaluationWithPositionalNeuralNetwork(moves CurrentlyAvailable, pieces, pieceCoords);
board.AIMakeMove(coordsOfPieceToMove, coordsToMoveTo);
} }