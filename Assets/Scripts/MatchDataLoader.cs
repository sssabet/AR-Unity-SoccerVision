using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchDataLoader : MonoBehaviour
{
    [System.Serializable]
    public class Player
    {
        public int id;
        public int team_id;
        public List<float> position;
    }

    [System.Serializable]
    public class Referee
    {
        public List<float> position;
    }

    [System.Serializable]
    public class Frame
    {
        public int frame_index;
        public List<Player> players;
        public List<Player> goalkeepers;
        public List<Referee> referees;
    }

    [System.Serializable]
    public class FrameList
    {
        public List<Frame> frames;
    }

    public GameObject playerPrefab;
    public Material team0Material;
    public Material team1Material;
    public Material refereeMaterial;

    public float frameInterval = 1f / 25f; // 0.04 seconds per frame for 25 FPS video
    public float pitchWidth = 7f;
    public float pitchHeight = 12f;

    private FrameList matchData;
    private int currentFrameIndex = 0;
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    
    private List<GameObject> refereeObjects = new List<GameObject>();

    void Start()
    {
        Color DarkGreen = new Color(0.2f, 0.4f, 0.1f);
        team0Material = new Material(Shader.Find("Standard"));
        team0Material.color = Color.blue;

        team1Material = new Material(Shader.Find("Standard"));
        team1Material.color = Color.red;

        refereeMaterial = new Material(Shader.Find("Standard"));
        refereeMaterial.color = Color.yellow;

        LoadMatchData();
        StartCoroutine(PlayMatchData());
    }

    void LoadMatchData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("matchData");
        matchData = JsonUtility.FromJson<FrameList>(jsonData.text);

        if (matchData != null && matchData.frames != null)
        {
            Debug.Log("Number of frames loaded: " + matchData.frames.Count);
        }
        else
        {
            Debug.LogError("Failed to load match data or frames are null.");
        }
    }

    private IEnumerator PlayMatchData()
    {
        float videoStartTime = Time.time;
        while (currentFrameIndex < matchData.frames.Count)
        {
            float elapsedTime = Time.time - videoStartTime;
            int expectedFrameIndex = Mathf.FloorToInt(elapsedTime * 25f);

            if (expectedFrameIndex >= currentFrameIndex)
            {
                DisplayFrame(currentFrameIndex);
                currentFrameIndex++;
            }

            yield return null;
        }
    }

    void DisplayFrame(int frameIndex)
    {
        if (frameIndex < 0 || frameIndex >= matchData.frames.Count)
        {
            Debug.LogError("Frame index out of range.");
            return;
        }

        Frame frame = matchData.frames[frameIndex];
        HashSet<int> currentFramePlayerIds = new HashSet<int>();

        // Process regular players (excluding goalkeepers)
        if (frame.players != null && frame.players.Count > 0)
        {
            foreach (Player player in frame.players)
            {
                // Skip players who are already classified as goalkeepers
                if (frame.goalkeepers.Exists(gk => gk.id == player.id))
                    continue;

                currentFramePlayerIds.Add(player.id);
                Vector3 position = MapPositionToPitch(player.position);

                GameObject playerGO;
                if (playerObjects.ContainsKey(player.id))
                {
                    playerGO = playerObjects[player.id];
                    playerGO.transform.position = position;
                    playerGO.transform.rotation = Quaternion.Euler(0, -90, 0);
                }
                else
                {
                    playerGO = Instantiate(playerPrefab, position, Quaternion.Euler(0, -90, 0), transform);
                    ApplyTeamMaterial(playerGO, player.team_id);
                    playerObjects[player.id] = playerGO;

                    Animator animator = playerGO.GetComponent<Animator>();
                }
            }
        }

        // Process goalkeepers separately
        if (frame.goalkeepers != null && frame.goalkeepers.Count > 0)
        {
            foreach (Player goalkeeper in frame.goalkeepers)
            {
                currentFramePlayerIds.Add(goalkeeper.id);
                Vector3 position = MapPositionToPitch(goalkeeper.position);

                GameObject goalkeeperGO;
                if (playerObjects.ContainsKey(goalkeeper.id))
                {
                    goalkeeperGO = playerObjects[goalkeeper.id];
                    goalkeeperGO.transform.position = position;
                    goalkeeperGO.transform.rotation = Quaternion.Euler(0, -90, 0);
                }
                else
                {
                    goalkeeperGO = Instantiate(playerPrefab, position, Quaternion.Euler(0, -90, 0), transform);
                    ApplyTeamMaterial(goalkeeperGO, goalkeeper.team_id);
                    playerObjects[goalkeeper.id] = goalkeeperGO;

                    Animator animator = goalkeeperGO.GetComponent<Animator>();
                }
            }
        }

        // Clean up any players that are not present in this frame
        List<int> playersToRemove = new List<int>();
        foreach (var kvp in playerObjects)
        {
            if (!currentFramePlayerIds.Contains(kvp.Key))
            {
                Destroy(kvp.Value);
                playersToRemove.Add(kvp.Key);
            }
        }
        foreach (int id in playersToRemove)
        {
            playerObjects.Remove(id);
        }

        ProcessReferees(frame);
    }

    void ApplyTeamMaterial(GameObject playerGO, int teamId)
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = playerGO.GetComponentsInChildren<SkinnedMeshRenderer>();
        Material teamMaterial = teamId == 0 ? team0Material : team1Material;

        foreach (var renderer in skinnedMeshRenderers)
        {
            renderer.material = teamMaterial;
        }
    }

    void ProcessReferees(Frame frame)
    {
        if (frame.referees != null && frame.referees.Count > 0)
        {
            AdjustRefereeObjectsList(frame.referees.Count);

            for (int i = 0; i < frame.referees.Count; i++)
            {
                Referee referee = frame.referees[i];
                Vector3 position = MapPositionToPitch(referee.position);

                GameObject refereeGO = refereeObjects[i];
                refereeGO.transform.position = position;
            }
        }
        else
        {
            ClearRefereeObjects();
        }
    }

    void AdjustRefereeObjectsList(int requiredCount)
    {
        while (refereeObjects.Count < requiredCount)
        {
            GameObject refereeGO = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, transform);

            SkinnedMeshRenderer[] skinnedMeshRenderers = refereeGO.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinnedMeshRenderers)
            {
                renderer.material = refereeMaterial;
            }
            
            refereeObjects.Add(refereeGO);
        }

        while (refereeObjects.Count > requiredCount)
        {
            GameObject refereeGO = refereeObjects[refereeObjects.Count - 1];
            Destroy(refereeGO);
            refereeObjects.RemoveAt(refereeObjects.Count - 1);
        }
    }

    void ClearRefereeObjects()
    {
        foreach (GameObject refereeGO in refereeObjects)
        {
            Destroy(refereeGO);
        }
        refereeObjects.Clear();
    }

    Vector3 MapPositionToPitch(List<float> position)
    {
        if (position == null || position.Count < 2)
        {
            Debug.LogError("Invalid position data.");
            return Vector3.zero;
        }

        float x = position[0];
        float y = position[1];
        float scaledX = x / 10f;
        float scaledY = y / 10f;
        float mirroredX = 12f - scaledX;

        return new Vector3(mirroredX, 0.1f, scaledY);
    }
}
