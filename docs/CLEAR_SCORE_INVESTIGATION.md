# CLEAR 점수 오류 조사 — 2026-10-09

## 결과
현재 코드에서 `CLEAR! +100`이 SCORE에 누락되는 현상은 재현되지 않았다. 원인이 확인되지 않아 점수 런타임 코드, 상수, 연출, 별빛 정책은 변경하지 않았다. Android 연결이 없어 보고된 실기기 현상의 원인은 미확인이다.

사용자 확인에 따라 Sprint 9.2의 현재 규칙을 유지한다: 배치 셀당 10점, 같은 턴의 제거 라인 순서별 100/150/200…점, 연속 배치 Combo 추가 점수 없음. 따라서 1칸+2줄은 260점이고, 연속 배치 Combo 2의 1칸+1줄은 110점이다. 요청 최초의 210/160 기준은 적용하지 않는다.

## 추적
- ScoreRules.LinePoints: CLEAR 연출과 실제 라인 점수 모두 동일한 계산 함수를 사용한다.
- GameSession.TryPlacePiece: 배치 점수 → 제거할 라인 Snapshot → 최종 논리 상태 자동 저장 → 순차 제거.
- GameSession.ResolveLine: PlayClear에 해당 라인 점수를 전달하고 Model.ClearCells 후 AddScore로 누적한다. 1줄은 배치10+라인100=110.
- AddScore: 누적 SCORE와 BEST를 갱신하고 BEST PlayerPrefs를 저장한다.
- GameVisualPresentation.LateUpdate: GameSession.Score/BestScore를 실제 화면 TMP Text에 표시한다.
- SaveCheckpoint(true)/Capture: 애니메이션 중 저장에는 아직 표시되지 않은 남은 라인 점수까지 포함한다. 2줄의 첫 프레임 실제110/최종 체크포인트260은 의도적인 시간 차이다.
- SuspendAndSave/CancelResolution: 남은 라인을 정산한 후 저장한다.
- ResumeSavedRun: HOME에서 사용자가 이어하기를 누를 때 저장 SCORE를 복원한다. 일반 라인 처리 중 저장 파일을 재로드해 점수를 덮어쓰는 경로는 없다.
- 실행 Scene에 GameSession과 GameVisualPresentation이 각각 1개임을 확인했다.

## 추가 파일
Assets/Editor/ClearScorePlayProbe.cs (+Unity meta)

## 재현 및 테스트
Unity batchmode -projectPath D:\Projects\CosmicBlock -executeMethod ClearScorePlayProbe.Run
COSMIC_RUN_SAVE_TEST_PATH는 Validation/clear-score-isolated.json으로 격리한다. 테스트 종료 시 관련 기존 PlayerPrefs를 복원한다. Clear 코루틴은 실제 시간으로 실행하며 동기 강제 완료를 사용하지 않는다.

79 assertions PASS, Compiler Warning 0, Runtime Warning/Error 0.
- 1칸, 제거 없음:10
- 1칸,1줄:110 (CLEAR +100)
- 2칸,1줄:120
- 1칸,2줄:260 (CLEAR +100, 두 번째 +150)
- 연속 두 번1칸/1줄:110→220, Combo2
- HOME→이어하기→1줄:220→330
- 2줄 제거 직후 pause→저장→복원:260, 중복/덮어쓰기 없음
- CLEAR 중 HOME→이어하기:110, stale callback 없음
각 완료 상태에서 실제 Score, 화면 TMP SCORE, JSON score, BEST, 화면 BEST, PlayerPrefs BEST를 비교했다.

증거: Validation/clear_score_tests.txt, Validation/clear_score.log.

## 남은 검증
ADB devices -l 결과 연결 기기 없음. Android에서 실제 오류가 발생한 배치 직전/직후 SCORE, 제거 줄 수, 설치 APK 버전, Logcat 및 current-run.json을 확보한 뒤 재현해야 한다. 원인 미확인 상태를 수정 완료 또는 Android PASS로 처리하지 않는다. APK 재빌드/설치, Commit/Push는 수행하지 않았다.