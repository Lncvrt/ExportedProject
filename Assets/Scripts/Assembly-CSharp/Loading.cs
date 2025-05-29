using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
	public float clientVersion;

	public TMP_Text statusText;

	public Button updateButton;

	private void Awake()
	{
		QualitySettings.vSyncCount = PlayerPrefs.GetInt("Setting5", 1);
		Screen.fullScreen = PlayerPrefs.GetInt("Setting1", 1) == 1;
		if (!Application.isMobilePlatform)
		{
			SetIfNone("Setting1", 1);
			SetIfNone("Setting2", 0);
			SetIfNone("Setting3", 0);
			SetIfNone("Setting4", 0);
			SetIfNone("Setting5", 1);
		}
		else
		{
			SetIfNone("Setting1", 1, overrideValue: true);
			SetIfNone("Setting2", 1, overrideValue: true);
			SetIfNone("Setting3", 0);
			SetIfNone("Setting4", 0);
			SetIfNone("Setting5", 1, overrideValue: true);
		}
		PlayerPrefs.SetFloat("latestVersion", clientVersion);
		StartCoroutine(CheckVersion());
		updateButton.onClick.AddListener(delegate
		{
			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				Application.OpenURL("https://berrydash.xytriza.com/download/windows");
			}
			else if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				Application.OpenURL("https://berrydash.xytriza.com/download/macos");
			}
			else if (Application.platform == RuntimePlatform.LinuxPlayer)
			{
				Application.OpenURL("https://berrydash.xytriza.com/download/linux");
			}
			else
			{
				Application.OpenURL("https://berrydash.xytriza.com/download");
			}
		});
	}

	private IEnumerator CheckVersion()
	{
		UnityWebRequest request = UnityWebRequest.Get("https://berrydash.xytriza.com/database/getLatestVersion.php");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			if (float.Parse(request.downloadHandler.text) != clientVersion)
			{
				statusText.text = "Outdated client. Please update the game to the latest version.";
				statusText.transform.localPosition = new Vector3(0f, 30f, 0f);
				statusText.fontSize = 25f;
				updateButton.gameObject.SetActive(value: true);
			}
			else
			{
				SceneManager.LoadScene("Menu");
			}
		}
		else
		{
			statusText.text = "Error fetching latest version. Please check your internet connection or try again later.";
			statusText.fontSize = 18f;
		}
	}

	private void SetIfNone(string key, int value, bool overrideValue = false)
	{
		if (!PlayerPrefs.HasKey(key) || overrideValue)
		{
			PlayerPrefs.SetInt(key, value);
		}
	}
}
