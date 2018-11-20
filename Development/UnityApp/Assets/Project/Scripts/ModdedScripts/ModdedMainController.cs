using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VusrCore.APIv1;
using VusrCore.APIv1.InputSystems;

public class ModdedMainController : MonoBehaviour {

	public static ModdedMainController Instance;
	private bool HasInteracted;

	public GameObject ExamplesCanvas;
	private Scene MainScene;

	private bool IsInExample;

	//The canvas containing any interaction needs to have a PointerCameraListener attached. 
	//The PointerCameraListener makes sure the UI and event camera will always be connected
	//This is a sanity check to make sure it exists on the object.
	private void Awake()
	{
		//PointerCameraListener listener = ExamplesCanvas.GetComponent<PointerCameraListener>();
		//if (listener == null)
		//{
		//	ExamplesCanvas.AddComponent<PointerCameraListener>();
		//}
	}

	private IEnumerator Start()
	{
		if(Instance == null)
		{
			Instance = this;
		}

		yield return null;

		MainScene = SceneManager.GetActiveScene();

		Fader.FadeToClear((b) =>
		{
			SetPointer(true);
		});
	}

	void Update()
	{
		//Press the back button to complete interactive.
		//This leaves the interactive and goes back into the app.
		if (!HasInteracted && !IsInExample && VusrInput.Back)
		{
			HasInteracted = true;
			Fader.FadeToBlack((b) => VusrCore.APIv1.Vusr.InteractiveComplete());
		}
	}

	private void SetPointer(bool active)
	{
		VusrInput.IsPointerVisible = active;
		VusrInput.IsControllerVisible = active;
	}

	public void LoadScene(string sceneName)
	{
		SetPointer(false);
		Fader.FadeToBlack((b) =>
		{
			Debug.Log("Loading scene");
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
			ExamplesCanvas.SetActive(false);
		});
	}

	public void UnloadScene()
	{
		Scene activeScene = SceneManager.GetActiveScene();
		if(activeScene.name != "StartScene")
		{
			SetPointer(false);
			Fader.FadeToBlack((b) =>
			{
				//unload
				Debug.Log("Unloading scene");
				SceneManager.sceneUnloaded += OnSceneUnloaded;
				SceneManager.UnloadSceneAsync(activeScene);
				ExamplesCanvas.SetActive(true);
			});
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("Scene loaded");
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.SetActiveScene(scene);
		Fader.FadeToClear((b) => SetPointer(true));
		IsInExample = true;
	}

	private void OnSceneUnloaded(Scene scene)
	{
		Debug.Log("Scene unloaded");
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
		SceneManager.SetActiveScene(MainScene);
		Fader.FadeToClear((b) => SetPointer(true));
		IsInExample = false;
	}
}
