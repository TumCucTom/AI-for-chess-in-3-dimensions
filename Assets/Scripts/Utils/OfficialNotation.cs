using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OfficialNotation
{
string moveNotation;
public void CreateNotation(Piece piece, Vector3Int atCoords, Vector3Int toCoords,
bool take, bool castle = false)
{
string pieceChar = piece.name[0].ToString();
Debug.Log(pieceChar);
string start = (atCoords.z.ToString() + (char)(65 + (atCoords.y)) +
atCoords.x.ToString());
string finish = (toCoords.z.ToString() + (char)(65 + (atCoords.y)) +
atCoords.x.ToString());
string combiation = take ? "x" : " ";
moveNotation = (pieceChar + start + combiation + finish);
if (castle) moveNotation = atCoords.z.ToString() + "O-O" +
toCoords.z.ToString();
}
public void DisplayNotation()
{
Debug.Log(moveNotation);
}
}
RANDOM HELPER – UNITY RANDOM FAILS DUE TO SEED ISSUES
The seed issues that arise form unity’s random mean AI makes same moves