# GAME 메시 디자인 적용

승인된 GAME 시안의 네이비 메시 패널·얇은 시안 테두리·은은한 빛·공통 TMP 글자체를 적용했다. 기존 우주 배경, 행성 Sprite와 보석 블록 PNG는 유지하며 원본 PNG를 수정하거나 삭제하지 않았다.

## 적용 내용

- HOME 아이콘: 기존 확인 이벤트를 유지하고 둥근 메시 버튼과 선 아이콘으로 교체.
- SCORE/BEST: 공통 Medium/SemiBold 폰트, 네이비 BEST 패널과 메시 트로피. 큰 점수에서도 두 표시 영역이 겹치지 않도록 분리.
- 행성 HUD: 행성 이름·복원도·현재 단계·별빛을 기존 Text 데이터에서 TMP로 동기화. 기존 전체 복원도 Fill Rect와 애니메이션을 그대로 사용해 메시 게이지로 표시.
- 보드: 실제 Board/Cell Rect와 좌표 계산을 보존하고 장식 프레임과 빈 셀 표시만 메시로 교체. 배치된 보석 PNG 및 기존 Gold/Red Preview 유지.
- 슬롯: PNG 표시만 억제하고 기존 Image의 Slot 전체 Hit Area와 Drag Handler 유지. 기존 선택 상태는 메시 골드 테두리에 반영.
- 새로고침/힌트: 기존 Runtime Button·이벤트·사용 횟수·쿨다운을 유지하고 메시 표면·선 아이콘·TMP 문구로 교체. 슬롯 기준 배치도 유지.

## 수정/추가 파일

- Assets/Scenes/Game.unity
- Assets/Scripts/UI/GameMeshPresentation.cs
- Assets/Scripts/UI/GameMeshIconGraphic.cs
- Assets/Scripts/UI/GameEmptyCellMesh.cs
- Assets/Scripts/UI/GameSlotMesh.cs
- Assets/Editor/GameMeshBuilder.cs
- Assets/Editor/GameMeshProbe.cs
- Assets/Editor/CollectionMeshBuilder.cs: 공통 폰트 재생성 시 GAME 글리프도 보존하도록 문자 목록 확장.
- Assets/Fonts/CosmicPopupMedium.ttf/.asset, CosmicPopupSemiBold.ttf/.asset: 블록 새로고침·힌트·무료 횟수 문구에 필요한 글리프 추가.
- CURRENT_TASK/NEXT_TASK/DEVLOG 및 docs의 대응 문서.

기존 HOME/도감 미커밋 작업을 그대로 보존했다. GameSession, BoardView, BlockPiece, BlockDragHandler, Score/Combo/별빛/저장/BGM 구현은 수정하지 않았다.

## GAME 전용 검증

GameMeshProbe: **339항목 PASS**, Runtime Warning/Error 0.

- 1080×1920, 1080×2400 + Safe Area, 1080×1440 실제 RenderTexture 캡처.
- 기존 레이아웃과 같은 Board 크기: 각각 Canvas 단위 708.74 / 797.51 / 613.78 정사각형.
- 0/19,000/999,999/2,147,483,647 SCORE 및 BEST: 잘림 및 영역 겹침 없음.
- 5개 행성의 전체 복원도 0/25/50/75/99/100%: 기존 Fill anchor와 메시 Fill 일치.
- 전체 Shape의 슬롯 가장자리 Drag, Gold Highlight, 유효 Preview, 배치, Consumed Slot 입력 차단.
- 무효 Preview 및 복귀, 힌트, 새로고침, 라인 클리어, HOME 확인/취소/복귀/이어하기, Game Over/Retry.
- 버튼 실제 Raycast, 슬롯/기능 버튼 Hit Area 분리, HUD/버튼 한글 글리프 및 잘림 없음.

초기 검증에서 SCORE/BEST Rect 겹침과 GAME 신규 글리프 누락을 수정했다. 후속 테스트의 행성 모델 미연결·잘못된 DragLayer 경로·기존 보조 버튼 쿨다운 미대기는 테스트 설정을 바로잡았다. 게임 기능이나 사용 제한은 변경하지 않았다.

캡처: Validation/game_mesh_ready_1920.png, game_mesh_ready_2400.png, game_mesh_ready_1440.png 및 game_mesh_used_*.png.
ready 화면은 UI 비교용 보드/점수 fixture이며 사용자 실제 Run 데이터가 아니다. 테스트는 격리 Run 파일과 PlayerPrefs 백업·복원으로 수행했다.

## 기존 회귀
다음 8개 기존 검증 모두 PASS:
- Sprint102PlayProbe: 보조 기능 사용 제한·쿨다운·저장·모달 입력 차단.
- ClearScorePlayProbe: 106항목, 표시/실제 점수/저장 일치 및 연속 콤보.
- Sprint92PlayProbe: 기존 Line Clear/점수·콤보 피드백.
- RunSavePlayProbe: 563항목, 자동 저장/이어하기·Run 행성/보드/남은 블록/사용 횟수/오류 복구.
- HomeMenuMeshProbe: 363항목, 기존 HOME 메시 및 버튼·저장.
- CollectionMeshProbe: 1,370항목, 기존 도감 메시·5개 행성·미리보기/선택/저장.
- Sprint1022AudioProbe: HOME/도감/GAME 음악 전환·BGM/SFX 설정·볼륨·Pause/Resume.
- MissingScriptDiagnostics: Missing Script 0.

최종 스타일 최적화 후 GAME 339항목을 재실행하여 PASS. 슬롯 빈 공간의 실제 Raycast 36항목을 추가했다. 빈 셀과 슬롯 메시 스타일은 상태/크기 변경 때만 갱신해 매 프레임 재생성을 방지한다.
Compiler Warning/Error 및 정상 Runtime Warning/Error 0. 저장 실패를 의도적으로 주입하는 기존 회귀의 예상 Warning은 일반 Runtime Warning과 구분했다.
씬 감사: GAME 전용 범위 밖의 Unity 자동 직렬화 4개 블록을 이전 상태로 복원했고, 기존 GameSession/BoardView/BlockPiece/BlockDragHandler/SlotDragHandler/PlanetRestorationView/GameFlowController/GameAssistView/GameFeedbackController/GameVisualPresentation 데이터 및 연결 불변을 확인했다.

## 미진행
Android APK 빌드·실기기 설치/테스트·Git Commit/Push는 진행하지 않았다. 기기의 실제 가독성·터치 느낌은 사용자 확인이 필요하다.

## 2026-10-11 후속 Commit/Push 및 Android 빌드
- UI 구현 커밋: 5ab1bdf806e3dabe2daa074147fde55380910619 (feat: unify home collection and game mesh UI).
- origin/main 일반 Push 성공 후 Development 빌드 수행. Remote/Force Push 변경 없음.
- Unity BuildResult Succeeded, Build Error 0, Build Warning 3.
- Warning은 기존 TMP_TextParsingUtilities.cctor / TextMeshPro.GenerateTextMesh / TextMeshProUGUI.GenerateTextMesh의 대형 메서드 C++ 파일 분리 안내.
- APK: Builds/Android/Development/CosmicBlock-dev.apk, 152,238,580 bytes.
- SHA-256: C797D6C802CCA2307A23197E31E15BCA1C54AE8DD8EF15CE6E92093AE2F56F6E.
- 빌드 로그: Validation/mesh-ui-android-build.log.
- Portrait/ARM64/IL2CPP 및 기존 Package Name 유지.
- 실기기 설치·테스트 미진행. APK와 디버그 심볼은 Git 제외.
