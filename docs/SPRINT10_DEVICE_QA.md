# Sprint 10 Android Device QA — 2026-10-09

Device: Samsung SM-S942N / R3KL20DY0KF / Android16 /1080×2340.
ADB Status: device. 검증된 기존 Sprint10 Development APK를 adb install -r로 업데이트 설치 성공. 재빌드/앱 삭제/패키지 변경 없음. 앱 실행 성공.

## 실제 수행 결과
- 사용자 실제 Run SCORE19000 / BEST19000 / Planet01 완료 / HOME 선택02 상태를 새 백업하고 그대로 이어하기 PASS. 64셀/3개 Piece/Combo 및 저장 JSON 불변.
- 별도 QA fixture로 모든5행성 ×4단계 경계+완료 =25회 실제 Single 드래그/1라인 제거 검사 PASS. 실제 플레이로46500 별빛을 모았다고 주장하지 않는다.
- 각 SCORE1000→1110, 저장 점수1110. 25개 실제 HUD의 금색 숫자 픽셀을 육안 확인한1110 기준 화면과 비교:25/25 동일, 최대차이0.0.
- 각 경계-5에서 +10, 다음단계 +5 overflow / 최종목표에서는 +5만 적립. 단계별 전/중앙 전환/후/이어하기 실제 스크린샷 보존. Sprite/Alpha/사각 배경/Portrait Safe Area를 확인했다.
- 완료 시 다음행성 해금 값1, 최종 행성24,000 완료 PASS.
- 25회 HOME→이어하기에서 score/planetEnergy/board/slot state 체크포인트 동일 PASS.
- Planet05 6395→6405 라인 처리 직후 force-stop→콜드HOME→이어하기:1110/6405 및 저장 모델 동일 PASS. 재지급/손실 없음.
- 실제 Collection에서01~05 이동, Locked02~05 선택 버튼 차단, 이전/다음 경계 및 AndroidBack→HOME PASS.
- 실제 남은 H2 Pieces가 배치 불가능한 체크보드에 마지막 Single을 배치:GAME OVER 정상 발생 / Run 파일 무효화 PASS. RETRY→Score0/빈Board/새3Piece/Planet05/별빛6405 유지, HOME→이어하기 동일 PASS.

## 첫 자동 검사 실패 기록
첫 시도에서는 외곽 빈 슬롯 좌표 드래그1회가 반영되지 않아 score1000 상태로 검사 실패했고 즉시 원래 데이터를 복원했다. 이를 게임 버그로 단정하지 않는다. Piece 중심 좌표와 저장 완료 polling을 적용한 독립 재검사에서는25회 모두 통과했다. 원인을 확정할 수 없으며, 슬롯 외곽 Touch Feel은 사용자 확인 대기다. 최초 결과/로그는 sprint10_native_first_attempt*.json/txt에 보존했다. 게임 코드는 변경하지 않았다.

## Logcat 및 복원
- FATAL EXCEPTION0 / AndroidJavaException0 / NullReferenceException0 / MissingReferenceException0.
- Unity Warning0. 기존 환경의 AssetPackManager ClassNotFound E Unity31회(반복 콜드실행 포함). 다른 E Unity0. 전체Logcat Error0이라고 주장하지 않는다.
- 모든 PlayerPrefs 노드/값과 키 존재 여부가 원래 XML과 동일. 원래 current-run.json은 백업과 바이트 단위 동일. SCORE19000/BEST19000/Planet01Energy1500/SelectedPlanet02 및 원래 설정 유지.
- 최종 앱은 원래 상태의 HOME에서 실행 중. 이어하기 버튼으로 원래19000 Run을 계속할 수 있다.
- Touch/SFX/Haptic 체감·디자인 선호는 사용자 확인 대기. 다른 실제 해상도 기기는 사용하지 않았으며 1920/2400Safe/1440은 이전 Editor 자동 렌더 결과다.

## 증거
Validation/sprint10_native_results.json, sprint10_native_extra_results.json, sprint10_native_final_result.json, sprint10_native_score_visual.json, sprint10_native_final_logcat.txt 및 sprint10_native_*.png.

## 25개 경계 결과
| 행성 | 현재 단계 | 이전 별빛 | 이후 별빛 | SCORE | HOME 이어하기 |
|---|---:|---:|---:|---:|---|
| 01 | 1 | 145 | 155 | 1110 | PASS |
| 01 | 2 | 395 | 405 | 1110 | PASS |
| 01 | 3 | 745 | 755 | 1110 | PASS |
| 01 | 4 | 1195 | 1205 | 1110 | PASS |
| 01 | 5 | 1495 | 1500 | 1110 | PASS |
| 02 | 1 | 295 | 305 | 1110 | PASS |
| 02 | 2 | 795 | 805 | 1110 | PASS |
| 02 | 3 | 1495 | 1505 | 1110 | PASS |
| 02 | 4 | 2395 | 2405 | 1110 | PASS |
| 02 | 5 | 2995 | 3000 | 1110 | PASS |
| 03 | 1 | 595 | 605 | 1110 | PASS |
| 03 | 2 | 1595 | 1605 | 1110 | PASS |
| 03 | 3 | 2995 | 3005 | 1110 | PASS |
| 03 | 4 | 4795 | 4805 | 1110 | PASS |
| 03 | 5 | 5995 | 6000 | 1110 | PASS |
| 04 | 1 | 1195 | 1205 | 1110 | PASS |
| 04 | 2 | 3195 | 3205 | 1110 | PASS |
| 04 | 3 | 5995 | 6005 | 1110 | PASS |
| 04 | 4 | 9595 | 9605 | 1110 | PASS |
| 04 | 5 | 11995 | 12000 | 1110 | PASS |
| 05 | 1 | 2395 | 2405 | 1110 | PASS |
| 05 | 2 | 6395 | 6405 | 1110 | PASS |
| 05 | 3 | 11995 | 12005 | 1110 | PASS |
| 05 | 4 | 19195 | 19205 | 1110 | PASS |
| 05 | 5 | 23995 | 24000 | 1110 | PASS |

이번 실기기 검증에서 게임 코드 변경/Commit/Push 없음. 기존 Sprint10 미커밋 구현은 유지하며 검증 문서만 갱신했다.
