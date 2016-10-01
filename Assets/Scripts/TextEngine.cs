using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// requires being in an object with a text component
public class TextEngine : MonoBehaviour {
    public string fullString;
    public float delayBetweenLetters = 1.0f;
    public bool animateText = false;
    private Text text;
    private float timeLastLetter = 0.0f;
    private int i = 0;


	// Use this for initialization
	void Start () {
        this.text = this.gameObject.GetComponent<Text>();
        text.text = "";
	}

    void Update() {
        if (animateText && i < fullString.Length) {
            if((Time.time - timeLastLetter) > delayBetweenLetters) {
                text.text += fullString[i];
                ++i;
                timeLastLetter = Time.time;
            }
        }

    }

    public void ClearText() {
        text.text = "";
        animateText = false;
    }
	
    public void AnimateText() {
        animateText = true;
        timeLastLetter = Time.time;
        i = 0;
        text.text = "";
    }
}
