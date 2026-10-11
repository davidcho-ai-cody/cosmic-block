# 행성 도감 코스믹 메시 디자인 적용

## 변경
승인된 시안에 맞춰 이름 장식 프레임을 제거하고, 복원도/5단계/메시지 패널을 반투명 네이비 메시·얇은 시안 테두리·약한 빛으로 표현했습니다. Back/Previous/Next와 행성 선택 버튼은 메시, 화살표와 잠금 아이콘도 텍스처 없는 메시입니다. 현재 단계 링을 썸네일 앞에 배치했습니다.
기존 Collection 로고와 배경, 실제 25개 Planet Stage Sprite 및 잠긴 행성 Sprite는 그대로 사용합니다. 원본 PNG는 수정·삭제하지 않았습니다.
공통 CosmicPopupMedium/SemiBold TMP 폰트에 기존 도감·행성 이름·설명에 필요한 글리프를 추가했습니다. HOME/기존 팝업과 같은 폰트 계열입니다.

## 기능 보호
PlanetCollectionView 및 행성/점수/별빛/저장 로직은 수정하지 않았습니다. 기존 Button Event와 legacy Text 데이터 연결을 보존하고, CollectionMeshPresentation이 기존 텍스트 값을 TMP로 표시합니다. 기존 Text 컴포넌트는 비활성(오브젝트는 활성) 상태로 두어 행성 선택 버튼의 기존 조회 경로도 유지합니다.
게이지는 기존 Fill Rect의 실제 너비를 메시 자식이 표현합니다. 계산을 새로 만들지 않고 동일한 전체 복원도 값을 사용합니다.
잠긴 단계/행성, 단계 미리보기, 현재 단계 강조, 경계 버튼 비활성, 행성 선택/이어하기 기능을 유지합니다. 시안에 없던 하단 선택 버튼도 기존 기능 보호를 위해 메시 디자인으로 유지합니다.

## 파일
- Assets/Scenes/Game.unity (CollectionRoot 범위만 추가 변경)
- Assets/Scripts/UI/CollectionMeshPresentation.cs (추가)
- Assets/Scripts/UI/CollectionLockMeshGraphic.cs (추가)
- Assets/Editor/CollectionMeshBuilder.cs (추가)
- Assets/Editor/CollectionMeshProbe.cs (추가)
- Assets/Fonts/CosmicPopupMedium.ttf/.asset 및 CosmicPopupSemiBold.ttf/.asset
- Assets/Editor/HomeMenuMeshBuilder.cs: 재실행 시 도감 글리프도 유지하도록 목록 연결
- 작업 문서
앞서 미커밋 상태인 HOME 메시 작업을 보존했습니다. 도감 외 Scene 변경은 자동 직렬화 차이만 복원했습니다.

## 검증
도감 전용 1,370항목 PASS. 5개 행성 각각 0/25/50/75/99/100% 실제 Fill Rect와 메시, 기존 Sprite, 단계 잠금/미리보기, 저장 불변 확인.
1080×1920 / 1080×2400 + Safe Area / 1080×1440에서 잠긴 행성, 5개 완료 행성, 푸른 별 75% 화면 확인. SafeArea, TMP glyph/잘림, 패널 간격, 실제 Raycast, Back/선택/Run 유지 확인.
Sprint10PlayProbe PASS: 5개 행성의 정의·25 Sprite·단계 전환·overflow·완료·해금·이어하기·Retry.
Sprint1021 Run 전환·선택·저장 실패/중단 복구 회귀 PASS, HOME 363항목 및 설정 96항목 PASS, Missing Script 0. 최종 중앙 정렬 반영 후 도감 1,370항목 재검증 PASS. 신규 Compiler Warning/Error 0, 정상 실행 Runtime Warning/Error 0. 저장 실패 주입의 예상 Warning은 해당 회귀에서 별도로 처리합니다.
완료 팝업 검사의 최초 실패는 Legacy 테스트 자동 닫기 설정 때문이었습니다. 해당 검사에서 자동 닫기를 해제한 재검증은 PASS이며 게임 코드 변경은 없습니다.
결과: Validation/collection-mesh-summary.txt 및 collection_mesh_tests.txt.
대표 화면: Validation/collection_mesh_planet1_75_1920.png, collection_mesh_planet1_75_2400.png, collection_mesh_planet1_75_1440.png, collection_mesh_locked2_1440.png.

APK 빌드·설치·실기기 테스트·Commit/Push는 별도 요청 대기입니다. 현재 설치 APK에는 HOME/도감 메시 변경이 포함되지 않습니다.

## 2026-10-11 후속 Commit/Push 및 Android 빌드
- UI 구현 커밋: 5ab1bdf806e3dabe2daa074147fde55380910619 (feat: unify home collection and game mesh UI).
- origin/main 일반 Push 성공 후 Development 빌드 수행. Remote/Force Push 변경 없음.
- Unity BuildResult Succeeded, Build Error 0, Build Warning 3.
- Warning은 기존 TMP_TextParsingUtilities.cctor / TextMeshPro.GenerateTextMesh / TextMeshProUGUI.GenerateTextMesh의 대형 메서드 C++ 파일 분리 안내.
- APK: Builds/Android/Development/CosmicBlock-dev.apk, 152,238,580 bytes.
- SHA-256: C797D6C802CCA2307A23197E31E15BCA1C54AE8DD8EF15CE6E92093AE2F56F6E.
- 빌드 로그: Validation/mesh-ui-android-build.log.
- Portrait/ARM64/IL2CPP 및 기존 Package Name 유지.
- 실기기 설치·테스트 미진행. APK와 디버그 심볼은 Git 제외.
