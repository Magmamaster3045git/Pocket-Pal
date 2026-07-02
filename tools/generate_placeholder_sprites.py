#!/usr/bin/env python3
"""
Generates placeholder pixel-art sprite frames for Pocket Pal.

Draws a tiny blob-critter on a small grid, then upscales with nearest-
neighbor sampling for a crisp retro look. Output goes to
PocketPal/Assets/Sprites/<AnimationName>/frame_000.png, frame_001.png, ...

This is throwaway art meant only to make the project runnable and
testable immediately. Swap any folder's PNGs with real art at any
time - the C# AssetLoader does not care how many frames exist or what
they look like, only that frame_*.png files are present.
"""

import os
from PIL import Image, ImageDraw

GRID = 24          # logical pixel-art canvas size (square)
SCALE = 4           # upscale factor -> 96x96 output frames
OUT_ROOT = os.path.join(os.path.dirname(__file__), "..", "PocketPal", "Assets", "Sprites")

BODY_COLOR = (255, 176, 59, 255)      # warm orange blob
BODY_SHADE = (219, 140, 38, 255)
EYE_COLOR = (40, 30, 20, 255)
OUTLINE = (60, 40, 20, 255)


def new_canvas():
    return Image.new("RGBA", (GRID, GRID), (0, 0, 0, 0))


def draw_blob(draw, cx, cy, w, h, squash=0.0):
    """Draws the critter's rounded body centered at (cx, cy)."""
    hw = w / 2
    hh = (h / 2) * (1 - squash)
    draw.ellipse([cx - hw, cy - hh, cx + hw, cy + hh], fill=BODY_COLOR, outline=OUTLINE)
    # simple shading crescent
    draw.ellipse([cx - hw * 0.5, cy - hh * 0.6, cx + hw * 0.9, cy + hh * 0.2], fill=BODY_SHADE)


def draw_eyes(draw, cx, cy, open_amount=1.0, look_dx=0):
    ex = 3.0
    ey = 2.0 if open_amount > 0.3 else 0.6
    for side in (-1, 1):
        x = cx + side * ex + look_dx
        draw.ellipse([x - 1.2, cy - ey, x + 1.2, cy + ey], fill=EYE_COLOR)


def draw_legs(draw, cx, cy, spread, lift_left, lift_right):
    for side, lift in ((-1, lift_left), (1, lift_right)):
        x = cx + side * spread
        y0 = cy - lift
        draw.rectangle([x - 1.5, y0, x + 1.5, y0 + 4], fill=BODY_SHADE, outline=OUTLINE)


def save(img, name, index):
    folder = os.path.join(OUT_ROOT, name)
    os.makedirs(folder, exist_ok=True)
    big = img.resize((GRID * SCALE, GRID * SCALE), Image.NEAREST)
    big.save(os.path.join(folder, f"frame_{index:03d}.png"))


def gen_idle():
    for i in range(4):
        img = new_canvas()
        d = ImageDraw.Draw(img)
        bob = [0, -1, 0, 1][i]
        draw_legs(d, 12, 19, 5, 0, 0)
        draw_blob(d, 12, 13 + bob, 14, 12)
        draw_eyes(d, 12, 11 + bob, open_amount=1.0 if i != 2 else 0.2)
        save(img, "Idle", i)


def gen_walk(direction):
    flip = direction == "Left"
    for i in range(4):
        img = new_canvas()
        d = ImageDraw.Draw(img)
        phase = [0, 3, 0, -3][i]
        draw_legs(d, 12, 19, 5, max(0, phase), max(0, -phase))
        draw_blob(d, 12, 13, 14, 12)
        draw_eyes(d, 12, 11, look_dx=(-2 if flip else 2))
        if flip:
            img = img.transpose(Image.FLIP_LEFT_RIGHT)
        save(img, f"Walk_{direction}", i)


def gen_run(direction):
    flip = direction == "Left"
    for i in range(4):
        img = new_canvas()
        d = ImageDraw.Draw(img)
        phase = [0, 5, 0, -5][i]
        lean = 1
        draw_legs(d, 12 - lean, 19, 6, max(0, phase), max(0, -phase))
        draw_blob(d, 12 - lean, 12, 15, 11, squash=0.05)
        draw_eyes(d, 12 - lean, 10, look_dx=(-2 if flip else 3))
        if flip:
            img = img.transpose(Image.FLIP_LEFT_RIGHT)
        save(img, f"Run_{direction}", i)


def gen_sit():
    for i in range(2):
        img = new_canvas()
        d = ImageDraw.Draw(img)
        draw_legs(d, 12, 20, 6, 0, 0)
        draw_blob(d, 12, 16, 16, 10, squash=0.1)
        draw_eyes(d, 12, 14, open_amount=1.0 if i == 0 else 0.15)
        save(img, "Sit", i)


def gen_sleep():
    for i in range(2):
        img = new_canvas()
        d = ImageDraw.Draw(img)
        draw_blob(d, 12, 18, 16, 8, squash=0.15)
        draw_eyes(d, 12, 16, open_amount=0.05)
        if i == 1:
            d.text((16, 4), "z", fill=EYE_COLOR)
        save(img, "Sleep", i)


def gen_jump():
    poses = [
        dict(cy=15, h=10, squash=0.25, legspread=4, liftl=0, liftr=0),   # crouch
        dict(cy=10, h=14, squash=-0.1, legspread=5, liftl=3, liftr=3),   # launch/extend
        dict(cy=8, h=12, squash=0.0, legspread=4, liftl=2, liftr=2),     # peak
    ]
    for i, p in enumerate(poses):
        img = new_canvas()
        d = ImageDraw.Draw(img)
        draw_legs(d, 12, p["cy"] + 6, p["legspread"], p["liftl"], p["liftr"])
        draw_blob(d, 12, p["cy"], 14, p["h"], squash=p["squash"])
        draw_eyes(d, 12, p["cy"] - 2)
        save(img, "Jump", i)


def gen_fall():
    for i in range(2):
        img = new_canvas()
        d = ImageDraw.Draw(img)
        stretch = 2 + i
        draw_legs(d, 12, 18 + i, 5, 1, 1)
        draw_blob(d, 12, 10, 13, 11 + stretch)
        draw_eyes(d, 12, 8, open_amount=1.0)
        save(img, "Fall", i)


def main():
    gen_idle()
    gen_walk("Left")
    gen_walk("Right")
    gen_run("Left")
    gen_run("Right")
    gen_sit()
    gen_sleep()
    gen_jump()
    gen_fall()
    print("Placeholder sprites generated under:", os.path.abspath(OUT_ROOT))


if __name__ == "__main__":
    main()
