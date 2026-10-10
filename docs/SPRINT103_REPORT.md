# Sprint 10.3 — HOME 종료 확인 팝업

## 범위 및 기존 동작

HOME의 QuitConfirmPanel만 교체했습니다. 기존 HomeViewController의 ShowQuit/CancelQuit/ConfirmQuit 이벤트와 GameFlowController.RequestQuit를 재사용합니다. 종료는 Run 초기화가 아니며 Home 이동 시 이미 저장된 Run 및 GameSession.OnApplicationQuit의 SuspendAndSave 경로를 유지합니다. Android Application.Quit, Editor QuitRequested 표시 정책을 바꾸지 않았습니다. ConfirmQuit에 팝업이 닫힌 뒤 중복 호출을 무시하는 가드만 추가했습니다. Android Back 정책은 기존대로입니다.

## 자산 및 Alpha

Assets/Art/UI/Popup/popup_frame_cosmic.png (1536×1024), popup_button_blue.png 및 popup_button_gold.png (각 2172×724)를 사용합니다. 세 파일 모두 실제 RGBA이고 버튼 Alpha는 0~255, 프레임은 0~254입니다. 바깥 대표 지점은 Alpha 0이며 (0,0)에 Alpha 1의 미세 잔여값이 있습니다. 따라서 외부 모든 픽셀이 정확히 0이라고 단정하지 않습니다. 렌더에서는 불투명 사각 배경이나 체크무늬가 보이지 않았고, 별/발광 장식과 내부 우주 배경을 보존하기 위해 원본은 가공하지 않았습니다. 내부 일부 반투명 영역에서 HOME 글자가 비쳐 프레임 안쪽에만 어두운 InteriorBacking을 추가했습니다.

Unity Import: Sprite Single, Alpha FromInput/IsTransparency ON, Mipmap OFF, Bilinear, Uncompressed, MaxSize 2048. PNG 원본 바이트 변경 없음.

## 구조

기존 직렬화 참조와 회귀 테스트 경로 호환을 위해 QuitConfirmPanel / Card / Cancel / Confirm 이름을 유지했습니다. 논리적으로 ExitConfirmPopup / PopupPanel / CancelButton / ExitButton에 해당합니다.

QuitConfirmPanel
- DimOverlay: 검정 Alpha .72, Canvas 전체 화면을 덮으며 바깥 터치 차단
- Card: 중앙 정렬, HomeQuitPopupLayout으로 Safe Area 기준 3:2 비율
  - InteriorBacking
  - CosmicFrame (preserveAspect)
  - TitleText: 게임을 종료할까요? / TMP 골드 Bold
  - DescriptionText: 현재 게임은 저장되어\n다음에 이어서 플레이할 수 있어요. / TMP 밝은 흰색
  - Cancel: 기존 Button/PressFeedback + Blue PNG + TMP 취소
  - Confirm: 기존 Button/PressFeedback + Gold PNG + TMP 종료

NotoSansKR-VF.ttf의 SIL OFL 라이선스 정보를 함께 보존했고, 팝업 문구의 한국어 글리프를 넣은 정적 HomeQuitKorean TMP Font Asset을 추가했습니다. 다른 UI에는 적용하지 않았습니다.

## 검증

Sprint103QuitProbe 최종 PASS(35 assertions, Warning 0). 실제 HOME 종료 Button, 팝업 표시, 취소, 기존 종료 요청, 반복 표시/취소와 중복 종료 가드, EventSystem Raycast로 뒤쪽 HOME 입력 차단, Run/Score/소비 상태/Best/행성/BGM 설정 보존 및 종료 이후 이어하기 검증 PASS.

1080×1920, 1080×2400 + Safe Area, 1080×1440 모두 프레임 비율/경계/한국어 글리프/텍스트 잘림/버튼 동일 크기·비겹침 PASS. 실제 렌더 육안 확인 후 제목을 상단 보석 아래로 내려 여백을 확보했습니다.

관련 기존 RunSavePlayProbe, Sprint1022AudioProbe, MissingScriptDiagnostics 모두 PASS. Compiler Warning/Error 0, 테스트 Runtime Warning/Error 0, Missing Script 0. Scene YAML 비교에서 종료 팝업 외 Unity 자동 직렬화 변경 4개 블록은 원복했고, 팝업 외 변경 0을 확인했습니다. 사용자 Run은 별도 파일로 격리하고 PlayerPrefs는 백업·복원합니다.

## 파일 및 상태

- Assets/Scenes/Game.unity
- Assets/Scripts/UI/HomeViewController.cs: 중복 종료 가드
- Assets/Scripts/UI/HomeQuitPopupLayout.cs 및 meta
- Assets/Editor/Sprint103QuitProbe.cs 및 meta
- Assets/Art/UI/Popup의 기존 사용자 PNG에 생성된 Import meta (PNG 원본 보존)
- Assets/Fonts의 Noto Sans KR, 라이선스, TMP Font Asset 및 meta
- CURRENT_TASK/NEXT_TASK/DEVLOG 및 docs 사본, 본 보고서

임시 제작 코드는 최종 변경에서 제거했습니다. Android 빌드/설치/실기기 테스트 및 Git Commit/Push는 미실행. 변경은 미커밋 상태입니다. 외부 Alpha 1 잔여값 외 Editor 검증에서 남은 문제는 없습니다. 실기기 디자인/종료 확인은 사용자 별도 요청 대기입니다.

렌더: Validation/sprint103_quit_1920.png, sprint103_quit_2400.png, sprint103_quit_1440.png.

## 2026-10-10 후속 승인 — APK 설치 및 Git 반영

사용자가 기기 설치와 Commit/Push를 승인했습니다. 실기기 테스트는 사용자 담당입니다. 위 미실행 기록은 최초 Editor 작업 시점의 이력입니다.

- Sprint 10.3 포함 Development APK 빌드 성공. Build Error 0, 기존 TextMeshPro 대형 메서드 C++ 파일 분리 경고 3.
- APK: Builds/Android/Development/CosmicBlock-dev.apk, 152,162,010 bytes, SHA-256 44cc54d6a751e3759db950a22df370a1ef5b82b602f5ea65f79b0faf657bc0db.
- SM-S942N / Android 16 / ADB device, adb install-r Success.
- 설치 전후 PlayerPrefs와 current-run.json의 바이트 동일 확인. 저장 데이터 삭제 없음.
- 앱 실행·터치 조작·Logcat 및 실기기 기능 테스트는 하지 않았습니다. 팝업 가독성·취소/종료·재실행 이어하기는 사용자 확인 대기입니다.
- 빌드 자동 설정 변경 및 임시 파일은 원복·정리했습니다. 의미가 동일한 Scene 블록의 직렬화 공백을 원복해 변경 범위를 줄였습니다.
- 사용자 승인에 따라 일반 Commit/Push 반영합니다. Force Push/Remote 변경 없음.
