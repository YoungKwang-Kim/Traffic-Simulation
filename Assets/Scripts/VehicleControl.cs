using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class VehicleControl : MonoBehaviour
{
    private WheelDriverControl wheelDriverControl;
    private float initMaxSpeed = 0f;
    // �ڵ����� �̵��� Ÿ�� ������ ����ü.
    public struct Target
    {
        public int segment;
        public int waypoint;
    }
    // �ڵ����� ����.
    public enum Status
    {
        GO,
        STOP,
        SLOW_DOWN,
    }
    [Header("���� ���� �ý���.")] [Tooltip("���� Ȱ��ȭ�� ���� �ý���.")]
    public TrafficHeadquater trafficHeadquarter;
    [Tooltip("������ ��ǥ�� ������ �ñ⸦ Ȯ���մϴ�. ���� ��������Ʈ�� �� ���� �����ϴµ� ����� �� �ֽ��ϴ�. (�� ���ڰ� �������� �� ���� ����˴ϴ�.)")]
    public float waypointThresh = 2.5f;

    [Header("���� ���̴�")] [Tooltip("���̸� �� ��Ŀ.")]
    public Transform raycastAnchor;
    [Tooltip("������ ����.")]
    public float raycastLength = 3f;
    [Tooltip("���� ������ ����.")]
    public float raycastSpacing = 3f;
    [Tooltip("������ ������ ��.")]
    public int raycastNumber = 8;
    [Tooltip("�����Ǵ� ������ �� �Ÿ� �̸��̸� �����մϴ�..")]
    public float emergentcyBrakeThresh = 1.5f;
    [Tooltip("�����Ǵ� ������ �� �Ÿ����� ���ų� �Ÿ����� ���� ��� �ڵ����� �ӵ��� �������ϴ�..")]
    public float slowDownThresh = 5f;

    public Status vehicleStatus = Status.GO;
    private int pastTargetSegment = -1;
    private Target currentTarget;
    private Target nextTarget;

    void Start()
    {
        wheelDriverControl = GetComponent<WheelDriverControl>();
        initMaxSpeed = wheelDriverControl.maxSpeed;

        if (raycastAnchor == null && transform.Find("Raycast Anchor") != null)
        {
            raycastAnchor = transform.Find("Raycast Anchor");
        }
    }

    void Update()
    {
        /* �׽�Ʈ �ڵ�. �ּ�ó�� �մϴ�.
        float accelation = 5f;
        float brake = 0f;
        float steering = 0f;
        wheelDriverControl.maxSpeed = initMaxSpeed;
        wheelDriverControl.Move(accelation, steering, brake);
        */

        if (trafficHeadquarter == null)
        {
            return;
        }
    }

    int GetNextSegmentID()
    {
        // hq�� ��� �ִ� ���� �߿� ���� ������ �����ִ� ���׸�Ʈ�� ���� �ִ� ���� �������� ���ɴϴ�.
        List<TrafficSegment> nextSegments = trafficHeadquarter.segments[currentTarget.segment].nextSegments;
        if (nextSegments.Count == 0)
        {
            return 0;
        }

        int randomCount = Random.Range(0, nextSegments.Count - 1);
        return nextSegments[randomCount].ID;
    }

    void SegWaypointVegicleIsOn()
    {
        foreach (var segment in trafficHeadquarter.segments)
        {
            // ���� ���� �� ���� �ȿ� �ִ��� Ȯ��.
            if (segment.IsOnSegment(transform.position))
            {
                currentTarget.segment = segment.ID;
                // ���� ������ ������ ���� ����� ��������Ʈ ã��
                float minDist = float.MaxValue;
                List<TrafficWaypoint> waypoints = trafficHeadquarter.segments[currentTarget.segment].wayPoints;
                for (int j = 0; j < waypoints.Count; j++)
                {
                    float distance = Vector3.Distance(transform.position, waypoints[j].transform.position);

                    Vector3 lSpace = transform.InverseTransformPoint(waypoints[j].transform.position);
                    if (distance < minDist && lSpace.z > 0f)
                    {
                        minDist = distance;
                        currentTarget.waypoint = j;
                    }
                }
                break;
            }
        }
        // ���� target ã��.
        nextTarget.waypoint = currentTarget.waypoint + 1;
        nextTarget.segment = currentTarget.segment;
        // ���� ������ ���� Ÿ���� waypoint�� ������ ����ٸ�, �ٽ� ó�� 0��° ��������Ʈ. ���� ���׸�Ʈ ���̵� ���մϴ�.
        if (nextTarget.waypoint >= trafficHeadquarter.segments[currentTarget.segment].wayPoints.Count)
        {
            nextTarget.waypoint = 0;
            nextTarget.segment = GetNextSegmentID();
        }
    }
}

