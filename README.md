# Bullet Hell Shooter



**Resumen**
Este es un proyecto escolar enfocado en realizar un nivel de videojuego de la categoría bullet hell shooter. Es un nivel completo en dónde se tienen tres oleadas de enemigos pequeños y un jefe final con varios tipos de ataques y movimientos.

Estaré llendo al nivel *lunático* para evaluar (espero les guste :D).

## 🎮 Video de demostración  
https://drive.google.com/file/d/1tg9TgB1qCMUe_ZsnhtdXUrN62-V-MoMP/view?usp=sharing

# Dinámicas clave

## Enemigos (minions)
- **Oleadas controladas:** `WaveManager` genera secuencias completas de enemigos con tiempo de aparición, cantidad y control del total de vida restante.
- **Movimientos oscilatorios:** cada minion utiliza un `MovementPattern` (lineal, senoidal o circular), lo que genera trayectorias con zigzags, ondas y curvas.
- **Disparo variado:** `MinionWeapon` soporta disparos rectos, diagonales y aleatorios, todos con temporizadores y velocidades configurables.

## Jefe (boss)
- **Entrada coreografiada:** el jefe entra con un movimiento fluido que mezcla desplazamiento y oscilación hasta alcanzar su posición principal.
- **Fases de combate:** `BossWeapon` alterna entre patrones como círculos, espirales, flores y ataques aleatorios. Cada fase cambia ritmo, timing y densidad.
- **Coordinación con oleadas:** algunas fases lanzan oleadas de minions y el jefe espera a que concluyan antes de continuar con sus ataques.
- **Movilidad circular y estados dinámicos:** `BossMovement` permite que el jefe trace círculos y patrones suaves mientras dispara.

## Jugador
- **Control preciso:** `PlayerMovement` permite moverse libremente y activar un modo de precisión para esquivar con movimientos más finos.
- **Disparo múltiple:** `PlayerShooting` usa múltiples fire points y un sistema de control de cadencia.
- **Supervivencia y feedback:** `PlayerHealth` implementa invulnerabilidad temporal (i-frames) y parpadeo visual después de recibir daño.

## Proyectiles y colisiones
- **Tipos diferenciados:** `PlayerProjectile`, `EnemyProjectile` y `MinionProjectile`, cada uno con su propio comportamiento.
- **Daño por contacto:** `ContactDamage` gestiona colisiones, knockback y cooldown individual para evitar daño duplicado.
- **Optimización:** `DestroyOffScreen` elimina proyectiles cuando salen del área de juego.

## UI y audio
- **UIManager:** actualiza barras de vida del jugador, jefe y mensajes de victoria/derrota.
- **AudioManager:** reproduce efectos y música, mientras `StageManager` coordina la transición entre gameplay, jefe y final.

# 📂 Estructura del proyecto (resumen)
- `Scripts/Patterns` — Lógica de patrones de ataque y movimiento.  
- `Scripts/Managers` — WaveManager, StageManager, UIManager, AudioManager.  
- `Scripts/Characters` — Boss, Minions, Player.  
- `Scripts/Projectiles` — Proyectiles del juego.  
- `Scripts/Utils` — Destrucción fuera de pantalla, límites, daño por contacto. 

# ⌨️ Controles
- Movimiento: `W/A/S/D`  
- Modo precisión: `Shift`  
- Disparo (solo en modo de precisión): `Espacio`  