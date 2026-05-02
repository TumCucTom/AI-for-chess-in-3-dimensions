using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Pawn : Piece
{
public override List<Vector3Int> SelectAvaliableSquares()
{
avaliableMoves.Clear();

Vector3Int direction = team == TeamColor.White ? new Vector3Int(0,1,0) : new
Vector3Int (0,-1,0);
float range = hasMoved ? 1 : 2;
for (int i = 1; i <= range; i++)
{
Vector3Int nextCoords = occupiedSquare + direction * i;
Piece piece = board.GetPieceOnSquare(nextCoords);
if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
break;
if (piece == null)
TryToAddMove(nextCoords);
else
break;




332
}

Vector3Int[] takeDirectionsWhite = new Vector3Int[]
{
new Vector3Int (1,1,1),
new Vector3Int (-1,1,1),
new Vector3Int (1,1,-1),
new Vector3Int (-1,1,-1)
};

Vector3Int[] takeDirectionsBlack = new Vector3Int[]
{
new Vector3Int (1,-1,1),
new Vector3Int (-1,-1,1),
new Vector3Int (1,-1,-1),
new Vector3Int (-1,-1,-1)
};

for (int i = 0; i < 4; i++)
{
Vector3Int nextCoords = team == TeamColor.White? (occupiedSquare +
takeDirectionsWhite[i]) : (occupiedSquare + takeDirectionsBlack[i]);
Piece piece = board.GetPieceOnSquare(nextCoords);
if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
continue;
if (piece != null && !piece.IsFromSameTeam(this))
{
TryToAddMove(nextCoords);
}
}
return avaliableMoves;
}

public override void MovePiece(Vector3Int coords,Piece piece = null, bool isPawn =
false)
{
base.MovePiece(coords,null,true) ;
CheckPromotion();
}

private void CheckPromotion()
{
int endOfBoardYCoord = team == TeamColor.White ? Board.BOARD_SIZE - 1 : 0;
if (occupiedSquare.y == endOfBoardYCoord)
{
board.PromotePiece(this);
}
}
}


KNIGHT



using System.Collections;