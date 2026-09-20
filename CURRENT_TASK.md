# Current Task
Sprint 5 — Home Screen & Game Flow 완료.

앱은 Game.unity 단일 Scene에서 HOME으로 시작한다. 기존 SafeArea 게임 오브젝트는 계층과 참조를 유지하고 CanvasGroup으로 표시/입력만 전환한다. PLAY는 새 Run을 시작하며 Game Over에서 RETRY 또는 HOME으로 이동한다.

Home은 기존 CosmicBackground를 공유하고 COSMIC BLOCK, PLAY YOUR NEXT WORLD, PLAY, Best Score, Best Journey만 표시한다. Best Journey는 저장하지 않고 기존 Best Score와 JourneyProgress에서 파생한다.

Android Back은 Game 중 무동작, Game Over에서 HOME, Home에서 종료 요청이다. Home 이동은 Retry cleanup을 재사용해 Board/Score/Combo/Drag/Preview/Feedback/Audio 상태를 초기화한다.

Sprint 5 Flow/20회 stress/1080×1920·2400·1440 렌더와 Sprint 0~4/Visual/Missing Script 회귀를 완료했다.
