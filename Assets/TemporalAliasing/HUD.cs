using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	private Aliasing test;
	const string format_UpdateFPS = "Update FPS: {0}";
	const string format_FixedUpdateFPS = "FixedUpdate FPS: {0}";
	public Text txt_FixedUpdateFPS;
	public Text txt_UpdateFPS;
	public Text txt_Warning;
	public Toggle tgl_UseTargetFrameRate;
	public Toggle tgl_UseLerp;
	public Toggle tgl_VSync;

	void Start()
	{

#if UNITY_EDITOR
		txt_Warning.gameObject.SetActive(true);
#endif

		test = gameObject.GetComponent<Aliasing>();
	}

	void Update()
	{
		txt_FixedUpdateFPS.text = string.Format(format_FixedUpdateFPS, test.FixedUpdateFPS);
		txt_UpdateFPS.text = string.Format(format_UpdateFPS, test.UpdateFPS);

		Application.targetFrameRate = tgl_UseTargetFrameRate.isOn ? 30 : 0;
		QualitySettings.vSyncCount = tgl_VSync.isOn ? 1 : 0;
		test.UseLerp = tgl_UseLerp.isOn;
	}
}
