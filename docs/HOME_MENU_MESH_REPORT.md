# HOME 메시 메뉴 디자인 적용

## 적용 내용
사용자가 승인한 배경과 어울리는 코스믹 시안을 기준으로 HOME 메뉴만 변경했습니다. 배경/로고/행성 Sprite와 위치, 모션은 보존합니다.
- 게임 시작/이어하기: 반투명 네이비 메시, 골드 테두리·은은한 외곽 빛, 선형 Play 아이콘, TMP 동적 문구.
- 행성 도감: 네이비 메시·시안 테두리, 메시 Orbit/Chevron 아이콘.
- 최고점수: 낮은 강조의 반투명 메시 패널, 선형 Trophy와 동적 숫자. 큰 수는 AutoSize로 보존합니다.
- 설정/게임 종료: 작은 블루 네이비 메시 버튼, 선형 Settings/Power 아이콘.
- 새 게임 시작: 기존 확인 흐름을 그대로 유지하며 작은 메시 받침으로 밝은 배경에서도 읽히도록 처리합니다.
폰트는 기존 승인된 CosmicPopupMedium/SemiBold를 공유하고 필요한 HOME 한국어 글리프만 추가했습니다. 원본 NotoSansKR와 PNG를 수정하거나 삭제하지 않았습니다.

## 기존 기능 연결
기존 Button과 Event를 보존했습니다. HomeRunControls는 동적 TMP 문구 연결 및 새로운 배치가 있을 때 기존 PNG용 배치를 건너뛰는 최소 변경만 합니다. HomeFinalLayout는 메시 레이아웃에 위임합니다.
기존 PNG Image 참조는 호환을 위해 유지하되 표시 Alpha 0, 장식 아이콘/기존 점수 Text는 비활성 처리했습니다. 기존 Best 데이터 연결은 유지하고 TMP가 동일 값을 표시합니다. 원본 PNG 정리는 추후 별도 작업입니다.
HomeMenuMeshLayout은 실제 버튼 Rect를 사용합니다. 1080×1440에서는 사용 가능한 높이에 맞춰 높이·간격·글자 크기를 함께 축소합니다. DEV는 기존 Debug Build 전용 정책과 위치를 유지합니다.

## 변경 파일
- Assets/Scenes/Game.unity (HOME 메뉴 범위만)
- Assets/Scripts/UI/HomeMenuMeshLayout.cs (추가)
- Assets/Scripts/UI/HomeMenuIconGraphic.cs (추가)
- Assets/Scripts/UI/HomeFinalLayout.cs
- Assets/Scripts/UI/HomeRunControls.cs
- Assets/Editor/HomeMenuMeshBuilder.cs (추가)
- Assets/Editor/HomeMenuMeshProbe.cs (추가)
- Assets/Fonts/CosmicPopupMedium.ttf/.asset, CosmicPopupSemiBold.ttf/.asset
- 작업 문서

## 검증
HOME 전용 363항목 PASS: 게임 시작/이어하기/새 게임 확인·취소/도감·복귀/설정/종료 취소/실제 Raycast/저장·점수·콤보·Best·행성 보존.
1080×1920 / 1080×2400 + Safe Area / 1080×1440, 저장 없음과 이어하기 화면 각각 확인했습니다. CTA 폭 동일, 메뉴/DEV 겹침 없음, 글리프·텍스트 잘림 없음. BEST 0/19,000/999,999/2,147,483,647 확인.
RunSavePlayProbe 563항목 PASS, SettingsVisual 96항목 PASS, Audio 회귀 PASS, HOME 종료/Game→HOME 팝업 회귀 PASS, ClearScore 106항목 PASS, Missing Script 0.
최종 작은 메시 받침 반영 후 HOME 전용 363항목 재검증 PASS. 신규 Compiler Warning/Error 0, Editor Runtime Warning/Error 0. 실제 세 해상도 렌더를 다시 검토했습니다.
Scene 블록 비교로 배경/TitleArea/HeroArea 및 기존 확인·설정 팝업이 보존됐음을 확인했습니다. Unity 자동 직렬화 차이는 복원했습니다.
결과: Validation/home-menu-summary.txt 및 home_menu_mesh_tests.txt.
스크린샷: Validation/home_menu_mesh_resume_1920.png / _2400.png / _1440.png (fresh도 별도 저장).

APK 빌드/설치·실기기 테스트·Commit/Push는 이번 디자인 적용 요청에서 진행하지 않습니다. 현재 설치된 APK에는 이번 HOME 개선이 포함되지 않습니다.
