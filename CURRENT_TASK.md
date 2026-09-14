# Current Task
Sprint 1 — Block Placement 완료.

## 구현/검증 완료
- 8종 Cell Offset Shape, 초기 랜덤 블록 3개, 하단 슬롯 3개.
- Mouse/Touch uGUI 드래그, finger offset, Board 좌표 변환, 유효/무효 미리보기.
- BoardModel.CanPlace/TryPlace: 모든 범위/점유 검사 후 상태 변경. 실패 시 변경 없음.
- 성공 시 해당 Piece 소비/슬롯 비우기. 실패/취소 시 원래 슬롯 복귀.
- 동시 포인터 보호, 포커스/일시정지 취소, 비활성화 시 다음 프레임 안전 복귀.
- 기존 Game Scene을 Unity API로 확장; 64 Cell/참조/기술 기준 보존.
- Cell spacing 8 → 12, padding 8 유지.
- 배치 Test 1~8 및 seed/경계/원자성 검증 PASS.
- Play Mode의 synthetic uGUI mouse/touch-id events와 UI raycast 테스트 PASS.
- 1080×1920 / 1080×2400 / 1080×1440 렌더, 정사각형 보드/64 좌표 매핑/슬롯 SafeArea 내부 검사 PASS.
- 긴 화면은 SafeArea inset을 모사했다. Android 실기기 터치와 실제 노치 검증은 아직 미실행.

현재 단계에서 수동 Unity 작업 없음. 사용자 육안/실입력 QA 절차: README.md.
Line Clear/Score/Combo/Game Over/새 Block Set/광고/효과 구현 없음.
다음 Sprint 사용자 승인 대기.
