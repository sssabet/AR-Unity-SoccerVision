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
    public float pitchWidth = 7f; // Adjusted to Unity's scale
    public float pitchHeight = 12f; // Adjusted to Unity's scale

    private FrameList matchData;
    private int currentFrameIndex = 0;
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    private List<GameObject> refereeObjects = new List<GameObject>(); // Changed to a list since referees don't have IDs

    void Start()
    {
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
        float videoStartTime = Time.time; // Record the start time
        while (currentFrameIndex < matchData.frames.Count)
        {
            float elapsedTime = Time.time - videoStartTime; // Get the elapsed time since the start
            int expectedFrameIndex = Mathf.FloorToInt(elapsedTime * 25f); // Calculate the frame based on elapsed time and FPS

            // Make sure not to skip frames but only display them when the elapsed time is greater than expected
            if (expectedFrameIndex >= currentFrameIndex)
            {
                DisplayFrame(currentFrameIndex);
                currentFrameIndex++;
            }

            yield return null; // Yield to the next frame
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

        Debug.Log($"Processing frame {frameIndex}, frame_index in data: {frame.frame_index}");

        if (frame.players != null)
        {
            Debug.Log($"Number of players in frame {frameIndex}: {frame.players.Count}");
        }
        else
        {
            Debug.LogWarning($"Frame {frameIndex} has no players data.");
        }

        // Keep track of player IDs in this frame
        HashSet<int> currentFramePlayerIds = new HashSet<int>();

        // Process players
        if (frame.players != null && frame.players.Count > 0)
        {
            foreach (Player player in frame.players)
            {
                currentFramePlayerIds.Add(player.id);

                // Divide player position by 10 to fit the Unity pitch coordinates
                Vector3 position = MapPositionToPitch(player.position);

                GameObject playerGO;

                if (playerObjects.ContainsKey(player.id))
                {
                    // Update position
                    playerGO = playerObjects[player.id];
                    playerGO.transform.position = position;
                    Debug.Log($"Updated Player {player.id} at scaled position ({position.x}, {position.z})");
                }
                else
                {
                    // Instantiate new player GameObject
                    playerGO = Instantiate(playerPrefab, position, Quaternion.identity, transform);

                    // Assign team material
                    Renderer renderer = playerGO.GetComponent<Renderer>();
                    if (player.team_id == 0)
                    {
                        renderer.material = team0Material;
                        Debug.Log($"Player {player.id} assigned to Team 0.");
                    }
                    else
                    {
                        renderer.material = team1Material;
                        Debug.Log($"Player {player.id} assigned to Team 1.");
                    }

                    // Add to dictionary
                    playerObjects[player.id] = playerGO;

                    Debug.Log($"Instantiated Player {player.id} at scaled position ({position.x}, {position.z})");
                }
            }
        }
        else
        {
            Debug.LogWarning("No players in frame " + frameIndex);
        }

        // Remove players who are not in the current frame
        List<int> playersToRemove = new List<int>();
        foreach (int playerId in playerObjects.Keys)
        {
            if (!currentFramePlayerIds.Contains(playerId))
            {
                Destroy(playerObjects[playerId]);
                playersToRemove.Add(playerId);
                Debug.Log($"Removed Player {playerId} who is not present in frame {frameIndex}");
            }
        }

        foreach (int playerId in playersToRemove)
        {
            playerObjects.Remove(playerId);
        }

        // Process referees without IDs
        ProcessReferees(frame);
    }

    void ProcessReferees(Frame frame)
    {
        // Check if referees data is available
        if (frame.referees != null && frame.referees.Count > 0)
        {
            Debug.Log($"Number of referees in frame {frame.frame_index}: {frame.referees.Count}");

            // Ensure the number of referee GameObjects matches the number of referees in the frame
            AdjustRefereeObjectsList(frame.referees.Count);

            // Update positions of referees
            for (int i = 0; i < frame.referees.Count; i++)
            {
                Referee referee = frame.referees[i];
                Vector3 position = MapPositionToPitch(referee.position);

                GameObject refereeGO = refereeObjects[i];
                refereeGO.transform.position = position;

                Debug.Log($"Referee {i} at scaled position ({position.x}, {position.z})");
            }
        }
        else
        {
            Debug.LogWarning("No referees in frame " + frame.frame_index);

            // Destroy any existing referee GameObjects
            ClearRefereeObjects();
        }
    }

    void AdjustRefereeObjectsList(int requiredCount)
    {
        // Add new referee GameObjects if needed
        while (refereeObjects.Count < requiredCount)
        {
            GameObject refereeGO = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, transform);

            // Assign referee material
            Renderer renderer = refereeGO.GetComponent<Renderer>();
            renderer.material = refereeMaterial;

            // Add to list
            refereeObjects.Add(refereeGO);
            Debug.Log($"Instantiated new Referee GameObject.");
        }

        // Remove excess referee GameObjects if needed
        while (refereeObjects.Count > requiredCount)
        {
            GameObject refereeGO = refereeObjects[refereeObjects.Count - 1];
            Destroy(refereeGO);
            refereeObjects.RemoveAt(refereeObjects.Count - 1);
            Debug.Log($"Destroyed excess Referee GameObject.");
        }
    }

    void ClearRefereeObjects()
    {
        foreach (GameObject refereeGO in refereeObjects)
        {
            Destroy(refereeGO);
        }
        refereeObjects.Clear();
        Debug.Log("Cleared all referee GameObjects.");
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

        // Scale positions to fit Unity's pitch dimensions
        float scaledX = x / 10f;
        float scaledY = y / 10f;

        // Mirror the x-axis around the center
        float mirroredX = 12f - scaledX;

        Debug.Log("Before x:" + x + " y:" + y + " after x:" + mirroredX + " y:" + scaledY);
        // Return the position in Unity's 3D space with Y (height) fixed at 0.5 meters
        return new Vector3(mirroredX, 0.1f, scaledY);
    }
}
