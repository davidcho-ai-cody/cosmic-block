# Sprint 10.3 — HOME 종료 팝업 새 프레임 적용

## 변경 범위

기존 HOME 종료 팝업만 수정했습니다. 사용자 교체본 Blue/Gold PNG와 신규 popup_exit_frame.png를 그대로 사용했습니다. 이전 popup_frame_cosmic.png는 보존하지만 종료 팝업에서는 더 이상 참조하지 않습니다.

## 자산

- Assets/Art/UI/Popup/popup_exit_frame.png: 1774×887, 2:1
- Assets/Art/UI/Popup/popup_button_blue.png: 1983×793
- Assets/Art/UI/Popup/popup_button_gold.png: 1983×793

모두 RGBA, Alpha 0~255. Unity Sprite Single, Alpha FromInput/IsTransparency ON, Mipmap OFF, Bilinear, Uncompressed, MaxSize 2048. 원본 PNG 편집/크롭/배경 제거 없음. 사용자 기존 미커밋 PNG 교체본을 덮어쓰거나 원복하지 않았습니다.

## 레이아웃 및 연결

기존 QuitConfirmPanel/Card/Cancel/Confirm 참조를 유지했습니다. HomeQuitPopupLayout은 CosmicFrame Sprite의 실제 비율로 크기를 계산하고 Safe Area 중앙에 배치합니다. 가로 폭은 Safe Area의 90% 이하, 세로 크기는 72% 이하입니다. 검정 Dim Alpha .72는 Canvas 전체를 덮습니다.

제목·설명은 기존 한국어 TMP Font Asset, 밝은 골드 Bold 제목과 밝은 본문을 유지합니다. Blue 취소/Gold 종료 버튼은 동일 Rect 크기이며 Sprite 비율을 유지합니다. 종료 TMP 글자색만 어두운 남색으로 변경했습니다. 추가 Glow/Outline/Shadow는 적용하지 않았습니다. 기존 클릭 피드백과 터치 Rect를 유지합니다.

ShowQuit/CancelQuit/ConfirmQuit 및 GameFlowController.RequestQuit는 이번 변경에서 수정하지 않았습니다. 이전 중복 종료 가드도 유지됩니다. 활성 Run은 HOME 이동과 GameSession.OnApplicationQuit의 기존 저장 경로로 보존됩니다. 종료는 Run 초기화가 아닙니다. 안내 문구는 진행 중인 Run이 있을 때의 이어하기 동작을 설명하며, Run이 없는 HOME에서 종료했다고 새 Run을 생성하지는 않습니다. 저장 정책은 변경하지 않았습니다.

## 검증 결과

Sprint103QuitProbe 최종 PASS: 47 assertions, Warning 0. PNG 적용 이름/비율 유지/종료 글자색, 실제 HOME 종료 버튼, 취소·기존 종료 요청·반복 호출, EventSystem Raycast 배경 입력 차단, SCORE/Run/BEST/행성/BGM 설정 보존 및 종료 이후 이어하기를 확인했습니다.

1080×1920 / 1080×2400 + Safe Area / 1080×1440 모두 PASS: 프레임 2:1 비율, 전체 화면 Dim, Safe Area 경계, 한국어 글리프/텍스트 잘림 없음, 버튼 동일 크기/비겹침. 렌더 3장을 저장했고 실제 육안 확인으로 상단 장식·본문·버튼 겹침 없음 및 좌우 균형을 확인했습니다.

RunSavePlayProbe 회귀 PASS. MissingScriptDiagnostics PASS, Missing Script 0. Compiler Error/Warning 0, 이번 Editor 테스트 Runtime Error/Warning 0. Scene YAML 비교에서 종료 팝업 밖의 의미 있는 변경 0. 자동 생성된 무관한 ProjectAuditor/Scene 직렬화 변경은 원복했습니다. 사용자 Run은 별도 테스트 파일로 격리하며 검사에서 변경한 PlayerPrefs는 백업·복원합니다.

BGM 재생 시스템·다른 팝업·게임 점수·별빛·진행·자동 저장/이어하기·광고 코드 변경 없음. 이번 작업에서 Android 빌드·설치·실기기 검사는 하지 않았으며 Android 전체 기능을 PASS로 판정하지 않았습니다.

## 수정/추가 파일

- Assets/Scenes/Game.unity: 종료 팝업 Sprite/버튼 Rect/글자색
- Assets/Scripts/UI/HomeQuitPopupLayout.cs: 실제 Sprite 비율 사용
- Assets/Editor/HomeExitDesignBuilder.cs 및 meta: 종료 팝업 전용 Editor 적용 도구
- Assets/Editor/Sprint103QuitProbe.cs: 새 디자인 검증 추가
- popup_exit_frame.png.meta: 신규 Import 설정 (원본은 사용자 제공)
- 사용자 교체된 popup_button_blue.png / popup_button_gold.png는 미커밋 상태로 보존
- CURRENT_TASK/NEXT_TASK 및 docs 사본, DEVLOG, 본 보고서

## 남은 확인 및 Git

Editor 검증에서 남은 문제 없음. 실제 폰의 글자 가독성·버튼 터치·취소/앱 종료·재실행 이어하기는 사용자 요청 후 실기기 검증 대상입니다. Commit/Push 없음. HEAD는 ed43c32이며 변경은 미커밋 상태입니다.

스크린샷: Validation/sprint103_quit_1920.png / sprint103_quit_2400.png / sprint103_quit_1440.png. 검사 로그: Validation/exit_design_summary.txt 및 exit-design-*.log.
