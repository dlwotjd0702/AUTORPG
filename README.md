# AUTO RPG

Unity 기반의 Idle RPG 게임 프로젝트입니다.

## 📋 목차

- [프로젝트 개요](#프로젝트-개요)
- [기술 스택](#기술-스택)
- [프로젝트 구조](#프로젝트-구조)
- [주요 시스템](#주요-시스템)
- [설치 및 실행](#설치-및-실행)
- [데이터 구조](#데이터-구조)
- [주요 기능](#주요-기능)

## 🎮 프로젝트 개요

AUTO RPG는 Unity 엔진으로 개발된 자동 전투 기반 RPG 게임입니다. 플레이어는 캐릭터를 강화하고, 장비를 수집하며, 스킬을 장착하여 무한히 진행되는 스테이지를 클리어하는 것이 목표입니다.

### 주요 특징

- **자동 전투 시스템**: 플레이어와 몬스터가 자동으로 전투를 진행
- **장비 수집 및 강화**: 무기, 방어구, 반지, 스킬을 수집하고 강화
- **가챠 시스템**: 다양한 등급의 장비를 뽑을 수 있는 가챠 시스템
- **무한 스테이지**: 스테이지와 웨이브가 무한히 진행되는 구조
- **클라우드 저장**: Firebase를 통한 클라우드 저장 및 랭킹 시스템
- **광고 연동**: AdMob을 통한 광고 수익화

## 🛠 기술 스택

### 엔진 및 버전
- **Unity**: 2022.3.62f1
- **언어**: C#

### 주요 패키지
- **Firebase SDK**: 인증, Firestore, 클라우드 저장
- **Google Mobile Ads**: AdMob 광고 연동
- **Unity Purchasing**: 인앱 결제
- **TextMesh Pro**: 고품질 텍스트 렌더링
- **CsvHelper**: TSV 파일 파싱

### 외부 서비스
- **Firebase Authentication**: 이메일/익명 로그인
- **Firebase Firestore**: 클라우드 데이터베이스
- **Google AdMob**: 광고 서비스

## �� 프로젝트 구조

```
AUTORPG/
├── Assets/
│   ├── Scripts/              # 게임 로직 스크립트
│   │   ├── Ads/             # 광고 관리
│   │   ├── Combat/          # 전투 시스템
│   │   ├── Datas/           # 데이터 클래스
│   │   ├── Gameloop/        # 게임 루프 관리
│   │   ├── Inventory/      # 인벤토리 시스템
│   │   ├── Save/            # 저장/로드 시스템
│   │   ├── Skills/          # 스킬 시스템
│   │   ├── SMB/             # State Machine Behaviour
│   │   ├── Stats/           # 스탯 관리
│   │   └── UI/              # UI 컨트롤러
│   ├── Prefabs/             # 프리팹
│   ├── Resources/           # 리소스 파일
│   ├── Scenes/              # 씬 파일
│   ├── StreamingAssets/     # 스트리밍 에셋 (TSV 데이터)
│   └── ...
├── ProjectSettings/         # Unity 프로젝트 설정
└── Packages/               # Unity 패키지 매니페스트
```

## 🎯 주요 시스템

### 1. 전투 시스템 (Combat)

#### State Machine Behaviour (SMB)
- **Player**: 플레이어 상태 머신 (Idle, Move, Attack, Death)
- **Monster**: 몬스터 상태 머신 (Idle, Move, Attack, Death)
- `SceneLinkedSMB`를 통한 씬별 상태 관리

#### 전투 인터페이스
- `IDamageable`: 피해를 받을 수 있는 객체
- `IAttackStat`: 공격력을 제공하는 객체

#### 전투 UI
- **HPBarUI**: 체력바 표시 (오브젝트 풀링 사용)
- **FloatingText3D**: 데미지 텍스트 표시
- **CombatPowerUI**: 전투력 표시

### 2. 스킬 시스템 (Skills)

#### 스킬 타입
- **Active Skill**: 액티브 스킬 (쿨타임 기반)
- **Passive Skill**: 패시브 스킬 (지속 효과)

#### 스킬 종류
- `SingleTargetDamageSkill`: 단일 타겟 데미지
- `AoeSkill`: 범위 공격
- `MultiHitSkill`: 다중 타격
- `BuffSkill`: 버프 스킬
- `PassiveSkill`: 패시브 스킬

#### 스킬 관리
- `SkillManager`: 스킬 사용 및 쿨타임 관리
- `SkillFactory`: 스킬 생성 팩토리 패턴
- 자동 스킬 사용 기능 지원

### 3. 인벤토리 시스템 (Inventory)

#### 아이템 타입
- **Weapon**: 무기 (공격력, 공격속도 증가)
- **Armor**: 방어구 (방어력, 체력 증가)
- **Ring**: 반지 (치명타율, 치명타 데미지 증가)
- **Skill**: 스킬 (특수 효과)

#### 인벤토리 기능
- 장비 장착/해제
- 아이템 합성 (Combine)
- 아이템 강화 (Enhance)
- 스킬 퀵슬롯 (최대 6개)

#### 스탯 계산
- **보유 효과**: 아이템을 보유만 해도 적용되는 효과
- **장착 효과**: 아이템을 장착해야 적용되는 효과
- 타입별 곱셈 계산 (무기, 방어구, 반지)

### 4. 스탯 시스템 (Stats)

#### 기본 스탯
- **Attack**: 공격력
- **Attack Speed**: 공격 속도
- **Defense**: 방어력
- **HP**: 체력
- **Crit Rate**: 치명타 확률
- **Crit Damage**: 치명타 데미지

#### 스탯 계산
- 기본 스탯 + 장비 보너스 + 버프
- 레벨업 시 기본 스탯 증가
- 강화 시스템을 통한 스탯 증가

### 5. 스테이지 시스템 (Gameloop)

#### 스테이지 구조
- 무한 스테이지 시스템
- 스테이지당 10개 웨이브
- 웨이브당 8마리 몬스터

#### 진행 모드
- **Advance**: 자동으로 다음 웨이브 진행
- **Repeat**: 현재 웨이브 반복

#### 난이도 조절
- 스테이지/웨이브에 따른 몬스터 스탯 증가
- 보상도 난이도에 비례하여 증가

### 6. 가챠 시스템 (Inventory/GachaSystem)

#### 가챠 레벨 시스템
- 아이템 타입별 가챠 레벨 존재
- 가챠 뽑기 시 경험치 획득
- 레벨업 시 고등급 확률 증가

#### 등급 시스템
- 최대 20등급
- 등급별 확률 자동 계산
- 레벨에 따라 등급 해금

### 7. 저장 시스템 (Save)

#### 저장 방식
- **로컬 저장**: Unity PlayerPrefs (임시)
- **클라우드 저장**: Firebase Firestore

#### 저장 데이터
- 플레이어 레벨 및 경험치
- 인벤토리 정보
- 스테이지 진행도
- 강화 레벨
- 스킬 장착 정보

#### 랭킹 시스템
- Firebase Firestore를 통한 랭킹 저장
- 스테이지/웨이브 클리어 기록 기반
- 상위 랭킹 조회 기능

### 8. 광고 시스템 (Ads)

#### 광고 타입
- **배너 광고**: 화면 하단 고정
- **보상형 광고**: 버프 획득용

#### 광고 버프
- 공격력 버프 (일시적)
- 광고 시청 후 버프 활성화

### 9. UI 시스템

#### 주요 UI
- **InventoryUIController**: 인벤토리 관리
- **GachaUIController**: 가챠 UI
- **UpgradeUIController**: 강화 UI
- **SkillQuickSlotUI**: 스킬 퀵슬롯
- **FirebaseUI**: Firebase 로그인 UI

## 🚀 설치 및 실행

### 요구사항
- Unity 2022.3.62f1 이상
- Android SDK (Android 빌드용)
- Firebase 프로젝트 설정

### 설정 방법

1. **Firebase 설정**
   - `Assets/StreamingAssets/google-services.json` 파일 추가
   - Firebase 프로젝트에서 다운로드한 설정 파일 사용

2. **AdMob 설정**
   - AdMob 앱 ID 설정
   - 광고 단위 ID 설정

3. **데이터 파일**
   - `Assets/StreamingAssets/equipment.tsv` 파일 확인
   - 장비 데이터가 올바르게 로드되는지 확인

### 빌드
1. Unity Editor에서 프로젝트 열기
2. File > Build Settings에서 플랫폼 선택
3. Build 실행

## 📊 데이터 구조

### EquipmentData (장비 데이터)

TSV 파일 형식으로 저장되며, 다음 필드를 포함합니다:

- `id`: 아이템 고유 ID (예: `weapon_01`)
- `type`: 아이템 타입 (weapon, armor, ring, skill)
- `name`: 아이템 이름
- `grade`: 등급 (1~20)
- `ownedAtkPercent`: 보유 시 공격력 증가율
- `equipAtkPercent`: 장착 시 공격력 증가율
- `ownedAtkSpdPercent`: 보유 시 공격속도 증가율
- `equipAtkSpdPercent`: 장착 시 공격속도 증가율
- `ownedDefPercent`: 보유 시 방어력 증가율
- `equipDefPercent`: 장착 시 방어력 증가율
- `ownedHpPercent`: 보유 시 체력 증가율
- `equipHpPercent`: 장착 시 체력 증가율
- `ownedCritRatePercent`: 보유 시 치명타율 증가율
- `equipCritRatePercent`: 장착 시 치명타율 증가율
- `ownedCritDmgPercent`: 보유 시 치명타 데미지 증가율
- `equipCritDmgPercent`: 장착 시 치명타 데미지 증가율
- `skillType`: 스킬 타입 (Active, Passive)
- `skillOwnedValue`: 보유 시 스킬 효과값
- `skillEquipValue`: 장착 시 스킬 효과값
- `cooldown`: 쿨타임 (초)
- `skillPower`: 스킬 파워
- `maxLevel`: 최대 레벨
- `description`: 설명

### SaveData (저장 데이터)

Firebase Firestore에 저장되는 데이터 구조:

```csharp
- level: 플레이어 레벨
- exp: 경험치
- expToLevelUp: 레벨업 필요 경험치
- gold: 골드
- atkUpgradeLevel: 공격력 강화 레벨
- defUpgradeLevel: 방어력 강화 레벨
- hpUpgradeLevel: 체력 강화 레벨
- atkSpdUpgradeLevel: 공격속도 강화 레벨
- critRateUpgradeLevel: 치명타율 강화 레벨
- critDmgUpgradeLevel: 치명타 데미지 강화 레벨
- dropRateUpgradeLevel: 드롭률 강화 레벨
- currentStage: 현재 스테이지
- currentWave: 현재 웨이브
- maxClearedStage: 최대 클리어 스테이지
- maxClearedWave: 최대 클리어 웨이브
- clearedStageWave: 클리어한 스테이지/웨이브 목록
- progressMode: 진행 모드 (0: Repeat, 1: Advance)
- inventorySlots: 인벤토리 슬롯 정보
- equippedSkillIds: 장착된 스킬 ID 목록
- gems: 보석
- savedAt: 저장 시간
- nickname: 닉네임
```

## 🔧 주요 기능

### 1. 자동 전투
- 플레이어와 몬스터가 자동으로 전투
- State Machine Behaviour를 통한 상태 관리
- 애니메이션 기반 전투 연출

### 2. 스킬 시스템
- 최대 6개 스킬 장착 가능
- 자동 스킬 사용 옵션
- 쿨타임 기반 스킬 관리

### 3. 장비 시스템
- 무기, 방어구, 반지 장착
- 보유 효과 + 장착 효과
- 아이템 합성 및 강화

### 4. 가챠 시스템
- 타입별 가챠 레벨 시스템
- 레벨업 시 고등급 확률 증가
- 단일/10연 뽑기 지원

### 5. 클라우드 저장
- Firebase Firestore 연동
- 자동 저장 기능
- 랭킹 시스템 연동

### 6. 광고 연동
- AdMob 배너 광고
- 보상형 광고
- 광고 버프 시스템

## 📝 개발 노트

### 네임스페이스 구조
- `IdleRPG`: 플레이어/몬스터 관련
- `Stats`: 스탯 시스템
- `Inventory`: 인벤토리 시스템
- `Combat`: 전투 시스템

### 디자인 패턴
- **Factory Pattern**: 스킬 생성 (`SkillFactory`)
- **Object Pooling**: HPBar, FloatingText 최적화
- **Observer Pattern**: 이벤트 기반 통신
- **State Pattern**: SMB를 통한 상태 관리

### 성능 최적화
- 오브젝트 풀링 (몬스터, HPBar, FloatingText)
- 딕셔너리 기반 데이터 조회
- 이벤트 기반 UI 업데이트


## 👥 기여자

개인프로젝트


