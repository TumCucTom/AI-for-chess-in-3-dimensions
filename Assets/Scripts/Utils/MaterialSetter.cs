using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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