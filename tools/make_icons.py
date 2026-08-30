"""Генерирует иконку контакта (256px) и thumbnail (512px) для мода NY Taxi."""
from PIL import Image, ImageDraw, ImageFont

YELLOW = (247, 197, 30, 255)
BLACK = (25, 25, 25, 255)
WHITE = (245, 245, 245, 255)


def font(size: int) -> ImageFont.FreeTypeFont:
    return ImageFont.truetype("/System/Library/Fonts/Helvetica.ttc", size)


def draw_icon(size: int, title: str) -> Image.Image:
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    pad = size // 32
    d.ellipse([pad, pad, size - pad, size - pad], fill=YELLOW)
    # Шашечная лента через центр
    cell = size // 10
    band_top = size // 2 - cell
    for row in range(2):
        for col in range(size // cell + 1):
            x0 = col * cell
            y0 = band_top + row * cell
            color = BLACK if (row + col) % 2 == 0 else WHITE
            d.rectangle([x0, y0, x0 + cell, y0 + cell], fill=color)
    # Обрезаем всё по кругу
    mask = Image.new("L", (size, size), 0)
    ImageDraw.Draw(mask).ellipse([pad, pad, size - pad, size - pad], fill=255)
    img.putalpha(mask)
    d = ImageDraw.Draw(img)
    d.ellipse([pad, pad, size - pad, size - pad], outline=BLACK,
              width=max(2, size // 48))
    # Подпись
    f = font(size // 5)
    text_box = d.textbbox((0, 0), title, font=f)
    tw = text_box[2] - text_box[0]
    d.text(((size - tw) / 2, size * 0.60), title, font=f, fill=BLACK)
    return img


if __name__ == "__main__":
    base = "sdk/Assets/Mods/NYTaxi"
    draw_icon(256, "TAXI").save(f"{base}/nytaxi-contactname.png")
    draw_icon(512, "TAXI").save(f"{base}/thumbnail.png")
    print("icons written")
