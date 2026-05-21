# Changelog
## 0.3.2 — 2026-05-21
* **Docs:** adicionado README em Chinês Simplificado (`docs/README.zh-cn.md`).
* **Docs:** links de idioma adicionados no `README.md` e `docs/README.pt-br.md`.
* **Style:** Typing SVG do `README.pt-br.md` traduzido para português nas categorias de mecânica.

## 0.3.1 — 2026-05-21
* **Fix:** correção do arquivo `LICENSE` (Apache 2.0).

## 0.3.0 — 2026-05-20
* **Breaking – Licença:** alterada de MIT para Apache 2.0; header de copyright adicionado em todos os `.cs`.
* **Breaking – Telemetria:** `apiBase` padrão atualizado de `localhost:8080` para `https://thinklib.thinktedlab.org`.
* **Breaking – Renomeações de arquivos e pasta:**
  * `TipoColetavel.cs` → `CollectibleType.cs` (enum `TipoColetavel` → `CollectibleType`; valores `Moeda` → `Coin`, `Vida` → `Life`)
  * `PatrulheiroAI.cs` → `PatrollerAI.cs` / pasta `Patrulheiro/` → `Patroller/`
  * `Itens.cs` → `Items.cs`
  * `TimedPlatfom.cs` → `TimedPlatform.cs`
  * `PlatformerJump Controller.cs` → `PlatformerJumpController.cs`
* **Breaking – API pública (`CollectibleItem` / `GameManager`):** campos e métodos renomeados de português para inglês (`valor` → `value`, `somColeta` → `collectSound`, `destruirAutomaticamente` → `destroyOnCollect`, `AdicionarColetavel` → `AddCollectible`, `moedas` → `coins`, etc.).

## 0.2.3 — 2026-05-11
* **Chore:** removidos `Thinklib_Core_Manual_v0.2.1.docx` e `generate_manual.py` do repositório; adicionado `.gitignore` para evitar reinclusão acidental.

## 0.2.2 — 2026-05-11
* **Fix:** adicionado `using Thinklib.Telemetry;` nos scripts instrumentados que estavam com erro `CS0103: ThinklibTelemetry does not exist`.
  * `RewardChest.cs`, `SawHazard.cs`
  * `CommandQueueManager.cs`, `PlayerAgent.cs`
  * `GridAgent.cs`, `GridCommandManager.cs`, `GridManager.cs`
  * `Dropzone.cs`, `ItemSlot.cs`

## 0.2.1 — 2026-04-24
* **Novo – Mecânicas de Platformer:**
  * `RewardChest.cs` — baú de recompensa que cura o jogador ao toque (via `LifeSystemController.Heal`); muda sprite ao ser usado e desativa collider.
  * `SawHazard.cs` — perigo de serra que causa dano ao jogador (via `LifeSystemController.TakeDamage`); suporta modo de patrulha entre pontos A e B com Gizmos de debug.

* **Novo – Mecânicas de Point & Click:**
  * **CommandsQueue** — sistema de fila de comandos estilo "Hora do Código":
    * `ICommand.cs`, `MoveForwardCommand.cs`, `TurnLeftCommand.cs`, `TurnRightCommand.cs`
    * `PlayerAgent.cs` — executa sequência de ações (mover, girar ±90°) com suporte a reset.
    * `CommandQueueManager.cs` — gerencia a fila e atualiza UI da lista de comandos.
  * **GridCoordinates** — navegação de agente por coordenadas em grid:
    * `IGridCommand.cs`, `GridMoveCommand.cs`
    * `GridAgent.cs` — move célula a célula até o destino via coroutine.
    * `GridCommandManager.cs` — aceita input de linha/coluna e executa fila.
    * `GridManager.cs` — singleton que converte coordenadas (linha, coluna) para posição no mundo.

* **Melhoria – Dropzone (Point & Click):**
  * `DropZone.cs` — items com `hasTimer` têm vida útil: se expirar antes do puzzle ser concluído, o item é removido automaticamente.
  * `DropZoneManager.cs` — para todos os timers ao concluir o puzzle corretamente.
  * `ItemSlot.cs` — exibe quantidade de itens stackáveis e contador de timer em vermelho no slot.

* **Scripts instrumentados (MIA):**
  * **Common / Enviroment:** `RewardChest.cs`
  * **Platformer / Enviroment:** `SawHazard.cs`
  * **PointAndClick / CommandsQueue:** `PlayerAgent.cs`, `CommandQueueManager.cs`
  * **PointAndClick / GridCoordinates:** `GridAgent.cs`, `GridCommandManager.cs`, `GridManager.cs`
  * **PointAndClick / Dropzone:** `DropZone.cs`, `ItemSlot.cs`

## 0.2.0 — 2025-12-12
* **Novo – MIA (Métricas & Instrumentação Analítica):** telemetria unificada no runtime.
  * Eventos padrão: `mechanic_instantiated`, `mechanic_used`, `mechanic_error`.
  * Config via **ThinklibTelemetryConfig.asset** (apiBase/route, batch window, etc.).
  * **Compat:** ajustado DTO para o backend (campo **`plataform`** – grafia conforme API).
  * Logs detalhados no `TelemetrySender` (preview do payload, status/erro, duração).

* **Scripts instrumentados (MIA):**
  * **Common / Effects**
    * `DeathEffect.cs`
    * `PlayerHurtEffect.cs`
  * **Common / LifeSystem**
    * `LifeUIBar.cs`
    * `LifeUIIcons.cs`
    * `UILockerAndFollower.cs`
  * **Platformer / Collectibles**
    * `CollectibleItem.cs`
    * `GameManager.cs`
  * **Platformer / Combat**
    * `PlatformerProjectileAttackController.cs`
    * `PlayerMeleeAttackController.cs`
    * `PlayerShooterController.cs`
    * `ProjectileDamageDealer.cs`
  * **Platformer / Enemy / Types**
    * **Patroller:** `DamageOnTouch.cs`, `PatrollerAI.cs`
    * **Shooter:** `EnemyShooterAI.cs`
  * **Platformer / Environment**
    * `MovingPlatform.cs`
    * `TimedPlatform.cs`
  * **Platformer / Movement**
    * `PlatformerJumpController.cs`
    * `PlatformerMovementController.cs`
  * **Topdown / Combat**
    * `PlayerTopdownMeleeAttackController.cs`
    * `PlayerTopdownShooterController.cs`
  * **Topdown / Enemy / Types**
    * **Patroller:** `TopdownDamageOnTouch.cs`, `TopdownPatrollerAI.cs`
    * **Shooter:** `TopdownEnemyShooterAI.cs`
  * **Topdown / Movement**
    * `TopdownMovementController.cs`
  * **Topdown / NPC**
    * `DialogueBubble.cs`
    * `TopdownNPCController.cs`
  * **TowerDefense**
    * **Defeat system:** `PlayerHealth.cs`
    * **Defense:** `Bullet.cs`, `TowerPlacement.cs`, `TowerShooter.cs`
    * **Enemy progression:** `EnemyHealth.cs`, `EnemyPath.cs`, `EnemySpawner.cs`
    * **Resource management:** `PlayerScore.cs`, `TowerShop.cs`
    * **Tower upgrade:** `TowerUpgrade.cs`


## 0.1.17 — 2025-09-01
* **Novo:** adição do menu **Thinklib → Point and Click**, com categorias de componentes para **Dropzone**, **Graph** e **Inventory**:
  * Scripts atualizados com `AddComponentMenu` / `CreateAssetMenu` para aparecerem no menu **Component → Thinklib → Point and Click → …**.
  * Organização interna dos scripts em subpastas lógicas (Dropzone, Graph e Inventory).
* **UX:** padronização de nomes de menu conforme os arquivos originais, garantindo consistência entre código e interface.
* **Fix:** corrigido uso incorreto de argumentos no atributo `[CreateAssetMenu]` em `Item` e `CombinationRecipe`, que causava erros de compilação (**CS1016**, **CS1729**) ao importar o pacote.
* **Chore:** pequenas revisões no código para unificação do estilo e estrutura dos atributos `[AddComponentMenu]` e `[CreateAssetMenu]`.


## 0.1.16 — 2025-09-01
* **Novo:** adição do menu **Thinklib → Point and Click**, com categorias de componentes para **Dropzone**, **Graph** e **Inventory**:
  * Scripts atualizados com `AddComponentMenu` / `CreateAssetMenu` para aparecerem no menu **Component → Thinklib → Point and Click → …**.
  * Organização interna dos scripts em subpastas lógicas (Dropzone, Graph e Inventory).
* **UX:** padronização de nomes de menu conforme os arquivos originais, garantindo consistência entre código e interface.
* **Chore:** pequenas revisões no código para unificação do estilo e estrutura dos atributos `[AddComponentMenu]` e `[CreateAssetMenu]`.


## 0.1.15 — 2025-08-15
* **Style:** melhorias de estilização no `README.md` para torná-lo mais agradável e informativo para o usuário.

## 0.1.14 — 2025-08-15
* **Style:** melhorias de estilização no `README.md` para torná-lo mais agradável e informativo para o usuário.

## 0.1.13 — 2025-08-15
* **Style:** melhorias de estilização no `README.md` para torná-lo mais agradável e informativo para o usuário.
* **Chore:** remoção de arquivos `.meta` desnecessários fora da pasta `Assets/`.

## 0.1.12 — 2025-08-15
* **Style:** melhorias de estilização no `README.md` para torná-lo mais agradável e informativo para o usuário.
* **Chore:** remoção de arquivos `.meta` desnecessários fora da pasta `Assets/`.

## 0.1.11 — 2025-08-15
* **Style:** melhorias de estilização no `README.md` para torná-lo mais agradável e informativo para o usuário.
* **Chore:** remoção de arquivos `.meta` desnecessários fora da pasta `Assets/`.

## 0.1.10 — 2025-08-15
* **Style:** melhorias de estilização no `README.md` para torná-lo mais agradável e informativo para o usuário.
* **Chore:** remoção de arquivos `.meta` desnecessários fora da pasta `Assets/`.

## 0.1.9 — 2025-08-15
* **Style:** melhorias de estilização no `README.md` para torná-lo mais agradável e informativo para o usuário.
* **Chore:** remoção de arquivos `.meta` desnecessários fora da pasta `Assets/`.

## 0.1.8 — 2025-08-15
* **Chore:** ajustes de `.workflows`.

## 0.1.7 — 2025-08-15
* **Style:** melhorias de estilização no `README.md` para torná-lo mais agradável e informativo para o usuário.
* **Chore:** remoção de arquivos `.meta` desnecessários fora da pasta `Assets/`.

## 0.1.6 — 2025-08-15
* **Fix:** resolução de problemas de missing scripts em novos prefabs.

## 0.1.5 — 2025-08-15

* **Novo:** adicionados mais **prefabs** de inimigos:
  * **Patroller** (patrulheiro)
  * **Shooter** (atirador)
  * **Patrol** (variação de patrulha)
  * **Sniper** (franco-atirador)
* Prefabs seguem o padrão de organização e referências internas da biblioteca, compatíveis com o sistema de importação via **Thinklib → Import Resources**.

## 0.1.4 — 2025-08-13

* **Novo:** menu **Thinklib → Import Resources** que importa os **prefabs** disponibilizados pela lib para `Assets/Thinklib/Resources/Prefabs` (somente `.prefab`, dependências continuam apontando para os assets do pacote).
* **Change:** reorganização dos prefabs em `Runtime/Resources/Prefabs/...` e atualização das referências internas para usar assets do próprio pacote (sprites, materials, anims etc.).
* **Fix:** eliminação de conflitos de **GUID** e de **Missing (Mono Script)** ao importar recursos — o importador ignora `.cs/.asmdef/.asmref`, evitando duplicar código no `Assets/`.
* **DX:** importação silenciosa (sem diálogos), com log simples no Console indicando a quantidade de prefabs importados.

## 0.1.3 — 2025-08-12

* **Fix:** erros de compilação (**CS1671**) corrigidos ao mover atributos (`AddComponentMenu`, `RequireComponent` etc.) **para dentro** dos `namespace` nos scripts:

  * `Runtime/Platformer/Core/ProjectileShooterBase.cs`
  * `Runtime/Platformer/Enemy/Types/Shooter/EnemyShooterAI.cs`
  * `Runtime/Topdown/Combat/ProjectileTopdownShooterBase.cs`
  * `Runtime/Topdown/Enemy/Types/Shooter/TopdownEnemyShooterAI.cs`
* **Fix:** ajustes nos **asmdefs**:

  * `com.thinklib.core` (Runtime) **não** referencia o assembly de Editor.
  * `com.thinklib.core.Editor` com `includePlatforms: ["Editor"]` e referência ao Runtime.
  * Resolve aviso/erro do Burst sobre `com.thinklib.core.Editor`.
* **Chore:** limpeza de `.meta` órfãos e pastas de **Editor** fora do lugar sob `Runtime/`.
* **Fix:** estabilidade de prefabs (remoção/ajuste de referências aninhadas ausentes).
* **UX:** componentes Thinklib aparecendo corretamente no menu **Component → Thinklib → …**.

## 0.1.2 — 2025-08-12 *(deprecated)*

* **IMPORTANTE:** versão descontinuada por conter erros de namespace que impediam a compilação em alguns projetos.
* **Novo:** menu **Component → Thinklib → …** para adicionar componentes rapidamente.
* Adicionados `AddComponentMenu` nos scripts principais para aparecerem no menu *Component*.
* **Editor:** ajustes de UX nos *inspectors* (rótulos padronizados, seções agrupadas, avisos).
* **Chore:** limpeza de *namespaces* e referências de asmdefs.

## 0.1.1 — 2025-08-11

* **Release funcional; 0.1.0 marcado como *deprecated*.**
* Corrigida instalação via UPM (Git URL) com `package.json` válido e metadados.
* Scripts de Editor realocados para `Editor/` (root) garantindo separação de compilação.
* Geradores de assets/prefabs agora salvam em `Assets/Thinklib/...` (evita erro de pasta imutável em `Packages/`).
* **Inspectors personalizados:**

  * Platformer: `EnemyShooterAI`
  * Topdown: `TopdownEnemyShooterAI`
* Estabilidade nos criadores de Animator Controllers e na auto-criação de pastas.

## 0.1.0 — 2025-08-10 *(deprecated)*

* Initial public release
* Package structure (Runtime/Editor) with asmdefs
* Animator Controller creators:

  * Platformer: Player & Enemy
  * Topdown: Player & Enemy (2D Blend Trees)
* Point & Click menu items: Item and Combination Recipe (ScriptableObjects)
* Auto-creation of asset folders under `Assets/Thinklib/...`
* Dependencies via UPM: TextMesh Pro, UGUI
