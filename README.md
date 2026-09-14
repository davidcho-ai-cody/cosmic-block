# COSMIC BLOCK
PLAY YOUR NEXT WORLD의 첫 Android 출시작.
The primary goal of COSMIC BLOCK v1.0 is shipping.

## 현재 상태
Sprint 0(d85d878) 기반으로 Sprint 1 Block Placement 완료.
하단 랜덤 블록 3개를 드래그하여 8×8 보드에 배치한다. 실패 시 원래 슬롯으로 복귀한다.
세 개를 모두 사용하면 슬롯은 빈 상태로 남는다. 라인 제거/점수/Combo/Game Over/광고는 아직 없다.

## 시작
Unity Hub → Add → 이 프로젝트 폴더 → Unity 6000.5.8f1로 열기.
Project 창 → Assets → Scenes → Game.unity 더블클릭 → Play.
Game Scene 및 슬롯/참조는 자동화로 이미 구성되어 있다. 현재 단계에서 수동 Unity 작업 없음.

## 기술 기준
- 설치 Editor와 프로젝트 버전: Unity 6000.5.8f1 고정. 업그레이드 없음; LTS로 단정하지 않는다.
- URP 17.6.0 / 기존 2D Renderer, uGUI 2.5.0, Input System 1.20.0 (activeInputHandler=1).
- Portrait, Canvas 1080×1920, SafeArea 하위 UI, 정사각형 보드.
- Android Build Support/SDK/NDK/OpenJDK 확인. SDK platforms 34/36/37.0.
- Target API 36 명시, minSdk 26 유지.
- Google Play 기준 확인(2026-09-14): https://support.google.com/googleplay/android-developer/answer/11926878?hl=en
- Unity 6.5 최소 Android 기준: https://discussions.unity.com/t/planned-breaking-changes-in-unity-6-5-updated-2026-03-27/1694205
- 기존 SampleScene/Settings Asset 보존. 외부 의존성 추가 없음.

## Architecture / 주요 파일
- Assets/Scripts/Board/BoardModel.cs: bool[8,8], CellChanged, CanPlace/TryPlace.
- Assets/Scripts/Board/BoardView.cs: 64 Image, 실제 Grid 중심/간격 기반 좌표 변환, preview.
- Assets/Scripts/Core/GameSession.cs: 모델 소유, 초기 슬롯 공급, 활성 drag 하나로 제한.
- Assets/Scripts/Blocks/BlockShape.cs: 불변 offset 집합과 bounding dimensions.
- Assets/Scripts/Blocks/BlockCatalog.cs: 8종 pool과 System.Random 기반 BlockGenerator.
- Assets/Scripts/Blocks/BlockPiece.cs: Shape의 Image 표시, 슬롯/드래그 geometry, 소비 상태.
- Assets/Scripts/Blocks/BlockDragHandler.cs: uGUI mouse/touch 이벤트, 이동/미리보기/배치/복귀/취소.
- Assets/Scripts/UI/SafeArea.cs: Screen.safeArea → anchor.
- Assets/Scripts/UI/SquareBoardLayout.cs: min(SafeArea 너비−48, 높이×0.58).
- Assets/Editor/Sprint1Builder.cs: 기존 Game Scene 확장 및 domain test.
- Assets/Editor/Sprint1PlayProbe.cs: 배치 전용 Play Mode/입력 callback/화면비 렌더 검증.
- Assets/Scenes/Game.unity: 기존 64 Cell 및 새 BlockArea/Slot_0~2/DragLayer.
- .meta는 Unity가 생성한 참조용 파일. .gitignore는 Library/Temp/Logs/Validation/빌드 출력을 제외한다.
- 루트 문서: PRODUCT_VISION / GAME_DESIGN / DESIGN_SYSTEM / ROADMAP / CURRENT_TASK / NEXT_TASK / DEVLOG / BACKLOG.

## Coordinate / Drag
원점 좌상단, x 오른쪽, y 아래쪽. Shape offset과 Board cell 좌표를 합하여 판정한다.
포인터를 Canvas local 좌표로 바꾸고 Piece 중심을 위로 띄운다.
GameSession Inspector → Drag Finger Offset = 110 Canvas 단위 (조절 위치 한 곳).
Piece bounding-box 좌상단 Cell 중심을 Screen→Board local로 변환하고 실제 첫 Cell 중심과 Grid pitch로 가장 가까운 anchor를 계산한다.
그 anchor에 drag visual을 snap하고 동일 좌표로 preview 및 최종 drop을 처리한다.
Reverse L은 (0,0)이 비어 있어도 bounding-box 좌상단이 anchor다.
블록 원점이 보드 밖이면 preview를 지운다. 부분적으로 밖에 걸치는 경우 보드 안의 Cell을 red로 표시한다.

## Unity 자동화
- COSMIC BLOCK → Sprint 1 → Upgrade Game Scene: 기존 Scene 확장. 반복 실행해도 슬롯/Cell을 중복 생성하지 않는다.
- COSMIC BLOCK → Sprint 1 → Validate Placement Logic: Single/H3/L/Reverse L/범위/점유/모든 Shape 가장자리/seed 테스트.
- Sprint 0 메뉴/검증 코드도 보존한다.
- PlayProbe는 -executeMethod Sprint1PlayProbe.Run으로 배치 실행하며 완료 후 Editor를 종료한다. 대화형 Editor에서는 실행하지 않는다.

## 직접 QA
1. Game Scene → Play → 하단 3개 블록, 8×8 보드를 확인.
2. 마우스 왼쪽 버튼으로 블록을 드래그. 블록이 커지고 포인터 위로 이동한다.
3. 빈 곳의 gold preview를 확인하고 release → 보라색 점유 Cell, 사용한 슬롯만 비워짐.
4. 다른 블록을 이미 점유한 Cell 위로 이동 → red preview → release → 원래 슬롯 복귀, 보드 변화 없음.
5. 폭/높이 2 이상 블록을 오른쪽/아래 경계에 걸치게 놓기 → 실패/복귀.
6. Shape 전체가 들어가는 가장자리로 이동 → 성공.
7. 보드 밖에서 release, 드래그 중 Escape 또는 다른 창으로 포커스 이동 → 복귀.
8. 세 블록을 모두 사용 → 3개 슬롯 비어 있음, 자동 공급/Line Clear 없음.
9. Game View 해상도 드롭다운 → + → Fixed Resolution으로 1080×1920, 1080×2400, 1080×1440 추가 → 각각 Play하여 정사각형/슬롯/미리보기 확인.
10. Console에 게임 코드 오류/예외가 없어야 한다. Android 실기기에서는 손가락 드래그/다중 터치/노치 SafeArea를 추가 QA한다.

원하는 Shape가 없다면 Play를 종료하고 다시 시작한다. 재현용 seed는 Play 전에 Hierarchy GameSession → Inspector → Use Fixed Seed 체크 → Fixed Seed 값 지정.
점유 데이터는 Play 중 BoardView.Model 또는 GameSession.Model에서 확인 가능하며 Editor domain tests가 정확한 Cell 집합을 검사한다.
한국어/영어 UI 및 폰트는 후속 UI Sprint에서 구현한다. 현재 영문 placeholder/LegacyRuntime 폰트는 최종 UI가 아니다.

## 검증 결과와 한계
Test 1~8 PASS: domain 및 Play Mode synthetic uGUI 이벤트(-1 mouse / 42,43 touch-style pointer ids).
초기 3개와 UI raycast, exact offset 점유, 미리보기 모델 불변, 실패 복귀, slot consumption, 취소/다중 포인터 보호, 추가 프레임 후 자동 공급 없음 PASS.
RenderTexture 1080×1920 / 1080×2400(모사 inset) / 1080×1440에서 보드 비율/64 cell mapping/slot bounds PASS.
실제 블록을 넣은 spacing 8/12 비교 렌더를 확인하여 12를 선택했다. 테스트 이미지/로그는 Validation 및 루트 .log에 생성되고 Git에서 제외된다.
이 검증은 하드웨어 입력에서 InputSystem→EventSystem까지의 전체 경로나 Android APK/AAB/실기기 QA를 대체하지 않는다.
출시 전 application identifier/서명/ARM64/IL2CPP/AAB 및 정책을 별도 확정한다.
