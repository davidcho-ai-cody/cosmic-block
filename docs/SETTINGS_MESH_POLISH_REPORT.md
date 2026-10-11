# HOME 설정 창 메시 디자인 통일

2026-10-11. 실기기에서 승인된 종료/HOME 이동 팝업 디자인을 설정 창에 적용했습니다.

## 기존 구조 분석

- HomeRoot/SettingsPanel/Card, HomeViewController의 ShowSettings/CloseSettings로 열고 닫습니다.
- AudioControls의 AudioSettingsView가 배경음악 ON/OFF, 볼륨 Slider, 효과음 ON/OFF를 BackgroundMusicController에 연결합니다.
- 음악 설정 PlayerPrefs 키/저장·복원 및 SFX 볼륨 정책은 기존 그대로입니다.
- 진동 ON은 조작 버튼이 없는 기존 고정 안내 문구입니다. 이번 디자인 작업에서 진동 기능을 새로 추가하지 않았습니다.

## 디자인

- HomeQuitMeshGraphic의 네이비 패널, 얇은 골드 테두리, 외곽 빛/하이라이트/그림자를 재사용.
- 배경음악·효과음은 블루 메시 버튼, 닫기는 골드 메시 버튼.
- 기존 CosmicPopupMedium/SemiBold 폰트를 그대로 공유하며 설정 문구 glyph만 확장. 원본 Noto Sans KR 및 PNG는 유지.
- PopupTypography의 제목26 / 본문18 / 버튼21 기준과 패널 폭 비례 스케일을 유지합니다. 본문 역할 목록으로 볼륨·진동 안내를 지정했습니다. 기존 두 확인 팝업에는 영향이 없습니다.
- 설정 패널은 폭의 90% 높이로 충분한 행 간격을 확보하고 Safe Area 중앙에 배치합니다.
- Slider는 넓은 투명 입력 영역 안에 얇은 Cyan Fill과 원형 Gold 손잡이를 구성합니다. FillArea/HandleArea를 분리하여 Unity Slider가 세로 앵커를 구동해도 막대·손잡이가 늘어나지 않습니다.
- 전체 Canvas Dim으로 뒤 HOME 버튼 입력을 차단합니다.

## 연결 변경

AudioSettingsView에 TMP_Text용 Configure와 Caption 연결을 추가했습니다. 기존 Text용 Configure는 호환을 위해 유지합니다. Toggle/볼륨 계산/리스너 등록·해제/PlayerPrefs 경로는 변경하지 않습니다.

## 변경 파일

- Assets/Scenes/Game.unity: 설정 창 하위만 변경
- Assets/Scripts/UI/SettingsPopupLayout.cs
- Assets/Scripts/UI/AudioSettingsView.cs
- Assets/Scripts/UI/PopupTypography.cs
- Assets/Editor/SettingsPolishBuilder.cs
- Assets/Editor/Sprint1022SettingsVisualProbe.cs
- Assets/Editor/Sprint1022AudioProbe.cs
- Assets/Fonts/CosmicPopupMedium.ttf/.asset 및 CosmicPopupSemiBold.ttf/.asset: 설정 glyph 추가
- CURRENT_TASK/NEXT_TASK/DEVLOG와 docs 대응 문서

## 검증

- 1080×1920 / 1080×2400 + Safe Area / 1080×1440.
- 실제 설정 버튼으로 열기, 닫기·10회 반복, 전체 Dim/뒤 입력 차단, 한국어 glyph/줄 잘림/폰트·크기 공통 규격.
- 실제 Slider fillRect 0/50/100%, 얇은 트랙 및 손잡이 원형 비율.
- BGM OFF에서도 SFX 정상, SFX OFF 독립, 음악 볼륨 즉시 반영, 설정 저장/복원, 백그라운드 복귀·빠른 음악 전환.
- 기존 GAME → HOME 281항목 PASS, HOME 종료80항목 PASS, RunSavePlayProbe PASS, Missing Script 0.
- 최종 설정 검증 96항목 PASS, 오디오 회귀 PASS. Compiler Warning/Error 0, Editor Runtime Warning/Error 0, Missing Script 0. 결과: Validation/settings_final_summary.txt 및 settings_mesh_tests.txt.
- 첫 렌더에서 과하게 늘어난 슬라이더를 발견해 전용 영역 구조로 수정하고 실제 비율 검사를 추가했습니다.
- 사용자 저장 데이터는 기존 테스트 격리 경로·설정 백업/복원으로 보호합니다.

화면: Validation/settings_mesh_1920.png / settings_mesh_2400.png / settings_mesh_1440.png.
APK 빌드/설치·실기기 테스트 및 Git Commit/Push는 이번 요청에서 수행하지 않았습니다. 설치된 APK는 기존 버전이며 설정 개선은 아직 포함되지 않습니다.
