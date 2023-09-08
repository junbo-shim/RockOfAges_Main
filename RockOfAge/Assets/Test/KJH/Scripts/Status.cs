using UnityEngine;

[CreateAssetMenu(fileName = "RockStatus", menuName = "ScriptableObjects/RockStatus", order = 1)]
public class RockStatus : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string stoneName;
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float damage;
    [SerializeField] private float weight;
    [SerializeField] private float cooldown;
    [SerializeField] private string tempString;

    public int Id { get => id; set => id = value; }
    public string StoneName { get => stoneName; set => stoneName = value; }
    public float Health { get => health; set => health = value; }
    public float Speed { get => speed; set => speed = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float Damage { get => damage; set => damage = value; }
    public float Weight { get => weight; set => weight = value; }
    public float Cooldown { get => cooldown; set => cooldown = value; }
    public string TempString { get => tempString; set => tempString = value; }
}

[CreateAssetMenu(fileName = "ObstacleStatus", menuName = "ScriptableObjects/ObstacleStatus", order = 2)]
public class ObstacleStatus : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string obstacleName;
    [SerializeField] private float price;
    [SerializeField] private Vector3 size;
    [SerializeField] private int buildLimit;

    public int Id { get => id; set => id = value; }
    public string ObstacleName { get => obstacleName; set => obstacleName = value; }
    public float Price { get => price; set => price = value; }
    public Vector3 Size { get => size; set => size = value; }
    public int BuildLimit { get => buildLimit; set => buildLimit = value; }
}