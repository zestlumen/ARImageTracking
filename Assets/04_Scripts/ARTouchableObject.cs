using UnityEngine;

public class ARTouchableObject : MonoBehaviour
{
    private string imageName;
    private ARImageTracking imageTracking;

    public void Setup(string imageName, ARImageTracking tracking)
    {
        this.imageName = imageName;
        this.imageTracking = tracking;
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            Debug.Log($"🎯 {imageName} 오브젝트 터치됨! 변경 중...");
            imageTracking.ChangeObject(imageName);
        }
    }
}