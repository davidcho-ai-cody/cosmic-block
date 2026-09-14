# Devlog
## 2026-09-14 — Sprint 0
- 사용자 제공 최종 지시문을 확인하고 Sprint 0 범위로 제한.
- 기존 프로젝트는 Unity 6000.5.8f1, URP 17.6.0, uGUI 2.5.0, Input System 1.20.0.
- 기존 URP 2D Renderer와 SampleScene 보존. Unity 버전 변경 없음.
- Android Build Support/SDK 36/NDK/OpenJDK 확인. Target 36, minSdk 26, Portrait 기준 적용.
- Git main 초기화 및 Unity ignore 구성.
- bool[8,8] 모델/상태 변경 event/셀 표시 분리. 좌상단 원점.
- Editor API로 Game Scene/Canvas/SafeArea/Grid/64 Cell/EventSystem/reference/build scene 생성.
- 블록 생성/드래그/게임 규칙/광고/아트/사운드/효과 없음.
- 작은 자체 현지화 구조를 선택하되 실제 구현과 한글 폰트는 후속 UI Sprint.
- 샌드박스 helper setup refresh 오류로 기본 파일 쓰기/실행 불가; 승인된 외부 실행으로 작업.

- 검증 완료: Unity 배치 컴파일 및 Game Scene 로드 PASS; 64 Cell, 모델 독립성/event/경계 검사 PASS.
- Play Mode PASS: GameSession 모델 연결, 보드/Cell 정사각형, 상태→Image 색상 갱신과 초기화. 검증 보드 723.3044×723.3044 Canvas 단위.
- 게임 코드 컴파일 오류/예외 없음. Unity 라이선스 연결 초기 오류 후 정상 기동; 로그에 종료 중 Curl 메시지가 남음. Android 빌드/실기기/노치 육안 QA는 미실행.
- 현재 단계에서 수동 Unity 구성 작업 없음. Game Scene은 이미 생성되고 Build Settings에 등록됨.
- 프로젝트 생성 파일과 기존 템플릿을 포함한 최초 커밋: project foundation: Sprint 0 board prototype and documentation.
- Git 저장소가 sandbox 계정 소유여서 외부 실행에서 safe.directory를 프로젝트 한정 -c 옵션으로 적용. 전역 Git 설정 변경 없음.

## 2026-09-14 — Sprint 1 / Block Placement
- 시작 working tree clean, 기준 commit d85d878. 사용자 Sprint 1 지시문 범위로 진행.
- 기존 BoardModel/BoardView/GameSession에 최소 추가. Scene YAML 직접 수정 없이 Unity API로 Game Scene 확장.
- 불변 Shape offset 8종과 작은 System.Random generator. 독립 선택/중복 허용, Inspector seed 지원.
- uGUI EventSystem의 Mouse/Touch callbacks, Canvas drag layer, finger offset 110, 실제 Grid 기반 nearest anchor mapping.
- preview와 drag visual snap 및 drop이 동일한 anchor 사용. 모델 CanPlace/TryPlace로 UI와 판정 분리.
- 점유 검사 실패의 원자성, 성공 시 해당 Slot만 소비. 3개 모두 사용 후 공급 없음.
- 포인터 id 고정/동시 drag lock, Escape/focus/pause 취소. 비활성화 중 SetParent 오류를 발견하여 session-hosted next-frame 복귀로 해결.
- 컴포넌트 RequireComponent 인자 오류 및 테스트의 CanvasScaler Update 호출 오류 수정. 설치된 uGUI source에서 Canvas.preWillRenderCanvases 갱신 방식 확인.
- spacing 8/12를 실제 Shape와 함께 렌더/육안 비교하여 12 선택. padding/전체 보드 비율 유지.
- 자동 domain: Single/H3/L/Reverse L, 범위/negative/overflow, 점유 충돌, 모든 Shape edge, full row 유지, deterministic seed pool PASS.
- Play Mode: 지시문 Test 1~8, UI raycast, preview 불변/visual anchor 일치, invalid return, cancellation/multi-pointer, deferred disable return, 슬롯 소비/자동 공급 없음 PASS.
- 렌더: 1080×1920 / 1080×2400(모사 SafeArea inset) / 1080×1440, 정사각형 보드/64 mapping/슬롯 bounds PASS.
- synthetic uGUI 이벤트 검증이며 실제 Mouse/Android touch 입력 전체 경로와 APK/AAB 빌드는 미실행. 실제 노치/UX QA는 README 절차로 안내.
- Unity 자동화 완료: 현재 단계에서 수동 Unity 작업 없음. 기술 기준/패키지/기존 SampleScene 보존, 의존성 추가 없음.
- 다음 후보: Line Clear → Score → Combo → New Block Set → Game Over Detection. 승인 전 진행하지 않는다.
- 최종 재검증: Scene upgrade 반복 실행 후 슬롯 중복 없음; domain/Play Mode 전체 PASS, 런타임 오류 없음. 긴/짧은 화면 비교 이미지도 육안 확인. 검증된 Sprint 1을 gameplay: add block drag and placement로 커밋.
