# Design System
Dreamy Cosmic / Space. Deep Navy, Purple, Blue, Warm Gold. Cute, Cozy, Magical, Premium Casual.
Horror, Realistic Sci-Fi, War, 복잡한 UI, 과도한 Particle를 피한다. 퍼즐 가독성 우선.

## Placeholder
배경 #0E1129 계열, 빈 Cell 반투명 Blue/Navy, 점유 Cell Purple, 텍스트 Warm Gold.
Art는 최종 제작하지 않는다. Background Image와 Cell Image의 sprite/color를 나중에 교체한다.
Art에 UI Text를 넣지 않는다.

## Layout
Portrait 9:16 (1080×1920). 상단 제목/점수 자리, 중앙 8×8 정사각형, 하단 Block 자리.
SafeArea 기준 보드 크기 = min(너비−48, 높이×0.58). Grid 간격 12, 패딩 8 (Canvas 단위).
노치와 다른 비율에서 중앙 보드 및 주변 영역을 보존한다.

## Localization 결정
v1.0 규모에서는 key 기반 작은 자체 한국어/영어 테이블을 우선 채택한다. 일본어 열 추가 가능.
현재는 구현하지 않는다. 다음 UI Sprint에서 텍스트 키, 언어 선택/저장, fallback, 폰트 교체를 구현.
한글 글리프를 포함하는 라이선스 확인 폰트와 줄바꿈/문자 길이 검증이 필수.
현재 LegacyRuntime 폰트/영문 placeholder는 최종 현지화 폰트가 아니다.
공식 Localization Package는 현재 소규모 문자열에 비해 설치/테이블 관리 복잡도가 높아 보류한다.


## Sprint 1 — Block Placement
- Board spacing 12×12 Canvas 단위 (Sprint 0의 8에서 +4), padding 8 유지. Board 전체 크기/정사각형 규칙 보존.
- 실제 H3/Square/L 점유 및 Reverse L preview를 넣어 spacing 8/12 비교 렌더 확인. 12에서 Cell 구분이 더 명확하고 Shape 읽기/보드 크기를 유지한다.
- Piece는 Purple Image Cell. 슬롯에서는 max 64, 슬롯 너비/높이×0.24 중 최소값으로 균일 크기, spacing 6.
- 드래그에서는 Board cell size/spacing과 정확히 일치. Preview valid Warm Gold, invalid Soft Red. Glow/Tween/Final Art 없음.
- BlockArea: SafeArea anchor (0.04,0.025)~(0.96,0.19), slot 간격 24/패딩 12, 동일 폭 3개.
- DragLayer는 전체 Canvas를 덮고 포인터 위 offset 110 (GameSession에서 한 곳 조절).
- 9:16/긴 화면/짧은 화면 및 모사 SafeArea inset 검증 완료. 실제 Android 노치/터치 UX는 별도 확인.
