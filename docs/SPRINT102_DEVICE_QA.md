# Sprint 10.2 Android 실기기 QA — 2026-10-09

Samsung SM-S942N / R3KL20DY0KF / Android 16 / ADB device.

## 빌드 / 설치

최신 소스로 Development IL2CPP ARM64 APK 빌드 성공. Build Error 0, Warning 3 (기존 TMP의 큰 메서드를 별도 cpp로 분리하는 경고). C# Compiler Warning/Error 0. APK 152097304 bytes, SHA256 `ee402b9f5aa2643400895dd467f1373af9d64dda0081f3146f9346f1c5bf573e`.

`adb install -r` Success. 정상 HOME 실행 및 이어하기 성공. Package Name/게임 코드/Remote 변경 없음. Commit/Push 없음.

## 실제 기능 결과

- 제공 두 버튼 Sprite 및 무료 잔여 표시 정상.
- 실제 Single의 라인 완성 위치 추천, 자동 배치/점수/별빛/보드 변경 없음. 중복 터치 hintUses 1만 차감.
- 권장 위치에 실제 Android swipe: SCORE 1000→1110, Combo 1, 별빛 330→340, 행 제거 및 consumed 슬롯 정상.
- 새로고침: 사용 완료 슬롯 유지, 미사용 두 슬롯 교체. SCORE/Combo/별빛/보드/set 번호 불변. 두 번 터치해 refreshUses 1만 적용.
- HOME modal에서 Assist 차단, Cancel 정상. HOME→이어하기 및 강제종료→재실행 후 Run/횟수 동일.
- hint 2/3회 추가 사용, 4초 이후 표시 해제. 무료 소진 후 두 버튼 비활성, 추가 차감/게임 변경 없음.
- background 복귀 저장 상태 유지.
- 실제 배치 불가능 checkerboard에서 마지막 Single 배치→Game Over→저장 Run 무효화 정상.
- Retry: SCORE 0/빈 8×8/새 3 Piece/무료 1회·3회 초기화 정상.

## Responsive

Android wm size로 1080×1920 / 1080×2400 / 1080×1440에서 ready/hint/expired 9캡처. 실제 버튼 터치와 힌트 횟수 변경 검증 PASS. 화면 잘림/겹침 없이 표시되며 1440에서는 보드가 가용 공간에 맞게 작아집니다. 실제 Android Safe Area 사용; 모든 다른 하드웨어 Cutout을 대표하는 검사는 아닙니다.

검증 후 원래 Physical 1080×2340 / override 없음 복원.

## Logcat

앱 UID로 테스트 시작 이후 남은 Logcat 수집:

- FATAL EXCEPTION: 0
- AndroidJavaException: 0
- NullReferenceException: 0
- MissingReferenceException: 0
- Unity Warning: 0
- Unity Error: 9, 모두 기존 `com.google.android.play.core.assetpacks.AssetPackManager` ClassNotFoundException.
- 기타 게임 Runtime Error: 0.

Crash 없음. Android 전체 Error 0은 아님. Missing Script 0은 직전 Editor Scene 검증 결과이며 이번 코드 변경은 없습니다. 기존 전체 Editor Regression PASS 유지.

## 데이터 복원 / 남은 항목

이번 연결 직후 새로 백업한 원본 Run을 바이트 동일하게 복원했습니다. PlayerPrefs 모든 항목/값은 원본과 동일하며 실행 후에도 비교 확인했습니다. ADB shell 전송의 CRLF 변환 때문에 최초 XML 바이트 비교가 실패했으며, transport 개행을 정규화하고 XML 항목 전체 비교와 exec-out 읽기로 복원을 확인했습니다. 게임 데이터 손실/회귀가 아닙니다.

현재 SCORE 0 / Planet01 Energy 330 / BEST 19000 원본 저장 상태로 HOME 실행 중. 테스트 fixture 및 임시 해상도는 남지 않았습니다.

Touch Feel / Haptic Feel / SFX Feel / Clear Timing / Gold Visibility / Display Readability는 사용자 확인 대기입니다. ADB 터치 성공만으로 체감 PASS를 판단하지 않습니다. 힌트 최적성/실기기 프레임 성능 프로파일링은 별도 미실행입니다. 광고 SDK/보상 기능도 구현 대상이 아니므로 테스트하지 않았습니다.

증거: `Validation\sprint102_device_20261009_161425`의 result.json / screenshot / logcat.txt. 빌드 로그: Validation/sprint102_android_build.log.
