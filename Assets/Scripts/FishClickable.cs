using UnityEngine;

public class FishClickable : MonoBehaviour
{
    private FishingController controller;

   

    void OnMouseDown()
    {
        FishingController.instance.CollectFish();
    }
}
