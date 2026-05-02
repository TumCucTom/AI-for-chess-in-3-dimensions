using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class King : Piece
{
Vector3Int[] directions = new Vector3Int[]
{
new Vector3Int(0,0,-1),
new Vector3Int(-1,-1,-1),
new Vector3Int(-1,0,-1),
new Vector3Int(-1,1,-1),
new Vector3Int(0,-1,-1),
new Vector3Int(0,1,-1),
new Vector3Int(1,-1,-1),
new Vector3Int(1,0,-1),
new Vector3Int(1,1,-1),




338
new Vector3Int(-1,-1,1),
new Vector3Int(-1,0,1),
new Vector3Int(-1,1,1),
new Vector3Int(0,-1,1),
new Vector3Int(0,1,1),
new Vector3Int(1,-1,1),
new Vector3Int(1,0,1),
new Vector3Int(1,1,1),
new Vector3Int(0,0,1),

new Vector3Int(-1,-1,0),
new Vector3Int(-1,0,0),
new Vector3Int(-1,1,0),
new Vector3Int(0,-1,0),
new Vector3Int(0,1,0),
new Vector3Int(1,-1,0),
new Vector3Int(1,0,0),
new Vector3Int(1,1,0),

};

private Piece leftRook;
private Piece rightRook;

private Vector3Int leftCastlingMove;
private Vector3Int rightCastlingMove;

public override List<Vector3Int> SelectAvaliableSquares()
{
avaliableMoves.Clear();
AssignStandardMoves();
AssignCastlingMoves();
return avaliableMoves;

}

private void AssignCastlingMoves()
{
leftCastlingMove = new Vector3Int(-1, -1, 0);
rightCastlingMove = new Vector3Int(-1, -1, 0);
if (!hasMoved)
{
leftRook = GetPieceInDirection<Rook>(team, new Vector3Int(-1, 0, 0));
if (leftRook && !leftRook.hasMoved)
{
leftCastlingMove = occupiedSquare + new Vector3Int(-1, 0, 0) * 2;
avaliableMoves.Add(leftCastlingMove);
}
rightRook = GetPieceInDirection<Rook>(team, new Vector3Int(1, 0, 0));
if (rightRook && !rightRook.hasMoved)
{
rightCastlingMove = occupiedSquare + new Vector3Int(1, 0, 0) * 2;
avaliableMoves.Add(rightCastlingMove);
}
}
}

private Piece GetPieceInDirection<T>(TeamColor team, Vector3Int direction)
{
for (int i = 1; i <= Board.BOARD_SIZE; i++)
{




339
Vector3Int nextCoords = occupiedSquare + direction * i;
Piece piece = board.GetPieceOnSquare(nextCoords);
if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
return null;
if (piece != null)
{
if (piece.team != team || !(piece is T))
return null;
else if (piece.team == team && piece is T)
return piece;
}
}
return null;
}

private void AssignStandardMoves()
{
float range = 1;
foreach (var direction in directions)
{
for (int i = 1; i <= range; i++)
{
Vector3Int nextCoords = occupiedSquare + direction * i;
Piece piece = board.GetPieceOnSquare(nextCoords);
if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
break;
if (piece == null)
TryToAddMove(nextCoords);
else if (!piece.IsFromSameTeam(this))
{
TryToAddMove(nextCoords);
break;
}
else if (piece.IsFromSameTeam(this))
break;
}
}
}

public override void MovePiece(Vector3Int coords, Piece piece = null, bool isPawn
= false)
{
base.MovePiece(coords);
if (coords == leftCastlingMove)
{
board.UpdateBoardOnPieceMove(coords + new Vector3Int(1,0,0),
leftRook.occupiedSquare, leftRook, null);
leftRook.MovePiece(coords + new Vector3Int(1,0,0));
}
else if (coords == rightCastlingMove)
{
board.UpdateBoardOnPieceMove(coords + new Vector3Int(-1, 0, 0),
rightRook.occupiedSquare, rightRook, null);
rightRook.MovePiece(coords + new Vector3Int(1, 0, 0));
}
}

}

BOARD

using System;




340
using System.IO;

using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI;

using UnityEditor;

using Unity;

