public class InstantTweener : MonoBehaviour, IObjectTweener
{
public void MoveTo(Transform transform, Vector3 targetPosition)
{
transform.position = targetPosition;
}
}
I OBJECT TWEENER (INTERFACE)
using UnityEngine;