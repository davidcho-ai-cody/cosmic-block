# Current Task
Sprint 4.5 Android 실기기 UX 개선 완료.

작은 Piece의 시각 크기는 유지하면서 각 Bottom Slot 전체를 Drag hit area로 사용한다. SlotDragHandler가 기존 BlockDragHandler로 이벤트를 전달하며, consumed slot은 raycast를 즉시 비활성화한다. Drag visual, 1.05 scale, Gold highlight, Board preview, placement validation과 Drag Offset 110은 유지했다.

HUD 가독성은 COSMIC BLOCK 44→56, BEST/SCORE 36→44 Bold, Journey 25→30으로 개선했다. Progress Bar, Board, Bottom Slot의 크기와 게임 규칙은 변경하지 않았다.

Single/H2/V2/2x2/L/Reverse L slot-edge drag, consumed slot, 1·3·4·5자리 Score, 1080×1920/2400/1440 렌더, Sprint 0~4/Visual 회귀와 Missing Script 검증을 완료했다.
