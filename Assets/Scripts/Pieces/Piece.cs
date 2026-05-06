using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class Piece : MonoBehaviour
{
[SerializeField] private MaterialSetter materialSetter;
public Board board { protected get; set; }
public Vector3Int occupiedSquare { get; set; }
public TeamColor team { get; set; }
public bool hasMoved { get; private set; }
public List<Vector3Int> avaliableMoves;
private IObjectTweener tweener;
public abstract List<Vector3Int> SelectAvaliableSquares();
private void Awake()
{
avaliableMoves = new List<Vector3Int>();
tweener = GetComponent<IObjectTweener>();
materialSetter = GetComponent<MaterialSetter>();
hasMoved = false;
}
public void SetMaterial(Material selectedMaterial)
{
materialSetter.SetSingleMaterial(selectedMaterial);
}
public bool IsFromSameTeam(Piece piece)
{
return team == piece.team;
}
public bool CanMoveTo(Vector3Int coords)
{
return avaliableMoves.Contains(coords);
}
public virtual void MovePiece(Vector3Int coords, Piece piece = null, bool
isPawn = false)
{
Vector3 targetPosition = board.CalculatePositionFromCoords(coords);
occupiedSquare = coords;
hasMoved = true;
tweener.MoveTo(transform, targetPosition);
}
protected void TryToAddMove(Vector3Int coords)
{
avaliableMoves.Add(coords);
}
public void SetData(Vector3Int coords, TeamColor team, Board board)
{
this.team = team;
occupiedSquare = coords;
this.board = board;
transform.position = board.CalculatePositionFromCoords(coords);
}
public bool IsAttackingPieceOfType<T>() where T : Piece
{
foreach (var square in avaliableMoves)
{
if (board.GetPieceOnSquare(square) is T)
return true;
}
return false;
}
}