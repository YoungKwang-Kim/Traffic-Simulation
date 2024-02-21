using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficHeadquater : MonoBehaviour
{
    // ���׸�Ʈ�� ���׸�Ʈ ������ ���� ����.
    public float segDetectThresh = 0.1f;
    // ��������Ʈ�� ũ��.
    public float waypointSize = 0.5f;
    // �浹 ���̾��.
    public string[] collisionLayers;

    public List<TrafficSegment> segments = new List<TrafficSegment>();
    public TrafficSegment curSegment;

    public const string VehicleTagLayer = "AutonomouseVehicle"; // ����������.
    
    
    public List<TrafficWaypoint> GetAllWaypoints()
    {
        List<TrafficWaypoint> waypoints = new List<TrafficWaypoint>();
        foreach (var segment in segments)
        {
            waypoints.AddRange(segment.wayPoints);
        }
        return waypoints;
    }
}
