# POLISH-001 — 라인 클리어 SCORE / 별빛 표시

## 변경 내용

기존 CLEAR! 제목과 순차 제거의 N COMBO 제목을 유지했습니다. 라인 보너스를 `+100 SCORE` 형식의 골드 텍스트로 표시하고, 별도 하늘색 텍스트 `별빛 +10`을 추가했습니다. 기존 0.68초 Fade/제목 Scale/Particle/SFX/Haptic은 유지합니다. SCORE 글꼴은 48→38, 별빛은 30으로 구성해 세 줄 간격을 확보했습니다.

## 값의 의미

- SCORE 팝업은 기존과 동일한 해당 라인 제거 보너스입니다. 배치 점수를 포함한 턴 총점이 아닙니다.
- 1칸 + 1줄: HUD 총 증가 110, 팝업 +100 SCORE, 별빛 +10.
- 1칸 + 2줄: HUD 총 증가 260, 팝업 +100 SCORE → +150 SCORE. 별빛은 +10 → +15로 합계 25.
- 3줄 별빛 표시 합계 45, 4줄 합계 70. 기존 별빛 지급 정책 유지.
- 실제 적립은 기존 턴 단위 사전 Commit 방식 그대로입니다. 표시만 `clearEnergyAfter - clearEnergyBefore` 실제 증가량을 기존 누적 AwardForLines에 맞춰 순차 분배합니다.
- 목표까지 5만 남은 2줄 제거: 별빛 +5 → +0. 완료된 행성: 별빛 +0. 허위 +10 없음.

ScoreRules / PlanetRestoration / Combo 계산 / Board / RunSaveStore / 저장 순서는 변경하지 않았습니다. GameSession의 피드백 호출에 읽기 전용 표시값만 추가했습니다. 표시 합계는 실제 적립 및 저장값과 일치합니다.

## 수정 파일

- Assets/Scripts/Effects/GameFeedbackController.cs: SCORE 형식, 별빛 표시·Fade·cleanup
- Assets/Scripts/Core/GameSession.cs: 실제 적립량을 피드백 인자로 전달
- Assets/Scenes/Game.unity: EnergyPop 연결, Score 글꼴·크기/색상, CLEAR 색상
- Assets/Editor/Polish001Setup.cs (+meta): 해당 UI 변경 설치 도구
- Assets/Editor/Polish001PlayProbe.cs (+meta): 신규 검증과 3해상도 렌더
- CURRENT_TASK / NEXT_TASK / DEVLOG 및 docs 사본, 본 보고서

## 테스트

신규 Editor 테스트 PASS: 단일/2/3/4줄, 각 라인 SCORE 표시와 실제 누적 점수, 별빛 표시 합계와 영구 적립값, 목표 초과 적립, 완료 행성01~05, 연속 Combo2, Run 저장 일치, Fade 후 stale 텍스트 없음, HOME/이어하기 cleanup.

1080×1920 / 1080×2400 Safe Area / 1080×1440: 세 텍스트의 잘림 검사 및 렌더 육안 확인 PASS.

기존 회귀 최종 결과는 아래에 추가합니다. 기존 ClearScore 테스트의 첫 실패는 남아 있던 완료 팝업이 Combo 테스트 입력을 막은 것으로, 기존 CompletionNoticeProbeScope의 Legacy 모드로 테스트 격리 후 PASS했습니다. 실제 게임 판정이나 팝업 로직 변경 없음.

증거: Validation/polish001_tests.txt, polish001_summary.txt, polish001_clear_*.png, 각 probe 로그.

Android 빌드/설치/실기기 및 Git Commit/Push는 수행하지 않았습니다. 현재 설치된 APK에는 이번 표시 변경이 없습니다.

## 최종 회귀 결과

Polish001PlayProbe / ClearScorePlayProbe / Sprint92PlayProbe / Sprint1021PlayProbe / RunSavePlayProbe / Sprint1022AudioProbe / MissingScriptDiagnostics: 최종 모두 PASS. Compiler Warning/Error 0, 일반 Runtime Warning/Error 0, Missing Script 0. Sprint1021의 의도적 저장 실패 Warning 1은 예상 결과로 별도입니다. Scene의 관련 없는 Editor 자동 변경과 ProjectAuditor 자동 변경은 원복했습니다. Git HEAD는 ad34309로 유지합니다.

## 추가 승인 — Git 반영

사용자가 Commit/Push를 요청하여 검증된 변경을 일반 커밋 후 origin/main으로 Push합니다. 앞의 Git 미실행 문구는 최초 요청 시점의 이력입니다. Android 빌드/설치/실기기 검증은 수행하지 않았습니다. Remote 변경/Force Push 없음. 최종 해시 및 동기화 결과는 완료 응답에 기록합니다.
