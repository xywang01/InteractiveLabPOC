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
    public GameObject baseMarkerPrefab;
    public GameObject upperMarkerPrefab;
    public Material lineMaterial;
    public bool generateMissingMaps = false;

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

    private void Start()
    {
        if (generateMissingMaps)
        {
            StartCoroutine(GenerateMissingMaps());
        }
    }

    public IEnumerator GenerateMap(Transform newLocation)
    {
        yield return new WaitForEndOfFrame();

        Vector3 newPosition = newLocation.position;
        Debug.Log(newPosition.y);
        float difference = 284.5f - newPosition.y;
        newPosition.y += 1.5f + difference;

        if (difference > 0.1f)
        {
            Instantiate(baseMarkerPrefab, newPosition, Quaternion.identity);
        }
        else
        {
            Instantiate(upperMarkerPrefab, newPosition, Quaternion.identity);
        }

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

        string fileName = outputManager.GetMapOutputName();

        File.WriteAllBytes(fileName, bytes);
    }

    private IEnumerator GenerateMapFromList(string fileName)
    {
        yield return new WaitForEndOfFrame();

        foreach (Vector3 position in positions)
        {
            Vector3 newPosition = position;
            float difference = 284.5f - newPosition.y;
            newPosition.y += 1.5f + difference;

            if (difference > 0.1f)
            {
                Instantiate(baseMarkerPrefab, newPosition, Quaternion.identity);
            }
            else
            {
                Instantiate(upperMarkerPrefab, newPosition, Quaternion.identity);
            }
        }

        DrawAllLines();

        yield return new WaitForEndOfFrame();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();

        RenderTexture.active = currentRT;

        byte[] bytes = image.EncodeToPNG();

        Destroy(image);

        File.WriteAllBytes(fileName, bytes);

        yield return StartCoroutine(DeleteMarkers());
        positions = new List<Vector3>();
    }

    public IEnumerator GenerateMissingMaps()
    {
        string path = Application.persistentDataPath;

        string[] directories = Directory.GetDirectories(path);

        foreach (string dir in directories)
        {
            string[] csvFiles = Directory.GetFiles(dir, "*movement*.csv");

            foreach (string file in csvFiles)
            {
                string[] lines = File.ReadAllLines(file);

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];

                    string locationString = line.Substring(line.IndexOf("("));

                    locationString = locationString
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace("\"", "");

                    string[] coords = locationString.Split(',');

                    float x = float.Parse(coords[0]);
                    float y = float.Parse(coords[1]);
                    float z = float.Parse(coords[2]);

                    Vector3 location = new Vector3(x, y, z);

                    Debug.Log(location);
                    positions.Add(location);
                }

                string newFile =
                    Path.GetFileNameWithoutExtension(file)
                    + "_generated"
                    + ".png";
                newFile = dir + "/" + newFile;
                newFile = newFile.Replace("movement", "map");
                Debug.Log(newFile);
                Debug.Log(file);

                yield return StartCoroutine(GenerateMapFromList(newFile));
            }
        }

        Debug.Log("Done making missing maps");
    }

    private void DrawAllLines()
    {
        if (positions.Count < 2)
        {
            return;
        }

        for (int i = 1; i < positions.Count; i++)
        {
            GameObject lineObj = new GameObject("MapLine");
            lineObj.layer = LayerMask.NameToLayer("MapRecording");

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, positions[i]);
            lr.SetPosition(1, positions[i-1]);

            lr.widthMultiplier = 0.1f;
            lr.material = lineMaterial;
            lr.useWorldSpace = true;
        }
    }

    private IEnumerator DeleteMarkers()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("MapRecording"))
            {
                Destroy(obj);
            }
        }
        yield return null;
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
