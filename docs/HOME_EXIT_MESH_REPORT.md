# HOME 종료 팝업 — 절제된 코스믹 메시 적용

## 적용 범위

사용자가 선택한 절제된 코스믹 시안을 HOME 종료 확인 팝업과 내부 취소·종료 버튼에 적용했습니다. HOME 하단의 기존 게임 종료 진입 아이콘, 배경/로고 및 다른 팝업은 이번 작업에서 수정하지 않았습니다.

## 디자인 및 구현

- 차분한 네이비 패널과 얇은 Warm Gold 테두리, 둥근 모서리.
- Blue/Cyan 취소 버튼과 절제된 Gold 종료 버튼. 같은 크기와 충분한 터치 Rect.
- 제목/본문은 기존 한국어 TMP Font Asset을 유지하며 색상을 시안에 맞춤.
- 상단 작은 궤도 표시도 텍스처 없이 메시로 구성.
- HomeQuitMeshGraphic(MaskableGraphic/VertexHelper)에서 둥근 면·테두리·투명 외곽을 직접 생성. HomeQuitOrbitGraphic에서 원/타원 선 생성. 프레임/버튼에는 Sprite/PNG 참조가 없습니다.
- Safe Area 폭 82% 이하와 높이 60% 이하에 맞춰 중앙 배치. 패널 높이는 폭의 64%, Canvas 전체 Dim 유지. 모서리/테두리는 화면 크기에 맞춰 조정.
- 기존 ButtonPressFeedback과 Button Event를 재사용하며 targetGraphic만 메시 Graphic으로 연결.

PNG 파일은 삭제·가공·원복하지 않았습니다. 기존 사용자 교체본과 신규 popup_exit_frame도 그대로 보존합니다. 이전 PNG 적용 보고서는 이력이며 현재 종료 팝업의 표시 방식은 메시입니다. 사용하지 않는 PNG의 일괄 정리는 별도 작업으로 남깁니다.

## 동작 보호

HomeViewController/GameFlowController/GameSession/RunSaveStore의 종료·저장 동작은 이번 작업에서 변경하지 않았습니다. 취소는 HOME 유지, 종료는 기존 RequestQuit와 OnApplicationQuit 저장 경로, 중복 종료 가드도 유지됩니다. SCORE/BEST/행성/설정/BGM 및 다른 팝업 로직을 변경하지 않았습니다.

## 검증

최종 Sprint103QuitProbe PASS(53 assertions). 프레임·버튼 PNG Image 0, 실제 CanvasRenderer의 메시 정점/삼각형 및 궤도 메시 생성, 버튼 targetGraphic 연결, 터치 영역, HOME 진입 Button, 취소/종료/연속 호출, EventSystem 배경 터치 차단, SCORE/Run/BEST/행성/BGM 설정 보존 및 이어하기 확인.

1080×1920 / 1080×2400 + Safe Area / 1080×1440 모두 PASS. 텍스트/한국어 글리프/잘림/버튼 동일 크기/간격/Canvas 전체 Dim 검증. 세 렌더를 육안 확인했습니다.

기존 RunSavePlayProbe PASS. MissingScriptDiagnostics PASS(Missing Script 0). 최종 Compiler Warning/Error 0, Editor Runtime Warning/Error 0. 초기 검사 API 호출과 궤도 렌더 연결 문제는 수정 후 재검증했습니다. Scene 비교에서 종료 팝업 외 의미 있는 변경 0이며 자동 직렬화·ProjectAuditor 변경은 원복했습니다.

테스트 Run은 별도 경로로 격리하고 검사에서 변경한 PlayerPrefs는 기존 도구로 백업·복원합니다. 사용자 저장 데이터 삭제 없음.

## 수정/추가 파일

- Assets/Scenes/Game.unity
- Assets/Scripts/UI/HomeQuitMeshGraphic.cs 및 meta
- Assets/Scripts/UI/HomeQuitOrbitGraphic.cs 및 meta
- Assets/Scripts/UI/HomeQuitPopupLayout.cs
- Assets/Editor/HomeExitDesignBuilder.cs 및 meta (이전 미커밋 빌더를 메시 적용 도구로 변경)
- Assets/Editor/Sprint103QuitProbe.cs
- CURRENT_TASK/NEXT_TASK/DEVLOG 및 docs 사본, 본 보고서

기존 사용자 PNG 변경은 보존된 미커밋 변경이며 이번 구현에서 추가 가공한 것이 아닙니다.

## 상태 및 남은 확인

Android APK 빌드/설치/실기기 테스트 및 Commit/Push 미실행. 실제 스마트폰의 가독성/터치/종료는 별도 요청 후 확인합니다. 현재 변경은 미커밋 상태입니다.

렌더: Validation/exit_mesh_1920.png, exit_mesh_2400.png, exit_mesh_1440.png. 검사: Validation/exit_mesh_summary.txt 및 exit-mesh-*.log.
