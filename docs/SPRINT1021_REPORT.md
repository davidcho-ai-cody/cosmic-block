# Sprint 10.2.1 — 행성 전환 시 Run 유지

## 변경 원인 및 처리

기존 완료 팝업은 Next → PlanetMoveConfirm → ConfirmMove → TryStartNewRunForPlanet → PrepareNewRun → ResumeSavedRun 순으로 실행됐습니다. PrepareNewRun이 score=0/combo=0/빈 보드/새 Piece를 만들기 때문에 행성 이동을 새 게임으로 처리하고 있었습니다.

이 경로를 제거하고 GameSession.TrySwitchRunPlanet으로 현재 snapshot의 planetId/planetEnergy/완료 확인 영수증만 바꿉니다. 전체 snapshot을 먼저 File.Replace 기반 저장으로 확정하며 저장 실패 시 모델/선택값을 바꾸지 않습니다. 성공 후 PlanetSelection 및 Restoration을 갱신합니다. 실제 GAME 전환에서는 보드와 Piece를 재초기화하거나 재생성하지 않으며 기존 generator도 유지합니다. HOME의 저장 Run은 동일 저장/복원 경로로 복원합니다.

보존: SCORE/BEST/64셀 색상/슬롯 Shape·색상·Consumed/Combo/공급 번호/무료 새로고침·힌트 횟수/유효 Run. 변경: 행성 ID·이름·Sprite·영구 별빛·단계·게이지. snapshot version 1 및 기존 저장 키/점수/별빛 규칙 유지.

## 팝업 / HUD / 도감

- 완료 연출과 완료 팝업은 유지합니다. 버튼은 다음 행성으로 이동 / 계속 플레이입니다.
- 추가 PlanetMoveConfirm 객체를 만들지 않습니다. 다음 이동은 GAME 화면에서 바로 현재 Run을 전환합니다.
- 기존 행성 HUD Fade Out 0.28초 → HUD 갱신 → Fade In 0.28초, 총 약 0.56초. SCORE/보드/슬롯은 Fade 대상이 아닙니다.
- 전환 동안 ModalInputBlocked로 Drag/Placement/Assist를 차단합니다. 중복 이동도 거부합니다.
- HOME/pause/종료 cleanup으로 coroutine·HUD alpha·입력 잠금을 정리합니다. 다음 행성의 대기 완료 팝업은 Fade 종료 후 표시합니다.
- 계속 플레이는 같은 행성과 Run을 유지하고 알림을 확인 처리합니다. 완료 행성에 추가 별빛/완료 연출을 반복 지급하지 않습니다.
- 도감의 해금 행성 선택도 같은 전환 API를 사용합니다. 저장 Run이 있으면 GAME으로 복귀하며 선택 버튼 문구는 '이 행성에서 이어하기'입니다.
- 같은 행성 선택은 Fade 생략, 잠긴 행성 선택은 거부합니다. 완료한 이전 행성으로 돌아갈 수 있습니다.
- 저장 Run이 없으면 기존 행성 선택→HOME→PLAY 새 Run 흐름을 유지합니다. 도감 전체 디자인/배치 변경 없음.
- HOME의 명시적인 새 게임 확인/Retry/저장 Run 없는 PLAY만 기존 새 Run 초기화를 수행합니다.

## 별빛 / 저장 / 중단 보호

대상 행성 planetEnergy는 해당 행성의 PlayerPrefs에서 읽습니다. 이전 행성의 별빛/LastClear를 새 행성으로 이월하지 않습니다. ResetTransientFeedback에서 이전 turn token을 무효화하고 callback/fragment/transition을 정리하며, energyCommitted=true와 대상 행성의 기준 별빛으로 새 경계를 설정합니다. 이후 실제 새 Line Clear만 기존 보상 규칙으로 적립됩니다.

기존 completionAcknowledgedPlanet 필드를 거래 영수증으로 활용합니다. 저장 이후 process가 중단되어도 ResumeSavedRun이 저장 행성 ID·별빛·확인 상태·PlanetSelection을 복구합니다. 이전 별빛을 다시 AddEnergy하지 않습니다. 실패 시 기존 파일은 그대로 남습니다. 저장 데이터 삭제/버전 migration 없음.

## 변경 파일

- Assets/Scripts/Core/GameSession.cs: 현재 Run 전환/원자적 저장/선택값 복원/HUD 갱신 진입점.
- Assets/Scripts/UI/GameFlowController.cs: 공통 행성 전환 및 Fade/입력·cleanup, 도감 연결.
- Assets/Scripts/UI/PlanetCompletionPopup.cs: 추가 새 게임 확인 제거, 직접 이동 및 완료 대기 guard.
- Assets/Scripts/UI/PlanetCollectionView.cs: Run 이어하기 선택 문구.
- Assets/Scripts/UI/PlanetRestorationView.cs / GameVisualPresentation.cs: Fade 중 표시 행성 ID와 별빛을 묶어 복원도 문구 계산.
- Assets/Editor/RunSavePlayProbe.cs / Sprint94PlayProbe.cs: 새로운 단일 Run 정책에 맞춰 선택값 복원 및 도감→GAME 기대값 갱신.
- Assets/Editor/Sprint101PlayProbe.cs: 변경된 정책에 맞춰 기존 완료 팝업 회귀 기대값 갱신.
- Assets/Editor/Sprint1021PlayProbe.cs 및 meta: 보존/별빛/도감/저장 실패/중단/Retry 테스트 추가.
- Assets/Editor/Sprint1021VisualProbe.cs 및 meta: 세 비율 팝업/전환/도감 Render 검사.
- README.md / PRODUCT_VISION.md / CURRENT_TASK.md / NEXT_TASK.md / DEVLOG.md 및 docs의 상태 문서.

Scene/아트 원본/Board·Piece·ScoreRules·별빛 보상·광고 SDK/엔딩 변경 없음. 기존 미커밋 Sprint10/10.1/10.2 변경은 보존했습니다. 기존 Git main은 origin/main보다 3 commit 앞서 있으며 이번 작업에서 Git Commit/Push/Remote 변경 없음.

## 자동 검증

최종 신규 전환 검사 및 기존 회귀 전부 PASS. 최종 Compiler Warning/Error 0, 일반 Play Console Warning/Error 0, Missing Script 0. 저장 실패 보호 검사는 의도적인 IO warning 1개를 별도로 발생시켰으며 정상 Runtime 경고와 구분합니다. 초기 컴파일/이전 정책 기대값 실패는 수정 후 최종 PASS로 대체했습니다. 증거: Validation/sprint1021_regression_summary.txt, sprint1021_play_tests.txt, sprint1021_visual_tests.txt 및 sprint1021_check_*.log.

실제 Android process 강제종료는 미실행입니다. Editor의 저장 snapshot/선택값/알림 영수증 복원 경로와 전환 중 HOME cleanup을 검사하며, 실기기 강제종료 및 Fade 체감은 별도 요청 후 확인합니다.

## 미실행 및 남은 사항

문서 지시대로 Android APK 빌드/설치/실기기/ADB 테스트는 수행하지 않습니다. 현재 설치된 APK는 Sprint10.2이며 10.2.1 변경을 포함하지 않습니다. 실기기 UX/Touch/Haptic/SFX와 전환 중 실제 process 종료는 사용자 요청 대기입니다.

기존 Android AssetPackManager 오류 및 TMP Build Warning은 이번 Editor 작업에서 해결/재검증한 것으로 간주하지 않습니다. Commit/Push 미실행.

## 최종 검증 결과

| 검증 | 결과 |
|---|---|
| 행성 01→02 / 02→03 | 점수·BEST·보드 색상·슬롯·Consumed·Combo 3·무료 사용 횟수 유지 PASS |
| 전환 후 별빛 | 대상 행성 기존 37 유지, 새 Line Clear 후 47, 이전 행성 목표값 유지 PASS |
| HUD Fade | 0.28초 Out + 0.28초 In, 이전 행성/100% 유지, 중간 SCORE 0 없음, 입력 차단/cleanup PASS |
| 도감 | 잠금 거부/완료 행성 복귀/동일 행성 Fade 생략/현재 Run으로 GAME 복귀 PASS |
| 계속 플레이 / 최종 행성 | 완료 재지급/팝업 반복 없음, 행성05에는 다음 행성 대신 도감 PASS |
| 저장 실패 / 중단 | 파일 잠금 실패 시 기존 Run 바이트·모델·선택값 유지, 저장만 확정된 transaction의 행성/영수증 복구 PASS |
| HOME / 복원 / pause | 점수·보드·슬롯·Combo·횟수 유지, Fade/입력 잠금 정리 PASS |
| Game Over / Retry / 새 게임 | Run 무효화 및 새 Run 초기화, 무료 1/3 복구 PASS |
| 3해상도 | 1080×1920 / 1080×2400 + Safe Area / 1080×1440, 팝업·중간 HUD·행성02·도감 총 12 Render PASS |
| 기존 회귀 | Sprint101/Sprint102/RunSave/Sprint10/Sprint2/Sprint92/Sprint94/Sprint66/Sprint5NewRunGameOver/Sprint8Collection PASS |
| 최종 추가 검사 | ClearScore/Sprint102 버튼 12렌더/MissingScript 재검사 PASS |

기존 회귀 중 RunSave와 Sprint94의 'Run 행성과 선택값 불일치 유지', '도감 선택 후 HOME' 기대값을 현재 정책으로 갱신했습니다. 점수·별빛·보드·Combo 관련 assertion은 그대로 유지했습니다.

Android Build/Install/실기기/실제 앱 강제종료: NOT RUN. Git Commit/Push: 미실행. 기존 미커밋 작업과 기존 원격 설정 보존. 테스트 격리 파일 및 PlayerPrefs backup을 사용하고 테스트 종료 후 원래 Editor 설정값을 복원했습니다.

## 2026-10-09 추가 요청 — Android 빌드 및 Git 반영

사용자가 Android 빌드/설치/실기기/Commit·Push를 요청했습니다. 최신 Development APK 빌드 성공, Compiler Error/Warning 0, Build Error 0 / 기존 TMP 큰 메서드 C++ 분리 Warning 3. APK CRC PASS.

APK SHA256: `a39740fdfd052a3f0c32bc71735355ec2b92401f58992985b0408e0d685360ff`, 크기 152092325 bytes. 출력: Builds/Android/Development/CosmicBlock-dev.apk.

시작 시 SM-S942N(Android16) ADB device 연결을 확인하고 이번 연결의 원본 PlayerPrefs/Run을 새로 백업했습니다. 원본 Run은 행성03/SCORE0/별빛0입니다. 빌드 완료 후에는 ADB 목록에서 기기가 사라져 install -r이 실패했습니다. 따라서 최신 APK 설치·앱 실행·실기기 기능/Logcat 검증은 NOT RUN이며 재연결 요청 대기입니다. 테스트 fixture는 기기에 적용하지 않아 원본 저장 데이터는 변경하지 않았습니다. 기존 APK가 최신으로 바뀐 것으로 간주하지 않습니다.

사용자가 새로 추가한 Assets/Audio/BGM/cosmic_block_main_bgm.mp3 및 관련 meta는 명시적 요청에 따라 이번 커밋에서 제외하고 보존합니다. BGM 연결/재생 로직 변경 없음.

기존 검증된 Sprint10~10.2.1 미커밋 소스·에셋 설정·테스트·문서를 일반 Commit/Push 대상으로 정리했습니다. APK/Validation/Unity 임시 캐시는 제외합니다. 오래된 빈 index.lock은 실행 중인 Git이 없는 것을 확인하고 제거했습니다. Remote 변경/Force Push 없음. Git 최종 해시 및 동기화 결과는 완료 응답에 기록합니다.
