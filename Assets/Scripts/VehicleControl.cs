using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class VehicleControl : MonoBehaviour
{
    private WheelDriverControl wheelDriverControl;
    private float initMaxSpeed = 0f;
    // 자동차가 이동할 타겟 데이터 구조체.
    public struct Target
    {
        public int segment;
        public int waypoint;
    }
    // 자동차의 상태.
    public enum Status
    {
        GO,
        STOP,
        SLOW_DOWN,
    }
    [Header("교통 관제 시스템.")] [Tooltip("현재 활성화된 교통 시스템.")]
    public TrafficHeadquater trafficHeadquarter;
    [Tooltip("차량이 목표에 도달한 시기를 확인합니다. 다음 웨이포인트를 더 일찍 예상하는데 사용할 수 있습니다. (이 숫자가 높을수록 더 빨리 예상됩니다.)")]
    public float waypointThresh = 2.5f;

    [Header("감지 레이더")] [Tooltip("레이를 쏠 앵커.")]
    public Transform raycastAnchor;
    [Tooltip("레이의 길이.")]
    public float raycastLength = 3f;
    [Tooltip("레이 사이의 간격.")]
    public float raycastSpacing = 3f;
    [Tooltip("생성될 레이의 수.")]
    public int raycastNumber = 8;
    [Tooltip("감지되는 차량이 이 거리 미만이면 정지합니다..")]
    public float emergentcyBrakeThresh = 1.5f;
    [Tooltip("감지되는 차량이 이 거리보다 낮거나 거리보다 높을 경우 자동차의 속도가 느려집니다..")]
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
        /* 테스트 코드. 주석처리 합니다.
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
        // hq가 들고 있는 구간 중에 현재 차량이 속해있는 세그먼트가 갖고 있는 다음 구간들을 얻어옵니다.
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
            // 현재 차가 이 구간 안에 있는지 확인.
            if (segment.IsOnSegment(transform.position))
            {
                currentTarget.segment = segment.ID;
                // 구간 내에서 시작할 가장 가까운 웨이포인트 찾기
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
        // 다음 target 찾기.
        nextTarget.waypoint = currentTarget.waypoint + 1;
        nextTarget.segment = currentTarget.segment;
        // 위에 지정한 다음 타겟의 waypoint가 범위를 벗어났다면, 다시 처음 0번째 웨이포인트. 다음 세그먼트 아이디를 구합니다.
        if (nextTarget.waypoint >= trafficHeadquarter.segments[currentTarget.segment].wayPoints.Count)
        {
            nextTarget.waypoint = 0;
            nextTarget.segment = GetNextSegmentID();
        }
    }
}

