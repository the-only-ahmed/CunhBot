using UnityEngine;
using System.Collections.Generic;
using Parse;

public class Graphic : MonoBehaviour {

    private Dictionary<ParseObject, int> choozy = new Dictionary<ParseObject, int>();
    private List<ParseObject> toDel = new List<ParseObject>();
    private bool done = false;

    public void setTextViews(List<ParseObject> choozy) {
        foreach (ParseObject obj in choozy)
            this.choozy.Add(obj, 30);
        done = true;
    }

    public void decrementVote(ParseObject post) {
        //print(post.ObjectId);
        toDel.Add(post);
    }

    void OnGUI() {
        if (done) {
            int i = 0;
            int j = 0;
            GUIStyle style = new GUIStyle();
            Dictionary<ParseObject, int> copy = new Dictionary<ParseObject, int>(choozy);
            foreach (KeyValuePair<ParseObject, int> post in copy) {
                if (post.Value > 0)
                    style.normal.textColor = Color.cyan;
                else
                    style.normal.textColor = Color.red;

                GUI.Label(new Rect(15 + j, 10 + i, 300, 50), post.Key.Get<string>("title"));
                GUI.Label(new Rect(325 + j, 10 + i, 50, 50), post.Value.ToString(), style);
                i += 15;
                if (i + 30 > Screen.height) {
                    i = 0;
                    j += 350;
                    Drawing.DrawLine(new Vector2(j - 1, 0), new Vector2(j - 1, Screen.height), Color.green, 2.5f);
                }
            }
            foreach (ParseObject post in toDel)
                choozy[post]--;
            toDel.Clear();
        }
    }
}