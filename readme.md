# Proyecto Árboles y Grafos - Innovatec

## Descripción
Este proyecto implementa un sistema para administrar la **jerarquía organizativa** y las **rutas internas** del Parque Tecnológico “Innovatec” usando **C# y Windows Forms**.  

- La **jerarquía organizativa** se representa mediante un **árbol general**.
- Las **rutas internas entre edificios** se representan mediante un **grafo no dirigido y ponderado**.
- Se incluyen funcionalidades de recorrido (BFS y DFS), búsqueda, conteo de niveles y cálculo de la ruta más corta (Dijkstra).

---

## Funcionalidades

### Árbol
- Insertar nodos en la jerarquía.
- Recorrido **BFS** (Breadth-First Search).
- Recorrido **DFS** (Depth-First Search).
- Conteo de hijos de cada nodo.
- Búsqueda de nodos.

### Grafo
- Agregar edificios (vértices) y conexiones (aristas con peso).
- Ver conexiones de cada edificio.
- Verificar si el grafo es conexo.
- Calcular la ruta más corta entre dos edificios usando el **algoritmo de Dijkstra**.

---

## Instalación

1. Clonar el repositorio:
```bash
git clone https://github.com/Kayro232/inatec_proyecto.git
