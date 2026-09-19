# COSMIC BLOCK
PLAY YOUR NEXT WORLD의 첫 Android 출시작.
The primary goal of COSMIC BLOCK v1.0 is shipping.

## 현재 상태
Sprint 0(d85d878), Sprint 1(061ed6e)에 이어 Sprint 2 Core Game Loop 구현.
블록 배치 → 가로/세로 동시 제거 → Score/Combo/Best → 소비 → 3개 재공급 → 남은 블록 배치 가능 여부 검사.
Game Over에서는 Retry만 가능하며 광고 버튼은 비활성 Placeholder다.

## 시작
Unity Hub → Add → 이 프로젝트 폴더 → Unity 6000.5.8f1.
Project 창 → Assets → Scenes → Game.unity 더블클릭 → Play.
UI/Inspector 참조는 이미 Editor 자동화로 구성되어 있다. 현재 단계에서 수동 Unity 구성 작업 없음.

## 기술 기준
Unity 6000.5.8f1, URP 17.6.0/기존 2D Renderer, uGUI 2.5.0, New Input System 1.20.0 유지.
Portrait, Canvas 1080×1920, SafeArea, 8×8 정사각형. Grid spacing 12/padding 8.
Android Target 36/minSdk 26. SDK/NDK/OpenJDK 설치 확인. Sprint 2에서 버전/SDK/패키지 변경 없음.
Google Play API 기준 확인(2026-09-14): https://support.google.com/googleplay/android-developer/answer/11926878?hl=en
기존 SampleScene/Settings/Board/Drag 구조 보존. 외부 의존성 추가 없음.

## 코드/변경 파일
- BoardModel: 점유/배치 검사/전수 검색, ClearCompletedLines로 완성 Row/Column을 먼저 수집 후 일괄 제거.
- LineClearResult: ClearedRows/ClearedColumns/UniqueClearedCells/LineCount. 교차 Cell은 한 번.
- BoardView: 기존 64 Image/좌표 mapping/preview 재사용. Clear event로 Cell 표시 갱신.
- GameSession: Core Loop 순서/상태/Score/Combo/Best/Set 공급/Game Over/Retry.
- ScoreRules: placed cell 10, line 100, Combo bonus step 50을 한 곳 관리.
- BlockShape/BlockCatalog/BlockGenerator: 기존 8종 offset pool/독립 랜덤 선택/seed 재사용.
- BlockPiece: 반복 공급 시 기존 Cell Visual 비활성화/삭제 후 새 Shape 표시, 같은 슬롯 root 재사용.
- BlockDragHandler: drag를 먼저 종료/복귀한 뒤 GameSession.TryPlacePiece 호출. 재공급한 root를 다시 숨기지 않는다.
- GameHud: Score/Best/Combo/Game Over 표시 및 Retry 버튼 연결.
- Sprint2Builder: 기존 Scene의 Score/Combo/Panel/Button/Hud 참조 자동 연결.
- Sprint2PlayProbe: Test 1~13, synthetic Mouse/Touch uGUI 이벤트, 반복 재공급/입력 잠금/Retry/화면비 렌더.
- Game.unity 및 관련 .meta: Unity API가 생성/연결. Scene YAML 직접 편집 없음.
- CURRENT_TASK/NEXT_TASK/DEVLOG/GAME_DESIGN/DESIGN_SYSTEM/ROADMAP/README: 인수인계/결정/검증/QA.

## 규칙
유효 배치 점수 = Cell 수×10 + 완성 Line 수×100 + (Line Clear 발생 시 max(0,Combo−1)×50).
Combo는 연속 Line Clear 유효 배치 횟수. Line Clear 없는 유효 배치에서 0. 실패/취소는 변경 없음.
Score가 Best를 초과하면 PlayerPrefs의 CosmicBlock.BestScore에 즉시 저장. Retry에서 Best 유지.
세 슬롯 모두 소비했을 때만 재공급. 제거/재공급 후 남은 Shape 중 하나라도 보드 어디든 들어가면 Playing.
Score는 int 범위를 넘기지 않도록 상한 처리한다.

## 좌표/Drag
좌상단 (0,0), X 오른쪽/Y 아래쪽. Shape offset은 bounding-box 좌상단 기준.
Canvas local pointer + 위쪽 offset → Piece 원점 중심 → Board local → 실제 Cell 중심/Grid pitch로 nearest anchor.
Visual snap/preview/drop은 같은 anchor. GameSession Inspector Drag Finger Offset=110 Canvas 단위.
Preview Gold=valid, Red=invalid. 원점이 보드 밖이면 preview 없음.
New Input System의 uGUI 이벤트로 Mouse/Touch를 처리하고 동시 drag를 하나로 제한한다.

## 직접 QA
1. Game Scene → Play → Score 0, 저장 Best, 랜덤 블록 3개 확인.
2. 마우스 왼쪽 드래그 → Gold 위치에 release → 점유 및 Cell당 +10. Red/보드 밖 release → 복귀/점수·Combo 변화 없음.
3. 세 블록을 모두 배치 → 새 3개. 하나/두 개만 사용했을 때는 공급 없어야 한다.
4. Hierarchy → GameSession 선택 → Inspector GameSession 컴포넌트의 ⋮ 또는 우클릭 → Debug → Prepare Row And Column Clear.
   새 게임/Single 3개/4번째 Row·Column 빈 교차점을 준비한다. 그 교차점(좌표 3,3)에 Single을 drop.
   두 Line/15 unique Cell 제거, Score 210, COMBO 1 확인.
5. 같은 컴포넌트 메뉴 → Debug → Prepare Next Row Clear → 같은 빈 Cell(3,3)에 Single drop.
   Score 370, COMBO 2. 반복하면 580/STAR COMBO 3, 840/COSMIC COMBO 4.
   그 후 빈 곳에 Line Clear 없이 배치 → Combo text 사라짐.
6. 같은 메뉴 → Debug → Force Game Over.
   Game Over의 Current/Best 확인, 블록 입력 잠금, 광고 버튼 비활성 확인.
7. RETRY 클릭 → 빈 보드/Score 0/Combo 없음/새 3개/Playing, Best 유지.
8. Play 종료 후 다시 Play → 저장 Best가 다시 표시되어야 한다.
9. Game View 해상도 + → Fixed Resolution: 1080×1920, 1080×2400, 1080×1440.
   Board 정사각형, Score/Combo/Block Area 구분, Game Over Card/Retry가 SafeArea 안에 있는지 확인.
10. Console 게임 코드 오류/예외가 없어야 한다. Android 실기기 Touch/다중 터치/실제 노치 및 빌드는 별도 QA한다.

Debug ContextMenu는 Editor/Development Build에만 포함된다. 출시 게임 화면에는 Debug Text/버튼을 추가하지 않는다.
재현용 랜덤: Play 전에 GameSession → Use Fixed Seed 체크 → Fixed Seed 지정. Retry는 같은 seed의 새 게임을 시작한다.
기존 플레이어 Best를 초기화하지 않는다. 테스트 Probe는 별도 키를 사용하고 테스트 키도 종료 시 이전 값으로 복구한다.

## 자동화/테스트
COSMIC BLOCK → Sprint 2 → Upgrade Core Loop UI: 기존 Scene 확장. 반복 실행 시 중복 Panel/Slot/Cell 없음.
COSMIC BLOCK → Sprint 2 → Validate Line And Score Rules: Row/Column/다중/교차 중복/점수/배치 가능 검색.
배치 전용: -executeMethod Sprint2PlayProbe.Run. 성공 시 Validation/sprint2.txt에 SPRINT2_PLAY_PASS.
Probe는 Editor를 종료하므로 대화형 Editor에서는 실행하지 않는다.
이전 Sprint 1 domain 검증은 여전히 사용 가능하다. Sprint 1 PlayProbe는 당시 규칙(재공급 없음)의 역사적 테스트로 현재 Core Loop에는 사용하지 않는다.
Validation 이미지/로그는 .gitignore로 제외된다. RenderTexture 렌더/SafeArea 모사/synthetic 이벤트는 하드웨어 입력·Android 빌드를 대체하지 않는다.

## 후속 작업
Visual/Audio Polish는 승인 후 별도 Sprint. Localization/최종 한글 폰트/Home/AdMob/실기기 Release 설정은 후속 계획.
최종 Art를 임의 생성하지 않는다. 현재는 코드 색상과 영문 placeholder/LegacyRuntime 폰트다.
시작 시 Git main/working tree clean, remote 없음. Remote repository를 임의 생성하거나 force push하지 않는다.

최종 Test 1~13/Play Mode 검증 PASS. Local Commit과 Remote 상태는 DEVLOG 및 작업 완료 보고 참고.

## Sprint 3 직접 확인
1. Assets/Scenes/Game.unity를 열고 Play.
2. Hierarchy에서 GameSession을 선택한 뒤 Component Context Menu의 Debug/Journey/Set Score 950, 2950, 5950, 9950을 사용한다.
3. 다음 유효 배치로 milestone을 넘기거나 DebugSetScore를 호출해 도달 카드가 한 번만 표시되는지 확인한다.
4. Retry 후 START와 유지된 Best Journey를 확인한다.

## Sprint 4 Feedback 직접 확인
1. Game Scene Play 후 GameSession의 Debug/Prepare Next Row Clear를 실행하고 (3,3)에 Single을 배치한다.
2. Gold Flash, Cell Pop, 8개 Star, 실제 +Score, CLEAR!, 짧은 Clear SFX를 확인한다.
3. 같은 준비/배치를 연속 실행해 STAR COMBO, COSMIC COMBO와 소폭 상승한 Pitch를 확인한다.
4. Debug/Prepare Row And Column Clear 후 교차점에 배치해 15개 unique Cell과 한 번의 SFX/Haptic 요청을 확인한다.
5. 연출 중 다음 Piece를 즉시 Drag하고, Retry 시 모든 임시 연출이 사라지는지 확인한다.
6. Android 실기기에서는 Line Clear에만 짧은 진동이 발생하는지 확인한다. Editor에서는 Haptic이 no-op이다.

Clear SFX 교체 경로는 Assets/Audio/SFX/clear.wav다. Feedback 객체는 Sprint4Builder가 중복 없이 구성하며 새 Tween/Audio/Haptic 플러그인은 없다.

## Android Development Build
Unity 메뉴 `COSMIC BLOCK → Build → Android Development APK`로 `Builds/Android/Development/CosmicBlock-dev.apk`를 생성한다. 현재 검증 설정은 Portrait, minSdk 26, targetSdk 36, IL2CPP, ARM64, Vulkan/OpenGLES3, New Input System, version 1.0/code 1이다.

현재 패키지명은 `com.DefaultCompany.CosmicBlock`이다. QA 동안 유지하며 출시 서명/AAB 전에 `com.playyournextworld.cosmicblock` 같은 최종 식별자로 확정한다. APK와 symbol zip은 Git에 포함하지 않는다.

## Android 실기기 QA 체크리스트
1. 개발자 옵션과 USB 디버깅을 켜고 USB 연결 후 기기의 RSA 허용 창을 승인한다.
2. Unity 포함 ADB의 `adb devices -l`에서 기기 상태가 `device`인지 확인한다.
3. Development APK를 `adb install -r`로 설치하고 Portrait로 실행한다.
4. 상단 행성/하단 지형, 중앙 Board, STAR JOURNEY, 세 Block Slot이 실제 SafeArea 안에서 선명한지 확인한다.
5. 손가락 Drag가 자연스럽고 Gold/Red preview와 drop 위치가 일치하는지 확인한다. 빠른 반복과 멀티터치에서도 입력이 잠기지 않아야 한다.
6. Row/Column/Cross Clear에서 Gold Flash, Cell Pop, Star Burst, Score/Combo가 0.68초 안에 표시되고 플레이 입력을 막지 않는지 확인한다.
7. Clear SFX의 음량과 combo pitch, Line Clear에만 발생하는 짧은 진동의 강도/지속이 적절한지 확인한다.
8. 앱 일시정지/복귀, 화면 껐다 켜기, Game Over/Retry 후 입력·Audio·임시 연출이 정상인지 확인한다.
9. 장시간 반복 플레이 중 프레임 저하, 발열, 비정상 종료가 없는지 확인한다.
10. 실행 직전 Logcat을 비우고 앱 PID 중심으로 Fatal/Exception/Unity 오류를 확인한다. 시스템 전체 로그는 보관하지 않는다.

2026-09-18 자동 검증 결과는 APK build Warning 0/Error 0, Sprint 0~4/Visual Readability PASS, Missing Script 0이다. 당시 ADB 연결 기기가 없어 위 실기기 항목은 아직 수동 확인 전이다.

## Sprint 4.5 Device UX 개선
- Bottom Slot 전체가 해당 Piece의 Touch/Drag 영역이다. 소비된 Slot은 raycast가 비활성화된다.
- Piece 시각 크기와 Drag Offset 110, drag scale 1.05, Gold/Red preview는 유지된다.
- HUD Font Size: COSMIC BLOCK 56, BEST/SCORE 44 Bold, Journey 30.
- 자동 검증은 Single/H2/V2/2x2/L/Reverse L, consumed slot, 1~5자리 점수와 1080×1920/2400/1440을 포함한다.
