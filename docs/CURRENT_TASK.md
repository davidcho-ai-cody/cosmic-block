# Sprint 9.4 — 크리스탈리아 연동

기준 main 7c3b77d, Unity6000.5.8f1. Planet02 실제 플레이/도감 선택/영구 진행도를 통합했습니다. 기존 게임 규칙과 Sprint9.3 Burst/Flight/0.32초 순차 Clear/중앙 Stage Transition 타이밍은 유지합니다.

## 정의와 이미지
푸른 별1500 / 크리스탈리아3000 / 이그니스6000 / 글라시아12000 / 루미나24000. PlanetDefinition이 총량, 구간, 이름, 선행 해금, 준비 상태, Sprite 경로를 관리합니다. PlanetArtCatalog(Resources asset)이 실제 Sprite 참조와 본체 크기·중심 보정을 관리합니다. 03~05는 ContentReady=false로 선택 불가입니다.
Planet02 PNG5개와 공통 silhouette/silver lock2개 모두1254×1254 RGBA, alpha0~255, 외부 투명 픽셀 확인. 체크무늬/흰회색 배경 픽셀 미발견. 원본 PNG 수정/생성 없음. SpriteSingle/AlphaFromInput/AlphaTransparency/Bilinear/Clamp/NoMip/AndroidASTC6x6.
구간 필요량300/500/700/900/600, 시작0/300/800/1500/2400. 논리 Stage5의2400~2999는 ‘최종 복원’과 Sprite4; Sprite5는3000에서만 표시합니다. 기존 Planet01의1200 Stage5 아트 관례는 유지하며 완료는1500입니다.

## 선택·저장
01완료→02영구 해금. 도감 선택→HOME→PLAY, 플레이 중 전환 불가. 잠금/미준비 선택 거부, 잘못된 ID 기본01. HOME02에는 이름/총 별빛/복원도와 동적 Hero 표시, 기존 HOME01 디자인 유지. 현재 Run은 완료 후에도 계속합니다.
CosmicBlock.SelectedPlanet 신규 키. PlanetNNEnergy/PlanetNNUnlocked, BestScore/PlanetRestorationVersion2 기존 키 유지.02의 예전2000 저장값은 절대량2000으로 보존하고3000 기준 재계산.9.3에는 별도02완료 플래그가 없었으며 영구 해금 플래그는 보존합니다.500→400 legacy migration 유지, 비정상값 상한/음수 clamp. 전체 삭제 없음.
보상10/25/45/70, 현재 플레이 행성만 적립. 즉시 저장 후 표시만 Fragment 도착까지 지연. 최대량 이후0, 마지막5만 지급 가능하면 실제 표시도5. 초과 다음 행성 이월 없음. 전환/완료/해금 피드백 중복 없음.

## 변경 파일
Core: PlanetDefinition(new), PlanetRestoration, GameSession. UI: PlanetArtCatalog(new), PlanetCollectionView/Data, GameFlowController, HomeViewController/Atmosphere base scale(HomeAmbientMotion), PlanetRestorationView, GameVisualPresentation, PlanetDebugPanel. Scene Game.unity 및 Resources/PlanetArtCatalog.asset. Editor Sprint94Builder/PlayProbe/VisualProbe(new), 기존 Sprint8CollectionPlayProbe/Sprint93PlayProbe 기대값을 새 정의로 갱신.
Board/Blocks/ScoreRules/SequentialClearPlan/GameFeedbackController 변경 없음. DEV presets는 선택 행성 경계−10으로 공통 계산하며 Release 비노출 유지.

## 자동 검증
전체29 Probe entrypoint 최종 PASS(Sprint0~9.3 +94), 실패 후 수정한 초기 실행 기록은 로그에 보존. 최종94Play172개 assertion PASS. P1기존 경계/P2열두 경계, 실제 Sprite/fill/표시,1490+10해금/Run유지,02독립 적립/전환5개,3000완료1회,부분5지급,교차+25/3줄+45,빠른HOME callback,GameOver/Retry,globalBest/reload/선택fallback 검증.
마지막 관련8 Probe 전부 PASS. 후속 HOME 보정 후94Visual/761HOME 재검증 PASS.39 렌더(각3해상도×13상태):1080×1920 /1080×2400 SafeArea /1080×1440. 잠금02/미준비03,02각단계/2999/3000,HOME02/GAME02 완료·최종구간. 본체 크기/중심 보정, Hero Frame 간격,Text 잘림,선택버튼SafeArea,실제Bar폭,Nav비율 검증. SCORE/BEST24 RGB-mask 조합 overlap0. MissingScript0, 신규 컴파일 Warning/Error0.
근거: Validation/sprint94_tests.txt, sprint94_visual_tests.txt, sprint94_final_regression_summary.txt, sprint94_release_summary.txt, sprint94_missing.log, sprint94_final_*.log 및 렌더PNG. QA 산출물/APK는git제외.

Android/Git 최종 결과는 아래 완료 기록에 추가합니다. 주관적 Touch/SFX/Haptic/화면가독성은 사용자 확인 대기입니다.

## 최종 완료 검증
실기기 GameOver QA에서 발견한 행성01 고정 문구를 GameHud의 선택 행성 정의로 교체했습니다. 기존 검사는 GameOver 상태/패널만 확인했으므로 표시명 assertion을 추가했습니다. HOME fallback/legacy stage 표기도 같은 정의를 사용합니다. DEV preset의 오래된90/190/290/390 표시도 선택 행성의 실제 경계−10으로 동기화: P1 0/140/390/740/1490, P2 0/290/790/1490/2990. 판정/점수/연출/입력 규칙 변경 없음.
최종94Play172 assertion PASS; 관련65/66/5NewRunGameOver/761Home 모두 PASS. GameOver를 추가한39개 반응형 렌더 PASS(각3해상도×13상태), 긴 크리스탈리아 결과 Text 잘림 없음. MissingScript0. 최종 cached Development build Warning0/Error0. 첫 IL2CPP 빌드의 기존 TMP3 notice와 첫 Editor shader import의 URP414 warning은 원 로그에 보존; 변경 없는 재검증에서 Compiler/Shader Warning/Error0. SDK/패키지 경고를 숨기거나 수정하지 않았습니다.

Android: SM-S942N / Android16 /1080×2340, adb device 정상. 기존 앱 위 install-r 성공/실행 성공. 업데이트 직전/직후 사용자 진행도240/Best15930/Version2 유지.
임시 QA fixture를 사용하여 잠금02→P1 1490+실제1줄=1500→02해금→도감 선택→HOME→PLAY를 확인했습니다. P2 0→10,290→300,790→800,1490→1500,2390→2400,2990→3000을 실제 Touch/Line Clear로 확인.2400에서4번 이미지/최종복원,3000에서5번 최종고리 이미지. 01 진행도와 글로벌BEST 불변, 완료 이후 Run 유지. 단계 핵심 QA 이후 패널 문구만 보정한 최종 APK에서41회배치/6줄/Score1710,3000 clamp/완료연출 반복없음/자연GameOver/수정된02표기/Retry/Home→PLAY/재실행을 재검증했습니다. Retry/Home→PLAY의 실제64셀 empty 및Piece3개는 화면 픽셀과 별도 Board 모델로 확인했습니다. 재실행 후 선택2/진행3000/Best15930 유지. 최종 DEV UI에서도290/790/1490/2990 확인.
QA 전에 PlayerPrefs XML 전체를 백업했고, 종료 후 모든 원본 preference를 비교·복원했습니다. 현재 기기는 원래 사용자 상태240/15930/Version2, 기본 선택01의 HOME에서 실행 중입니다. 임시 해금/행성02·03/선택 키는 원복했습니다.
Logcat: FATAL EXCEPTION0, AndroidJavaException0, NullReferenceException0, MissingReferenceException0, 신규 게임 코드 오류0. 기존 AssetPackManager ClassNotFound E Unity와 DexFile finalizer AssertionError 환경 로그는 남아 있음(각4회 시작 로그); 전체 Android Logcat Error0으로 주장하지 않습니다. Crash 없음/프로세스 실행 확인.
실행하지 않은 항목: Release APK 자체 설치, 주관적 Touch/SFX/Haptic/휴대폰 가독성 평가. 기존 Release DEV guard의 false 조건은 자동 회귀 검사로 확인했습니다. 역사적인921 before/after 측정 도구는59px 과거 baseline용이며 현재93의80px Burst 회귀는93/92 검증을 사용합니다.

추가된 Planet03 PNG5장은 사용자 원본 자산으로만 보관하며 아직 alpha/art/content 검증·Sprite 연결·플레이 활성화하지 않았습니다.03~05 ContentReady=false. 후속 구현 때 별도 자산 검사와 단계 검증을 수행해야 합니다.
APK SHA256: ECD0A7417B4E8126F2515A40263B3C25D378AF0A9B71617CC22316397E8B9556.
Evidence: Validation/sprint94_context_* 및 context_visual_clean.log, sprint94_tests.txt/visual_tests.txt, device_stage2/stage3/stage4/complete 결과JSON·PNG, final_gameover_context/retry/home_play/relaunch2/original_home PNG, device_final_logcat.txt, device_final_result.json. QA 로그/이미지/APK는Git 제외.
Git commit message: feat: integrate crystal planet and collection progression. 기존main/origin으로 일반 push하며 force push/remote 변경 없음. 실제 hash와 동기화 결과는 완료 보고 및 Validation의 Git 기록에 남깁니다.

## 사용한 행성02 파일
- planet02_stage01_barren.png: 0~299
- planet02_stage02_sprout.png: 300~799
- planet02_stage03_awakening.png: 800~1499
- planet02_stage04_restoration.png: 1500~2999(2400부터 최종 복원 구간)
- planet02_stage05_complete.png: 3000 완료에서만
공통: Assets/Art/Planets/Common/planet_locked.png, Assets/Art/UI/Common/planet_lock_simple.png. 모든 원본 PNG 변경 없음.

작업 후반 추가된 Assets/Art/Planets/Planet04/ PNG5장은 사용자 원본을 그대로 보존하며 이번 Sprint9.4 커밋에서 제외합니다. 이 untracked 폴더 때문에 전체 working tree는 clean이 아니지만 본 Sprint의 구현 변경은 모두 커밋합니다.

