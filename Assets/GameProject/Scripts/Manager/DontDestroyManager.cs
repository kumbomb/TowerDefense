using UnityEngine;

public class DontDestroyManager : Singleton<DontDestroyManager>
{
    protected override bool IsPersistent => true;
    // private void Start() {
    //     InitializeSingleton();
    // }
}
