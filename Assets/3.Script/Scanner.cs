using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    // ============================================
    //  #1. 플레이어 주변을 탐색하기 위한 스캐너
    // --------------------------------------------
    //  scanRange   : 원 형태 탐지 범위
    //  targetLayer : 어떤 레이어만 찾아볼지 지정 (Enemy만 필터링)
    //  targets     : 탐지된 모든 대상 저장 배열
    //  nearstTarget: 그 중 가장 가까운 대상
    // ============================================
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearstTarget;


    // ============================================
    //  #2. 물리 프레임마다 탐색 수행 (FixedUpdate)
    // --------------------------------------------
    //  - CircleCastAll : 원 범위로 모든 충돌체 탐지
    //  - 방향 Vector2.zero : ‘정지된’ 원형 탐색 (이동하지 않음)
    //  - 거리 0 : cast를 이동 없이 제자리에서만 수행
    //  - LayerMask 적용 : Enemy만 찾는다
    //  - 탐지된 리스트에서 가장 가까운 적을 구한다
    // ============================================
    private void FixedUpdate()
    {
        targets = Physics2D.CircleCastAll(
            transform.position,   // 기준 위치 = 플레이어 위치
            scanRange,            // 탐색 반경
            Vector2.zero,         // 이동 방향 없음 (그냥 원을 펼쳐서 찾기)
            0,                    // 이동거리 0
            targetLayer           // Enemy 레이어만 찾기
        );

        nearstTarget = GetNearest(); // 탐지된 적 중 가장 가까운 타겟 저장
    }


    // ============================================
    //  #3. 가장 가까운 적을 계산하는 함수
    // --------------------------------------------
    //  - diff : 현재 가장 짧은 거리값 (기본은 충분히 큰 값으로 설정)
    //  - foreach : 탐지된 모든 타겟을 돌면서 거리 측정
    //  - Vector3.Distance : 두 좌표 간 거리 계산
    //  - curDiff < diff 이면 갱신 → 가장 가까운 적이 된다.
    // ============================================
    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100; // 임의의 큰 값. 거리 비교 위해 기준값으로 둠.

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;

            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;         // 더 가까운 거리기 때문에 갱신
                result = target.transform;
            }
        }

        return result; // 가장 가까운 적 반환 (없으면 null)
    }
}

