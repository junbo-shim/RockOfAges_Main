using UnityEngine;
[CreateAssetMenu(fileName = "ObstacleStatus", menuName = "ScriptableObjects/ObstacleStatus", order = 2)]
public class ObstacleStatus : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string obstacleName;
    [SerializeField] private float price;
    [SerializeField] private float health;
    [SerializeField] private Vector2Int size;
    [SerializeField] private int buildLimit;
    [SerializeField] private string tempString;

    public ObstacleStatus(ObstacleStatus obstacle) 
    {
        this.id = obstacle.Id;
        this.obstacleName = obstacle.ObstacleName;
        this.price = obstacle.Price;
        this.health = obstacle.Health ;
        this.size = obstacle.Size ;
        this.buildLimit = obstacle.BuildLimit ;
        this.tempString = obstacle.TempString;
    }

    public int Id { get => id; set => id = value; }
    public string ObstacleName { get => obstacleName; set => obstacleName = value; }
    public float Price { get => price; set => price = value; }
    public float Health { get => health; set => health = value; }
    public Vector2Int Size { get => size; set => size = value; }
    public int BuildLimit { get => buildLimit; set => buildLimit = value; }
    public string TempString { get => tempString; set => tempString = value; }
}