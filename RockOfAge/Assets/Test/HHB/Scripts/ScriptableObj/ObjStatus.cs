using UnityEngine;
[CreateAssetMenu(fileName = "ObstacleStatus", menuName = "ScriptableObjects/ObstacleStatus", order = 2)]
public class ObstacleStatus : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string obstacleName;
    [SerializeField] private float price;
    [SerializeField] private float health;
    [SerializeField] private Vector2 size;
    [SerializeField] private int buildLimit;
    [SerializeField] private string tempString;

    public int Id { get => id; set => id = value; }
    public string ObstacleName { get => obstacleName; set => obstacleName = value; }
    public float Price { get => price; set => price = value; }
    public float Health { get => health; set => health = value; }
    public Vector2 Size { get => size; set => size = value; }
    public int BuildLimit { get => buildLimit; set => buildLimit = value; }
    public string TempString { get => tempString; set => tempString = value; }
}