# COSMIC BLOCK / 별빛 블록

Android Portrait 8×8 블록 퍼즐. HOME → GAME 또는 행성 도감으로 진입하며, Best Score와 행성 01 복원도는 PlayerPrefs에 보존합니다.

## 현재 상태

Sprint 9.3: 기존 GAME Layout/Density와 원 Sprite Pop을 유지하면서 별빛 Burst/Planet Flight, 0.32초 순차 Clear, 단계별 별빛과 전체 복원도 HUD 및 확대된 BEST 표시를 적용했습니다. SCORE는 현재 Run, BEST는 모든 행성 공통 최고 기록, STARLIGHT는 영구 행성 진행도입니다. Board/Slot/Drag와 점수·별빛 보상 규칙은 유지합니다.

## 실행

Unity Hub에서 프로젝트를 열고 Unity 6000.5.8f1의 Assets/Scenes/Game.unity를 실행합니다. 기존 Safe Area/Canvas와 New Input System을 사용합니다. TMP 점수 표시는 설치된 uGUI에 포함된 필수 리소스를 사용합니다.

## 화면과 표시 구조

- 좌상단 HOME 아이콘: 기존 확인창 후 HOME 이동.
- 중앙 상단 SCORE와 현재 점수, 우상단 Best 프레임과 동적 TMP 숫자.
- Planet Status: 현재 단계 Sprite/행성 이름/N단계·Stage Name/별빛 Current·Required/전체 복원도 %. Bar와 %는 동일한 전체 진행률을 사용합니다.
- BoardFrame은 장식 형제 객체이며 실제 Board의 64 Cell/좌표/배치 계산을 담당하지 않습니다.
- Empty Cell과 Blue/Purple/Gold Block Sprite를 구분하며 배치 색상을 유지합니다.
- Slot 3개 전체가 Drag 시작 Hit Area입니다. 소비된 Slot은 입력을 받지 않습니다.
- 기존 Valid Gold / Invalid Red Preview, Drag Gold Outline, Clear/Score/Combo/Planet 피드백은 유지합니다.

## 핵심 규칙

배치 점수는 Cell 수×10이며 같은 턴의 각 Clear Line은 100/150/200/250…점입니다. 1/2/3/4 Line 총점은 100/250/450/700이고 placement streak 추가 보너스는 없습니다. Row/Column 교차 Cell은 한 번만 제거합니다. 세 Piece를 모두 소비한 뒤에만 다시 세 Piece를 공급합니다. 남은 모든 Piece가 배치 불가능할 때 Game Over가 발생합니다.

Planet 01 총 필요 별빛은 1500입니다. 황폐/싹틈/깨어남/회복/완성의 필요량은 150/250/350/450/300이며 시작점은 0/150/400/750/1200입니다. Stage 5 진입은 완료가 아니며 1500에서만 100%·Complete입니다. 전체 Bar=Clamp(total/1500), %는 floor입니다. 보상 10/25/45/70과 overflow는 유지하며 별빛은 즉시 저장하고 화면만 Fragment 도착까지 지연합니다. 완료 후에도 현재 Run은 계속됩니다. Retry/HOME은 영구 복원도와 Best를 삭제하지 않습니다.

Planet 02는 2000 목표와 200/300/450/600/450 단계 데이터·독립 저장만 준비했습니다. Planet 01이 1500에 도달하면 영구 해금됩니다. 실제 Planet 02 아트/플레이는 아직 추가하지 않았으며 Collection은 발견 완료·탐험 준비 중 상태를 표시합니다.

## 입력과 저장

좌상단 Cell은 (0,0), X 오른쪽/Y 아래쪽입니다. 실제 Grid Cell 중심을 기준으로 Drag/Preview/Drop을 일치시킵니다. Finger Offset 110 Canvas 단위와 Drag Scale 1.05를 유지합니다. Mouse/Android Touch는 동일한 uGUI 경로를 사용합니다.

PlayerPrefs: CosmicBlock.BestScore, CosmicBlock.Planet01Energy, CosmicBlock.PlanetRestorationVersion, CosmicBlock.Planet02Energy, CosmicBlock.Planet02Unlocked. Migration Version 2와 기존 저장값은 유지하며 1500 초과값만 clamp합니다. 기존 500→400 legacy migration도 유지합니다. Collection Preview는 저장값을 변경하지 않습니다. DEV Planet QA는 Development Build 전용이며 Release에서는 숨깁니다.

## 검증과 빌드

1080×1920, 1080×2400 + Safe Area, 1080×1440에서 검증합니다. 기존 Sprint Probe들과 Sprint9GamePlayProbe를 실행하고 실제 Cell 좌표, 모든 Shape, Clear/Combo/Retry/Save를 검사합니다. GAME 배치 상태와 Game Over 렌더는 Validation/sprint9_*.png에 저장합니다.

Android Development APK는 COSMIC BLOCK/Build/Android Development APK 또는 AndroidBuildAutomation.BuildDevelopment로 생성합니다. 출력: Builds/Android/Development/CosmicBlock-dev.apk. 설치는 기존 앱에 adb install -r을 사용합니다. 기존 AssetPackManager 환경 예외는 게임 코드 오류와 구분하여 기록합니다.

상세 상태와 검증 결과: [CURRENT_TASK](docs/CURRENT_TASK.md), [DEVLOG](docs/DEVLOG.md), [NEXT_TASK](docs/NEXT_TASK.md).
## Sprint 9.3 피드백 수치

Clear Burst 기준 Rect 59→80, 대표 실제 렌더 46×53→63×71px. Small/Medium/Large 변주, 12/Line·Pool48 유지. Planet Flight Rect140, 이동0.72초와 기존 stagger0.022초, 도착 Pulse1.07·0.25초, Bar0.30초. 중앙 Stage 전환의 기존 연출 시간은 유지합니다. BEST Frame330×110→412.5×137.5(+25%), 숫자 최대 Font37→53.65(+45%), 큰 값은 AutoSize입니다.
