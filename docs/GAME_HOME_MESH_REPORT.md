# GAME → HOME 확인 팝업 메시 적용

2026-10-11. 사용자 승인한 ‘통일된 코스믹’ 시안을 적용했습니다.

## 구현

- GameCanvas/SafeArea/ExitConfirm/Card의 패널과 Continue/GoHome 버튼을 HomeQuitMeshGraphic으로 구성합니다. 텍스처 없는 둥근 네이비 패널, 얇은 골드 테두리, 시안색 집 아이콘을 사용합니다.
- 제목: 홈으로 돌아갈까요?
- 안내: 진행 중인 게임은 저장돼요. / 홈에서 이어서 플레이할 수 있어요.
- TMP 한국어 전용 정적 폰트 GameHomeKorean을 추가했습니다. 제목과 버튼은 Bold, 설명은 밝은 블루 화이트입니다.
- 기존 버튼 오브젝트·이벤트 및 GameFlowController/게임 저장 로직은 유지합니다. 계속하기는 팝업만 닫고, 홈으로는 기존 ConfirmHome → ShowHome → LeaveForHome 저장 경로를 호출합니다.
- 기존 ThemeFill 장식 오브젝트만 제거했습니다. PNG 원본 파일은 전부 보존합니다.
- GameHomePopupLayout은 Safe Area 폭의 82% 이내, 높이의 60% 이내로 패널을 제한하며 전체 Canvas를 덮는 Dim을 유지합니다.

## 수정 및 추가 파일

- Assets/Scenes/Game.unity
- Assets/Scripts/UI/GameHomePopupLayout.cs
- Assets/Scripts/UI/GameHomeIconGraphic.cs
- Assets/Editor/GameHomeDesignBuilder.cs
- Assets/Editor/GameHomeMeshProbe.cs
- Assets/Fonts/GameHomeKorean.asset
- Assets/Editor/RunSavePlayProbe.cs: 새 TMP 제목에 맞춰 문구 assertion만 갱신
- CURRENT_TASK/NEXT_TASK/DEVLOG 및 docs의 대응 문서

## 검증

- 새 팝업 Editor 검증: 227 assertions PASS. 실제 HOME 버튼 열기, 배경 입력 차단, 계속하기, 반복 열기/닫기, 중복 확인 무시, HOME 저장 후 이어하기의 SCORE·Combo·보드64칸·공급 블록·소비 상태 일치.
- 1080×1920 / 1080×2400 + Safe Area / 1080×1440: 메시 정점/삼각형, PNG Image 미사용, 한글 글리프/텍스트 overflow, 버튼 동일 크기/간격, Safe Area, 전체 Dim 검사 PASS. 렌더 이미지 직접 확인.
- 최초 터치 차단 검증 실패는 렌더 깊이 -1인 프레임에서 Raycast를 검사한 테스트 조건 때문이었습니다. 두 프레임의 Canvas 렌더 완료 후 실제 Canvas 카메라 좌표로 검사해 통과했습니다.
- RunSavePlayProbe: PASS.
- 기존 HOME 종료 팝업: 53 assertions PASS. Missing Script 검사: 0, PASS. 모든 검증 프로세스 exit=0. Compiler Warning/Error 및 테스트 Runtime Warning/Error: 0. 실행 로그: Validation/game_home_summary.txt.
- 팝업 밖 씬 블록 변경 없음. 기존 HOME 종료 팝업의 미커밋 수정도 보존.

## 검증 범위

Android 빌드/설치/실기기 테스트 및 Git Commit/Push는 수행하지 않습니다. 실기기의 글자 크기·터치 느낌은 사용자 확인 대기입니다. 사용자 저장 데이터 삭제 없이 테스트 전용 저장 경로와 PlayerPrefs 복원 방식으로 검사했습니다.

스크린샷: Validation/game_home_mesh_1920.png / game_home_mesh_2400.png / game_home_mesh_1440.png.
