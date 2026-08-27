# Cyberpunker 2026

플레이어가 기밀 시설에 침투해 적의 감시를 피하고,  
퍼즐과 전투 기믹을 활용해 목표 지점까지 탈출하는 TPS 기반 잠입 액션·방 탈출 게임입니다.

> 실제 개발 과정의 버전 관리는 Unity Version Control을 사용했습니다.  
> 본 저장소는 포트폴리오 공개를 위해 직접 작성한 소스 코드를 별도로 정리한 저장소이므로, 실제 개발 과정의 커밋 기록은 포함하지 않습니다.  
> 외부 에셋과 프로젝트 설정은 포함하지 않아 단독 실행과 빌드를 지원하지 않습니다.

## 프로젝트 정보

- 개발 기간: 2026.03.01 ~ 2026.03.20
- 개발 인원: 1인 개발
- 개발 환경: Unity 6000.3.6f1, C#
- 주요 기술: Cinemachine, NavMesh, FSM, Object Pooling
- 협업 및 버전 관리: Unity Version Control
- 실행 환경: Windows PC
- 장르: TPS 기반 방 탈출·잠입 액션

## 프로젝트 자료


- [프로젝트 기획서](https://app.notion.com/p/3c9aa1b05e3e80de8428d95ba45ba444?source=copy_link)
- [프로젝트 소개 및 플레이 영상](https://youtu.be/KYtJ9fC7hlI)
- [게임 개발 포트폴리오 보기 (PDF)](docs/Game-Development-Portfolio-Lee-Chansu.pdf)
- [게임 개발 포트폴리오 원본 다운로드 (PPTX, GIF 포함)](docs/Game-Development-Portfolio-Lee-Chansu.pptx)

## 개발 목표

- 이동, 시점 제어, 적 감지와 전투 등 기본 시스템을 게임 기믹으로 연결
- HUD와 안내 UI를 최소화하고 레벨 구성, 오브젝트 배치와 연출로 진행 방향 전달
- 잠입, 백어택, 원거리 전투와 퍼즐 풀이를 하나의 플레이 흐름으로 구성

## 주요 구현

### 1. 감지 범위 기반 잠입·전투와 적 AI

적의 피격 영역, 전방 감지 영역과 후방 암살 영역을 별도 Collider로 구성해  
플레이어가 잠입, 백어택과 원거리 전투 중 상황에 맞는 방식을 선택하도록 설계했습니다.

적 행동은 대기, 순찰, 직접 추적, 경보 추적, 전투, 피격과 사망 상태로 구분했습니다.  
플레이어를 직접 감지하면 `SELFCHASE`, CCTV가 위치를 전달하면 `ALERTCHASE`로 전환하고,  
NavMeshAgent의 목적지와 이동 속도를 상태에 맞게 변경합니다.

- [EnemyAI](Assets/Scripts/Enemy/EnemyAI.cs)
- [EnemySensor](Assets/Scripts/Enemy/EnemySensor.cs)
- [EnemyBackZone](Assets/Scripts/Enemy/EnemyBackZone.cs)
- [EnemyAttack](Assets/Scripts/Enemy/EnemyAttack.cs)

### 2. CCTV 경보와 스캐너

CCTV 감시 구역에 플레이어가 진입하면 연결된 적에게 위치를 전달해 경보 추적 상태로 전환합니다.  
스캔 모드에서는 Cinemachine Camera Priority를 바꿔 1인칭 시야로 전환하고,  
평상시 보이지 않는 CCTV 감지 영역과 상호작용 가능한 오브젝트를 시각화합니다.

- [SecurityCameraController](Assets/Scripts/CCTV/SecurityCameraController.cs)
- [CCTVController](Assets/Scripts/CCTV/CCTVController.cs)
- [ScanConeView](Assets/Scripts/CCTV/ScanConeView.cs)
- [ScanMode](Assets/Scripts/Player/ScanMode.cs)

### 3. 아이템을 활용한 탐색 기믹

맵에서 카드키를 획득하면 카드 리더가 보유 여부를 확인해 잠긴 문을 개방합니다.  
조건 조합형 밸브 퍼즐은 Raycast 상호작용으로 밸브의 회전과 상태를 변경하고,  
모든 밸브가 목표 상태가 되면 다음 구역의 문을 엽니다.

일정 횟수 이상 공격하면 파괴되는 벽을 배치해 숨겨진 구역을 발견할 수 있도록 구성했습니다.

- [PlayerItemController](Assets/Scripts/Player/PlayerItemController.cs)
- [CardKeyItem](Assets/Scripts/Item/CardKeyItem.cs)
- [CardReader](Assets/Scripts/Item/CardReader.cs)
- [ValveRayInteractor](Assets/Scripts/Pipe/ValveRayInteractor.cs)
- [ValveSwitch](Assets/Scripts/Pipe/ValveSwitch.cs)
- [ValvePuzzleManager](Assets/Scripts/Pipe/ValvePuzzleManager.cs)
- [BreakableSecretWall](Assets/Scripts/HiddenWall/BreakableSecretWall.cs)

### 4. 플레이어 중심 시간 감속

Unity 전체의 `Time.timeScale`을 변경하지 않고 월드 속도 배율을 별도로 관리해,  
적 유닛과 투사체에만 감속을 적용하고 플레이어는 정상 속도로 조작할 수 있도록 구현했습니다.

시간 감속 상태에서는 Global Volume의 Motion Blur와 Color Adjustments를 제어해  
플레이어와 주변 세계의 시간 차이를 시각적으로 전달합니다.

- [TimeControlManager](Assets/Scripts/Manager/TimeControlManager.cs)
- [PlayerItemController](Assets/Scripts/Player/PlayerItemController.cs)
- [BulletController](Assets/Scripts/Bullet/BulletController.cs)

### 5. 투사체와 이펙트 오브젝트 풀링

사격할 때마다 총알과 발사 이펙트를 생성·제거하던 구조를 개선하기 위해  
투사체와 이펙트를 Key별 Queue로 관리하는 오브젝트 풀을 구현했습니다.

미리 생성한 객체가 부족하면 새 객체를 추가하고, 반환된 객체의 중복 등록을 방지해  
반복적인 Instantiate와 Destroy 호출을 줄였습니다.

- [PoolManager](Assets/Scripts/Bullet/PoolManager.cs)
- [BulletController](Assets/Scripts/Bullet/BulletController.cs)
- [PooledParticle](Assets/Scripts/Bullet/PooledParticle.cs)

## 레벨 디자인

- 적의 감지 범위를 이용한 잠입·전투 선택 구간
- CCTV 감시를 우회하거나 경보 상태의 적과 전투하는 구간
- 카드키 획득과 카드 리더를 통한 순차적 구역 개방
- 밸브 상태 조합으로 문을 여는 환경 퍼즐
- 파괴 가능한 벽 뒤에 배치한 히든 구역
- Cinemachine 전환을 활용한 다음 진행 구역 연출

## 프로젝트 회고

본격적으로 게임을 개발한 첫 개인 프로젝트로, 플레이어 조작, 전투, AI, 아이템과 퍼즐을 구현하고  
각 시스템을 하나의 완결된 플레이 흐름으로 연결했습니다.

초기에는 기능 구현에 집중하면서 여러 핵심 시스템이 서로 직접 참조하는 강한 결합 구조가 형성되었습니다.  
또한 switch-case 기반 FSM은 상태가 추가될수록 하나의 클래스에 로직이 집중되어 수정과 확장이 어려워지는 한계를 경험했습니다.  
이후 프로젝트에서는 상태별 책임 분리와 이벤트 기반 통신을 적용하며 확장성과 유지보수성을 우선하는 방향으로 설계를 개선했습니다.

## 공개 범위

본 저장소에는 프로젝트에서 직접 작성한 C# 코드만 포함합니다.

다음 자료는 라이선스 및 용량상의 이유로 공개하지 않습니다.

- Asset Store 및 제3자 에셋
- 모델, 애니메이션, 텍스처, VFX, 음원과 폰트
- 씬, 프리팹과 외부 패키지
- 빌드 파일과 프로젝트 설정
