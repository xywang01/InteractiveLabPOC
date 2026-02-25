using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MapRecorder : MonoBehaviour
{
    public Camera cam;
    public int textureSize = 1024;

    private List<Transform> locations = new List<Transform>();
    public Recording.OutputManager outputManager;
    public GameObject markerPrefab;

    public static MapRecorder Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public IEnumerator GenerateMap(Transform newLocation)
    {
        yield return new WaitForEndOfFrame();

        locations.Add(newLocation);
        Vector3 newPosition = newLocation.position;
        newPosition.y += 1.5f;
        Instantiate(markerPrefab, newPosition, Quaternion.identity);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();

        RenderTexture.active = currentRT;

        byte[] bytes = image.EncodeToPNG();

        Destroy(image);

        string fileName = "map" + ".png";
        string filePath = Path.Combine(outputManager.OutputFolder, fileName);

        File.WriteAllBytes(filePath, bytes);
    }

    //private void PositionCamera()
    //{
    //    if (locations.Count == 0) return;

    //    Vector3 min = locations[0].position;
    //    Vector3 max = locations[0].position;

    //    // 🔁 LOOP through all positions
    //    foreach (Transform t in locations)
    //    {
    //        Vector3 pos = t.position;

    //        min = Vector3.Min(min, pos);
    //        max = Vector3.Max(max, pos);
    //    }

    //    // center of all points
    //    Vector3 center = (min + max) / 2f;

    //    // size of area
    //    float width = max.x - min.x;
    //    float height = max.z - min.z;

    //    float size = Mathf.Max(width, height);

    //    // position camera above center
    //    mapCamera.transform.position = new Vector3(center.x, 50f, center.z);

    //    // adjust zoom
    //    mapCamera.orthographicSize = size / 2f + 5f;
    //}
}
