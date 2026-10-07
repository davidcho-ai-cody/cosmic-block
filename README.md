# COSMIC BLOCK / 별빛 블록

Android Portrait 8×8 블록 퍼즐. HOME → GAME 또는 행성 도감으로 진입하며, Best Score와 행성 01 복원도는 PlayerPrefs에 보존합니다.

## 현재 상태

Sprint 9.1.1 GAME Layout/Density를 유지하며 Sprint 9.2 순차 Line Clear, 원 Sprite Pop, 별빛 Pool과 같은 턴의 숫자 Combo 피드백을 적용했습니다. 배치 Cell 10점과 Planet Energy/복원 규칙은 유지합니다. HOME/Collection과 Board/Slot 외곽 배치 및 Drag 좌표는 변경하지 않습니다.

## 실행

Unity Hub에서 프로젝트를 열고 Unity 6000.5.8f1의 Assets/Scenes/Game.unity를 실행합니다. 기존 Safe Area/Canvas와 New Input System을 사용합니다. TMP 점수 표시는 설치된 uGUI에 포함된 필수 리소스를 사용합니다.

## 화면과 표시 구조

- 좌상단 HOME 아이콘: 기존 확인창 후 HOME 이동.
- 중앙 상단 SCORE와 현재 점수, 우상단 Best 프레임과 동적 TMP 숫자.
- Planet Status: 현재 단계 Sprite/행성 이름/전체 복원도 %/기존 단계별 Bar. 상세 Stage/Energy Text는 화면에서 숨깁니다.
- BoardFrame은 장식 형제 객체이며 실제 Board의 64 Cell/좌표/배치 계산을 담당하지 않습니다.
- Empty Cell과 Blue/Purple/Gold Block Sprite를 구분하며 배치 색상을 유지합니다.
- Slot 3개 전체가 Drag 시작 Hit Area입니다. 소비된 Slot은 입력을 받지 않습니다.
- 기존 Valid Gold / Invalid Red Preview, Drag Gold Outline, Clear/Score/Combo/Planet 피드백은 유지합니다.

## 핵심 규칙

배치 점수는 Cell 수×10 + 완성 Line 수×100 + 연속 Clear Combo 보너스입니다. Row/Column 교차 Cell은 한 번만 제거합니다. 세 Piece를 모두 소비한 뒤에만 다시 세 Piece를 공급합니다. 남은 모든 Piece가 배치 불가능할 때 Game Over가 발생합니다.

Planet 01은 Line Clear 에너지를 누적하여 0/100/200/300/400에서 Stage 1~5로 진행합니다. 단계별 100 Energy, overflow, Fragment 도착 후 Bar 증가와 기존 중앙 전환 연출을 유지합니다. Retry/HOME은 영구 복원도와 Best를 삭제하지 않습니다.

## 입력과 저장

좌상단 Cell은 (0,0), X 오른쪽/Y 아래쪽입니다. 실제 Grid Cell 중심을 기준으로 Drag/Preview/Drop을 일치시킵니다. Finger Offset 110 Canvas 단위와 Drag Scale 1.05를 유지합니다. Mouse/Android Touch는 동일한 uGUI 경로를 사용합니다.

PlayerPrefs: CosmicBlock.BestScore, CosmicBlock.Planet01Energy, CosmicBlock.PlanetRestorationVersion. Collection Preview는 저장값을 변경하지 않습니다. DEV Planet QA는 Development Build 전용이며 Release에서는 숨깁니다.

## 검증과 빌드

1080×1920, 1080×2400 + Safe Area, 1080×1440에서 검증합니다. 기존 Sprint Probe들과 Sprint9GamePlayProbe를 실행하고 실제 Cell 좌표, 모든 Shape, Clear/Combo/Retry/Save를 검사합니다. GAME 배치 상태와 Game Over 렌더는 Validation/sprint9_*.png에 저장합니다.

Android Development APK는 COSMIC BLOCK/Build/Android Development APK 또는 AndroidBuildAutomation.BuildDevelopment로 생성합니다. 출력: Builds/Android/Development/CosmicBlock-dev.apk. 설치는 기존 앱에 adb install -r을 사용합니다. 기존 AssetPackManager 환경 예외는 게임 코드 오류와 구분하여 기록합니다.

상세 상태와 검증 결과: [CURRENT_TASK](docs/CURRENT_TASK.md), [DEVLOG](docs/DEVLOG.md), [NEXT_TASK](docs/NEXT_TASK.md).