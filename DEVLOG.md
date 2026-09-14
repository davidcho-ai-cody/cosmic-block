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
