using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; // Ensure you have Newtonsoft.Json installed

[System.Serializable]
public class PlayerData
{
    public int id;
    public int team_id;
    public List<float> position; // [x, y]
}

[System.Serializable]
public class RefereeData
{
    public List<float> position; // [x, y]
}

[System.Serializable]
public class FrameData
{
    public int frame_index;
    public List<PlayerData> players;
    public List<PlayerData> goalkeepers;
    public List<RefereeData> referees;
}

[System.Serializable]
public class MatchData
{
    public List<FrameData> frames;
}

public class FootballVisualizer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;       // Assign a prefab for players
    public GameObject goalkeeperPrefab;   // Assign a prefab for goalkeepers
    public GameObject refereePrefab;      // Assign a prefab for referees

    [Header("Visualization Settings")]
    public float updateInterval = 0.1f;   // Time in seconds between frame updates

    private MatchData matchData;
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> goalkeeperObjects = new Dictionary<int, GameObject>();
    private List<GameObject> refereeObjects = new List<GameObject>();

    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        // Load and parse the JSON file from the Resources folder
        TextAsset jsonData = Resources.Load<TextAsset>("matchData"); // Ensure the file is named "matchData.json" and placed in Resources
        if (jsonData == null)
        {
            Debug.LogError("Failed to load matchData.json from Resources folder.");
            return;
        }

        // Log the JSON content
        Debug.Log("JSON Data Loaded:\n" + jsonData.text);

        // Parse the JSON using Newtonsoft.Json
        try
        {
            matchData = JsonConvert.DeserializeObject<MatchData>(jsonData.text);
            if (matchData == null || matchData.frames == null || matchData.frames.Count == 0)
            {
                Debug.LogError("Failed to parse match data or no frames found.");
                return;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception during JSON parsing: " + ex.Message);
            return;
        }

        // Initialize objects for the first frame
        InitializeFrameObjects(matchData.frames[0]);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            if (currentFrame < matchData.frames.Count)
            {
                UpdateFrameObjects(matchData.frames[currentFrame]);
                currentFrame++;
            }
        }
    }

    void InitializeFrameObjects(FrameData frame)
    {
        // Initialize player objects
        foreach (var player in frame.players)
        {
            Vector3 position = ConvertPosition(player.position);
            GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
            playerObject.name = $"Player_{player.id}";
            playerObjects[player.id] = playerObject;
        }

        // Initialize goalkeeper objects
        foreach (var goalkeeper in frame.goalkeepers)
        {
            Vector3 position = ConvertPosition(goalkeeper.position);
            GameObject goalkeeperObject = Instantiate(goalkeeperPrefab, position, Quaternion.identity);
            goalkeeperObject.name = $"Goalkeeper_{goalkeeper.id}";
            goalkeeperObjects[goalkeeper.id] = goalkeeperObject;
        }

        // Initialize referee objects
        foreach (var referee in frame.referees)
        {
            Vector3 position = ConvertPosition(referee.position);
            GameObject refereeObject = Instantiate(refereePrefab, position, Quaternion.identity);
            refereeObject.name = $"Referee_{refereeObjects.Count + 1}";
            refereeObjects.Add(refereeObject);
        }
    }

    void UpdateFrameObjects(FrameData frame)
    {
        // Update player positions
        foreach (var player in frame.players)
        {
            if (playerObjects.ContainsKey(player.id))
            {
                Vector3 newPos = ConvertPosition(player.position);
                playerObjects[player.id].transform.position = newPos;
            }
            else
            {
                // Instantiate new player
                Vector3 position = ConvertPosition(player.position);
                GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
                playerObject.name = $"Player_{player.id}";
                playerObjects[player.id] = playerObject;
            }
        }

        // Update goalkeeper positions
        foreach (var goalkeeper in frame.goalkeepers)
        {
            if (goalkeeperObjects.ContainsKey(goalkeeper.id))
            {
                Vector3 newPos = ConvertPosition(goalkeeper.position);
                goalkeeperObjects[goalkeeper.id].transform.position = newPos;
            }
            else
            {
                // Instantiate new goalkeeper
                Vector3 position = ConvertPosition(goalkeeper.position);
                GameObject goalkeeperObject = Instantiate(goalkeeperPrefab, position, Quaternion.identity);
                goalkeeperObject.name = $"Goalkeeper_{goalkeeper.id}";
                goalkeeperObjects[goalkeeper.id] = goalkeeperObject;
            }
        }

        // Update referee positions
        for (int i = 0; i < frame.referees.Count; i++)
        {
            if (i < refereeObjects.Count)
            {
                Vector3 newPos = ConvertPosition(frame.referees[i].position);
                refereeObjects[i].transform.position = newPos;
            }
            else
            {
                // Instantiate new referee
                Vector3 position = ConvertPosition(frame.referees[i].position);
                GameObject refereeObject = Instantiate(refereePrefab, position, Quaternion.identity);
                refereeObject.name = $"Referee_{refereeObjects.Count + 1}";
                refereeObjects.Add(refereeObject);
            }
        }
    }

    Vector3 ConvertPosition(List<float> pos)
    {
        // Convert the 2D position from JSON to Unity's 3D position
        // Assuming [x, y] maps to [x, 0, y] in Unity
        return new Vector3(pos[0], 0.1f, pos[1]); // Adjust the y-value (height) as needed
    }
}
