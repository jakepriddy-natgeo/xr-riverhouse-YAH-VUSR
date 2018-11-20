using UnityEngine;

public class ModdedChoiceHotspot : MonoBehaviour
{
	public string SceneToLoad;

	public void Click()
	{
		if (!string.IsNullOrEmpty(SceneToLoad))
		{
			ModdedMainController.Instance.LoadScene(SceneToLoad);
		}
	}
}