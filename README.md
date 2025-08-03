# Capstone Project

## 개요

Unity 2D 플랫포머 게임

## 게임명

PAUSE

## 게임 소개
Pause.apk를 안드로이드 핸드폰에 설치하면 플레이 할 수 있습니다.

Unity를 사용하여 개발한 2D 플랫포머 게임으로, 몬스터를 제거하고 무기를 교체하며 스테이지를 진행하는 방식입니다.
최종 목표는 보스 몬스터를 제거하여 스테이지를 클리어하는 것입니다.

<img src="https://github.com/user-attachments/assets/7c065b9f-2de5-41d4-b90e-505ddd344f78" width="500" height="250"/>
<img src="https://github.com/user-attachments/assets/070ece04-c692-4445-a1b1-7a759ffd9777" width="500" height="250"/>
<img src="https://github.com/user-attachments/assets/f1467cb5-f797-4b38-a0bc-857e37693a81" width="500" height="250"/>
<img src="https://github.com/user-attachments/assets/05b7ac0d-1a82-409b-a103-7763ae73597e" width="500" height="250"/>
<img src="https://github.com/user-attachments/assets/e9744254-c9a6-48c2-a82a-0ef38902ef4c" width="500" height="250"/>

## 개발 기간

개발 기간 : 24.03 ~ 24.06

## 개발 환경

Unity
C#

## 사용 기술

싱글톤 패턴을 사용한 매니저 기반 관리 구조 및
FSM을 사용한 몬스터의 상태 관리

## 구현 기능

플레이어
  이동, 공격, 피격, 인벤토리, 무기 교체

일반 몬스터
  이동, 공격, 피격

보스 몬스터
  이동, 공격, 피격, 스킬

UI
  메인 화면, 체력, 장비창, 설정, 게임 오버, 게임 클리어

## 팀원

팀장 : 서상영
* 몬스터 이동, 공격 및 피격
* 플레이어 이동, 공격 및
* 무기 교체 및 투사체 변경
* 맵 구성
  
팀원 : 민지규
* 게임 매니저
* 사운드 매니저
* 인벤토리
* 맵 구성
  
팀원 : 오지영 
* 플레이어 이동
* 인벤토리
* UI 구성
* 맵 구성

#  🗡️ Dungreed (모작)

🛠️ **개발 도구**
  <img src="https://img.shields.io/badge/C++-00599C?style=flat-square&logo=cplusplus&logoColor=white"/> <img src="https://img.shields.io/badge/SFML-8CC445?style=flat-square&logo=sfml&logoColor=white"/>

📅 **개발 기간**
 24.11.26 ~ 24.12.09 (2주)

🧑‍💻 **개발진**
 <img src="https://img.shields.io/badge/민지규, 황규영, 박지광-80247B?style=flat-square&logo=&logoColor=white"/>

SFML로 제작한 PC 2D 로그라이트 액션 게임 모작 프로젝트입니다.

> 다양한 몬스터를 무찌르며, 다음 스테이지를 향해 끝없이 나아가세요. 
> 플레이어는 근/원거리 무기를 번갈아 사용하며 공격 할 수 있습니다.
> 원작 던전의 1,2층까지 구현하였습니다.

---

## 🛠️ 주요 구현 요소
<table>
  <tr>
    <td align="center"><strong>스와이프로 레인 이동, 점프</strong></td>
    <td align="center"><strong>스킬 선택</strong></td>
    <td align="center"><strong>보스 전투</strong></td>
  </tr>
  <tr>
    <td><img src="./Screenshot/플레이화면.png" width="250"/></td>
    <td><img src="./Screenshot/스킬선택화면.png" width="250"/></td>
    <td><img src="./Screenshot/스킬과보스전화면.png" width="250"/></td>
  </tr>
</table>

<table>
  <tr>
    <td align="center"><strong>동물 캐릭터 가챠</strong></td>
    <td align="center"><strong>동물 강화</strong></td>
  </tr>
  <tr>
    <td><img src="./Screenshot/가챠화면.png" width="260"/><img src="./Screenshot/가챠결과.png" width="255"/></td>
    <td><img src="./Screenshot/동물강화화면.png" width="250"/></td>
  </tr>
 
</table>
<table>
  <tr>
    <td align="center"><strong>로컬라이제이션</strong></td>
    <td align="center"><strong>애드몹 보상 광고</strong></td>
  </tr>
  <tr>
    <td><img src="./Screenshot/설정화면.png" width="250"/></td>
    <td><img src="./Screenshot/광고.jpg" width="500"/></td>
  </tr>
</table>

- **플레이어 조작** 구현 👉 [PlayerMove.cs](https://github.com/KALI-UM/Unity-AnimalBreakOut/blob/main/Assets/Scripts/Player/PlayerMove.cs)
   
- **구글 애드몹 연동하여 보상형 광고** 구현

- **어드레서블 에셋 시스템**을 활용하여 캐릭터 리소스 비동기 로드 구현 👉 [PlayerLoadManager.cs](https://github.com/KALI-UM/Unity-AnimalBreakOut/blob/main/Assets/Scripts/Managers/PlayerLoadManager.cs)

- **행동 트리** 구현 👉 [BehaviorTree](https://github.com/KALI-UM/Unity-AnimalBreakOut/tree/main/Assets/Scripts/BehaviourTree)
  
- **개발 툴**
  - 게임 오브젝트를 캡처해 png파일로 생성하는 **아이콘 이미지 캡처 툴** 개발 👉 [GameObjectToTexture.cs](https://github.com/KALI-UM/Unity-AnimalBreakOut/blob/main/Assets/Scripts/IconStudio/GameObjectToTexture.cs#L22)
  - **유니티 웹리퀘스트**를 활용해 최신 데이터 테이블 파일로 갱신하는 **데이터 테이블 갱신 툴** 구현 👉 [GoogleSheetManager.cs](https://github.com/KALI-UM/Unity-AnimalBreakOut/blob/main/Assets/Scripts/Managers/GoogleSheetManager.cs#L59)
  - 스테미나, 경험치, 보스 HP 등을 테스트 할 수 있는 **에디터 툴** 개발
    - 👉 [BossStatusEditor.cs](https://github.com/KALI-UM/Unity-AnimalBreakOut/blob/main/Assets/Editor/BossStatusEditor.cs)
    - 👉 [GameDataManagerEditor.cs](https://github.com/KALI-UM/Unity-AnimalBreakOut/blob/main/Assets/Editor/GameDataManagerEditor.cs)
    - 👉 [OutGameUIManagerEditor.cs](https://github.com/KALI-UM/Unity-AnimalBreakOut/blob/main/Assets/Editor/OutGameUIManagerEditor.cs)

