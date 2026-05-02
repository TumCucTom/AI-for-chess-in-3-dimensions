using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChessPlayer
{
public TeamColor team { get; set; }
public PlayerType type { get; set; }
public Board board { get; set; }
public List<Piece> activePieces { get; private set; }

public ChessPlayer(TeamColor team, Board board, PlayerType type)
{
activePieces = new List<Piece>();
this.board = board;
this.team = team;
this.type = type;
}

public void AddPiece(Piece piece)
{
if (!activePieces.Contains(piece))
activePieces.Add(piece);
}

public void RemovePiece(Piece piece)
{
if (activePieces.Contains(piece))
activePieces.Remove(piece);
}

public void GenerateAllPossibleMoves()
{
foreach (var piece in activePieces)
{
if(board.HasPiece(piece))
piece.SelectAvaliableSquares();
}
}

public (List<List<Vector3Int>>, List<Piece>, List<Vector3Int>)
ReturnAllPossibleMoves()
{
List<List<Vector3Int>> movesCurrentlyAvailable = new
List<List<Vector3Int>>();
List<Piece> pieces = new List<Piece>();
List<Vector3Int> pieceCoords = new List<Vector3Int>();
for (int i=0;i<activePieces.Count;i++)
{
if ((activePieces[i].SelectAvaliableSquares()).Count != 0)
{
pieces.Add(activePieces[i]);
pieceCoords.Add(activePieces[i].occupiedSquare);




368
movesCurrentlyAvailable.Add(activePieces[i].SelectAvaliableSquares());
}
}
for (int i = 0; i < movesCurrentlyAvailable.Count; i++)
{
for (int j = 0; j < movesCurrentlyAvailable.ElementAt(i).Count;
j++)
{
string nameOfPiece;
if
(board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)) != null)
{
nameOfPiece =
board.GetPieceOnSquare(movesCurrentlyAvailable.ElementAt(i).ElementAt(j)).name;
if (nameOfPiece == "King" || nameOfPiece ==
"King(Clone)")
{

movesCurrentlyAvailable.ElementAt(i).RemoveAt(j);
}
}
}
}
return (movesCurrentlyAvailable, pieces, pieceCoords);
}

public Piece[] GetPieceAtackingOppositePiceOfType<T>() where T : Piece
{
return activePieces.Where(p => p.IsAttackingPieceOfType<T>()).ToArray();
}

public Piece[] GetPiecesOfType<T>() where T : Piece
{
return activePieces.Where(p => p is T).ToArray();
}

public void RemoveMovesEnablingAttackOnPieceOfType<T>(ChessPlayer opponent,
Piece selectedPiece) where T : Piece
{
List<Vector3Int> coordsToRemove = new List<Vector3Int>();

coordsToRemove.Clear();
foreach (var coords in selectedPiece.avaliableMoves)
{
Piece pieceOnCoords = board.GetPieceOnSquare(coords);
board.UpdateBoardOnPieceMove(coords,
selectedPiece.occupiedSquare, selectedPiece, null);
opponent.GenerateAllPossibleMoves();
if (opponent.CheckIfIsAttacingPiece<T>())
coordsToRemove.Add(coords);
board.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare,
coords, selectedPiece, pieceOnCoords);
}
foreach (var coords in coordsToRemove)
{
selectedPiece.avaliableMoves.Remove(coords);
}
}


internal bool CheckIfIsAttacingPiece<T>() where T : Piece




369
{
foreach (var piece in activePieces)
{
if (board.HasPiece(piece) && piece.IsAttackingPieceOfType<T>())
return true;
}
return false;
}

public bool CanHidePieceFromAttack<T>(ChessPlayer opponent) where T : Piece
{
foreach (var piece in activePieces)
{
foreach (var coords in piece.avaliableMoves)
{
Piece pieceOnCoords = board.GetPieceOnSquare(coords);
board.UpdateBoardOnPieceMove(coords, piece.occupiedSquare,
piece, null);
opponent.GenerateAllPossibleMoves();
if (!opponent.CheckIfIsAttacingPiece<T>())
{
board.UpdateBoardOnPieceMove(piece.occupiedSquare,
coords, piece, pieceOnCoords);
return true;
}
board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords,
piece, pieceOnCoords);
}
}
return false;
}

internal void OnGameRestarted()
{
activePieces.Clear();
}

public bool CheckForMoves()
{

if(activePieces.Count != 0)
{
foreach(Piece piece in activePieces)
{

if (piece.avaliableMoves.Count != 0)
{
return false;
}
}
}
return true;
}
}

PIECE (THE PIECE CLASS- PARENT TO ALL PIECE CLASSES)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



