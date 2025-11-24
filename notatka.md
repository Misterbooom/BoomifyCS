

Rozbudowany przewodnik po właściwościach CSS tła z przykładami i wyjaśnieniem wartości.

---

## background-color
**Opis:** Ustawia kolor tła.

**Przykładowe wartości:**
- `lightblue` – jasnoniebieski kolor
- `red` – czerwony kolor
- `rgba(255,0,0,0.5)` – czerwony z przezroczystością 50%

```css
background-color: lightblue;
```
<div style="background-color: lightblue; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

```css
background-color: rgba(255,0,0,0.5);
```
<div style="background-color: rgba(255,0,0,0.5); height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

---

## background-image
**Opis:** Ustawia obraz lub gradient jako tło.

**Przykładowe wartości:**
- `url('image.jpg')` – obraz z URL
- `linear-gradient(red, yellow)` – gradient liniowy od czerwonego do żółtego
- `radial-gradient(circle, blue, green)` – gradient radialny od niebieskiego do zielonego

```css
background-image: url('https://picsum.photos/300/100');
```
<div style="background-image: url('https://picsum.photos/300/100'); background-size: cover; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

```css
background-image: linear-gradient(rgba(255,0,0,0.5), rgba(0,0,255,0.5));
```
<div style="background-image: linear-gradient(rgba(255,0,0,0.5), rgba(0,0,255,0.5)); height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

---

## background-repeat
**Opis:** Określa powtarzanie obrazu.

**Wartości:**
- `repeat` – powtarza obraz w poziomie i pionie (domyślna)
- `no-repeat` – obraz nie jest powtarzany
- `repeat-x` – powtarza tylko w poziomie
- `repeat-y` – powtarza tylko w pionie

```css
background-repeat: no-repeat;
```
<div style="background-image: url('https://picsum.photos/50'); background-repeat: no-repeat; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

```css
background-repeat: repeat-x;
```
<div style="background-image: url('https://picsum.photos/50'); background-repeat: repeat-x; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

---

## background-position
**Opis:** Określa położenie obrazu tła.

**Wartości:**
- `center` – środek elementu
- `top left`, `top right`, `bottom left`, `bottom right` – rogi elementu
- Można użyć procentów lub pikseli np. `50% 50%` lub `20px 30px`

```css
background-position: center;
```
<div style="background-image: url('https://picsum.photos/100'); background-position: center; background-repeat:no-repeat; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

```css
background-position: top right;
```
<div style="background-image: url('https://picsum.photos/100'); background-position: top right; background-repeat:no-repeat; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

---

## background-size
**Opis:** Określa rozmiar obrazu tła.

**Wartości:**
- `cover` – wypełnia cały element, obraz może być przycięty
- `contain` – obraz zmieści się w całości, może zostać wolna przestrzeń
- `auto` – oryginalny rozmiar
- Można też podać np. `100px 50px`

```css
background-size: cover;
```
<div style="background-image: url('https://picsum.photos/200'); background-size: cover; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

```css
background-size: contain;
```
<div style="background-image: url('https://picsum.photos/200'); background-size: contain; background-repeat:no-repeat; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

---

## background-attachment
**Opis:** Określa, czy tło przewija się ze stroną.

**Wartości:**
- `scroll` – tło przewija się razem z zawartością (domyślne)
- `fixed` – tło pozostaje nieruchome podczas przewijania strony
- `local` – tło przewija się tylko wewnątrz elementu przewijalnego

```css
background-attachment: fixed;
```
<div style="background-image: url('https://picsum.photos/300'); background-attachment: fixed; background-size: cover; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

```css
background-attachment: local;
```
<div style="overflow:auto; height:100px; background-image: url('https://picsum.photos/300'); background-attachment: local; background-size: cover; border-radius:12px; margin:10px 0;"></div>

---

## background-clip
**Opis:** Określa obszar wyświetlania tła.

**Wartości:**
- `border-box` – tło obejmuje także obszar obramowania
- `padding-box` – tło obejmuje obszar paddingu i treści
- `content-box` – tło tylko w obszarze treści

```css
background-clip: content-box;
```
<div style="padding:20px; background-image:url('https://picsum.photos/200'); background-clip: content-box; border:10px solid black; height:100px; border-radius:12px; margin:10px 0;"></div>

---

## background-origin
**Opis:** Punkt odniesienia dla położenia obrazu tła.

**Wartości:**
- `border-box` – odniesienie do obramowania
- `padding-box` – odniesienie do paddingu
- `content-box` – odniesienie do treści

```css
background-origin: border-box;
```
<div style="padding:20px; border:10px solid black; background-image:url('https://picsum.photos/200'); background-origin: border-box; height:100px; border-radius:12px; margin:10px 0;"></div>

---

## background-blend-mode
**Opis:** Określa sposób mieszania koloru tła z obrazem.

**Wartości:**
- `multiply` – mnoży kolory tła i obrazu
- `screen` – jaśniejsze połączenie kolorów
- `overlay` – połączenie zachowujące kontrast

**Przykład:** czerwony kolor tła z obrazem w trybie multiply
```css
background-color: red;
background-image: url('https://picsum.photos/300');
background-blend-mode: multiply;
```
<div style="background-image: url('https://picsum.photos/300'); background-color: red; background-blend-mode: multiply; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

**Przykład:** niebieski kolor tła z obrazem w trybie screen
```css
background-color: blue;
background-image: url('https://picsum.photos/300');
background-blend-mode: screen;
```
<div style="background-image: url('https://picsum.photos/300'); background-color: blue; background-blend-mode: screen; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

---

## Skrót background
**Opis:** Ustawia wszystkie właściwości tła jednocześnie.

```css
background: url('https://picsum.photos/200') no-repeat center/cover red;
```
<div style="background: url('https://picsum.photos/200') no-repeat center/cover red; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>

---

## Multiple backgrounds
**Opis:** Nakładanie kilku obrazów tła naraz.

```css
background-image: url('https://picsum.photos/100'), url('https://picsum.photos/50');
background-position: left top, right bottom;
background-repeat: no-repeat, repeat;
```
<div style="background-image: url('https://picsum.photos/100'), url('https://picsum.photos/50'); background-position:left top, right bottom; background-repeat:no-repeat, repeat; height:100px; width:100%; border-radius:12px; margin:10px 0;"></div>
