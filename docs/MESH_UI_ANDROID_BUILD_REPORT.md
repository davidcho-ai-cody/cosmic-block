# 메시 팝업·최고점수 UI — Android 빌드 및 커밋 준비

2026-10-11 사용자 요청으로 현재 변경 전체를 커밋·origin/main 일반 Push하고 Android Development APK를 빌드합니다.

포함: HOME 종료 메시 팝업, GAME → HOME 메시 팝업, 최고점수 패널 12% 확대/여백 개선, 한국어 TMP 폰트, Editor 검증 코드, 문서 및 사용자 제공 팝업 PNG 교체본. PNG는 삭제하지 않습니다.

Editor 검증: HOME 종료 53항목 PASS, GAME → HOME 227항목 PASS, 최고점수 표시 54항목 PASS, RunSavePlayProbe PASS, Missing Script 0. 각 UI는 1080×1920 / 1080×2400 + Safe Area / 1080×1440 렌더 확인.

Android Development 빌드: Succeeded, errors=0, warnings=3, Unity exit=0.
경고 3건은 TMP_TextParsingUtilities.cctor / TextMeshPro.GenerateTextMesh / TextMeshProUGUI.GenerateTextMesh의 IL2CPP 대형 메서드 별도 CPP 파일 생성 안내입니다. 게임 코드 Compiler Warning/Error는 0입니다.

APK: Builds/Android/Development/CosmicBlock-dev.apk
APK 실제 파일 크기: 152,167,076 bytes (약 145.12 MiB)
SHA256: A3731AE133A83B43A77E68D58798A9A9210CB7D36E986E566F33569191F0D18A
빌드 로그: Validation/mesh-best_android_build.log
빌드 임시 파일 .utmp는 Validation/mesh-best-build-utmp에 보존했습니다. 원래 깨끗했던 자동 변경 URP/프로젝트 설정은 복원했습니다.

기기 설치와 실기기 테스트는 사용자 요청대로 수행하지 않습니다. 실기기 평가는 사용자가 설치 요청한 이후 직접 진행합니다. Remote 변경 및 Force Push는 하지 않습니다. APK와 Validation 산출물은 기존 .gitignore대로 Git에 포함하지 않습니다.
