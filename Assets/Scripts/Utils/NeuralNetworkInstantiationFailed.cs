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