using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {

    [SerializeField]private InputField id;
    [SerializeField]private InputField key;
    [SerializeField]private Button start;
    [SerializeField]private Text text;

    public void StartButton() {
        if (string.IsNullOrEmpty(id.text) || string.IsNullOrEmpty(key.text))
            StartCoroutine("ShowError");
        else {
            PlayerPrefs.SetString("id", id.text);
            PlayerPrefs.SetString("key", key.text);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }

    private IEnumerator ShowError()
    {
        key.gameObject.SetActive(false);
        id.gameObject.SetActive(false);
        start.gameObject.SetActive(false);
        text.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        key.gameObject.SetActive(true);
        id.gameObject.SetActive(true);
        start.gameObject.SetActive(true);
        text.gameObject.SetActive(false);
    }
}