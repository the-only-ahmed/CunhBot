using UnityEngine;
using Parse;

public class MyParseInitializer : ParseInitializeBehaviour {

    public override void Awake()
    {
        base.applicationID = PlayerPrefs.GetString("id");
        base.dotnetKey = PlayerPrefs.GetString("key");
        base.Awake();
    }
}