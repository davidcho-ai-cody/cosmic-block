# Sprint 10.2.2 — HOME/GAME BGM 및 오디오 설정

## 구현

- HOME/행성 도감: cosmic_block_home_bgm.mp3. GAME/행성 전환/복원 완료/Game Over: cosmic_block_main_bgm.mp3.
- GameFlowController.Screen을 읽는 독립 BackgroundMusicController 한 개와 2D AudioSource 두 개. 기존 화면 흐름과 게임 코드를 변경하지 않습니다.
- 약 1.5초 선형 크로스페이드. 빠른 왕복은 현재 혼합값에서 방향만 바꾸며, 재생 중인 소스에 Play를 다시 호출하지 않습니다. 페이드 완료 시 이전 소스를 정지합니다. 같은 BGM 화면 간 이동은 재시작하지 않습니다.
- 기본 BGM ON/볼륨 35%. OFF는 두 소스 볼륨을 즉시 0으로 변경합니다. 기존 효과음 소스와 AudioListener 전체 볼륨은 건드리지 않습니다.
- 백그라운드 pause/focus 상태를 함께 추적해 Pause/UnPause로 복귀합니다. 중단 중에는 페이드를 진행하지 않습니다.
- 별도의 DontDestroyOnLoad 매니저나 새 AudioListener를 추가하지 않습니다. 현재 Game Scene의 항상 활성인 BackgroundMusic에서 관리합니다. 중복 컨트롤러가 생성되면 비활성화합니다.

## 설정과 저장

기존 HOME 설정 Modal에 배경음악 ON/OFF, 음악 볼륨 Slider, 효과음 ON/OFF를 연결했습니다. 기존 진동 표시와 닫기 동작은 유지합니다. 기존 비동작 효과음 텍스트는 비활성화하고 새 버튼으로 대체했습니다.

PlayerPrefs:
- CosmicBlock.Audio.BgmEnabled (int, 기본 1)
- CosmicBlock.Audio.BgmVolume (float, 기본 0.35)
- CosmicBlock.Audio.SfxEnabled (int, 기본 1)

변경 즉시 저장하며 Awake에서 복원합니다. SCORE/BEST/Run/행성 저장 키는 사용하지 않습니다. SFX 기존 볼륨 0.58/피치/재생 정책 유지. SFX 전용 볼륨 Slider는 추가하지 않았습니다.

## Import

두 실제 MP3 모두 AudioClip 연결, Loop, Streaming, Vorbis 품질 0.7, 원래 샘플레이트 유지, Preload 비활성화. 두 소스에 같은 Clip을 중복 로드하지 않으며 Streaming으로 긴 음악 전체 PCM 메모리 로딩을 피합니다. MP3 자체 편집 없음. Android 메모리/CPU는 실기기 미측정입니다.

## 수정 파일

- Assets/Scenes/Game.unity: BGM 객체/설정 컨트롤 연결
- Assets/Scripts/Effects/BackgroundMusicController.cs (+meta)
- Assets/Scripts/UI/AudioSettingsView.cs (+meta)
- Assets/Audio/BGM/ 두 MP3 및 Import meta, BGM.meta
- Assets/Editor/Sprint1022AudioSetup.cs / Sprint1022AudioProbe.cs / Sprint1022SettingsVisualProbe.cs (+meta)
- CURRENT_TASK.md / NEXT_TASK.md / DEVLOG.md 및 docs 사본

## 검증

오디오 Playmode: HOME 최초 35%, 도감 음악 시간 연속, 실제 페이드 중간 두 소스/총 볼륨, GAME 완료 후 이전 소스 정지, 같은 GAME 재시작 없음, 빠른 왕복, HOME 복귀, BGM OFF 실제 SFX 재생, 즉시 볼륨, SFX 독립, 저장 설정 Reload, pause/focus 순서 및 복귀 PASS.

기존 회귀 및 설정 화면 Render: 최종 결과는 아래 추가 기록.

실기기 설치/테스트, APK 빌드, Git Commit/Push 미실행. Android 실제 음량/음질/백그라운드 복귀/메모리와 앱 재실행은 사용자 요청 후 검증합니다. 기존 AssetPackManager 오류와 TMP Build Warning은 이 작업에서 해결하지 않았습니다. Editor 재생 상태 검증은 주관적인 음악 믹스 품질 PASS를 의미하지 않습니다.

API 확인: https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/audiosource

## 최종 Editor 결과

Sprint1022AudioProbe / Sprint1021PlayProbe / Sprint102PlayProbe / RunSavePlayProbe / ClearScorePlayProbe / Sprint92PlayProbe / Sprint71HomePlayProbe / MissingScriptDiagnostics: 모두 PASS. 설정 화면 1080×1920 / 1080×2400 Safe Area / 1080×1440 렌더: 텍스트 잘림·Slider Safe Area 검사 및 캡처 육안 확인 PASS. Compiler Warning/Error 0, 일반 Runtime Warning/Error 0, Missing Script 0. Sprint1021의 의도적 저장 실패 Warning 1은 별도 예상 결과입니다. 초기 개발 중 컴파일/테스트 실패를 수정한 후 최종 재실행 결과입니다.

검증 증거: Validation/sprint1022_summary.txt, sprint1022_audio_tests.txt, sprint1022-settings-visual.log, sprint1022_settings_*.png. 최종 diff에서 Editor 자동 생성된 기존 UI Font/Background 크기 변경을 제거해 설정 UI와 새 BGM 객체만 유지했습니다. 원래 게임 로직 파일 수정 없음.

## 2026-10-09 추가 요청 — Android 실기기 / Git 반영

사용자 요청으로 Development APK 빌드·설치·실기기 검증 완료. HOME/도감/GAME 크로스페이드, OFF/볼륨/SFX 설정 실제 재실행 복원, 빠른 왕복, 백그라운드, 완료/행성 전환/GameOver/Retry 음악 유지 PASS. 원본 PlayerPrefs/Run 재실행 동일 복원 완료. Build Error0/기존 TMP Warning3, Crash·게임 예외0/Unity Warning0. 기존 AssetPackManager 오류10회는 남음. 음질·체감은 사용자 확인 대기. 상세 docs/SPRINT1022_DEVICE_QA.md. 이전 실기기/Commit 미실행 문구는 최초 요청 시점의 이력입니다. 두 BGM 음원은 이번 적용 요청에 따라 함께 Git 반영 대상이며 원본 파일 자체 변경 없음.
