# Game Design
8×8 보드에 하단 랜덤 블록 3개를 드래그 배치한다. 완성한 가로/세로 라인을 제거하고 점수를 얻는다.
3개를 모두 사용하면 다시 공급한다. 남은 블록 중 하나도 배치할 수 없으면 Game Over.
레벨 없이 Local Best Score에 도전한다.

## v1.0
Home / Game / Game Over, Board, Random Block 3개, Drag & Drop, Placement Validation, Line Clear, Score, Combo, Local Best, Restart, Sound, Vibration, Simple Glow, Star Particle, Combo Animation, 한국어/영어, AdMob Rewarded/Interstitial.
광고는 Core 플레이 테스트 이후 별도 Sprint.

Combo는 연속 Line Clear 횟수. 1/2: 별 피드백, 3: STAR COMBO, 4+: COSMIC COMBO.
새 규칙이나 특수 블록 효과는 없다. 점수 공식은 아래 Sprint 2 규칙으로 확정.

## 제외
Login, Server, Backend, Online Ranking, Database, IAP, Shop, Stage/Level, Account, Cloud Save, Battle Pass, Daily Mission, Achievement, Social, Special/Star Block, Character Animation, 3D, Cutscene.

## Sprint 0
빈 보드 표시와 bool Cell 상태 API만 구현. 블록 생성/랜덤/드래그/배치 검사/라인 제거/점수/Combo/Game Over는 구현하지 않는다.


## Sprint 1 — Shape와 Placement Rule
- Pool 8종: Single, Horizontal 2/3, Vertical 2/3, Square 2×2, L Small, Reverse L Small.
- 불변 Cell Offset 좌표 집합. 좌상단 bounding origin; x 오른쪽/y 아래쪽. Reverse L은 (1,0),(0,1),(1,1).
- Game Start에서 System.Random으로 pool에서 독립 균등 선택 3회. 중복 허용. seed 옵션으로 재현 가능.
- anchor + 모든 offset이 범위 0..7에 있고 비점유일 때만 성공. 전체 검사 후 모델 상태 변경.
- Preview는 모델을 변경하지 않는다. gold valid/red invalid; 보드 밖 원점이면 preview 없음.
- 성공: 점유 갱신/해당 Piece 소비/슬롯 비움. 배치한 블록 재이동 불가.
- 실패/취소: 보드 변경 없이 원래 슬롯으로 snap 복귀. 비활성화 취소는 Unity lifecycle 충돌을 피하여 다음 프레임 복귀.
- 초기 3개 모두 소비 후 빈 슬롯 유지. Line Clear/Score/Combo/Game Over/새 Set 공급 없음.

## Sprint 2 — 현재 Core Game Rules
Sprint 1의 '재공급/Score/Line Clear 없음'은 당시 제한이며, 현재 다음 규칙으로 확장되었다.
- 모든 완성 Row/Column을 제거 전 수집하고 교차 Cell은 한 번만 제거. 다중 Row/Column 동시 지원.
- 유효 Placement Cell당 +10, 완료 Line당 +100.
- Combo = 연속 'Line Clear가 발생한 유효 Placement' 횟수. 여러 Line을 한 번에 제거해도 Combo는 +1.
- Combo bonus: Clear 있을 때 max(0,Combo−1)×50. Combo1 +0 / 2 +50 / 3 +100 / 4 +150.
- Clear 없는 유효 배치는 Combo0. Invalid/Cancel은 점수/Combo/점유/슬롯 변화 없음.
- Best: Current>Best일 때 PlayerPrefs local 즉시 저장. Retry/게임 재시작 시 유지.
- 슬롯 3개 모두 소비 후 새 랜덤 3개 공급. 한 개/두 개만 소비하면 기존 남은 Shape 유지.
- 최종 Clear/재공급 완료 후 남은 Shape 모두의 64 anchor를 검색. 하나라도 가능하면 Playing; 모두 불가능하면 GameOver.
- Resolving 중/게임 종료 후 배치 입력 차단. 동기 처리 후 Playing 또는 GameOver.
- Retry는 현재 Session의 보드/Score/Combo/슬롯/State/UI reset, Scene reload 없음.
- 광고 Continue는 비활성 Placeholder. 특수 블록/게임 규칙 추가 없음.
