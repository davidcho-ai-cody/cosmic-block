# Current Task
Sprint 2 — Core Game Loop 완료.

## 구현
- Row/Column/여러 Line 동시 제거, 교차 Cell 중복 제거.
- Placement/Line/Combo 점수, Local Best 저장, Score/Best/Combo 표시.
- 초기 3개 소비 후 새 랜덤 3개 공급. Piece/슬롯을 재사용하고 이전 Visual 제거.
- 남은 Shape 전체를 8×8 anchor로 검색하여 Game Over 판정.
- Playing/Resolving/GameOver, 입력 잠금, Game Over Panel, Retry.
- 기존 Game Scene을 Editor API로 확장. Portrait/SafeArea/spacing 12 유지.
- 광고 버튼은 비활성 Placeholder. SDK/효과/사운드/최종 아트 구현 없음.

## 검증
Unity 컴파일/Scene 확장/라인·점수 domain 검증 완료.
Test 1~13, 반복 재공급/Visual cleanup/입력 잠금/Retry/Best 저장 및 여러 화면비 Play Mode 검증 PASS. 상세 결과는 DEVLOG와 Validation/sprint2.txt에 기록.
현재 단계에서 수동 Unity 구성 작업 없음. README의 직접 QA를 진행할 수 있다.
실기기 Touch/실제 노치/APK·AAB는 이번 검증에 포함하지 않는다.
다음 Sprint 사용자 승인 대기.
