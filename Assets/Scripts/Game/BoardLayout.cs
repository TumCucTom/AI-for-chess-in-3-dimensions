using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BoardLayout : ScriptableObject
{
[Serializable]
private class BoardSquareSetup
{
public Vector3Int position;
public PieceType pieceType;
public TeamColor teamColor;
}

[SerializeField] private BoardSquareSetup[] boardSquares;

public int GetPiecesCount()
{
return boardSquares.Length;
}


public Vector3Int GetSquareCoordsAtIndex(int index)
{
return new Vector3Int(boardSquares[index].position.x - 1,
boardSquares[index].position.y - 1, boardSquares[index].position.z -1);
}
public string GetSquarePieceNameAtIndex(int index)
{
return boardSquares[index].pieceType.ToString();
}
public TeamColor GetSquareTeamColorAtIndex(int index)
{
return boardSquares[index].teamColor;
}




367
}



CHESS PLAYER
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;