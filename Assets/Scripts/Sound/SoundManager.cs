using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
[SerializeField] private GameObject muteBG;
[SerializeField] private GameObject unmuteBG;
[SerializeField] private GameObject muteSF;
[SerializeField] private GameObject unmuteSF;
public void ActivateMuteBG()
{
muteBG.SetActive(true);
unmuteBG.SetActive(false);
}
public void ActivateUnmuteBG()
{
muteBG.SetActive(false);
unmuteBG.SetActive(true);
}
public void ActivateMuteSF()
{
muteSF.SetActive(true);
unmuteSF.SetActive(false);
}
public void ActivateUnmuteSF()
{
muteSF.SetActive(false);
unmuteSF.SetActive(true);
}
}