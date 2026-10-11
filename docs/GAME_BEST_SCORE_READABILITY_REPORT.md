# GAME 최고점수 패널 여백 개선

2026-10-11. 사용자 요청에 따라 최고점수 15,930 / 99,999 / 999,999를 비교했습니다.

## 변경

- 런타임 기준 패널 크기 412.5×137.5 → 462×154: 가로·세로 모두 12% 확대, 비율 3:1 유지.
- 오른쪽 Safe Area 여백 12q, 상단 여백 2q. q는 기존 반응형 스케일입니다.
- 숫자 영역 anchor (0.40,0.17)~(0.90,0.59) → (0.43,0.20)~(0.88,0.55).
- 숫자 TMP margin (1,1,1,1)q → (9,4,9,4)q. 왕관·프레임 및 PNG에 포함된 최고점수 제목으로부터 여백 확보.
- 최대 Font Size 53.65는 유지. 실제 Font Size는 내부 여백에 맞춰 자동 조절합니다. 1080×1920에서 15,930·99,999=41.05, 999,999=39.9.
- 확대된 왕관이 중앙 점수에 접근하지 않도록 중앙 점수 숫자의 Y만 24q 아래로 이동. 중앙 SCORE 라벨, 숫자 크기/색상/가로 위치·보드·행성 HUD·슬롯·기능 버튼은 유지.
- PNG 원본 및 Import 설정 변경 없음. SCORE/BEST 계산·저장·이어하기 및 게임 규칙 변경 없음.

## 변경 파일

- Assets/Scripts/UI/GameVisualPresentation.cs: 위 배치와 숫자 여백만 수정.
- Assets/Editor/BestScoreReadabilityProbe.cs: 현재 표시 글리프를 기준으로 숫자 경계/중앙 SCORE 및 라벨과의 겹침/잘림/비율/Safe Area 검증, 전후 렌더 생성.
- CURRENT_TASK/NEXT_TASK/DEVLOG와 docs의 대응 문서.

## 검증

- 1080×1920 / 1080×2400 + Safe Area / 1080×1440.
- 각 해상도에서 15,930 / 99,999 / 999,999, 중앙 현재 점수도 같은 값으로 표시하여 비교.
- 변경 전후 18개 렌더, 최종 자동 판정: 54 assertions PASS. Validation/best_panel_results.txt에 기록. Compiler Warning/Error 및 테스트 Runtime Warning/Error: 0.
- 기존 RunSavePlayProbe PASS, MissingScriptDiagnostics PASS(0).
- 테스트는 별도 Run 파일과 완료 안내 격리 경로를 사용합니다. 사용자 저장 데이터를 삭제하지 않습니다.
- 최종 렌더는 PNG 원본을 유지한 Unity Camera 출력입니다. 숫자가 길어질 때만 추가 축소됩니다. 실기기 가독성은 미검증입니다.

스크린샷: Validation/best_panel_comparison.png 및 best_panel_after_해상도_점수.png.
Android 빌드/설치·실기기 테스트 및 Git Commit/Push는 수행하지 않았습니다. 이전 메시 팝업 작업과 사용자 PNG 미커밋 변경은 보존했습니다.
