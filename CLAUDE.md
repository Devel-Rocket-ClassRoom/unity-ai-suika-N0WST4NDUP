# CLAUDE.md — 수박 게임 개발 가이드

> 이 문서는 Claude Code가 매 세션마다 자동으로 읽는다. 모든 작업은 여기에 정의된 컨벤션과 게이트 규칙을 따른다.

---

## 1. 프로젝트 개요

- **목표**: Unity로 수박 게임(Suika Game) 클론 구현 — 떨어뜨리기 · 충돌 · 머지 · 게임오버 루프
- **엔진**: Unity 6 (URP 2D) + Input System
- **과제 출처**: [Readme.md](Readme.md) — 필수 미션 3개(메인 메커닉 / 애셋 / UI·마무리) + 자유 구현
- **저장소**: `Devel-Rocket-ClassRoom/unity-ai-suika-N0WST4NDUP` (origin)
- **기본 브랜치**: `master`
- **문서 위치**: 모든 산출 문서는 [Docs/](Docs/) 하위에 둔다. (예: [Docs/TASKS.md](Docs/TASKS.md), [Docs/GDD.md](Docs/GDD.md))

---

## 2. 개발 플로우 (미션 게이트)

모든 작업은 [Docs/TASKS.md](Docs/TASKS.md)의 Task 단위로 진행한다. Task 하나 = 이슈 하나 = 브랜치 하나 = PR 하나.

### 표준 절차

1. **Task 시작 전**: `gh issue create` 로 GitHub 이슈를 만든다 → 발급된 이슈 번호를 받는다.
2. **브랜치 생성**: `git checkout master && git pull && git checkout -b feature/<kebab>`
3. **구현 + 커밋**: 작은 단위로 커밋한다. 커밋 메시지에 이슈 번호를 포함한다.
4. **PR 생성**: `gh pr create` — 본문 끝에 `Closes #N` 을 넣어 머지 시 이슈가 자동 종료되게 한다.
5. **강사 리뷰 → 승인** 후 master squash merge.
6. **이슈 자동 close** 확인. [Docs/TASKS.md](Docs/TASKS.md)의 상태 표를 ✅로 갱신.

### 🚦 미션 게이트 STOP 규칙 (가장 중요)

- 미션 1의 마지막 Task PR이 merge되면 → **미션 2의 Task 2.1만 [Docs/TASKS.md](Docs/TASKS.md)에 기록하고 즉시 멈춘다.**
- 미션 2의 마지막 Task PR이 merge되면 → **미션 3의 Task 3.1만 기록하고 즉시 멈춘다.**
- 미션 3의 마지막 Task PR이 merge되면 → **자유 구현 후보만 나열하고 멈춘다.**
- Claude는 절대로 다음 미션을 자발적으로 시작하지 않는다. 사용자(또는 강사)가 명시적으로 "다음 미션 진행" 이라고 말한 뒤에만 진행한다.

---

## 3. 이슈 컨벤션 (GitHub Issues)

- **제목 형식**
  - 미션 작업: `[미션N] <한 줄 요약>` 예: `[미션1] 같은 단계 과일 머지 로직 추가`
  - 메타 작업: `[chore] <요약>` 예: `[chore] 개발 컨벤션 문서 초기 셋업`
- **라벨**: `mission-1` / `mission-2` / `mission-3` / `feature` / `bug` / `chore` / `docs`
- **본문 템플릿**

```markdown
## 배경
<왜 필요한지, 어떤 미션 항목에 해당하는지>

## 완료 조건
- [ ] 조건 1
- [ ] 조건 2

## 관련
- Docs/TASKS.md#task-1-3
```

---

## 4. 브랜치 컨벤션

- 형식: `feature/<kebab-case-요약>` — 예: `feature/drop-input`, `feature/merge-logic`
- 메타/문서: `chore/<요약>` — 예: `chore/init-conventions`, `chore/gdd-draft`
- 버그 수정: `fix/<요약>`
- **1 Task = 1 브랜치 = 1 이슈 = 1 PR** 원칙. master에 직접 커밋하지 않는다.

---

## 5. 커밋 컨벤션 (Conventional Commits + 한글)

- **접두사**: `feat` / `fix` / `chore` / `docs` / `refactor` / `test` / `style`
- **형식**: `<type>: <한글 요약> (#이슈번호)`
- **예시**
  - `feat: 과일 드롭 입력 추가 (#3)`
  - `fix: 머지 직후 콜라이더 중복 트리거 차단 (#5)`
  - `chore: 개발 컨벤션 및 작업 분해 문서 초기 셋업`
  - `docs: GDD 과일 단계 표 보강 (#2)`
- 본문(선택)은 한글로 자유 작성. 무엇이 아니라 **왜**를 적는다.

---

## 6. PR 컨벤션

- **제목**: 브랜치의 대표 커밋과 같은 형식
- **본문 템플릿**

```markdown
## 변경 사항
- <핵심 변경 1>
- <핵심 변경 2>

## 테스트
- [ ] 에디터 플레이로 <시나리오> 확인
- [ ] (해당 시) MCP/콘솔 에러 없음

Closes #N
```

---

## 7. 코드 컨벤션 (Unity C#)

### 폴더 구조

```
Assets/
  Scripts/
    Gameplay/      // 플레이어 입력, 과일 행동, 충돌·머지
    Data/          // ScriptableObject 정의
    UI/            // HUD, 결과 화면
    Manager/       // GameManager, SpawnManager
  Sprites/
    Fruits/        // 미션 2 산출물 최종 위치
  Prefabs/
    Fruits/        // 과일 단계별 프리팹
  ScriptableObjects/
  Scenes/
  Generated/       // AI Generators 임시 산출물 (정리 후 Sprites/Fruits로 이동)
```

### 네이밍

- 클래스 / 메서드 / 프로퍼티: `PascalCase`
- private 필드: `_camelCase`
- `[SerializeField] private`: `camelCase`
- 상수: `PascalCase` (예: `public const int MaxStage = 11;`)
- namespace: `Watermelon.<영역>` 예: `Watermelon.Gameplay`, `Watermelon.Data`

### 원칙

- 한 파일 한 MonoBehaviour (DTO/SO는 예외)
- 매직 넘버 금지 — `ScriptableObject` 또는 `[SerializeField]` 상수로
- 컴포넌트 참조는 `[SerializeField] private` 또는 `GetComponent`를 `Awake`에서 캐싱
- `Update`에서 매 프레임 `GetComponent`/`Find` 호출 금지

---

## 8. Unity MCP / AI 도구 사용

- 씬·게임오브젝트 편집은 가능한 한 Unity MCP의 `ManageScene` / `ManageGameObject` 사용
- 스크립트 편집은 일반 Edit 도구 우선, Unity 컴파일 결과 확인은 `Unity_GetConsoleLogs`
- Generators 산출물은 일단 `Assets/Generated/` 로 받고, 임포트 설정과 콜라이더를 정리한 뒤 `Assets/Sprites/Fruits/` 로 이동
- 씬 저장/플레이 전 콘솔 에러 0개 유지

---

## 9. 작업 시작 시 체크리스트

새 Task를 시작할 때 Claude는 다음을 순서대로 한다.

1. [Docs/TASKS.md](Docs/TASKS.md) 에서 현재 Task의 완료 조건과 브랜치명을 확인한다.
2. 사용자에게 "지금 Task X.Y를 시작합니다 — 이슈 생성해도 될까요?" 라고 확인을 받는다.
3. `gh issue create` 로 이슈를 만들고 번호를 받는다.
4. feature 브랜치를 만들고 작업한다.
5. 미션의 **마지막** Task 라면, PR merge 후 다음 미션 첫 Task만 [Docs/TASKS.md](Docs/TASKS.md)에 기록하고 멈춘다.

---

## 10. 참고 문서

- [Readme.md](Readme.md) — 과제 명세
- [Docs/TASKS.md](Docs/TASKS.md) — Task 분해 및 진행 상황
- [Docs/GDD.md](Docs/GDD.md) — 게임 디자인 문서 (Task 0 산출물)
