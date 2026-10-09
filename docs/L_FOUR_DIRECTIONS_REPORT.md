# L자 블록 4방향 지원

이번 작업은 기존 POLISH-001(CLEAR 표시)과 별개인 L자 형태 확장입니다.

## 좌표 및 저장 호환성

좌표는 좌상단 원점, X 오른쪽/Y 아래입니다.

| 방향 | Shape ID | 저장 인덱스 | 셀 좌표 |
|---|---|---:|---|
| A 기존 | l_small | 6 | (0,0), (0,1), (1,1) |
| B 기존 | reverse_l_small | 7 | (1,0), (0,1), (1,1) |
| C 추가 | l_small_top | 8 | (0,0), (1,0), (0,1) |
| D 추가 | reverse_l_small_top | 9 | (0,0), (1,0), (1,1) |

네 방향은 서로 다른 3칸 형태입니다. 기존 8종의 ID/좌표/인덱스는 유지하고 신규 2종을 끝에 추가했습니다. RunSnapshot은 Shape를 목록 인덱스로 저장하므로 기존 파일은 그대로 읽습니다. 신규 인덱스 8/9를 포함한 파일을 변경 전 구버전 앱이 읽는 하위 버전 호환은 보장하지 않습니다.

## 공급

BlockGenerator의 기존 균등 random.Next(Shapes.Count)를 그대로 사용합니다. 총 8종→10종이며 각 형태 확률은 1/8→1/10입니다. 기존 종류 제거, 별도 가중치, 색상/입력/미리보기 변경은 없습니다. 힌트·새로고침·배치 가능성 판정은 동일한 Catalog를 참조합니다.

## 변경 파일

- Assets/Scripts/Blocks/BlockCatalog.cs: C/D 끝에 추가
- Assets/Editor/LFourDirectionsPlayProbe.cs 및 meta: 전용 Editor 회귀 검사
- 현재/다음 작업 문서, DEVLOG 및 이 보고서

## 검증

전용 검사에서 좌표/3칸/2x2/기존 인덱스, 고정 Seed 10,000회 랜덤 공급, 실제 새로고침 공급을 확인했습니다. 각 L 방향별 실제 드래그/Gold Highlight/유효·무효 프리뷰/배치, 1줄 제거(+30 배치 +100 Clear), 정확한 L 빈 공간의 힌트 및 Game Over 판정, 자동 저장→HOME→이어하기의 Shape/점수/소비 상태를 검증했습니다.

기존 Sprint102PlayProbe(힌트/새로고침 제한 및 흐름), RunSavePlayProbe(저장/이어하기), Polish001PlayProbe(점수/별빛 피드백), MissingScriptDiagnostics를 실행합니다. 최종 결과는 아래 기록합니다. 테스트 Run은 COSMIC_RUN_SAVE_TEST_PATH로 격리하고 기존 PlayerPrefs는 검사 도구에서 백업·복원합니다. 사용자 저장 파일 삭제, Android 빌드/실기기 테스트, Commit/Push는 하지 않습니다.

## 최종 결과

LFourDirectionsPlayProbe / Sprint102PlayProbe / RunSavePlayProbe / Polish001PlayProbe / MissingScriptDiagnostics: 전부 exit 0, PASS. Compiler Warning/Error 0, 테스트 중 예상하지 않은 Runtime Warning/Error 0, Missing Script 0. Android 빌드/실기기 테스트는 미실행. Commit/Push 없이 사용자 확인 대기입니다.

## 2026-10-10 추가 승인 — Android 및 Git 검증 완료

사용자가 Android 빌드·실기기 테스트·Commit/Push를 별도로 승인했습니다. 위 미실행 기록은 최초 Editor 작업 시점의 이력입니다.

- Android Development APK 빌드 성공. Build Error 0, 기존 TextMeshPro 대형 메서드 C++ 분리 경고 3.
- SM-S942N / Android 16 / 1080×2340 / ADB device. adb install-r 성공, 설치 전후 PlayerPrefs/Run 동일.
- 실제 Android 터치 swipe로 A/B/C/D 각각 검사: 올바른 형태 표시, 점유 위치 무효 드롭 시 변경 없음, (2,2) 유효 드롭의 정확한 3칸 배치·SCORE +30, 앱 프로세스 종료 후 이어하기의 Shape/색상/보드/소비 상태 유지 모두 PASS.
- 테스트 Run 및 PlayerPrefs는 사전 백업 후 원복했으며 원복 직후와 앱 재실행 후에도 원본과 동일함을 확인했습니다. 최종 앱은 원본 데이터의 HOME에서 실행 중입니다.
- Logcat: FATAL EXCEPTION 0, AndroidJavaException 0, NullReferenceException 0, MissingReferenceException 0, Unity Warning 0. 기존 com.google.android.play.core.assetpacks.AssetPackManager ClassNotFoundException은 10회 남았습니다. 따라서 Android 전체 Error 0으로 보고하지 않습니다. 이전 Sprint1022 실기기 보고에도 동일 오류가 기록되어 있으며 이번 L 형태 작업에서는 Android 의존성을 변경하지 않습니다.
- 실기기 랜덤 출현 확률·힌트·새로고침·라인 제거·Game Over를 모두 별도로 반복한 것은 아닙니다. 해당 기능은 앞서 Editor 자동 테스트로 검증했습니다. 실제 손으로 느끼는 Touch/Haptic/SFX 체감은 사용자 확인 대상입니다.
- 증거: Validation/l_four_device_20261010_012036/result.json 및 각 방향 before/invalid_return/placed/resumed.png, logcat.txt.
- 의도하지 않은 빌드 자동 ProjectSettings/URP 변경은 원복. 사용자 승인대로 일반 Commit/Push 반영.
