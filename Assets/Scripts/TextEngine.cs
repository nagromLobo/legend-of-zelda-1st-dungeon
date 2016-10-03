using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// requires being in an object with a text component
public class TextEngine : MonoBehaviour {
    public string fullString;
    public float delayBetweenLetters = 1.0f;
    public bool animateText = false;
    private Text uiText;
    private float timeLastLetter = 0.0f;
    private int i = 0;


	// Use this for initialization
	void Start () {
        this.uiText = this.gameObject.GetComponent<Text>();
        uiText.text = "";
	}

    void Update() {
        if (animateText && i < fullString.Length) {
            if((Time.time - timeLastLetter) > delayBetweenLetters) {
                uiText.text += fullString[i];
                ++i;
                timeLastLetter = Time.time;
            }
        }

    }

    public void ClearText() {
        if(uiText == null) {
            uiText = this.gameObject.GetComponent<Text>();
        }
        uiText.text = "";
        animateText = false;
    }
	
    public void AnimateText() {
        if (uiText == null) {
            uiText = this.gameObject.GetComponent<Text>();
        }
        animateText = true;
        timeLastLetter = Time.time;
        i = 0;
        uiText.text = "";
    }
}
