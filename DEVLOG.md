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
