using System.Collections;
using System.Collections.Generic; using UnityEngine;
public class AI1 : AIManager {
    public AI1 (ChessPlayer currentAiPlayer, ChessPlayer opposingPlayer, Board board) {
        this.currentAiPlayer = currentAiPlayer; this.board = board;
        this.opposingPlayer = opposingPlayer;
    }
    public override void MoveMakerController(ChessPlayer aiPlayer, ChessPlayer opposingPlayer) {
        
        NewAiPlayers(aiPlayer, opposingPlayer);
        List<List<Vector3Int>> movesCurrentlyAvailable = new List<List<Vector3Int>>(); List<Piece> pieces = new List<Piece>();
        List<Vector3Int> pieceCoords = new List<Vector3Int>(); (movesCurrentlyAvailable, pieces, pieceCoords) =
        currentAiPlayer.ReturnAllPossibleMoves();
        Vector3Int coordsOfPieceToMove; Vector3Int coordsToMoveTo;

        if(rnd.Next(1,11) == 5) {
            (coordsOfPieceToMove, coordsToMoveTo) = GetMoveFrom1LookAheadWithAccuracy(movesCurrentlyAvailable, pieceCoords, -0.1f);
            Debug.Log("Blunder"); }
        else if (rnd.Next(1, 3) ==1 ) {
            (coordsOfPieceToMove, coordsToMoveTo) = GetMoveFrom1LookAheadWithAccuracy(movesCurrentlyAvailable, pieceCoords,0.25f);
        }
        else if(rnd.Next(1,6) == 5) {
            (coordsOfPieceToMove, coordsToMoveTo) = GetMoveRandom(movesCurrentlyAvailable, pieceCoords);
            Debug.Log("Blunder"); }
        else
        {
            (coordsOfPieceToMove, coordsToMoveTo) =
            GetMoveMostValueFrom2LookAheadWithAccuracy(movesCurrentlyAvailable,pieces, pieceCoords, 0.25f);
        }

        board.AIMakeMove(coordsOfPieceToMove, coordsToMoveTo);
    } 
}