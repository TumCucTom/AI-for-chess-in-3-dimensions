public class Commoner : Piece
{
private Vector3Int[] directions = new Vector3Int[]
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
public override List<Vector3Int> SelectAvaliableSquares()
{
avaliableMoves.Clear();
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
return avaliableMoves;
}
}