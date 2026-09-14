# COSMIC BLOCK
PLAY YOUR NEXT WORLD의 첫 Android 출시작. Sprint 0은 상태 관리가 가능한 8×8 보드 기반까지 구현한다.

## 시작
Unity Hub에서 이 폴더를 Unity 6000.5.8f1로 열고 Assets/Scenes/Game.unity를 연 뒤 Play.
Scene이 없으면 COSMIC BLOCK → Sprint 0 → Create Board Prototype.
검증 메뉴: COSMIC BLOCK → Sprint 0 → Validate Prototype.

## 기술 기준
- 기존 Unity 6000.5.8f1 고정. 설치 버전과 프로젝트 버전 일치. LTS로 단정하지 않으며 업그레이드하지 않는다.
- URP 17.6.0 / 기존 2D Renderer 유지. Quality 모두 기존 URP 사용; Graphics 기본에도 연결.
- uGUI 2.5.0 / New Input System 1.20.0 (activeInputHandler=1).
- Portrait, Canvas 1080×1920, SafeArea 하위 UI, 보드 정사각형.
- Android Build Support, SDK, NDK, OpenJDK 설치 확인.
- SDK platforms: 34, 36, 37.0. Target API 36 명시; 자동 최고 버전 사용 방지.
- minSdk 26 유지: 기존 값이며 Unity 6.5 Android 최소 지원 기준. 불필요한 상향 없음.
- Google Play API 36 요구 확인(2026-09-14): https://support.google.com/googleplay/android-developer/answer/11926878?hl=en
- Unity Android 최소 기준 참고: https://discussions.unity.com/t/planned-breaking-changes-in-unity-6-5-updated-2026-03-27/1694205
- Git main / Unity .gitignore. Library, Logs, UserSettings, 빌드 출력 제외.

## 검증
1. Game Scene → Play → 8행×8열, 빈 Cell 64개 확인.
2. Game View 해상도 1080×1920, 720×1280, 1080×2400 및 4:3에서 보드 정사각형/중앙 정렬 확인.
3. Device Simulator가 설치된 경우 노치 기기 선택; SafeArea 내부에 상단과 하단 표시가 남는지 확인.
4. Console Error가 없어야 한다. 자동 검증 PASS는 정적 Scene/모델 검증이며 실기기 렌더링을 보증하지 않는다.
5. 상태 API: GameSession.Model.SetOccupied(0,0,true); 해당 Cell만 보라색. Clear()로 초기화.
6. 좌표 원점은 좌상단, x 오른쪽, y 아래쪽. 터치/마우스 UI 입력 모듈만 준비; 드래그는 다음 Sprint.

Android APK/AAB 빌드와 실기기 설치는 이번 검증에 포함하지 않는다. 출시 전 application identifier, 서명, ARM64/IL2CPP, AAB, 광고 정책을 별도 확정한다.
기존 SampleScene과 Settings Asset은 보존한다.


## 생성/변경 파일
- Assets/Scripts/Board/BoardModel.cs: 8×8 Cell 상태와 변경 event.
- Assets/Scripts/Board/BoardView.cs: 64개 Image 연결/상태 표시/Grid 크기.
- Assets/Scripts/Core/GameSession.cs: Prototype 상태와 모델 소유.
- Assets/Scripts/UI/SafeArea.cs: Screen.safeArea를 UI anchor에 반영.
- Assets/Scripts/UI/SquareBoardLayout.cs: SafeArea 크기에 맞는 정사각형 보드.
- Assets/Editor/Sprint0Builder.cs: Scene 생성 및 정적 검증 메뉴.
- Assets/Editor/Sprint0PlayProbe.cs: 배치 Play Mode 연결/레이아웃/상태 표시 검증. -executeMethod Sprint0PlayProbe.Run (자동 종료하므로 대화형 Editor에서는 실행하지 않는다).
- Assets/Scenes/Game.unity와 관련 .meta/폴더 .meta: Unity API가 생성한 Scene/reference.
- ProjectSettings/ProjectSettings.asset: Portrait, 1080×1920, Android Target 36.
- ProjectSettings/GraphicsSettings.asset: 기본 URP 연결.
- ProjectSettings/EditorBuildSettings.asset: Game Scene 등록.
- .gitignore와 루트 Markdown 9개: 버전 관리/제품/디자인/작업 인수인계.

자동 검증 결과: Scene/64 Cell/상태 event/경계 검사 PASS. Play Mode 모델 연결/정사각형/색상 갱신·초기화 PASS.
시각적 화면 QA와 Android 빌드는 별도로 수행해야 한다.
