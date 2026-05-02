using System;
public class NeuralNetworkInstantiationFailed : Exception
{
public NeuralNetworkInstantiationFailed(string reason) : base(reason)
{

}
}

MATERIAL SETTER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialSetter : MonoBehaviour
{
[SerializeField] private MeshRenderer _meshRenderer;
private MeshRenderer meshRenderer
{
get
{
if (_meshRenderer == null)
_meshRenderer = GetComponent<MeshRenderer>();
return _meshRenderer;
}
}

public void SetSingleMaterial(Material material)
{
meshRenderer.material = material;
}
}



OFFICIAL NOTATION – UNSUED DURING GAMEPLAY (COVERED IN BOARD)
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




410
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

TIME HELPER
using System.Collections;
using System.Collections.Generic;