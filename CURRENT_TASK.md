# Current Task
Sprint 4 — Juice & Feedback 완료.

기존 LineClearResult, ScoreRules, Combo를 변경하지 않고 확정된 결과를 GameFeedbackController에 전달한다. FeedbackLayer는 64 Clear Cell과 24 Star를 미리 생성해 재사용하며 입력을 막지 않는다.

Line Clear 연출은 Gold Flash → Cell Pop/Fade → Star Burst → 실제 Score Delta Pop → Combo Presentation → Clear SFX → Android Short Haptic 순서다. Combo 표시는 CLEAR! / STAR COMBO / COSMIC COMBO로 강화된다.

Unity 6000.5.8f1에서 Sprint 4 Play Mode, Sprint 0~3 Core/Journey 회귀, Missing Script, 1080×1920/2400/1440 렌더를 검증한다. 수동 Scene 구성은 필요 없다.
