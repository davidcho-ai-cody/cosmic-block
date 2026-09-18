# Next Task
Android 실기기를 연결해 Sprint 4.5 수동 QA를 완료한다.

1. Android 개발자 옵션과 USB 디버깅을 켠다.
2. USB로 PC에 연결한다.
3. 기기에 표시되는 RSA 디버깅 허용 창을 승인한다.
4. `adb devices -l`에서 상태가 `device`인지 확인한다.
5. `Builds/Android/Development/CosmicBlock-dev.apk`를 설치하고 실행한다.

실기기에서 Portrait/SafeArea, Touch drag/drop, Gold preview, Board/Block/Journey 가독성, Line Clear SFX와 진동, 0.68초 연출, Combo, 일시정지/복귀, Game Over/Retry, 성능과 비정상 종료를 확인한다. 결과를 근거로만 Game Feel 값을 조정한다.

후속 후보는 Sprint 5 — Home, Localization & Release UI다. 패키지명 `com.DefaultCompany.CosmicBlock`은 출시 서명/AAB 전에 `com.playyournextworld.cosmicblock` 같은 최종 식별자로 확정한다.
