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