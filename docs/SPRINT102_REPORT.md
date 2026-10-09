# Sprint 10.2 — 블록 새로고침 / 힌트

2026-10-09. Editor 구현 및 자동 검증 완료. 사용자 요청에 따라 Android 실기기 테스트 NOT RUN. APK 빌드/설치 및 수동 ADB 명령 미실행. 기존 APK는 Sprint 10.1이며 이번 기능을 포함하지 않습니다. Commit/Push 미실행.

## 구현 및 변경 파일

- `Assets/Scripts/Core/GameSession.cs`: Assist 실행 조건, 미사용 슬롯 교체, 무료 횟수, 힌트 수명/정리, 저장/복원 연결.
- `Assets/Scripts/Core/RunSaveStore.cs`: refreshUses/hintUses 및 향후 광고용 필드. 기존 version 1 유지, 누락 필드 0으로 호환.
- `Assets/Scripts/Core/PlacementHint.cs`: 실제 보드를 복제해 합법적인 위치를 순회하는 결정적 추천.
- `Assets/Scripts/Core/RewardedAdContract.cs`: RewardType/확인된 보상 ID/provider 인터페이스만 정의.
- `Assets/Scripts/UI/GameAssistView.cs`: 하단 두 버튼, 잔여 횟수, 별도 비입력 힌트 표시 및 cleanup.
- `Assets/Scripts/UI/GameVisualPresentation.cs`: 하단 Assist 영역을 반응형 레이아웃에 예약.
- `Assets/Scenes/Game.unity`: GameCanvas에 Assist component와 Sprite reference 추가. 기존 완료 팝업 보존.
- `Assets/Editor/Sprint102Builder.cs`, `Sprint102PlayProbe.cs`, `Sprint102VisualProbe.cs` 및 각 meta: Import 및 자동 검증.
- 두 버튼 PNG의 meta 및 CURRENT_TASK/NEXT_TASK/DEVLOG, 본 보고서.

기존 Sprint 10/10.1 미커밋 변경은 보존했습니다. 위 목록은 이번 Sprint의 범위이며 git 전체 diff에는 이전 작업도 포함됩니다.

## 에셋

원본 `game_block_refresh_button.png`, `game_hint_button.png` 그대로 사용. 모두 1922×818 RGBA, alpha 0~255, 네 모서리 투명. 완전 투명 픽셀 각각 520112/509633개, 외곽 5px 불투명 검정 픽셀 0개. 원본 SHA256은 Validation/sprint102_assets.json과 최종 비교 일치.

Texture2D / Sprite Single / FullRect / Alpha From Input + Transparency / MipMap off / Clamp / Bilinear. Desktop uncompressed, Android ASTC 6×6, 최대 2048. Preserve Aspect 적용. 초기 Import에서 TextureShape를 명시하지 않아 Cubemap으로 남은 문제는 Texture2D로 지정해 해결했으며 실제 Sprite localID 21300000 로드까지 검증했습니다. 원본 이미지 가공 없음.

왼쪽 새로고침 / 오른쪽 힌트. 이미지에 포함된 아이콘·글자를 중복 생성하지 않으며 무료 잔여 횟수만 별도 표시합니다. 현재 Scene에 기존 Playing 하단 다시하기/힌트 실버튼은 없었으며 Game Over Retry는 유지했습니다.

## 새로고침

미사용 슬롯에만 기존 generator/catalog로 새 Piece를 공급합니다. Consumed 슬롯, board, SCORE/BEST, Combo, 별빛, 공급 set 번호는 유지합니다. 카탈로그 중 배치 가능한 shape가 존재하면 최소 하나를 보장하고 이후 Game Over를 재평가합니다. 가능한 shape가 단 하나라면 shape 자체가 같을 수 있습니다. 배치 가능한 shape가 전혀 없거나 미사용 슬롯이 없으면 무료 횟수를 차감하지 않습니다.

Run당 무료 1회. 빠른 중복 입력은 실행 잠금과 0.25초 cooldown으로 방지합니다.

## 힌트

미사용 Piece의 모든 합법적 위치를 검사합니다. 라인 수 → 중복을 제외한 제거 셀 수 → 제거 후 빈 공간 → 이후 가능한 카탈로그 배치 수 순으로 비교하며 동률은 slot/y/x 순으로 안정적으로 선택합니다. 휴리스틱 추천이며 미래 최적해를 보장하지 않습니다.

권장 Piece 테두리와 보드 셀을 별도 cyan 표시로 강조합니다. 자동 배치/점수/별빛 변경 없음. 4초 후 또는 Drag/HOME/pause/새 Run cleanup 시 해제합니다. 표시 성공 이후만 차감하며 Run당 무료 3회입니다. Editor probe의 마지막 측정은 66ms; 모바일 성능은 미검증입니다.

## 저장 / 입력 보호 / 광고 확장

무료 사용 횟수를 실제 Run snapshot에 저장하고 HOME→이어하기/복원 후 유지합니다. 새 Run/Retry에서만 초기화합니다. 구버전 파일은 추가 필드가 없으면 0으로 시작합니다. 기존 게임 상태 및 완료 확인 상태를 보존합니다.

HOME, Game Over, resolving, Drag, modal, background에서는 실행을 막습니다. 힌트 객체는 즉시 비활성화 후 제거해 stale 표시를 방지합니다. 버튼 계층은 기존 modal/완료/Game Over overlay 아래에 배치했습니다.

BlockRefresh/HintRecharge/GameOverRevive의 인터페이스 및 completion ID 확장 지점만 준비했습니다. SDK/provider/광고 버튼/가짜 보상/부활 구현 없음. 광고 counter와 receipt는 현재 0/비어 있음만 허용하며 실제 중복 보상 지급 구현은 후속 provider 작업입니다. 무료 소진 후 비활성 표시와 '광고 보상 준비 중' 문구만 노출합니다.

## 자동 테스트

`Validation/sprint102_play_tests.txt`: 7개 묶음 PASS. 미사용 교체/Consumed 유지, board·SCORE·BEST·Combo·별빛 불변, 최소 배치 가능 보장, 무료 1/3 제한, 중복 실행, 실제 라인 우선 합법 힌트, 자동배치 없음, 불가능 시 미차감, 실제 Drag 이벤트 cleanup, 4초 timeout, 저장/이어하기, legacy save, HOME/modal/resolving/background guard, Game Over/Retry 검사.

기존 Sprint101 / ClearScore / RunSave / Sprint10 / Sprint1 / Sprint2 / Sprint92 / Sprint94 / Sprint66 / Sprint5NewRunGameOver / Sprint8Collection probe 모두 exit 0. 배치·라인·Combo·BEST·행성 복원/전환·자동 저장·완료 팝업 회귀 PASS. `Validation/sprint102_regression_summary.txt`에 기록했습니다.

최종 컴파일 Warning/Error 0, 일반 Play Console Warning/Error 0, Missing Script 0. Sprint101의 의도적인 저장 실패 테스트에서 예상 IO warning 1개가 발생하는 것은 테스트 조건이며 일반 Runtime 경고가 아닙니다. 초기 실패/Import 진단 로그는 최종 PASS로 대체되었습니다.

## Responsive

1080×1920 / 1080×2400 + Safe Area / 1080×1440에서 ready·hint·refresh_used·exhausted 총 12개 Render PASS. 실제 Sprite, preserveAspect, SafeArea bounds, 버튼 활성/비활성, 횟수 글자 잘림, 추천 셀 geometry를 자동 검사하고 렌더도 확인했습니다. 새 영역 때문에 짧은 화면의 정사각 보드는 가용 공간에 맞게 축소됩니다. 8×8 구조와 게임 규칙은 유지하며 기존 HUD/슬롯 디자인을 보존합니다.

대표 이미지: Validation/sprint102_ready_1920.png, sprint102_ready_2400.png, sprint102_hint_1440.png, sprint102_exhausted_1440.png.

## Android 및 남은 확인

Android Build / Install / Launch / Logcat / 실기기 Touch 모두 NOT RUN (사용자 요청). 빌드 Warning/Error는 이번 작업에서 판정하지 않습니다. Unity batch 종료 과정의 자체 ADB server cleanup 로그는 존재하지만 기기 검증 명령은 실행하지 않았습니다.

별도 요청 후 Sprint10.2 APK 빌드 및 실기기 버튼 크기/터치/힌트 가독성/성능 검증이 필요합니다. 이전 Sprint10.1의 TMP build warning 3개와 Android AssetPackManager 환경 오류는 이번 Editor 작업에서 재검증하거나 해결한 것으로 간주하지 않습니다. Commit/Push 없음.

## 후속 실기기 검증 완료

사용자의 연결/테스트 요청에 따라 최신 Development APK를 빌드·설치하고 기능/3비율/Logcat 검증 완료. 이전 NOT RUN 문구는 Editor 작업 시점의 기록입니다. Build Error 0/기존 TMP Warning 3, Crash 0/기타 게임 오류 0/기존 AssetPackManager Error 9. 원본 Run/Prefs/해상도 복원. 상세 [실기기 QA](SPRINT102_DEVICE_QA.md). Commit/Push 없음.
