# Geometrix (C# Windows Forms)

[RU] **Geometrix** — это аркадная логическая игра на языке C#, написанная с использованием технологии **Windows Forms (.NET)**. Игрок должен укладывать падающие геометрические фигуры (жетоны) на игровое поле, заполняя сетку.

[PL] **Geometrix** to zręcznościowa gra logiczna napisana w języku C# przy użyciu technologii **Windows Forms (.NET)**. Zadaniem gracza jest układanie spadających figur geometrycznych (żetonów) na planszy będącej siatką.

> 🤖 **Примечание о разработке / Uwaga dotycząca tworzenia:**
> 
> **RU:** Данный проект и весь его программный код были целиком сгенерированы с помощью **AI (Large Language Model)** по пошаговому текстовому запросу (промпту) с четким описанием архитектуры, правил игры и механики управления.
> 
> **PL:** Ten projekt oraz cały jego kod źródłowy zostały w całości wygenerowane przez **Sztuczną Inteligencję (AI)** na podstawie szczegółowego promptu opisującego strukturę, reguły gry oraz mechanikę sterowania.

---

## 🇵🇱 Opis w języku polskim

### 🎮 Mechanika i reguły gry

1. **Plansza:**
   * Prostokątna siatka o wymiarach **20 × 20** jednostek (rozmiar pojedynczej komórki to `40 × 40 px`).
   * Tło w kolorze jasno-brązowym z naniesioną siatką przerywaną.
   * Lewy, prawy i dolny bok stanowią krawędzie zamykające planszę. Górny bok jest **otwarty** — żetony pojawiają się nad nim i płynnie opadają w dół.

2. **Żetony (Figury geometryczne):**
   * Generowane losowo z wirtualnego repozytorium obiektów (`TokenRepository`).
   * Zestaw zawiera 5 rodzajów żetonów:
     - Kwadrat (2 × 2)
     - Prostokąt 2 × 1
     - Prostokąt 3 × 1
     - Prostokąt 2 × 3
     - Prostokąt 2 × 3 z usuniętymi przeciwległymi kwadratami jednostkowymi (kształt Z / S)

3. **Sterowanie:**
   * `←` / `→` — przesuwanie żetonu w lewo lub w prawo (z blokadą przekroczenia krawędzi planszy).
   * `↑` — obrót żetonu o **90° w prawo**.
   * `↓` — obrót żetonu o **90° w lewo**.

4. **Warunek końca gry:**
   * Gra kończy się w momencie, gdy unieruchomiony żeton choć częściowo wystaje ponad górną krawędź planszy.

---

## 🇷🇺 Описание на русском языке

### 🎮 Механика и правила игры

1. **Игровое поле:**
   * Сетка размером **20 × 20** клеток (размер одной клетки — `40 × 40 px`).
   * Светло-коричневый фон с пунктирной разметкой.
   * Левая, правая и нижняя границы закрыты. Верхняя граница **открыта** — фигуры появляются над ней и плавно опускаются вниз.

2. **Фигуры (Жетоны):**
   * Генерируются случайным образом из виртуального репозитория объектов (`TokenRepository`).
   * Доступно 5 типов фигур:
     - Kwadrat (Квадрат 2x2)
     - Prostokąt 2x1 (Прямоугольник 2x1)
     - Prostokąt 3x1 (Прямоугольник 3x1)
     - Prostokąt 2x3 (Прямоугольник 2x3)
     - Prostokąt 2x3 bez narożników (Z / S-форма)

3. **Управление:**
   * `←` / `→` — перемещение фигуры влево или вправо (без выхода за края поля).
   * `↑` — поворот фигуры на **90° вправо** (по часовой стрелке).
   * `↓` — поворот фигуры на **90° влево** (против часовой стрелки).

4. **Условия окончания игры:**
   * Игра завершается, если зафиксированная фигура хотя бы частично выходит за пределы верхней границы поля.

---

## 🛠 Technologia / Технологии

* **Język / Язык:** C#
* **Framework:** .NET / Windows Forms
* **Grafika / Графика:** `System.Drawing` (GDI+ custom rendering / `PaintEventArgs`)
* **Struktura / Структура:** `Form1.cs`, `TokenRepository`, `System.Windows.Forms.Timer`

---

## 🚀 Jak uruchomić / Как запустить

1. Klonuj repozytorium / Склонируйте репозиторий:
   ```bash
   git clone [https://github.com/YOUR_USERNAME/Geometrix.git](https://github.com/YOUR_USERNAME/Geometrix.git)