# Sprint 10 — Five Planets Integration (2026-10-09)

## 구현
기존 PlanetDefinition / PlanetRestoration / PlanetArtCatalog / UI 및 단일 Run 구조를 확장했다. 새 복원 시스템을 중복 구현하지 않았다. 행성03~05 ContentReady 활성화, 영문 metadata 추가, 카탈로그5개 entry/25 Sprite 연결.

| 행성 | 목표 별빛 | 각 단계 구간 필요량 | 단계 시작 누적값 | 완료 |
|---|---:|---|---|---:|
| 01 푸른 별 | 1,500 | 150/250/350/450/300 | 0/150/400/750/1200 | 1500 |
| 02 크리스탈리아 CRYSTALIA | 3,000 | 300/500/700/900/600 | 0/300/800/1500/2400 | 3000 |
| 03 이그니스 IGNIS | 6,000 | 600/1000/1400/1800/1200 | 0/600/1600/3000/4800 | 6000 |
| 04 글라시아 GLACIA | 12,000 | 1200/2000/2800/3600/2400 | 0/1200/3200/6000/9600 | 12000 |
| 05 루미나 LUMINA | 24,000 | 2400/4000/5600/7200/4800 | 0/2400/6400/12000/19200 | 24000 |

합계46,500.단계 이름 황폐/싹틈/깨어남/회복/완성. 각 시작 누적값 이상에서 해당 단계 이미지 표시. 마지막 구간에서는 Stage5 이미지를 사용하지만 IsRestored/복원 완료/✓는 최종 목표에서만 표시한다. 기존02의 Sprite4 유지 정책은 Sprint10의 5단계 통일 요구에 맞춰 변경하고 기존 회귀 기대값도 갱신했다. 단계 구간, 점수, 보상, 저장값은 변경하지 않았다.

01→02→03→04→05 순차 해금, 기존 해금 플래그 유지. 잠긴 행성은 공통 실루엣+기존 작은 Lock, 선택/플레이 불가. 05 완료 후 모든 행성 선택과 현재 Run 계속 가능. 새 엔딩 연출 없음.

HOME: 행성01 기존 home_planet_hero 장식 아트 보존. 02~05는 동일 카탈로그의 현재 단계 Sprite 표시. GAME HUD / Collection / 중앙 전환도 공통 카탈로그 참조. 기존 Fragment Flight, Fade/Scale 타이밍·연출 구조는 유지했다. HUD 외곽 맞춤은 기존02의 catalog Bound를03~05에도 적용한다.

## 자산 사전검사
25개 모두 실제 RGBA Alpha 및 외곽 투명 픽셀 존재. 밝은/어두운 검사 배경으로 시각 확인했고 체크무늬/흰색·회색 사각 배경은 보이지 않았다. 원본 PNG 변경 없음. 누락된 실제 자산 없음.

Planet01은 지시문의 lowercase 규칙과 다른 기존 Planet01_StageNN_* 파일이다. 이름/GUID를 바꾸거나 복제하지 않고 기존 StagePaths alias를 보존했다. 02~05는 지시문 파일명과 일치한다.

| 파일 | 크기 | Alpha | 완전 투명 픽셀 수 |
|---|---|---|---:|
| `Assets/Art/Planets/Planet01/Planet01_Stage01_Desolate.png` | 1254×1254 | RGBA / PASS | 693,383 |
| `Assets/Art/Planets/Planet01/Planet01_Stage02_Awakening.png` | 1254×1254 | RGBA / PASS | 643,164 |
| `Assets/Art/Planets/Planet01/Planet01_Stage03_Recovering.png` | 1254×1254 | RGBA / PASS | 539,870 |
| `Assets/Art/Planets/Planet01/Planet01_Stage04_Thriving.png` | 1254×1254 | RGBA / PASS | 591,364 |
| `Assets/Art/Planets/Planet01/Planet01_Stage05_Restored.png` | 1254×1254 | RGBA / PASS | 539,049 |
| `Assets/Art/Planets/Planet02/planet02_stage01_barren.png` | 1254×1254 | RGBA / PASS | 672,537 |
| `Assets/Art/Planets/Planet02/planet02_stage02_sprout.png` | 1254×1254 | RGBA / PASS | 812,085 |
| `Assets/Art/Planets/Planet02/planet02_stage03_awakening.png` | 1254×1254 | RGBA / PASS | 773,889 |
| `Assets/Art/Planets/Planet02/planet02_stage04_restoration.png` | 1254×1254 | RGBA / PASS | 697,163 |
| `Assets/Art/Planets/Planet02/planet02_stage05_complete.png` | 1254×1254 | RGBA / PASS | 624,293 |
| `Assets/Art/Planets/Planet03/planet03_stage01_barren.png` | 1254×1254 | RGBA / PASS | 655,101 |
| `Assets/Art/Planets/Planet03/planet03_stage02_sprout.png` | 1254×1254 | RGBA / PASS | 675,292 |
| `Assets/Art/Planets/Planet03/planet03_stage03_awakening.png` | 1254×1254 | RGBA / PASS | 515,208 |
| `Assets/Art/Planets/Planet03/planet03_stage04_restoration.png` | 1254×1254 | RGBA / PASS | 512,042 |
| `Assets/Art/Planets/Planet03/planet03_stage05_complete.png` | 1254×1254 | RGBA / PASS | 480,310 |
| `Assets/Art/Planets/Planet04/planet04_stage01_barren.png` | 1254×1254 | RGBA / PASS | 575,841 |
| `Assets/Art/Planets/Planet04/planet04_stage02_sprout.png` | 1254×1254 | RGBA / PASS | 562,829 |
| `Assets/Art/Planets/Planet04/planet04_stage03_awakening.png` | 1254×1254 | RGBA / PASS | 529,338 |
| `Assets/Art/Planets/Planet04/planet04_stage04_restoration.png` | 1254×1254 | RGBA / PASS | 550,198 |
| `Assets/Art/Planets/Planet04/planet04_stage05_complete.png` | 1254×1254 | RGBA / PASS | 492,757 |
| `Assets/Art/Planets/Planet05/planet05_stage01_barren.png` | 1278×1230 | RGBA / PASS | 589,052 |
| `Assets/Art/Planets/Planet05/planet05_stage02_sprout.png` | 1254×1254 | RGBA / PASS | 609,845 |
| `Assets/Art/Planets/Planet05/planet05_stage03_awakening.png` | 1254×1254 | RGBA / PASS | 474,349 |
| `Assets/Art/Planets/Planet05/planet05_stage04_restoration.png` | 1254×1254 | RGBA / PASS | 507,315 |
| `Assets/Art/Planets/Planet05/planet05_stage05_complete.png` | 1254×1254 | RGBA / PASS | 394,044 |

공통 잠금 `Assets/Art/Planets/Common/planet_locked.png` 및 `Assets/Art/UI/Common/planet_lock_simple.png` 모두 연결. 25개 실제 Sprite 경로를 AssetDatabase로 검증했다.

Texture Type Sprite / Single / Alpha FromInput+Transparency / Bilinear / Clamp / Mipmap OFF / Readable OFF / 기본 Uncompressed / Android ASTC6×6 Quality100 Max2048. Android 투명도/메모리 균형을 고려했다. 원본1254 정사각 및 루미나01의1278×1230 비율은 Image.preserveAspect로 보존한다.

신규03~05의 Alpha>100 마스크 내부 구멍을 채운 최대 내접 원으로 본체 중심/상대 지름을 측정하고 .86 기준으로 UI scale/offset을 저장했다. 01/02 기존 보정값은 보존했다. 고리/파편을 PNG에서 자르지 않는다. 모든 화면이 같은 Art entry를 참조하며 HOME/HUD/Collection의 기존 외곽 fit 구조를 사용한다.

ASTC base-level25장 예상 용량 16.66 MiB (GPU 실제 측정은 NOT RUN). 자산 검사 JSON/시각 sheet: Validation/sprint10_asset_preflight.json, sprint10_asset_contact.jpg.

## 저장 호환
PlayerPrefs Key / Migration Version2 / RunSnapshot Version1 / 단일 current-run.json은 유지. 행성02의 기존 목표는 이미3000이며 이전2000 별빛 절대량2000 보존 및 기존03 해금 플래그 보존을 테스트했다. 새 마이그레이션/리셋/DeleteAll 없음. 글로벌 BEST 유지. 복원 시 절대 누적 별빛 회복만 수행하며 별빛 보상을 다시 호출하지 않는다.

자동화 테스트는 process env COSMIC_RUN_SAVE_TEST_PATH와 별도 Validation Run 파일로 사용자 저장 파일을 격리한다. 신규 테스트는 선택/마이그레이션/BEST/행성01~05 에너지와 해금 키의 존재 여부 및 값을 백업·복원한다. 실기기 사용자 데이터는 접근/수정하지 않았다.

## 검증
- Sprint10PlayProbe: PASS. 독립적으로 명시한5종 목표/구간/경계±1/완료직전/완료/legacy 절대량/기존 해금 검사. 실제25번 production 단계 전환(각행성4경계+완료), overflow, Fragment 지연, input lock, 중복완료 없음, 영구해금/무한플레이.
- 행성01~05 각각 HOME→이어하기: Run 행성/SCORE/BEST/Combo/64 Cell 색상/3 Piece Shape·팔레트·소비상태/별빛/현재Sprite 동일 PASS. 선택행성이 바뀌어도 기존Run 유지. GameOver/Retry PASS.
- ClearScorePlayProbe:79 assertions PASS (현행100/150… 정책 유지), actual Score/TMP/JSON/BEST 비교.
- RunSavePlayProbe:563 assertions PASS, 기존A-L 및 예약 공급·중단된Clear·손상데이터·마이그레이션 flush 손실 복구 유지. 기존 unprepared03 거부 케이스만 유효범위 밖06 거부로 변경;03~05 저장은 신규 테스트가 검증.
- Sprint2 / Sprint92 / Sprint94 / Sprint66 / Sprint5NewRunGameOver / Sprint8Collection: PASS. 이번에 모든 역사적 Sprint Probe를 재실행했다고 주장하지 않는다.
- Compiler Warning/Error0; 신규 PlayMode Warning/Error0. MissingScript0.
- Responsive 최종 검증/Android Build 결과는 아래 최종 확인에 기록한다.

증거: Validation/sprint10_tests.txt, clear_score_tests.txt, run_save_tests.txt, sprint10_regression_summary.txt와 각sprint10_*.log. 테스트 assertion 합계에는 프레임별 timeout 조건 확인도 포함하므로 의미 있는 전환 검증은25회로 별도 보고한다.

## 변경 파일
- Assets/Scripts/Core/PlanetDefinition.cs:5행성 준비 상태/영문metadata/Stage5 이미지 정책.
- Assets/Scripts/UI/PlanetRestorationView.cs:기존HUD outer Bound를03~05에도 적용.
- Assets/Resources/PlanetArtCatalog.asset:03~05 각5 Sprite 및 중심/크기 보정.
- Assets/Art/Planets/Planet01,03,04,05/*.png.meta:Import 설정 통일 (02는 이미 동일).
- Assets/Editor/Sprint10Builder.cs (+meta):자산 존재 검사/Importer/공통 카탈로그 빌더. Scene/HOME 디자인 수정 없음.
- Assets/Editor/Sprint10PlayProbe.cs (+meta), Sprint10VisualProbe.cs (+meta):신규5행성 기능/화면 검증.
- Assets/Editor/RunSavePlayProbe.cs, Sprint94PlayProbe.cs:새로 승인된 콘텐츠 및5단계 표시 기대값 갱신.
- README / PRODUCT_VISION / CURRENT_TASK / NEXT_TASK / DEVLOG 및docs 대응 문서, 본 보고서.

## 실기기 및 Git
SM-S942N은 사용자가 현재 연결할 수 없다고 응답. Device Install/Launch/Logcat/주관적 Touch·SFX·Haptic: NOT RUN. Android에서 기존 CLEAR 누락 제보도 미확인 상태이며 관련 점수 정책을 이번 작업에서 변경하지 않았다.

기준 HEAD d0aa465. 기존3개 로컬 커밋은 보존. Sprint10 커밋/원격Push는 수행하지 않는다. Remote 설정/Force Push 변경 없음. 다음 단계는 최신APK update install 후 실제5행성 전환/투명도/저장/Portrait Safe Area 검증.

## 최종 확인
- Responsive:1080×1920 /1080×2400(좌우2%, 상4%·하6%의 자동 Safe Area fixture) /1080×1440에서63개 최종 HOME/GAME/Collection 렌더 PASS. 모든5행성 Stage3 화면, Collection Stage5 미완료, Locked03 화면을 포함한다. Collection 텍스트 잘림/선택 버튼/Navigation Aspect/실제Bar width, GAME Board/Slot/HUD Safe Area 검사 PASS. 배경의 Screen 크기와 RenderTexture 크기 차이를 검사 도구에서 보정했으며 게임 Background 코드는 변경하지 않았다.
- Android Development Build:Succeeded, Build Error0, Build Warning3. 기존 Unity.TextMeshPro의 TMP_TextParsingUtilities static constructor 및 TextMeshPro/TextMeshProUGUI GenerateTextMesh를 큰 메서드용 별도 C++ 파일로 분리했다는 IL2CPP 알림3개다. 경고 억제/패키지 수정/경고 숫자를0으로 보이게 하는 재빌드는 하지 않았다. C# Compiler Warning/Error0. PlayMode Warning/Error0.
- APK:151,955,814 bytes, ZIP CRC PASS. SHA256 `d03865433d74f8300ef7fa00417be6321d9dd527a8086f6ceeacdbb85830a0b3`. 출력 `D:/Projects/CosmicBlock/Builds/Android/Development/CosmicBlock-dev.apk`. Package com.DefaultCompany.CosmicBlock / 기존Portrait 및 Safe Area 유지.
- Device install/launch/Logcat:NOT RUN(사용자 연결 불가). 실제 GPU 메모리·Touch/SFX/Haptic/화면 가독성은 사용자 실기기 확인 대기.
- Scene 파일 수정 없음, Missing Script0, 원본25 PNG 수정 없음.
- 새 Commit/Push 없음. 기준HEAD d0aa465 및 기존 ahead3 보존.

## 후속 Android 실기기 검증 완료
2026-10-09 SM-S942N 연결 후 기존 최신APK update install/launch 성공. 실제25경계/SCORE1110/별빛overflow/완료해금/HOME resume/강제종료/Locked navigation/GameOver/Retry PASS. 원래SCORE19000 Run과 전체Prefs 정확복원. Crash/Java/Null/MissingReference0, UnityWarning0. 기존AssetPackClassNotFound31회/다른UnityError0. 앞의NOT RUN 기록은 연결 전 상태다. 상세 [Device QA](SPRINT10_DEVICE_QA.md).
