using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageTracking : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public GameObject[] card1Objects;  // Card1 오브젝트 배열
    public GameObject[] card2Objects;  // Card2 오브젝트 배열

    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, int> objectIndices = new Dictionary<string, int>();

    void OnEnable() => imageManager.trackedImagesChanged += OnImageChanged;
    void OnDisable() => imageManager.trackedImagesChanged -= OnImageChanged;

    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var image in eventArgs.added)
            SpawnObject(image);

        foreach (var image in eventArgs.updated)
            UpdateObjectPosition(image);
    }

    private void SpawnObject(ARTrackedImage image)
    {
        string imageName = image.referenceImage.name;

        if (!spawnedObjects.ContainsKey(imageName))
        {
            GameObject obj = GetObjectByName(imageName, 0);
            if (obj != null)
            {
                // 기본 트래킹된 회전에 Y축 180도 회전 추가 (월드 좌표 기준)
                Quaternion correctedRotation = image.transform.rotation * Quaternion.Euler(0, 180, 0);

                GameObject newObj = Instantiate(obj, image.transform.position, correctedRotation, image.transform);
                spawnedObjects[imageName] = newObj;
                objectIndices[imageName] = 0;

                // 오브젝트에 터치 감지용 스크립트 추가
                newObj.AddComponent<ARTouchableObject>().Setup(imageName, this);
            }
        }
    }

    private void UpdateObjectPosition(ARTrackedImage image)
    {
        string imageName = image.referenceImage.name;
        if (spawnedObjects.ContainsKey(imageName))
        {
            GameObject obj = spawnedObjects[imageName];

            if (image.trackingState == TrackingState.Tracking)
            {
                obj.SetActive(true);
                obj.transform.position = image.transform.position;

                // 월드 좌표 기준으로 Y축 180도 회전
                obj.transform.rotation = image.transform.rotation * Quaternion.Euler(0, 180, 0);
            }
            else
            {
                obj.SetActive(false);  // 명함이 보이지 않으면 오브젝트 숨김
            }
        }
    }

    public void ChangeObject(string imageName)
    {
        if (!spawnedObjects.ContainsKey(imageName)) return;

        Destroy(spawnedObjects[imageName]);

        objectIndices[imageName] = (objectIndices[imageName] + 1) % 3;
        GameObject nextObj = GetObjectByName(imageName, objectIndices[imageName]);

        if (nextObj != null)
        {
            GameObject newObj = Instantiate(nextObj, imageManager.transform);
            spawnedObjects[imageName] = newObj;

            // 터치 감지 스크립트 추가
            newObj.AddComponent<ARTouchableObject>().Setup(imageName, this);
        }
    }

    private GameObject GetObjectByName(string imageName, int index)
    {
        if (imageName == "Card1") return card1Objects[index];
        if (imageName == "Card2") return card2Objects[index];
        return null;
    }
}
