using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Knight : Piece
{
Vector3Int[] offsets = new Vector3Int[]
{




333
new Vector3Int(1,1,2),
new Vector3Int(1,1,-2),
new Vector3Int(1,-1,2),
new Vector3Int(-1,1,2),
new Vector3Int(-1,-1,2),
new Vector3Int(-1,1,-2),
new Vector3Int(1,-1,-2),
new Vector3Int(-1,-1,-2),

new Vector3Int(1,2,1),
new Vector3Int(1,2,-1),
new Vector3Int(1,-2,1),
new Vector3Int(-1,2,1),
new Vector3Int(-1,-2,1),
new Vector3Int(-1,2,-1),
new Vector3Int(1,-2,-1),
new Vector3Int(-1,-2,-1),

new Vector3Int(2,1,1),
new Vector3Int(2,1,-1),
new Vector3Int(2,-1,1),
new Vector3Int(-2,1,1),
new Vector3Int(-2,-1,1),
new Vector3Int(-2,1,-1),
new Vector3Int(2,-1,-1),
new Vector3Int(-2,-1,-1),
};

public override List<Vector3Int> SelectAvaliableSquares()
{
avaliableMoves.Clear();

for (int i = 0; i < offsets.Length; i++)
{
Vector3Int nextCoords = occupiedSquare + offsets[i];
Piece piece = board.GetPieceOnSquare(nextCoords);
if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
continue;
if (piece == null || !piece.IsFromSameTeam(this))
TryToAddMove(nextCoords);
}
return avaliableMoves;
}
}



BISHOP



using System.Collections;