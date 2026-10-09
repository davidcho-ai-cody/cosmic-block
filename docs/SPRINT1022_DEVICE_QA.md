# Sprint 10.2.2 Android 실기기 QA — 2026-10-09

## 설치 환경

Samsung SM-S942N / Android 16 / ADB device / 1080×2340. 최신 Development APK 빌드 성공 및 install -r 업데이트/실행 완료. 패키지명과 기존 Remote 변경 없음.

Build Error 0 / 기존 TMP 큰 메서드 C++ 분리 Warning 3. APK CRC PASS, 크기 152128018 bytes.

SHA256: `22df3b31955adb7579f2c13d7111ba30f74d557ef97625655232c5882f564077`.

## 실제 검사 결과

- HOME 최초 35% 및 도감 HOME 음악 연속: PASS
- GAME 실제 1.5초 크로스페이드 및 백그라운드 복귀: PASS
- GAME에서 HOME BGM 복귀: PASS
- BGM OFF/독립 SFX ON 및 볼륨 즉시 저장: PASS
- 실제 앱 재실행 OFF/볼륨/SFX 설정 복원: PASS
- 빠른 HOME/GAME 왕복 후 단일 HOME 소스: PASS
- BGM OFF/SFX ON 실제 Line Clear 및 점수·별빛 정상: PASS
- 복원 완료 팝업/행성 전환 GAME BGM 연속: PASS
- 실제 GameOver/Retry GAME BGM 유지: PASS

Development 전용 BGM_QA 로그로 실제 AudioSource 재생 여부·볼륨·클립 재생 시간·혼합값·pause 상태를 확인했습니다. 정상 ON 크로스페이드 중 두 소스 볼륨 합은 설정 볼륨이며, 전환 완료 후 이전 소스가 정지합니다. HOME/도감 연속, 완료 팝업/행성 전환/실제 GameOver/Retry의 GAME 클립 시간 연속을 확인했습니다. OS HOME 키로 백그라운드 진입 후 같은 PID로 복귀해 정상 소스를 확인했습니다.

설정 UI를 실제 터치해 BGM OFF/볼륨/SFX OFF를 저장한 뒤 앱 프로세스를 종료·재실행하여 값 복원을 확인했습니다. BGM OFF는 두 BGM 소스 볼륨 0입니다. SFX ON 상태의 실제 Line Clear는 SCORE 230→340 / 별빛 37→47로 정상 처리됐습니다. 실제 SFX 청취 품질은 사용자 확인 대기이며, Editor에서는 OFF 상태 실제 SFX 소스 재생을 검증했습니다.

## 데이터 보호 및 테스트 보정

설치 후 검사 직전에 새 원본 백업을 다시 생성했습니다. 원본 Run은 행성03/SCORE0/별빛0입니다. 테스트 fixture 종료 후 PlayerPrefs 모든 키/값과 Run 바이트를 원복하고 앱 재실행 후 동일함을 검증했습니다. BEST/진행도/게임 설정 보존. 해상도 변경 없이 원래 1080×2340 유지. 앱은 HOME에서 실행 중이며 원래 오디오 키가 없으므로 기본 ON/35%입니다.

검사 중 USB가 한 차례 끊겨 사용자 재연결 후 우선 임시 오디오 설정을 원복하고 재개했습니다. 도감 Back 입력 좌표를 실제 캡처에 맞춰 보정했고, 오래된 음악 시간 로그와 Loop 래핑을 오판하지 않도록 새 실행에서 연속 시간을 검사했습니다. 이 과정에서 게임 코드 수정 없음.

## 로그 및 남은 항목

FATAL EXCEPTION / AndroidJavaException / NullReferenceException / MissingReferenceException: 각각 0.
Unity Warning: 0. 기존 AssetPackManager ClassNotFoundException 10회(재실행마다 반복), 그 외 Unity Error 0. 전체 Runtime Error 0은 아닙니다.

음질·체감 음량·크로스페이드 청감·BGM OFF 중 SFX 청취·Haptic은 사용자 확인 대기. Android 메모리/CPU 프로파일링과 Release APK 설치는 수행하지 않았습니다. 기존 AssetPackManager/TMP 이슈는 이 BGM 작업에서 수정하지 않았습니다.

## 변경 및 Editor 검증

두 BGM 및 Import meta, Game Scene, BackgroundMusicController, AudioSettingsView, AudioSetup/AudioProbe/SettingsVisualProbe, 작업 문서를 포함합니다. BGM_QA 진단 로그는 DEVELOPMENT_BUILD/UNITY_EDITOR에서만 컴파일되며 Release에는 포함되지 않습니다. 게임 로직 변경 없음.

Editor Audio / Sprint1021 / Sprint102 / RunSave / ClearScore / Sprint92 / Sprint71 / MissingScript 회귀 PASS. 설정 화면 1080×1920/2400 Safe Area/1440 렌더 PASS. 진단 로그 추가 후 Audio 재검사 PASS. Compiler Warning/Error 0, 일반 Runtime Warning/Error 0, Missing Script 0; 의도적 저장 실패 Warning은 예상 결과로 별도.

증거: Validation/sprint1022_device_20261009_214057/result.json, logcat.txt, PNG 및 Validation/sprint1022_android_build.log. Commit/Push 결과는 최종 완료 보고에 기록합니다.
