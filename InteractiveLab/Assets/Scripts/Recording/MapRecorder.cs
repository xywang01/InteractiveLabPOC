using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MapRecorder : MonoBehaviour
{
    public Camera cam;
    public int textureSize = 1024;

    private List<Vector3> positions = new List<Vector3>();
    public Recording.OutputManager outputManager;
    public GameObject markerPrefab;
    public Material lineMaterial;

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

        Vector3 newPosition = newLocation.position;
        newPosition.y += 1.5f;
        Instantiate(markerPrefab, newPosition, Quaternion.identity);
        positions.Add(newPosition);
        DrawLine();

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

    private void DrawLine()
    {
        if (positions.Count < 2)
        {
            return;
        }

        GameObject lineObj = new GameObject("MapLine");
        lineObj.layer = LayerMask.NameToLayer("MapRecording");

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, positions[positions.Count - 1]);
        lr.SetPosition(1, positions[positions.Count - 2]);

        lr.widthMultiplier = 0.1f;
        lr.material = lineMaterial;
        lr.useWorldSpace = true;
    }
}
