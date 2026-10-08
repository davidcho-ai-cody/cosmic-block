# 게임 자동 저장 및 이어하기

## 구현
진행 중인 Run을 전체 행성에 대해 하나만 `Application.persistentDataPath/current-run.json`에 저장합니다. HOME 이동은 Run을 종료하지 않습니다. HOME의 기존 메인 버튼은 저장 유무에 따라 `home_play_button.png` / `home_continue_button.png`로 전환하며 위치·크기·비율과 PNG 원본은 유지합니다. 이미지 위에 중복 문구를 추가하지 않습니다.

저장 Run이 있으면 하단 설정/종료 버튼 사이 빈 영역에 작은 `새 게임 시작` 보조 버튼을 표시합니다. 확인창의 취소는 파일을 유지하며 확인만 기존 Run을 폐기하고 현재 선택 행성으로 SCORE 0/빈 보드/새 블록 3개의 Run을 시작합니다. BEST와 행성별 별빛/해금/선택 PlayerPrefs 키는 삭제하지 않습니다.

## 데이터 및 확정 상태
Save Version 1, active, planetId, score, combo, blockSetNumber, planetEnergy, cells[64], shapes[3], appearances[3], consumed[3]. cells=-1은 빈칸, 0/1/2는 기존 Blue/Purple/Gold 팔레트입니다. UI 문구나 Sprite 이름은 저장하지 않습니다.

새 Run 공급 완료, 배치/라인 제거 완료, HOME 직전, 앱 Pause/정상 Quit에서 저장합니다. 드래그는 취소해 원위치로 정리합니다. Clear 중 Pause/HOME은 남은 Line을 연출 없이 기존 점수 규칙대로 확정한 뒤 공급/판정/저장을 마칩니다.

강제 종료 중간 상태를 막기 위해 Line Clear가 결정될 때 최종 제거 셀/점수/별빛 절대량을 계산한 확정 체크포인트를 먼저 저장합니다. 마지막 Piece라면 다음 3개 Shape/색상을 미리 예약하고 실제 턴 완료에서도 그 동일한 공급을 사용합니다. 화면의 기존 Clear/Fragment/Stage 연출과 시간은 유지합니다. 저장 파일 복원은 이미 반영된 별빛 AddEnergy나 점수 지급 이벤트를 재실행하지 않습니다. 파일 저장과 PlayerPrefs flush 사이 종료된 경우에만 별빛 절대량을 max(기존값, 체크포인트 값)으로 회복합니다. DEV reset은 같은 Run의 절대량도 동기화합니다.

임시 파일을 UTF-8로 기록하고 디스크 Flush 후 기존 파일 Replace/최초 Move로 교체합니다. 버전, ready/unlocked 행성, 배열 길이, 값 범위, 실제 배치 가능 여부와 미제거 Full Line을 검사하며 손상/없음/미지원 파일은 안전하게 새 게임 UI로 처리합니다. 저장 I/O 실패는 기존 파일을 보존하고 Warning으로 알립니다.

## 흐름
이어하기는 저장된 행성으로 복원하며 현재 HOME 선택값을 덮어쓰지 않습니다. 새 게임만 현재 HOME 선택 행성을 사용합니다. GAME OVER는 Run 파일을 제거하며 HOME 메인 버튼은 게임 시작으로 복귀합니다. Retry는 기존 Run 행성으로 새 게임을 시작합니다. HOME에서는 결과 패널과 transient feedback을 정리하되 모델의 Game Over 판정은 바꾸지 않습니다.

03~05는 현재 ContentReady=false여서 선택 불가 상태를 그대로 유지합니다. 문서의 행성02→03 선택 시나리오는 현재 ready 행성02 Run→HOME에서01 선택→이어하기02로 동일한 정책을 검증합니다.

## 변경 파일
- Core/RunSaveStore.cs: 파일 저장/유효성/데이터 구조.
- Core/GameSession.cs: 확정 체크포인트, 예약 공급, 복원, Pause/Quit/HOME 저장.
- Board/BoardView.cs: 셀 팔레트 캡처/복원.
- Blocks/BlockPiece.cs: 기존 팔레트 인덱스 복원.
- UI/GameFlowController.cs: 이어하기/명시적 새 게임/HOME 저장.
- UI/HomeRunControls.cs: 버튼 에셋 교체/보조 버튼/확인창.
- UI/GameHud.cs: HOME에서 Game Over 표시 정리.
- Scenes/Game.unity 및 home_continue_button.png.meta: 기존 Sprite/UI 구조 연결.
- Editor/RunSaveBuilder.cs, RunSavePlayProbe.cs, RunSaveProbeIsolation.cs, RunSaveColdProbe.cs: 빌더/검증/프로세스 재시작/사용자 Run 파일 격리.
- 이전 Sprint5/6.5/6.6/9.3/9.4 Probe: HOME에서 자동 폐기하던 이전 가정을 명시적 새 게임/이어하기 정책으로 갱신.

## 검증
RunSavePlayProbe: 563 assertions PASS. A-L 모델 저장·복원, 두 Piece 소비/보드 색상 일치, Combo, 취소/확인, Game Over/Retry, 절대량 복원/중복 지급 없음, Clear 중 HOME 정리, 손상/미지원 버전/불가능 Shape, 마지막 Piece 이후 예약 공급 일치.
1080×1920 /1080×2400 Safe Area /1080×1440에서 새로운 보조 버튼이 기존 UI Hit Area와 겹치지 않는 자동 검사를 통과했습니다. 게임 시작/이어하기/새 게임 확인창 3상태×3해상도=9개 최종 렌더 확인. 기존 버튼 Hit Rect/Safe Area와 겹침 없음. 렌더 비교: Validation/run_save_render_comparison.jpg.

## 최종 검증 — 2026-10-09
- Editor RunSavePlayProbe: 563 assertions PASS. 드래그 중 Pause 원위치 복귀/확정 데이터 동일, PlayerPrefs flush 손실을 모사한 절대량 125 회복도 PASS.
- 별도 Unity 프로세스 2회: SCORE 1,000 체크포인트 생성→프로세스 종료→콜드 HOME→이어하기에서 점수/보드 색상/소비된 Piece/행성/별빛 정확히 복원 PASS.
- 기존 회귀 14개 PASS: Sprint0,1,2,92,93,94,5,5JourneyFeedback,5NewRunGameOver,65,66,72Ui,761Home,8Collection. 과거 HOME=새 게임 가정만 명시적 새 게임/이어하기로 갱신했습니다. 전체 역사적 29개 Probe를 이번에 모두 재실행했다고 주장하지 않습니다.
- MissingScriptDiagnostics: MISSING_SCRIPT_COUNT=0.
- Compiler Warning/Error 0; 최종 PlayMode Console Warning/Error 0.
- 최종 Android Development Build: Succeeded, Build Warning 0, Build Error 0. 첫 IL2CPP 전체 컴파일에는 기존 TMP의 큰 메서드 C++ 파일 분할 알림 3개가 있었으며 로그를 보존했고 설정/경고 억제 없이 캐시를 사용하는 최종 패키지 빌드는 0입니다.
- APK: Builds/Android/Development/CosmicBlock-dev.apk (151,988,795 bytes).
- SHA256: 10BEC91E786EA8F4248E274C5B3F5D30CA8DECD26CD7A794B59794529653D76B.

## Android 실기기 결과
초기에는 ADB 기기가 없었으나 최종 확인에서 Samsung SM-S942N / R3KL20DY0KF / Android 16이 device 상태로 연결됐습니다. 기존 앱에 adb install -r 성공; Package Name 유지; 앱 설치 전 모든 PlayerPrefs와 기존 Run 파일 유무를 새 백업으로 기록했습니다.

- 실제 드래그로 V3 Blue / V2 Purple 두 Piece 배치: SCORE 50, 소비 상태 [true,true,false], 보드 JSON과 실제 64 Cell 렌더 일치 PASS.
- HOME 확인→이어하기, Android HOME key 백그라운드→복귀, am force-stop 후 콜드 실행→이어하기: SCORE 50/색상/남은 Piece/Combo/별빛 동일 PASS.
- 첫 콜드 캡처는 로딩 완료 전이라 화면 비교가 실패했으나, 로딩 완료를 기다린 별도 재실행에서 동일 데이터를 실제 렌더와 대조해 PASS. 게임 저장 실패로 처리하지 않았습니다.
- 새 게임 시작 확인창: 취소 시 Run 불변, 확인 시 SCORE 0/빈 Board/새 3 Piece/별빛 535 유지 PASS.
- 유효한 QA 저장 fixture SCORE 1,000→HOME→이어하기: 1,000 유지 PASS. 이 값은 QA fixture이며 실제 플레이로 1,000점을 얻었다고 주장하지 않습니다.
- 같은 fixture의 마지막 Single 배치로 1 Line Clear: 체크포인트 SCORE 1,110 / Combo 2 / 별빛 545 / 새 BlockSet 2가 저장됨. Swipe 시작 후 약 1.0초(0.85초 Drag 종료 직후)에 force-stop하여 연출 진행 중 종료. 재실행 후 점수/빈 Board/예약된 새 3 Piece 정확히 복원, 별빛 545 1회만 지급 PASS.
- Android에서 FileStream Flush/기존 파일 Replace를 포함한 실제 자동 저장 I/O 성공; 저장 실패 Warning/IOException/UnauthorizedAccessException 없음.
- QA 종료 후 전체 원래 PlayerPrefs를 XML 노드 단위로 검증하여 복원: Planet01 Energy 535, BEST 15,930, Migration Version 2. QA 전 Run 파일은 없었으므로 테스트 Run을 제거했습니다. 마지막 앱은 사용자 원래 상태의 HOME에서 실행 중입니다. 현재 메인 버튼이 게임 시작인 것은 QA 파일을 제거했기 때문이며 정상입니다.
- FATAL EXCEPTION / AndroidJavaException / NullReferenceException / MissingReferenceException: 0. Unity Warning 0, 새 게임/저장 관련 Unity Error 0.
- 기존 환경의 AssetPackManager ClassNotFoundException은 UID Logcat의 E Unity에 콜드 실행마다 남아 있습니다(6회). 따라서 전체 Android Logcat Error 0으로 보고하지 않습니다. 기존 DexFile finalizer AssertionError 6회와 Swappy libgame.so lookup 13회도 로그에 있습니다. 전체 로그: Validation/run_save_device_logcat.txt.

## 남은 수동 확인
실제 손가락 Touch Feel, SFX/Haptic Feel, 새 보조 버튼의 가독성/터치 편의는 사용자 확인 대기입니다. 저장 중 OS/전원 종료 및 디스크 용량 부족 같은 장치 스트레스 상황 전체를 보장하는 테스트는 하지 않았습니다. 일반 HOME/백그라운드/force-stop/라인 연출 중 종료는 실제 검증했습니다. 행성03~05는 아직 ContentReady=false여서 선택할 수 없습니다.

Git Commit/Push는 이번 지시문에 요청되지 않아 수행하지 않습니다. 기존 사용자 Planet04 자산은 이번 작업 범위 밖이며 그대로 보존합니다.
