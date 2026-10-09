# Sprint 10.2.1 Android 실기기 검증 — 2026-10-09

SM-S942N / Android 16 / ADB device. 기존 최신 Development APK를 재빌드 없이 install -r로 업데이트하고 실행했습니다. 구현 커밋 c43ae78은 origin/main에 반영되어 있습니다.

APK SHA256: `a39740fdfd052a3f0c32bc71735355ec2b92401f58992985b0408e0d685360ff`. 크기 152092325 bytes.

## 기능 결과

- 실제 완료 후 즉시 행성 이동/Run 유지/별빛 이월 없음 행성 1→2: PASS
- HOME 및 실제 process 종료 후 이어하기 동일: PASS
- 새 행성 새 Line Clear만 +10/점수 +110: PASS
- 실제 완료 후 즉시 행성 이동/Run 유지/별빛 이월 없음 행성 2→3: PASS
- 계속 플레이 후 도감에서 해금 행성03으로 동일 Run 전환: PASS
- 전환 후 힌트 잔여1회 및 무료 소진 제한 유지: PASS
- 완료 행성01 복귀/새 점수 +40/별빛 재지급 없음: PASS
- 05 완료는 도감 이동/새 Run 없음: PASS
- GameOver/Retry 새 Run/무료1·3 정상: PASS
- 실제 Android 해상도 팝업/행성 전환/Run 유지 1080x1920: PASS
- 실제 Android 해상도 팝업/행성 전환/Run 유지 1080x2400: PASS
- 실제 Android 해상도 팝업/행성 전환/Run 유지 1080x1440: PASS

전환 전후 score/Combo/blockSet/보드 색상/Shape/Appearance/Consumed/무료 Refresh·Hint 횟수를 저장 JSON으로 비교했습니다. SCORE 340을 유지하며 대상 별빛은 기존 37을 표시하고 새 Line Clear 후 47이 됩니다. 이전 행성 별빛 이월/중복 지급 없음. 행성01 전환 버튼 직후 약 0.141초에 실제 프로세스를 종료해도 재실행/이어하기로 새 행성과 같은 Run을 복구했습니다.

Game Over 테스트의 최초 0.7초 파일 검사 시점은 판정 완료보다 빨랐습니다. 완료를 기다린 뒤 게임 오버 패널과 Run 파일 무효화 및 Retry 초기화를 확인했습니다. 게임 코드 변경 없음.

## 화면 및 데이터 보호

1080×1920 / 1080×2400 / 1080×1440의 실제 기기 wm override에서 완료 팝업·전환 후 HUD와 Run 보존 PASS. 현재 기기의 Safe Area 기준이며 다른 기종의 노치 형태를 모두 검증한 것은 아닙니다. 캡처상 팝업/버튼 잘림 없음.

테스트 시작 시 새 백업을 생성해 격리 fixture를 사용했습니다. 종료 후 원본 PlayerPrefs의 모든 키/값 및 Run 바이트를 복원했습니다. 재실행 후에도 동일함을 검증했습니다. 원본 Run은 행성03/SCORE0/별빛0이며 BEST와 설정도 원복했습니다. wm override 제거 후 원래 1080×2340 복원. 앱은 HOME에서 실행 중입니다.

## 로그 및 한계

FATAL EXCEPTION / AndroidJavaException / NullReferenceException / MissingReferenceException: 각각 0.
Unity Warning: 0. 기존 AssetPackManager ClassNotFoundException: 21회(앱 재실행 시 반복). 그 외 Unity Error: 0. 따라서 전체 Runtime Error 0 조건은 충족하지 않습니다. 기존 빌드 결과는 Error 0 / TMP Warning 3이며 이번에는 빌드를 반복하지 않았습니다. Editor 회귀 및 Missing Script 0 결과는 기존 검증 기록을 유지합니다.

Touch Feel/Haptic Feel/SFX Feel/Clear Timing/Gold Visibility/Display Readability는 사용자 확인 대기입니다. ADB Drag와 UI 기능 성공이 체감 품질 PASS를 의미하지 않습니다.

증거: Validation/sprint1021_device_20261009_202606/result.json, logcat.txt 및 PNG. 새 BGM 파일은 요청대로 제외·보존하며 연결하지 않았습니다.
