# POLISH-002 — GAME 하단 기능 버튼 위치 개선

## 구현

기존 GameAssistActions는 Safe Area 하단 좌표 0에 고정되어 있었습니다. 보드/슬롯 배치 완료 후 BlockArea의 갱신된 RectTransform 하단을 기준으로 그룹을 배치하도록 변경했습니다. 같은 그룹에 있는 무료 횟수 표시도 함께 이동합니다.

슬롯 그룹과 버튼 Hit Rect 간격은 12×현재 화면 스케일 q, 그룹 하단 여백은 최소 8×q로 제한합니다. q는 기존 Safe Area 너비/높이 스케일을 사용합니다. 기존 ReservedHeight/보드 budget/슬롯 치수는 바꾸지 않아 보드·슬롯이 밀리지 않습니다. 짧은 화면에서는 기존 스케일에 맞춰 간격도 작아집니다.

좌우 버튼 크기·정렬·Sprite Aspect·원본 PNG 및 카운트 Font/문구는 유지합니다. 새로고침/힌트 Event와 게임·저장·오디오 로직은 변경하지 않습니다.

## 수정 전후 비교

아래 값은 Canvas 단위이며, 슬롯 Touch Rect 하단에서 버튼 Hit Rect 상단까지 측정했습니다. PNG의 내부 투명 여백은 별도로 유지됩니다.

| 해상도 | 이전 슬롯-버튼 간격 | 최종 간격 | 버튼 그룹 하단 여백 | 보드/슬롯/버튼 크기 |
|---|---:|---:|---:|---|
| 1080×1920 | 20.00 | 12.00 | 8.00 | 유지 |
| 1080×2400 | 111.57 | 10.30 | 101.27 | 유지 |
| 1080×1440 | 17.32 | 10.39 | 6.93 | 유지 |

Tall에서 과도한 간격을 줄였고, 이미 촘촘한 9:16/Short에서는 작은 이동만 적용했습니다. 3개 해상도의 준비/힌트/새로고침 사용/무료 소진 상태를 각각 전후 캡처하여 총 24개 PNG를 확인했습니다. 랜덤 공급 Piece 모양은 두 실행에서 다를 수 있으며 레이아웃 치수로 보존 여부를 별도 비교했습니다.

비교 파일:
- Validation/polish002_before_ready_1920.png / polish002_after_ready_1920.png
- Validation/polish002_before_ready_2400.png / polish002_after_ready_2400.png
- Validation/polish002_before_ready_1440.png / polish002_after_ready_1440.png
- Validation/polish002_geometry.csv

## 검사

새 레이아웃 PASS: 실제 슬롯 Hit Rect와 버튼 비겹침, 보드/슬롯/버튼 순서, 두 버튼 동일 크기/높이, 카운트 함께 이동/잘림 없음, Safe Area 내부, 최소 하단 여백, Sprite Aspect 유지. 수정 전후 보드 크기/슬롯 높이/버튼 크기 수치 동일 확인.

초기 자식 슬롯 Rect 기준 구현은 해상도 변경 직후 LayoutGroup 갱신보다 먼저 측정하여 겹침 검사에 실패했습니다. 슬롯 그룹의 갱신된 Rect를 직접 사용하도록 수정한 뒤 12개 최종 상태 모두 PASS했습니다. 새로고침/힌트 기능 로직으로 우회하지 않았습니다.

기존 기능 회귀 최종 결과는 아래 추가 기록에 정리합니다. Run 파일/PlayerPrefs는 기존 격리 도구로 보호·복원합니다.

## 변경 파일

- Assets/Scripts/UI/GameAssistView.cs: 슬롯 그룹 기준 버튼/카운트 위치
- Assets/Scripts/UI/GameVisualPresentation.cs: 슬롯 배치 후 버튼 Layout 호출 순서
- Assets/Editor/Polish002LayoutProbe.cs (+meta): 전후 렌더/위치/터치 경계 검증
- CURRENT_TASK / NEXT_TASK / DEVLOG 및 docs 사본, 본 보고서

Scene 및 Art 원본의 의도적인 변경 없음. Android 빌드/설치/실기기 테스트와 Git Commit/Push는 하지 않습니다. 현재 설치 APK에는 POLISH-001/002 표시 개선이 없습니다.

## 최종 결과

Polish002LayoutProbe / Sprint102PlayProbe / RunSavePlayProbe / Sprint1022AudioProbe / Polish001PlayProbe / MissingScriptDiagnostics: 모두 PASS. Compiler Warning/Error 0, Runtime Error 0, Missing Script 0. Scene/Art/Core/Audio 변경 없음. Editor 자동 ProjectAuditor 설정 변경은 원복했습니다. Git HEAD ed7bf84 유지, 이번 변경은 미커밋 상태입니다.

## 2026-10-10 추가 요청 — APK 설치 및 Git 반영

사용자가 실기기 설치와 Commit/Push를 승인했습니다. POLISH-001/002가 포함된 Development APK를 빌드해 SM-S942N에 install-r로 업데이트합니다. 실기기 게임 테스트는 사용자가 진행하므로 앱 실행/게임 조작/Logcat 기능 검사는 하지 않습니다. 기존 데이터는 삭제하지 않습니다. 최종 빌드/설치 결과는 아래 완료 기록에 추가합니다.

## 2026-10-10 완료 기록

- POLISH-001/002를 포함한 최신 Android Development APK 빌드 성공. Build Error 0, Build Warning 3. 경고는 기존 TextMeshPro의 대형 메서드를 별도 C++ 파일로 분리한다는 IL2CPP 알림 3건이며 이번 UI 변경의 컴파일 오류가 아닙니다.
- APK: Builds/Android/Development/CosmicBlock-dev.apk, 152,138,933 bytes. SHA-256: b009d04f53c7fe38c5d3df58636bee060848c875b4484af3a163d43691239ad1.
- Samsung SM-S942N / Android 16 / ADB device. adb install -r 성공.
- 설치 전후 PlayerPrefs와 current-run.json이 바이트 단위로 동일함을 확인. 저장 데이터 삭제 없음.
- 사용자 요청대로 앱 실행, 게임 조작, Logcat 및 실기기 기능 테스트는 하지 않았습니다. 가독성/버튼 접근성은 사용자 확인 대기입니다.
- Editor 회귀 및 3해상도 검증 PASS. Compiler Warning/Error 0, Editor Runtime Error 0, Missing Script 0. Android Runtime 검증은 수행하지 않았습니다.
- 빌드가 자동 변경한 프로젝트 설정은 원복했습니다. 사용자 승인에 따라 일반 Commit/Push를 진행합니다.
