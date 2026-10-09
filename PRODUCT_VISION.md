# Product Vision
The primary goal of COSMIC BLOCK v1.0 is shipping.
새로운 기능보다 완성을 우선한다.
v1.0 범위를 벗어나는 아이디어는 구현하지 않고 BACKLOG에 기록한다.

Simple Code. Small Scope. Strong Visual Identity. Ship First.
작지만 완성도 있는 게임을 Google Play에 출시하고 AdMob까지 적용하여 전체 출시 사이클을 경험한다.
브랜드: PLAY YOUR NEXT WORLD. 첫 플랫폼 Android. iOS는 범위 밖.


## STAR JOURNEY
COSMIC BLOCK gives score a sense of travel and progression.
The player is not simply earning points. The player is traveling deeper into space.
Core Question: **HOW FAR CAN YOU GO?**
Journey is a continuous score meaning layer, not a stage system.

## Planet Restoration — Sprint 10 current scope
점수는 현재 Run의 도전이며 별빛은 선택한 행성의 영구 복원도입니다. 푸른 별1500, 크리스탈리아3000, 이그니스6000, 글라시아12000, 루미나24000 순서로 이전 행성 완성 시 해금됩니다. 5개 행성의 실제 단계 이미지를 통합했으며 이전 행성 100% 복원 시 다음 행성을 플레이할 수 있습니다. 5단계 이미지 표시와 최종 복원 완료는 별개이며 모든 목표 합계는46,500입니다. 최종 엔딩/별빛의 수호자, AdMob/IAP/서버는 이번 Sprint에 구현하지 않습니다. 기존 STAR JOURNEY 설명은 초기 설계 기록이며 현재 영구 성장 구조는 Planet Restoration입니다.

## Single active Run — 자동 저장 및 이어하기
v1.0은 전체 행성에 걸쳐 진행 중인 Run을 하나만 저장합니다. HOME 이동·앱 재실행 이후에도 점수/보드/남은 Piece/Combo를 이어하며, 명시적 새 게임 확인 또는 Game Over에서만 종료합니다. Run 점수, 글로벌 BEST, 행성별 영구 별빛은 분리합니다.

## 행성 간 연속 플레이 — Sprint 10.2.1
행성 전환은 현재 도전을 이어가는 행위입니다. 완료 팝업과 해금 행성 도감 선택은 하나의 Run을 유지하며 SCORE·보드·남은 Piece·Combo·무료 보조 기능 횟수를 초기화하지 않습니다. 행성별 영구 별빛은 분리하고 이전 보상을 다음 행성에 이월하거나 재지급하지 않습니다. 명시적 새 게임 확정·Retry·저장 Run 없는 PLAY만 새 도전을 시작합니다.
