# Sprint 10.1 Android 실기기 QA — 2026-10-09

## 설치 및 기기

- Samsung SM-S942N / R3KL20DY0KF / Android 16 / ADB device
- 무결성 검증된 Sprint10.1 APK(152001968 bytes, SHA256 8d60b13361b8ce45e32c68cc0e5ecfe354a26673a927874f3dad9f5e35ff1dc4)를 adb install -r로 업데이트 설치 성공
- APK 재빌드 없음. Package Name/코드/Game Feel 변경 없음. Commit/Push 없음.
- 앱 정상 실행. 마지막 HOME PID 14849

## 기능 검증

- 5개 행성에서 실제 Single 배치 + 최종 Line Clear: SCORE1000→1110, 별빛 목표값 제한, 완료 팝업 표시 PASS
- 01~04 다음 행성 확인 → 취소: 현재 Run 완전 동일, 대기 알림 유지 PASS
- 확인 → 새 Run: 다음 행성/SCORE0/빈 보드/3개 미소비 Piece/거래 영수증 저장, 기존 BEST19000 및 복원 기록 유지 PASS
- 행성05 완료: 다음 행성 대신 도감 이동, 기존 Run 유지 PASS
- 최종 Line Clear 저장 직후 강제 종료 → 재실행 → 이어하기: SCORE1110/완료 별빛/대기 알림 복구 PASS
- 계속 플레이: 현재 Run JSON 내용 동일, 팝업 닫힘 PASS
- 완료 행성에서 추가 Single 배치: +10, 별빛 중복 지급/팝업 반복 없음 PASS
- HOME → 이어하기 및 확인 후 앱 강제 종료/재실행: Run 동일, 팝업 반복 없음 PASS
- 실제 배치 불가 Checkerboard/Remaining Domino로 Game Over → Run 무효화 → Retry → SCORE0/빈 보드/3Pieces/별빛330 유지 → HOME 이어하기 PASS

## 실제 게이지

5행성 × 0/25/50/75/99/100% = 30개 실제 Android 스크린샷에서 금색 Fill 픽셀을 측정했습니다. 전체 PASS.

| 목표 | 측정 |
|---|---|
| 0% | 0% |
| 25% | 25.1037% |
| 50% | 50% |
| 75% | 74.8963% |
| 99% | 98.9627% |
| 100% | 100% |

실제 트랙 중심 행은 y497, x514..995(482px)입니다. 반올림 오차 허용1.5% 이내. 100%는 오른쪽 끝까지 채워집니다. HOME/이어하기 및 앱 재실행 후에도 완료 상태가 유지됐습니다.

## Responsive

Android wm size를 임시 변경해1080×1920/1080×2400/1080×1440에서 완료 게이지와 완료 팝업을 실제 실행/캡처했습니다. 세 비율에서 문구 잘림/버튼 화면 잘림 없이 PASS. 2400은 실제 Android Safe Area를 사용합니다. 확인 후 원래 Physical size1080×2340/override 없음으로 정확 복원했습니다. 이 검사는 다른 기기 하드웨어의 Cutout 종류까지 대표하지는 않습니다.

## Logcat

최종 수집 앱 UID Logcat 기준:
- FATAL EXCEPTION0
- AndroidJavaException0
- NullReferenceException0
- MissingReferenceException0
- Unity Warning0
- 기타 Unity Error0
- 기존 java.lang.ClassNotFoundException: com.google.android.play.core.assetpacks.AssetPackManager: 35개

따라서 Android 전체 Error0이라고 보고하지 않습니다. AssetPack 환경 오류는 이전 Sprint10에서도 있었고 현재 요청 범위에서 코드를 수정하지 않았습니다. 테스트 중 반복 앱 시작으로 여러 번 기록됩니다. 초기~최종 각 단계에서 앱별 로그를 검사했으며 최종 파일은 Android Logcat 순환 버퍼에 남은 구간입니다.

기존 Editor Compiler/일반 Runtime Warning/Error0/MissingScript0/전체 회귀 PASS 및 Development Build Error0/기존 TMP IL2CPP Warning3 결과는 유지됩니다.

## 원본 데이터 복원

이번 연결에서 새로 백업한 원본은 Planet01/SCORE0/Combo0/Energy330, BEST19000입니다. 이전 Sprint10의 오래된 SCORE19000 Run 백업을 재사용하지 않았습니다.

- Prefs 전체를 원본으로 정확 복원한 후 실행
- 앱 실행 후 모든 기존 Prefs 항목 동일
- Run 파일 원본 바이트 완전히 동일
- 앱 실행에 따른 정상 새 알림 키 CosmicBlock.Planet01CompletionNotice=0만 추가
- 임시 해상도 복원, 임시 Android QA 파일 제거
- 테스트용 별빛/해금/Run 상태는 남지 않음
- 현재 HOME 실행 상태

백업/JSON/PNG/Logcat/픽셀 검사 결과: Validation/sprint101_device_20261009_140652/ (Git 제외).

Touch Feel/Haptic Feel/SFX Feel/Clear Timing/Gold Visibility/Display Readability의 체감 평가는 사용자 확인 대기입니다. 자동 드래그의 성공을 감각적인 PASS로 간주하지 않습니다.
