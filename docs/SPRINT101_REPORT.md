# Sprint 10.1 — 완료 팝업 및 복원 게이지

## 범위 및 Git

Sprint 10의 미커밋 변경을 보존했습니다. Commit/Push/Remote 변경 없음. 기존 SCORE/BEST, 별빛 지급량, 단계 기준, 보드/블록 및 HUD 원본 아트는 변경하지 않았습니다.

## 게이지 원인과 수정

전체 복원도 계산은 이미 누적 별빛 / 행성 목표 별빛이며 완료 시 1.0입니다. HUD 텍스트도 같은 전체 복원도를 표시합니다. 단계 별빛은 별도의 단계별 값입니다.

ProgressBar 부모는 예전 x=0.48..0.91 고정 앵커였으나, 실제 HUD 아트는 GameFrameMesh의 구간별 늘리기로 표시됩니다. 고정 앵커가 그 아트 내부 트랙 끝과 일치하지 않아 Image.fillAmount=1이어도 짧게 보였습니다. Fill Image는 Sliced이므로 실제 너비는 RectTransform 앵커로 결정됩니다.

GameFrameMesh에 내부 트랙의 원본 좌표 982..1858/2048를 기존 메시 변환으로 사상하는 함수를 추가했습니다. GameVisualPresentation에서 그 결과로 ProgressBar의 x 앵커만 맞춥니다. y 위치/높이, 둥근 Fill Sprite, 프레임 아트 및 보드/슬롯 레이아웃은 유지합니다.

5개 행성 × 0/25/50/75/99/100% × 1080×1920/1080×2400(Safe Area)/1080×1440의 90개 렌더에서 실제 금색 픽셀 끝을 측정했습니다. 전부 오차 1.5% 이내 PASS. 1920 화면에서 실제 비율은 0/0.2510/0.5000/0.7491/0.9898/1.0002입니다. 픽셀 반올림 오차 외에 100% 오른쪽 빈 구간 없음.

증거: Validation/sprint101_gauge_pixels.txt, sprint101_gauge_rects.csv, sprint101_gameN_percentP_H.png.

## 완료 알림 및 이동

행성별 CompletionNotice PlayerPrefs는 0 미완료 / 1 표시 대기 / 2 확인 완료로 관리합니다. 실제 목표 도달 시 별빛 저장과 함께 대기를 영구 저장하고, 기존 Fragment/행성 전환 및 라인 연출이 끝난 뒤 팝업을 표시합니다. 기존 완료 데이터에서 키가 없으면 확인 완료로 초기화하므로 과거 완료 팝업을 재생하지 않습니다.

기존 HOME 확인창의 폰트/버튼/패널을 조립해 완료 팝업과 이동 확인창을 구성합니다. 팝업 동안 입력을 막으며 계속 플레이는 Run을 유지합니다. 다음 행성은 별도 확인 후 새 Run을 먼저 원자적으로 저장하고 나서 화면/모델을 전환합니다. 선택한 다음 행성에 SCORE 0/빈 8×8 보드/3개 블록으로 진입하며 BEST와 모든 행성 기록을 유지합니다. 선택 취소 시 기존 Run 파일/점수는 변하지 않습니다.

새 Run 파일의 선택적 completionAcknowledgedPlanet 영수증은 파일 저장 이후 Prefs 확인 기록이 저장되기 전에 종료된 경우를 복구합니다. 저장 버전 1과 기존 파일 호환을 유지합니다. 체크포인트의 완료 별빛 복원 시에도 대기 알림을 복구하되 별빛을 다시 지급하지 않습니다. 행성05는 다음 행성 대신 행성 도감을 엽니다.

## 변경 파일 (이번 작업)

- Assets/Scripts/Core/PlanetCompletionNotice.cs
- Assets/Scripts/Core/PlanetRestoration.cs
- Assets/Scripts/Core/RunSaveStore.cs
- Assets/Scripts/Core/GameSession.cs
- Assets/Scripts/UI/PlanetCompletionPopup.cs
- Assets/Scripts/UI/GameFlowController.cs
- Assets/Scripts/UI/GameFrameMesh.cs
- Assets/Scripts/UI/GameVisualPresentation.cs
- Assets/Scenes/Game.unity
- Assets/Editor/Sprint101Builder.cs
- Assets/Editor/Sprint101PlayProbe.cs
- Assets/Editor/Sprint101VisualProbe.cs
- Assets/Editor/CompletionNoticeProbeScope.cs
- CURRENT_TASK/NEXT_TASK/DEVLOG 및 docs 대응 파일

## 검증 진행 상태

초기 완료/취소/이동/영수증 5행성, Continue/이어하기/체크포인트 대기 복구/HOME 정리 PASS. 최종 I/O 실패 보호 테스트 및 실제 Button 이벤트, 팝업 Fade 종료 화면/문구 잘림 검사도 PASS입니다. Continue 전후 Run JSON이 정확히 동일하며 추가 라인 제거에서 팝업 및 별빛 중복 없음 PASS입니다.

기존 회귀는 신규 완료 팝업을 자동 확인하는 Editor 전용 테스트 스코프를 사용합니다. 완료 팝업 자체는 별도 Sprint101PlayProbe에서 실제 연출 종료 후 대기/클릭/취소/이동을 검증합니다. 테스트 스코프는 원래 CompletionNotice Prefs를 복원합니다.

기존 ClearScore, RunSave, Sprint10, Sprint2, Sprint92, Sprint94, Sprint66, Sprint5NewRunGameOver, Sprint8Collection 회귀 PASS. Missing Script 0. 첫 Android Development Build 성공/Error0/Warning3(TMP 큰 IL2CPP 메서드 분할). 마지막 변경까지 포함한 최종 Android Development Build도 성공/Error0/Warning3입니다. 경고는 기존 TMP .cctor()/TextMeshPro.GenerateTextMesh()/TextMeshProUGUI.GenerateTextMesh()의 큰 메서드 C++ 파일 분할 알림입니다. 범위 밖 패키지 수정으로 경고를 숨기지 않았습니다. 후속 SM-S942N 실기기 설치/기능/게이지/Responsive 검증 완료. 체감 항목은 사용자 확인 대기입니다. 상세 SPRINT101_DEVICE_QA.md.


## QA A–O

| 항목 | Editor 결과 | Android 결과 |
|---|---|---|
| A 최초 완료 1회 | 5행성 실제 최종 Line Clear PASS | PASS (후속 실기기 QA) |
| B 계속 플레이 Run 보존 | JSON 완전 동일 PASS | PASS (후속 실기기 QA) |
| C HOME/이어하기 반복 없음 | 실제 Flow PASS | PASS (후속 실기기 QA) |
| D 앱 재실행 Run 복원 | 기존 RunSave 복원 회귀 PASS | PASS: 실제 강제 종료/재실행 |
| E/F 이동 확인/취소 | 실제 Button 이벤트, 기존 파일 동일 PASS | PASS (후속 실기기 QA) |
| G 새 행성 SCORE0 | 01→02→03→04→05 PASS | PASS (후속 실기기 QA) |
| H BEST/복원 기록 유지 | BEST19000/완료 별빛 유지 PASS | PASS (후속 실기기 QA) |
| I 02~04 해금 | 실제 완료 및 기존 25경계 회귀 PASS | PASS (후속 실기기 QA) |
| J 05 완료 | 도감 버튼/Collection 진입 PASS | PASS (후속 실기기 QA) |
| K 완료 종료 복구 | pending/checkpoint/transaction receipt 복구 PASS | PASS: 실제 완료 직후 강제 종료 |
| L 중복 지급 없음 | 완료 이후 추가 Line Clear PASS | PASS (후속 실기기 QA) |
| M/N 저장 및 점수 | 전체 기존 회귀 PASS | PASS (후속 실기기 QA) |
| O Responsive | 3해상도 90게이지 +6팝업 렌더 PASS | PASS (후속 실기기 QA) |

일반 신규 PlayMode 테스트 Error/Warning 0, 최종 C# Compiler Error/Warning 0. 파일 잠금으로 의도적으로 유발한 저장 실패 경고 1개는 예상된 테스트 결과로 별도 기록합니다. 최초 테스트 코드의 obsolete/변수 충돌은 수정 후 새 최종 실행으로 재검증했으며 최종 실행에 해당 Compiler Warning/Error는 없습니다.

Scene 전체를 HEAD로 재구성하려던 정리 명령은 자동 승인 검토에서 미커밋 변경 덮어쓰기 위험으로 거부되었습니다. 해당 작업은 실행되지 않았습니다. 확인된 자동 변경값만 개별 복구했으며 최종 Scene diff는 완료 팝업 컴포넌트 추가 15줄뿐입니다.


## 최종 APK 및 실기기

- APK: Builds/Android/Development/CosmicBlock-dev.apk
- 생성: 2026-10-09 11:26 KST
- 크기: 152,001,968 bytes
- SHA-256: 8d60b13361b8ce45e32c68cc0e5ecfe354a26673a927874f3dad9f5e35ff1dc4
- ZIP CRC 검사 PASS
- 최종 APK: Development + IL2CPP ARM64, 기존 Package Name 유지
- 후속 ADB SM-S942N device 확인. install-r/launch/Logcat/실제 강제 종료·재실행 QA 완료. 상세 SPRINT101_DEVICE_QA.md.
- Editor 테스트는 별도 Validation Run 및 Prefs 백업/복원을 사용했습니다. 후속 Android QA는 연결 시 최신 원본을 새로 백업한 뒤 테스트값을 사용하고 원본 Run 바이트·기존Prefs·해상도를 복원했습니다.
- 최종 Compiler Warning/Error 0, 일반 PlayMode Warning/Error 0, Missing Script 0, Build Error 0, Build Warning 3. 따라서 모든 Warning이 0이라고 보고하지 않습니다.
- Git HEAD d0aa465 유지. 현재 Sprint 10/10.1 미커밋 변경 보존. Commit/Push/Force Push/Remote 변경 없음.

빌드가 자동 변경한 시작 당시 clean인 설정 5개와 .utmp 부산물만 정리했습니다. Source PNG, 기존 HUD/보드/슬롯 Scene 값은 보존했습니다. 최종 Scene diff는 Popup 컴포넌트 15줄 추가입니다.


## 2026-10-09 실기기 후속 완료

SM-S942N Android16에 최신 APK 업데이트 설치/실행 완료. 5행성 완료/취소/다음 Run/05도감, 완료 직후 강제종료 복구, Continue/HOME/앱 재실행 중복 방지, 실제 Game Over/Retry/이어하기 PASS. 실제 게이지30픽셀 검사 및3 Android 해상도 PASS. 원래 최신SCORE0/Energy330 Run 바이트·기존Prefs·해상도를 복원하고 HOME 실행 중. Unity Warning0/게임예외0/Crash0. 기존 AssetPackManager ClassNotFoundException 로그는 남아 전체Error0은 아님. 앞의 NOT RUN/연결대기 표는 이전 상태이며 본 후속 검사 결과로 대체합니다. 체감 항목은 사용자 확인 대기. 코드/재빌드/Commit/Push 없음. 상세 [실기기 QA](SPRINT101_DEVICE_QA.md).
