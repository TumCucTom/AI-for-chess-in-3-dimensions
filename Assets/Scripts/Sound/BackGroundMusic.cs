using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BackGroundMusic : MonoBehaviour
{
private bool mute = false;
public void MuteSound()
{
if (mute == false)
{
AudioListener.volume = 0;
mute = true;
}
else
{
AudioListener.volume = 0.3f;
mute = false;
}
}
}



SOUND MANAGER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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




402
TWEENERS

INSTANT TWEENER
using System.Collections;