# Devlog
## 2026-09-14 — Sprint 0
- 사용자 제공 최종 지시문을 확인하고 Sprint 0 범위로 제한.
- 기존 프로젝트는 Unity 6000.5.8f1, URP 17.6.0, uGUI 2.5.0, Input System 1.20.0.
- 기존 URP 2D Renderer와 SampleScene 보존. Unity 버전 변경 없음.
- Android Build Support/SDK 36/NDK/OpenJDK 확인. Target 36, minSdk 26, Portrait 기준 적용.
- Git main 초기화 및 Unity ignore 구성.
- bool[8,8] 모델/상태 변경 event/셀 표시 분리. 좌상단 원점.
- Editor API로 Game Scene/Canvas/SafeArea/Grid/64 Cell/EventSystem/reference/build scene 생성.
- 블록 생성/드래그/게임 규칙/광고/아트/사운드/효과 없음.
- 작은 자체 현지화 구조를 선택하되 실제 구현과 한글 폰트는 후속 UI Sprint.
- 샌드박스 helper setup refresh 오류로 기본 파일 쓰기/실행 불가; 승인된 외부 실행으로 작업.

- 검증 완료: Unity 배치 컴파일 및 Game Scene 로드 PASS; 64 Cell, 모델 독립성/event/경계 검사 PASS.
- Play Mode PASS: GameSession 모델 연결, 보드/Cell 정사각형, 상태→Image 색상 갱신과 초기화. 검증 보드 723.3044×723.3044 Canvas 단위.
- 게임 코드 컴파일 오류/예외 없음. Unity 라이선스 연결 초기 오류 후 정상 기동; 로그에 종료 중 Curl 메시지가 남음. Android 빌드/실기기/노치 육안 QA는 미실행.
- 현재 단계에서 수동 Unity 구성 작업 없음. Game Scene은 이미 생성되고 Build Settings에 등록됨.
- 프로젝트 생성 파일과 기존 템플릿을 포함한 최초 커밋: project foundation: Sprint 0 board prototype and documentation.
- Git 저장소가 sandbox 계정 소유여서 외부 실행에서 safe.directory를 프로젝트 한정 -c 옵션으로 적용. 전역 Git 설정 변경 없음.

## 2026-09-14 — Sprint 1 / Block Placement
- 시작 working tree clean, 기준 commit d85d878. 사용자 Sprint 1 지시문 범위로 진행.
- 기존 BoardModel/BoardView/GameSession에 최소 추가. Scene YAML 직접 수정 없이 Unity API로 Game Scene 확장.
- 불변 Shape offset 8종과 작은 System.Random generator. 독립 선택/중복 허용, Inspector seed 지원.
- uGUI EventSystem의 Mouse/Touch callbacks, Canvas drag layer, finger offset 110, 실제 Grid 기반 nearest anchor mapping.
- preview와 drag visual snap 및 drop이 동일한 anchor 사용. 모델 CanPlace/TryPlace로 UI와 판정 분리.
- 점유 검사 실패의 원자성, 성공 시 해당 Slot만 소비. 3개 모두 사용 후 공급 없음.
- 포인터 id 고정/동시 drag lock, Escape/focus/pause 취소. 비활성화 중 SetParent 오류를 발견하여 session-hosted next-frame 복귀로 해결.
- 컴포넌트 RequireComponent 인자 오류 및 테스트의 CanvasScaler Update 호출 오류 수정. 설치된 uGUI source에서 Canvas.preWillRenderCanvases 갱신 방식 확인.
- spacing 8/12를 실제 Shape와 함께 렌더/육안 비교하여 12 선택. padding/전체 보드 비율 유지.
- 자동 domain: Single/H3/L/Reverse L, 범위/negative/overflow, 점유 충돌, 모든 Shape edge, full row 유지, deterministic seed pool PASS.
- Play Mode: 지시문 Test 1~8, UI raycast, preview 불변/visual anchor 일치, invalid return, cancellation/multi-pointer, deferred disable return, 슬롯 소비/자동 공급 없음 PASS.
- 렌더: 1080×1920 / 1080×2400(모사 SafeArea inset) / 1080×1440, 정사각형 보드/64 mapping/슬롯 bounds PASS.
- synthetic uGUI 이벤트 검증이며 실제 Mouse/Android touch 입력 전체 경로와 APK/AAB 빌드는 미실행. 실제 노치/UX QA는 README 절차로 안내.
- Unity 자동화 완료: 현재 단계에서 수동 Unity 작업 없음. 기술 기준/패키지/기존 SampleScene 보존, 의존성 추가 없음.
- 다음 후보: Line Clear → Score → Combo → New Block Set → Game Over Detection. 승인 전 진행하지 않는다.
- 최종 재검증: Scene upgrade 반복 실행 후 슬롯 중복 없음; domain/Play Mode 전체 PASS, 런타임 오류 없음. 긴/짧은 화면 비교 이미지도 육안 확인. 검증된 Sprint 1을 gameplay: add block drag and placement로 커밋.

## 2026-09-14 — Sprint 2 / Core Game Loop
- 기준 061ed6e, main, 시작 clean. git remote -v 결과 없음. 원격 저장소 생성/force push 없음.
- BoardModel/BoardView/Shape/Drag/슬롯 구조 유지. GameSession에 Core Loop를 집중하여 Manager 추가 최소화.
- ClearCompletedLines: Row/Column을 모두 수집 후 unique union clear. 결과 rows/columns/unique cells 제공.
- 점수 상수 ScoreRules(Cell10/Line100/ComboStep50), Combo 연속 Clear 정의. 실패/취소 불변.
- Best key CosmicBlock.BestScore, PlayerPrefs 기록 초과 시 SetInt/Save; 시작 GetInt, Retry 유지.
- Playing/Resolving/GameOver. handler를 먼저 복귀/종료하고 중앙 TryPlacePiece 호출하여 Set refill과 consume 충돌 방지.
- 슬롯 root 재사용, 이전 Cell image 비활성화/Destroy 후 새 Shape 표시. 3개 모두 소비할 때만 공급.
- Clear→Score/Combo/Best→Consume→Refill→Remaining search 순서. Clear 전 Game Over 판정 없음.
- GameHud 및 Sprint2Builder로 기존 Scene에 Score/Combo/Game Over/Retry/ad placeholder 자동 연결.
- 개발용 ContextMenu로 동시 Clear/연속 Combo/Game Over 재현. Release에서 제외.
- domain Row/Column/교차15/다중 rows/columns/4-line28/전체64/점수/전수 검색 PASS.
- Play Mode 검증 중 레이아웃 갱신 전 Retry world 좌표를 사용한 테스트 오류를 발견하고 갱신 후 현재 위치를 계산하도록 수정.
- Best 검증은 별도 테스트 namespace key로 실행/복원하여 실제 플레이어 기록을 변경하지 않는다.
- 패키지/Unity/SDK/Portrait/SafeArea/spacing 변경 없음. 광고/효과/사운드/최종 아트 구현 없음.
- 모달 활성화 직후 Graphic depth=-1인 Unity UI lifecycle을 확인. 실제 렌더 프레임을 기다린 뒤 Retry raycast/onClick 검증 PASS. 게임 코드의 입력 오류가 아니라 같은 프레임에 수행한 테스트 타이밍 문제였다.
- Test 1~13 PASS: Row/Column/다중/동시 clear, Score/Combo, 새 Set, 남은 블록 전체 검색, Game Over, Retry, Best 저장.
- UI/추가 검증 PASS: 1080×1920, 1080×2400(모사 inset), 1080×1440; 모델/64 mapping/Slot/Card bounds, invalid/cancel 불변, 12회 추가 재공급, 이전 Visual 제거, 잔상/게임 런타임 오류 없음.
- Score/Combo 및 Game Over의 9:16/짧은 화면 PNG를 육안 검토. 실제 Android touch/노치/APK·AAB는 미실행.
- 현재 단계에서 수동 Unity 구성 작업 없음. 사용자 직접 QA 및 Debug 메뉴 재현 방법은 README.
- Remote 없음: Local Commit만 수행하며 Remote Push는 미완료(대상 없음). 원격 저장소를 임의 생성하지 않는다.
- 최종 Scene upgrade 반복 실행/전체 Play Mode 검증 PASS. 게임/문서를 gameplay: complete core game loop로 Local Commit.

## 2026-09-15 — Sprint 3 / STAR JOURNEY + Cosmic Visual Foundation
- 시작 main/origin/main clean, 기준 309868f.
- JourneyProgress 단일 데이터 소스: START 0 / STAR FIELD 1,000 / MOON 3,000 / SATURN 6,000 / DEEP SPACE 10,000.
- Score에서 current/next/segment progress 계산. 마지막 목적지 이후 full 상태로 게임 지속.
- Run별 milestone HashSet과 uGUI coroutine fade/scale reached feedback. Retry reset, Best Score 기반 Best Journey.
- Game Over에 current route/progress와 Best Journey 추가.
- 최종 배경 asset 없이 replaceable Image 기반 Deep Navy foundation. Board/preview와 Piece Blue/Purple/Gold, 약한 Slot 배경 적용.
- Sprite/Texture/Bloom/Particle/Sound/Haptic/광고/Stage 기능 추가 없음.
- Sprint3 domain/Play Mode Test 1~10/12 PASS. Sprint2 전체 Core regression 및 1080x1920/2400/1440 SafeArea/정사각형/64 mapping PASS.
- 개발 빌드/Editor 전용 score presets 950/2950/5950/9950 제공.

## 2026-09-16 — Visual Readability & Drag Preview Polish
- Preserved final Cosmic Background, Aspect Fill, global Navy overlay, STAR JOURNEY and all game rules.
- Added Board blue-black container and subtle border; 64 empty cells now use dark fill plus cyan outline.
- Added low-cost uGUI highlight/shade to generated block cells.
- Restored three clear independent slot panels without reducing touch areas. Dragging slot uses warm-gold outline.
- Board preview reuses Shape offsets and BoardModel.CanPlace. Valid uses gold fill/outline; collision and boundary use one red state.
- Drag visual scale 1.05; nearest edge-cell mapping absorbs that visual offset while retaining 0..7 bounds.
- Automated visual tests 1-12, Sprint 2 Core and Sprint 3 Journey regression passed.
- 1080x1920, 1080x2400 with simulated inset, 1080x1440 renders reviewed. CS warnings/errors: 0/0.

## 2026-09-17 — Sprint 4 / Juice & Feedback
- 기준 cd963d1, main/origin/main clean. 게임 규칙과 STAR JOURNEY 변경 없음.
- GameFeedbackController가 확정된 LineClearResult/실제 점수 차이/Combo만 받아 0.68초 이내 비차단 연출.
- 64 Clear Cell + 24 Star UI Pool 재사용. 교차 Clear는 UniqueClearedCells로 한 번만 표시.
- Gold Flash 0.10초, 1.16 Pop/Fade, Gold/White/Blue Star Burst, Score 상승/Fade, CLEAR!/STAR COMBO/COSMIC COMBO.
- 자체 생성 Assets/Audio/SFX/clear.wav, 단일 AudioSource 재시작과 pitch 1.00/1.05/1.10. Android에서만 짧은 Handheld.Vibrate 요청.
- Drag Gold Outline 두께를 2.5에서 3.25로 소폭 강화.
- 실제 Row/Column/Cross/연속 Combo/Rapid Drag/Retry/Game Over/Pool cleanup 및 1080×1920/2400/1440 렌더 PASS.

## 2026-09-18 — Sprint 4.5 / Android Device QA
- Unity 6000.5.8f1 Android Build Support, SDK/NDK/OpenJDK와 ADB 실행 경로 확인.
- 현재 Android 설정 확인: Portrait, minSdk 26, targetSdk 36, IL2CPP, ARM64, Vulkan/OpenGLES3, New Input System, version 1.0/code 1.
- 현재 패키지명은 `com.DefaultCompany.CosmicBlock`. 이번 QA에서는 변경하지 않았으며 출시 전 `com.playyournextworld.cosmicblock` 같은 최종 식별자로 교체 권장.
- Development/Release APK 메뉴와 배치 빌드용 `AndroidBuildAutomation` 추가. Development는 Unity 6.5 `DebugSymbolLevel.SymbolTable`을 사용.
- 불필요한 Unity Engine Diagnostics를 비활성화해 Cloud symbol upload 경고를 제거. 게임 동작과 Android 런타임 설정은 변경하지 않음.
- Development APK 생성 성공: `Builds/Android/Development/CosmicBlock-dev.apk`, 89,878,495 bytes. BuildReport Warning 0/Error 0.
- Sprint 0/1/2/3/4 PlayProbe와 Visual Readability PASS. Missing Script 0. 컴파일 CS Warning/Error 0.
- ADB daemon은 정상 시작했지만 연결 기기 없음. 따라서 설치/실행/Logcat과 실제 Portrait/SafeArea/Touch/Audio/Haptic/성능 QA는 대기.
- Game Feel 값 변경 없음: 0.68초 전체, 0.10초 Flash, 0.18초 Pop/최대 1.16배, SFX 0.58, pitch 1.00/1.05/1.10, Drag outline 3.25, Android line-clear vibration 요청 유지.

## 2026-09-19 — Sprint 4.5 / Device UX Touch Target + HUD Readability
- Android 실기기 피드백에 따라 Piece 시각 크기 변경 없이 기존 Bottom Slot RectTransform 전체를 drag hit area로 사용.
- SlotDragHandler가 기존 BlockDragHandler로 begin/drag/end/cancel을 전달. Piece 재공급 시 활성화, consumed slot은 Image raycastTarget 비활성화.
- 기존 drag visual, scale 1.05, Gold highlight, Board preview, placement validation, Mouse/Touch 경로 유지.
- 실제 기기에서 Piece가 손가락에 가려지지 않았던 Drag Finger Offset 110은 유지.
- HUD 전→후: COSMIC BLOCK 44→56, BEST/SCORE 36→44 Bold, Journey 25→30. Progress Bar 크기/두께 유지.
- Title→BEST/SCORE→Journey Destination→Progress Value 계층으로 anchor 재정렬. Board 및 Bottom Slot anchor/게임 규칙/Game Feel/Haptic/SFX/0.68초 timing 변경 없음.
- 전용 Probe: Single/H2/V2/2x2/L/Reverse L을 Slot 가장자리에서 drag 시작 PASS, consumed slot raycast off PASS, 1/3/4/5자리 Score fit PASS.
- 자동 렌더 PASS: 1080×1920 board 1032.0, 1080×2400 inset board 879.3, 1080×1440 board 964.4. SafeArea/Text/Board/Slot bounds와 정사각 Board 확인.
- Sprint 0~4 및 Visual Readability PASS. C# Warning 0/Error 0, Missing Script 0.

## 2026-09-20 — Sprint 5 / Home Screen & Game Flow
- 단일 Game.unity 구조 선택. 기존 Scene 참조와 Core UI 계층을 유지하고 HomeRoot + 기존 SafeArea root별 CanvasGroup으로 화면 전환.
- APP LAUNCH→HOME→PLAY→GAME→GAME OVER→RETRY/HOME 흐름 구현.
- Home은 기존 CosmicBackground/Aspect Fill을 공유하고 Title, subtitle, PLAY, Best, Best Journey만 표시.
- Best는 CosmicBlock.BestScore 재사용. Best Journey는 JourneyProgress.At(BestScore)로 파생하며 추가 저장 없음.
- PLAY는 기존 GameSession.Retry를 재사용해 Score/Combo 0, 빈 Board, 3 Pieces, START, Best 유지.
- Game Over 광고 Placeholder 숨김. 기존 RETRY 유지, HOME 버튼 추가.
- Android Back: Playing 무동작, Game Over→Home, Home→Application.Quit 요청. New Input System Keyboard Escape 사용.
- Home 이동 시 active drag/preview/Board/Score/Combo/Journey feedback/Clear feedback/Audio를 기존 cleanup 경로로 초기화.
- PLAY/RETRY/HOME에 0.96 press scale feedback. 게임 Game Feel/Haptic/SFX/0.68초 timing/Board 규칙 변경 없음.
- Sprint5PlayProbe PASS: Home 시작, Best/Journey, PLAY, RETRY, HOME, Home loop x10, Retry loop x10, cleanup, Back.
- Home 자동 렌더 1080×1920/2400 inset/1440: SafeArea, hierarchy, bounds, no overlap/clipping PASS.
- Sprint 0~4와 Visual Readability PASS. C# Warning 0/Error 0, Missing Script 0.


## Sprint 6 — Planet Restoration

STAR JOURNEY user-facing progression was replaced by persistent Planet 01 restoration. Only line clears award Cosmic Energy: 10/25/45/70 for 1/2/3/4+ lines. Five supplied transparent sprites represent Desolate, Awakening, Recovering, Thriving, and Restored stages. Stage changes cross-fade and line clears send pooled sparks toward the planet with an energy label and pulse. Completion celebrates once without ending the run.

## Sprint 6.5 Planet Restoration Game Feel
Implemented stage-local 0–100 Energy, 0–400 persistence migration, cleared-cell Energy Fragment pooling, arrival-synchronized HUD growth, central healing cinematics, Stage 5 one-shot completion, and transition cleanup across Home/Retry/Game Over.

## Sprint 6.5 QA Hotfix — Planet Progress Reset
Development builds expose a subtle Home `DEV` launcher with Planet presets 0/90/190/290/390. Presets update only `CosmicBlock.Planet01Energy`, preserve migration version and Best Score, cancel transient Planet presentation, and refresh Home/Game HUD immediately. Release builds hide the launcher and panel through `Debug.isDebugBuild`. Stage 5 now shows one `100% RESTORED` line.

## Sprint 8 — Planet Collection

Added read-only CollectionRoot using nine supplied PNG components and existing Planet 01 stage sprites. HOME geometry/gameplay/save rules remain unchanged. Twenty existing regression entry points plus the new Collection probe passed; responsive renders passed at 1080×1920, 1080×2400 with Safe Area and 1080×1440. Android Development build/install/cold launch and actual Stage preview/navigation/save isolation passed on SM-S942N. See docs/DEVLOG.md for diagnostics, images and the known AssetPackManager exception. Extra hardware Back/relaunch confirmation awaits phone unlock/reconnection.


## Sprint9.4 — 크리스탈리아 연동 (2026-10-08)

기준 main 7c3b77d, Unity6000.5.8f1. Planet02 실제 플레이/도감 선택/영구 진행도를 통합했습니다. 기존 게임 규칙과 Sprint9.3 Burst/Flight/0.32초 순차 Clear/중앙 Stage Transition 타이밍은 유지합니다.

## 정의와 이미지
푸른 별1500 / 크리스탈리아3000 / 이그니스6000 / 글라시아12000 / 루미나24000. PlanetDefinition이 총량, 구간, 이름, 선행 해금, 준비 상태, Sprite 경로를 관리합니다. PlanetArtCatalog(Resources asset)이 실제 Sprite 참조와 본체 크기·중심 보정을 관리합니다. 03~05는 ContentReady=false로 선택 불가입니다.
Planet02 PNG5개와 공통 silhouette/silver lock2개 모두1254×1254 RGBA, alpha0~255, 외부 투명 픽셀 확인. 체크무늬/흰회색 배경 픽셀 미발견. 원본 PNG 수정/생성 없음. SpriteSingle/AlphaFromInput/AlphaTransparency/Bilinear/Clamp/NoMip/AndroidASTC6x6.
구간 필요량300/500/700/900/600, 시작0/300/800/1500/2400. 논리 Stage5의2400~2999는 ‘최종 복원’과 Sprite4; Sprite5는3000에서만 표시합니다. 기존 Planet01의1200 Stage5 아트 관례는 유지하며 완료는1500입니다.

## 선택·저장
01완료→02영구 해금. 도감 선택→HOME→PLAY, 플레이 중 전환 불가. 잠금/미준비 선택 거부, 잘못된 ID 기본01. HOME02에는 이름/총 별빛/복원도와 동적 Hero 표시, 기존 HOME01 디자인 유지. 현재 Run은 완료 후에도 계속합니다.
CosmicBlock.SelectedPlanet 신규 키. PlanetNNEnergy/PlanetNNUnlocked, BestScore/PlanetRestorationVersion2 기존 키 유지.02의 예전2000 저장값은 절대량2000으로 보존하고3000 기준 재계산.9.3에는 별도02완료 플래그가 없었으며 영구 해금 플래그는 보존합니다.500→400 legacy migration 유지, 비정상값 상한/음수 clamp. 전체 삭제 없음.
보상10/25/45/70, 현재 플레이 행성만 적립. 즉시 저장 후 표시만 Fragment 도착까지 지연. 최대량 이후0, 마지막5만 지급 가능하면 실제 표시도5. 초과 다음 행성 이월 없음. 전환/완료/해금 피드백 중복 없음.

## 변경 파일
Core: PlanetDefinition(new), PlanetRestoration, GameSession. UI: PlanetArtCatalog(new), PlanetCollectionView/Data, GameFlowController, HomeViewController/Atmosphere base scale(HomeAmbientMotion), PlanetRestorationView, GameVisualPresentation, PlanetDebugPanel. Scene Game.unity 및 Resources/PlanetArtCatalog.asset. Editor Sprint94Builder/PlayProbe/VisualProbe(new), 기존 Sprint8CollectionPlayProbe/Sprint93PlayProbe 기대값을 새 정의로 갱신.
Board/Blocks/ScoreRules/SequentialClearPlan/GameFeedbackController 변경 없음. DEV presets는 선택 행성 경계−10으로 공통 계산하며 Release 비노출 유지.

## 자동 검증
전체29 Probe entrypoint 최종 PASS(Sprint0~9.3 +94), 실패 후 수정한 초기 실행 기록은 로그에 보존. 최종94Play172개 assertion PASS. P1기존 경계/P2열두 경계, 실제 Sprite/fill/표시,1490+10해금/Run유지,02독립 적립/전환5개,3000완료1회,부분5지급,교차+25/3줄+45,빠른HOME callback,GameOver/Retry,globalBest/reload/선택fallback 검증.
마지막 관련8 Probe 전부 PASS. 후속 HOME 보정 후94Visual/761HOME 재검증 PASS.39 렌더(각3해상도×13상태):1080×1920 /1080×2400 SafeArea /1080×1440. 잠금02/미준비03,02각단계/2999/3000,HOME02/GAME02 완료·최종구간. 본체 크기/중심 보정, Hero Frame 간격,Text 잘림,선택버튼SafeArea,실제Bar폭,Nav비율 검증. SCORE/BEST24 RGB-mask 조합 overlap0. MissingScript0, 신규 컴파일 Warning/Error0.
근거: Validation/sprint94_tests.txt, sprint94_visual_tests.txt, sprint94_final_regression_summary.txt, sprint94_release_summary.txt, sprint94_missing.log, sprint94_final_*.log 및 렌더PNG. QA 산출물/APK는git제외.

Android/Git 최종 결과는 아래 완료 기록에 추가합니다. 주관적 Touch/SFX/Haptic/화면가독성은 사용자 확인 대기입니다.

## 최종 완료 검증
실기기 GameOver QA에서 발견한 행성01 고정 문구를 GameHud의 선택 행성 정의로 교체했습니다. 기존 검사는 GameOver 상태/패널만 확인했으므로 표시명 assertion을 추가했습니다. HOME fallback/legacy stage 표기도 같은 정의를 사용합니다. DEV preset의 오래된90/190/290/390 표시도 선택 행성의 실제 경계−10으로 동기화: P1 0/140/390/740/1490, P2 0/290/790/1490/2990. 판정/점수/연출/입력 규칙 변경 없음.
최종94Play172 assertion PASS; 관련65/66/5NewRunGameOver/761Home 모두 PASS. GameOver를 추가한39개 반응형 렌더 PASS(각3해상도×13상태), 긴 크리스탈리아 결과 Text 잘림 없음. MissingScript0. 최종 cached Development build Warning0/Error0. 첫 IL2CPP 빌드의 기존 TMP3 notice와 첫 Editor shader import의 URP414 warning은 원 로그에 보존; 변경 없는 재검증에서 Compiler/Shader Warning/Error0. SDK/패키지 경고를 숨기거나 수정하지 않았습니다.

Android: SM-S942N / Android16 /1080×2340, adb device 정상. 기존 앱 위 install-r 성공/실행 성공. 업데이트 직전/직후 사용자 진행도240/Best15930/Version2 유지.
임시 QA fixture를 사용하여 잠금02→P1 1490+실제1줄=1500→02해금→도감 선택→HOME→PLAY를 확인했습니다. P2 0→10,290→300,790→800,1490→1500,2390→2400,2990→3000을 실제 Touch/Line Clear로 확인.2400에서4번 이미지/최종복원,3000에서5번 최종고리 이미지. 01 진행도와 글로벌BEST 불변, 완료 이후 Run 유지. 단계 핵심 QA 이후 패널 문구만 보정한 최종 APK에서41회배치/6줄/Score1710,3000 clamp/완료연출 반복없음/자연GameOver/수정된02표기/Retry/Home→PLAY/재실행을 재검증했습니다. Retry/Home→PLAY의 실제64셀 empty 및Piece3개는 화면 픽셀과 별도 Board 모델로 확인했습니다. 재실행 후 선택2/진행3000/Best15930 유지. 최종 DEV UI에서도290/790/1490/2990 확인.
QA 전에 PlayerPrefs XML 전체를 백업했고, 종료 후 모든 원본 preference를 비교·복원했습니다. 현재 기기는 원래 사용자 상태240/15930/Version2, 기본 선택01의 HOME에서 실행 중입니다. 임시 해금/행성02·03/선택 키는 원복했습니다.
Logcat: FATAL EXCEPTION0, AndroidJavaException0, NullReferenceException0, MissingReferenceException0, 신규 게임 코드 오류0. 기존 AssetPackManager ClassNotFound E Unity와 DexFile finalizer AssertionError 환경 로그는 남아 있음(각4회 시작 로그); 전체 Android Logcat Error0으로 주장하지 않습니다. Crash 없음/프로세스 실행 확인.
실행하지 않은 항목: Release APK 자체 설치, 주관적 Touch/SFX/Haptic/휴대폰 가독성 평가. 기존 Release DEV guard의 false 조건은 자동 회귀 검사로 확인했습니다. 역사적인921 before/after 측정 도구는59px 과거 baseline용이며 현재93의80px Burst 회귀는93/92 검증을 사용합니다.

추가된 Planet03 PNG5장은 사용자 원본 자산으로만 보관하며 아직 alpha/art/content 검증·Sprite 연결·플레이 활성화하지 않았습니다.03~05 ContentReady=false. 후속 구현 때 별도 자산 검사와 단계 검증을 수행해야 합니다.
APK SHA256: ECD0A7417B4E8126F2515A40263B3C25D378AF0A9B71617CC22316397E8B9556.
Evidence: Validation/sprint94_context_* 및 context_visual_clean.log, sprint94_tests.txt/visual_tests.txt, device_stage2/stage3/stage4/complete 결과JSON·PNG, final_gameover_context/retry/home_play/relaunch2/original_home PNG, device_final_logcat.txt, device_final_result.json. QA 로그/이미지/APK는Git 제외.
Git commit message: feat: integrate crystal planet and collection progression. 기존main/origin으로 일반 push하며 force push/remote 변경 없음. 실제 hash와 동기화 결과는 완료 보고 및 Validation의 Git 기록에 남깁니다.
