# 연속 라인 클리어 콤보 수정

## 원인
GameSession.Combo는 클리어 배치 횟수만 세고, 실제 팝업과 점수는 매 턴 초기화되는 LineComboIndex를 사용했습니다. 따라서 3줄 동시 제거는 화면에서 3콤보이지만 다음 한 줄은 CLEAR!/+100으로 돌아갔습니다.

## 변경
연속 클리어 줄 수를 Combo에 저장하고, 해당 배치 시작의 콤보 + 현재 처리 줄 번호로 기존 ScoreRules.LinePoints를 호출합니다. 상수(100/50)는 유지했습니다. 클리어 없는 배치는 Combo 0, 새 Run/Retry는 기존 초기화, HOME/이어하기는 저장된 Combo를 보존합니다. 중간 저장의 미처리 줄도 동일한 번호로 점수를 계산합니다.
LineComboIndex는 해당 턴의 연출 순서 및 별빛 분배용으로 유지합니다. 별빛 보상, Clear 간격, UI 디자인, 저장 포맷은 변경하지 않았습니다. 기존 저장 파일은 그대로 읽으며 기존 combo 숫자를 출발점으로 사용합니다. 과거 기록에 실제 연속 제거 줄 수가 없으므로 이를 소급 추정하지 않습니다.

## 수정 파일
- Assets/Scripts/Core/GameSession.cs
- Assets/Editor/ClearScorePlayProbe.cs
- Assets/Editor/Polish001PlayProbe.cs
- Assets/Editor/Sprint92PlayProbe.cs
- README.md 및 작업 문서

## 검증
ClearScorePlayProbe 106항목 PASS: 단일/복수 줄, 3줄→1줄=4콤보, 이어하기→5콤보, 무클리어 초기화, 새 Run 초기화, SCORE/HUD/BEST/중간 저장 일치.
Polish001PlayProbe PASS: 연속 클리어의 SCORE 표시와 별빛 실제 적립 일치, 완료 행성 +0, overflow, 정리.
Sprint92PlayProbe 1,545항목 PASS(순서/타이밍/입력잠금/원본 Sprite/별빛/정리/공급/Home/Retry/설정/3해상도). RunSavePlayProbe 563항목 PASS. Missing Script 0, 신규 Compiler Warning/Error 0, 테스트 Runtime Warning/Error 0. 최종 회귀 결과: Validation/combo-fix-summary.txt.

APK 빌드·실기기 설치·Commit/Push는 이번 요청에서 수행하지 않습니다.
