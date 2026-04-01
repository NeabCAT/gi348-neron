using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Spawn Points (เรียงลำดับจากซ้ายไปขวา)")]
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private int currentSpawnIndex = -1; // -1 = ยังไม่ผ่าน spawnpoint ไหนเลย
    private Vector3 defaultSpawnPosition;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // หา Player เพื่อเก็บ spawn เริ่มต้น
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            defaultSpawnPosition = player.transform.position;
    }

    // เรียกจาก SpawnPoint เมื่อผู้เล่นเดินผ่าน
    public void TryActivate(SpawnPoint triggered)
    {
        int triggeredIndex = spawnPoints.IndexOf(triggered);

        if (triggeredIndex == -1) return;                  // ไม่ได้อยู่ใน list
        if (triggered.isActivated) return;                 // ผ่านแล้ว
        if (triggeredIndex <= currentSpawnIndex) return;   // ย้อนกลับไม่ได้

        currentSpawnIndex = triggeredIndex;
        triggered.Activate();

        Debug.Log($"SpawnPoint [{triggeredIndex}] activated!");
    }

    // คืนตำแหน่ง spawn ปัจจุบัน
    public Vector3 GetRespawnPosition()
    {
        if (currentSpawnIndex == -1)
            return defaultSpawnPosition;

        return spawnPoints[currentSpawnIndex].transform.position;
    }
}