# 확인 팝업 글꼴·크기 통일 및 메시 입체감

2026-10-11 실기기 피드백에 따라 HOME 종료 / GAME → HOME 확인 팝업을 다듬었습니다.

## 원인

- 두 팝업이 별도 TMP 폰트 자산을 사용하고 자동 글자 크기 조절 조건도 달랐습니다.
- 가변 폰트에서 만든 기존 TMP와 직접 지정한 크기가 서로 달라 HOME 버튼은 작고 GAME 버튼은 크게 보였습니다.

## 공통 기준

- 기존 Noto Sans KR 가변 원본에서 실제 Medium(500) / SemiBold(600) 정적 굵기를 추출했습니다. 원본 폰트는 유지합니다.
- PopupTypography에서 본문=Medium, 제목/버튼=SemiBold를 통일합니다. 인위적인 TMP Bold는 사용하지 않습니다.
- 패널 폭 440 기준 제목26 / 본문18 / 버튼21. 실제 패널 폭 비례로 조절하며 Auto Size는 OFF입니다.
- 같은 해상도에서 두 팝업의 제목·본문·버튼 크기가 같습니다. 취소/종료/계속하기/홈으로는 문구 길이에 따라 크기가 바뀌지 않습니다.
- 정적 폰트는 두 팝업의 한국어 문구와 기본 ASCII를 포함한 136문자 subset입니다. 새 문구 추가 시 glyph 확장이 필요합니다. 기존 Noto Sans KR OFL 저작권·라이선스 메타데이터를 유지했고 파생 family 이름은 Cosmic Popup으로 지정했습니다.
- 이번 적용 대상은 두 메시 확인 팝업입니다. 다른 팝업의 동작/디자인은 변경하지 않았습니다. 공통 크기·굵기 기준은 후속 팝업에도 재사용할 수 있습니다.

## 메시 효과

- 네이비/블루/골드 색상과 심플한 형태 유지.
- 위쪽 밝은 하이라이트와 아래쪽 어두운 면으로 부드러운 입체감.
- 테두리 명암 변화, 낮은 Alpha 외곽 빛, 얕은 하단 그림자.
- 모든 효과는 VertexHelper의 정점 색상과 메시 링으로 생성합니다. PNG 및 새 재질/광원/발광 애니메이션을 추가하지 않습니다.
- Button 이벤트와 targetGraphic/터치 Rect는 유지합니다. 메시 정점 수는 면·경계·빛·그림자 포함 253입니다.

## 변경 파일

- Assets/Scripts/UI/PopupTypography.cs
- Assets/Scripts/UI/HomeQuitMeshGraphic.cs
- Assets/Scripts/UI/HomeQuitPopupLayout.cs / GameHomePopupLayout.cs
- Assets/Fonts/CosmicPopupMedium.ttf/.asset, CosmicPopupSemiBold.ttf/.asset 및 meta
- Assets/Editor/PopupPolishBuilder.cs
- Assets/Editor/GameHomeMeshProbe.cs / Sprint103QuitProbe.cs
- Assets/Scenes/Game.unity: 두 확인 팝업만 변경
- 작업 문서

## 검증

1080×1920 / 1080×2400 + Safe Area / 1080×1440에서 실제 메시·한국어 glyph·잘림·고정 크기·공통 폰트·버튼 동일 크기·터치 차단을 검사합니다.
검증 결과: GAME → HOME 281항목 PASS, HOME 종료80항목 PASS, RunSavePlayProbe PASS, Missing Script 0. Compiler/테스트 Runtime Warning 및 Error 0. 모든 프로세스 exit=0. Validation/popup_polish_summary.txt 및 각 Probe 결과 파일.
화면: Validation/game_home_mesh_1920.png / exit_mesh_1920.png 및 2400/1440 대응 파일.

사용자 저장 데이터는 전용 Run/notice 테스트 경로와 PlayerPrefs 백업·복원으로 보호합니다. Run/SCORE/BEST/행성/BGM/SFX 및 기존 종료·취소·저장·이어하기 로직 변경 없음.
APK 빌드·설치·실기기 테스트 및 Git Commit/Push는 이번 디자인 피드백 작업에서 수행하지 않습니다. 기존 설치 APK에는 이 미커밋 변경이 아직 포함되지 않습니다.
