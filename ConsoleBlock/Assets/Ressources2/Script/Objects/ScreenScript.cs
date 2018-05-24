using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScreenScript : WInteractable {

    private const char EmptyChar = '\0';
    public Material ScreenMaterial;
    public MeshRenderer meshRenderer;
    public Texture2D display;

    public bool MouseDown = false;
    public bool MousePress = false;
    public bool MouseUp = false;
    public Vector2 TouchUVs;
    public Color CurrentColor = Color.black;
    int TextCursorX = 0;
    int TextCursorY = 0;

    private void Start () {
        InitScreen();

        GlobalVariable.Add(new Variable("ScreenWidth", VariableType.v_int, 96, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("ScreenHeight", VariableType.v_int, 72, new VariableParameters(true, VariableAccessType.v_readonly)));

        GlobalVariable.Add(new Variable("TactileTouchX", VariableType.v_int, 0, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("TactileTouchY", VariableType.v_int, 0, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("TactileTouchDown", VariableType.v_bool, false, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("TactileTouchPress", VariableType.v_bool, false, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("TactileTouchUp", VariableType.v_bool, false, new VariableParameters(true, VariableAccessType.v_readonly)));

        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("Fill", new List<VariableTemplate>() {
            new VariableTemplate("ColorR", VariableType.v_float),
            new VariableTemplate("ColorG", VariableType.v_float),
            new VariableTemplate("ColorB", VariableType.v_float)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("SetColor", new List<VariableTemplate>() {
            new VariableTemplate("ColorR", VariableType.v_float),
            new VariableTemplate("ColorG", VariableType.v_float),
            new VariableTemplate("ColorB", VariableType.v_float)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("SetTextCursor", new List<VariableTemplate>() {
            new VariableTemplate("CursorX", VariableType.v_int),
            new VariableTemplate("CursorY", VariableType.v_int)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("DrawText", new List<VariableTemplate>() {
            new VariableTemplate("Text", VariableType.v_string)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("DrawPixel", new List<VariableTemplate>() {
            new VariableTemplate("X", VariableType.v_int),
            new VariableTemplate("Y", VariableType.v_int)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("DrawFilledRect", new List<VariableTemplate>() {
            new VariableTemplate("X", VariableType.v_int),
            new VariableTemplate("Y", VariableType.v_int),
            new VariableTemplate("X2", VariableType.v_int),
            new VariableTemplate("Y2", VariableType.v_int)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("DrawRect", new List<VariableTemplate>() {
            new VariableTemplate("X", VariableType.v_int),
            new VariableTemplate("Y", VariableType.v_int),
            new VariableTemplate("X2", VariableType.v_int),
            new VariableTemplate("Y2", VariableType.v_int)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("DrawLine", new List<VariableTemplate>() {
            new VariableTemplate("X", VariableType.v_int),
            new VariableTemplate("Y", VariableType.v_int),
            new VariableTemplate("X2", VariableType.v_int),
            new VariableTemplate("Y2", VariableType.v_int)
        }));
    }

    override public void Update () {
        for(int i = 0; i < FunctionCall.Count; i++) {
            FunctionCaller fc = FunctionCall[0];
            FunctionCall.RemoveAt(0);
            int pr = i;
            i = 0;
            if(fc.Name == "Fill") {
                Color color = new Color {
                    r = (float)fc.parameters[0].source,
                    g = (float)fc.parameters[1].source,
                    b = (float)fc.parameters[2].source,
                    a = 1f
                };

                Color[] colors = display.GetPixels();
                for(int c = 0; c < colors.Length; c++) {
                    colors[c] = color;
                }

                display.SetPixels(colors);
                UpdateScreen();
            } else
            if(fc.Name == "SetColor") {
                CurrentColor = new Color {
                    r = (float)fc.parameters[0].source,
                    g = (float)fc.parameters[1].source,
                    b = (float)fc.parameters[2].source,
                    a = 1f
                };
            } else
            if(fc.Name == "SetTextCursor") {
                TextCursorX = (int)fc.parameters[0].source;
                TextCursorY = (int)fc.parameters[1].source;
            } else
            if(fc.Name == "DrawText") {
                Color[] colors = display.GetPixels();
                string text = (string)fc.parameters[0].source;
                for(int t = 0; t < text.Length; t++) {
                    byte b = (byte)(Converter.IndexOf(char.ToUpper(text[t])));
                    if(b > 127) {
                        b = 0;
                    }
                    int readb = 2 + b * 3;

                    byte width = BitmapFont[0];
                    byte height = BitmapFont[1];

                    for(int x = 0; x < width; x++) {
                        for(int y = 0; y < height; y++) {
                            if((BitmapFont[readb + x] >> y & 1) == 1) {
                                if((TextCursorX + x + (width + 1) * t) < display.width) {
                                    colors[(TextCursorX + x + (width + 1) * t) + (display.height - (TextCursorY + y)) * display.width] = CurrentColor;
                                }
                            }
                        }
                    }
                }
                display.SetPixels(colors);
                UpdateScreen();
            } else
            if(fc.Name == "DrawPixel") {
                display.SetPixel(
                    (int)fc.parameters[0].source,
                    display.height-(int)fc.parameters[1].source,
                    CurrentColor
                );
                UpdateScreen();
            } else
            if(fc.Name == "DrawFilledRect") {
                Color[] colors = display.GetPixels();
                for(int x = (int)fc.parameters[0].source; x < (int)fc.parameters[2].source; x++) {
                    for(int y = (int)fc.parameters[1].source; y < (int)fc.parameters[3].source; y++) {
                        if(x < display.width) {
                            colors[x + (display.height - y) * display.width] = CurrentColor;
                        }
                    }
                }
                display.SetPixels(colors);
                UpdateScreen();
            } else
            if(fc.Name == "DrawRect") {
                Color[] colors = display.GetPixels();
                for(int x = (int)fc.parameters[0].source; x < (int)fc.parameters[2].source; x++) {
                    for(int y = (int)fc.parameters[1].source; y < (int)fc.parameters[3].source; y++) {
                        if(x == (int)fc.parameters[0].source || x == (int)fc.parameters[2].source - 1 || y == (int)fc.parameters[1].source || y == (int)fc.parameters[3].source - 1) {
                            if(x < display.width) {
                                colors[x + (display.height - y) * display.width] = CurrentColor;
                            }
                        }
                    }
                }
                display.SetPixels(colors);
                UpdateScreen();
            } else
            if(fc.Name == "DrawLine") {
                display.SetPixels(DrawLine(
                    (int)fc.parameters[0].source,
                    (int)fc.parameters[1].source,
                    (int)fc.parameters[2].source,
                    (int)fc.parameters[3].source,
                    display.GetPixels(),
                    display.width,
                    display.height,
                    CurrentColor
                ));
                UpdateScreen();
            }
            i = pr;
        }

        int tactileX = Mathf.FloorToInt(TouchUVs.x * display.width);
        int tactileY = display.height - Mathf.FloorToInt(TouchUVs.y * display.height);

        GlobalVariable[2].source = tactileX;
        GlobalVariable[3].source = tactileY;
        GlobalVariable[4].source = MouseDown;
        GlobalVariable[5].source = MousePress;
        GlobalVariable[6].source = MouseUp;

        MouseDown = false;
        MousePress = false;
        MouseUp = false;
    }

    void InitScreen () {
        display = new Texture2D(96, 72);
        display.wrapMode = TextureWrapMode.Clamp;
        display.filterMode = FilterMode.Point;

        meshRenderer.material = new Material(ScreenMaterial);
        meshRenderer.material.mainTexture = display;
        meshRenderer.material.SetTexture("_MainTex",display);
        meshRenderer.material.SetTexture("_EmissionMap", display);
    }

    void UpdateScreen () {
        display.Apply();
    }

    Color[] DrawLine (int x, int y, int x2, int y2, Color[] colors, int width, int height, Color PaintColor) {
        Color[] texture = colors;
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if(w < 0)
            dx1 = -1;
        else if(w > 0)
            dx1 = 1;
        if(h < 0)
            dy1 = -1;
        else if(h > 0)
            dy1 = 1;
        if(w < 0)
            dx2 = -1;
        else if(w > 0)
            dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if(!(longest > shortest)) {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if(h < 0)
                dy2 = -1;
            else if(h > 0)
                dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for(int i = 0; i <= longest; i++) {
            if(x < width) {
                if((x + (height - y) * width) >= 0 && (x + (height - y) * width) < width*height) {
                    texture[x + (height - y) * width] = PaintColor;
                }
            }

            numerator += shortest;
            if(!(numerator < longest)) {
                numerator -= longest;
                x += dx1;
                y += dy1;
            } else {
                x += dx2;
                y += dy2;
            }
        }
        return texture;
    }

    List<char> Converter = new List<char>() {
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        EmptyChar,
        ' ',
        '!',
        '\"',
        '#',
        EmptyChar,
        '%',
        '&',
        '\'',
        '(',
        ')',
        '*',
        '+',
        ',',
        '-',
        '.',
        '/',
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        ':',
        ';',
        '<',
        '=',
        '>',
        '?',
        EmptyChar,
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'P',
        'Q',
        'R',
        'S',
        'T',
        'U',
        'V',
        'W',
        'X',
        'Y',
        'Z',
        '[',
        '\\',
        ']',
        '^',
        '_',
        '`'
    };

    byte[] BitmapFont = new byte[] {
        3,8,
    
        // font data
        0x00, 0x00, 0x00, // 0
        0x00, 0x00, 0x00, // 1
        0x00, 0x00, 0x00, // 2
        0x00, 0x00, 0x00, // 3
        0x00, 0x00, 0x00, // 4
        0x00, 0x00, 0x00, // 5
        0x00, 0x00, 0x00, // 6
        0x00, 0x00, 0x00, // 7
        0x00, 0x00, 0x00, // 8
        0x00, 0x00, 0x00, // 9
        0x00, 0x00, 0x00, // 10
        0x00, 0x00, 0x00, // 11
        0x00, 0x00, 0x00, // 12
        0x00, 0x00, 0x00, // 13
        0x00, 0x00, 0x00, // 14
        0x00, 0x00, 0x00, // 15
        0x00, 0x00, 0x00, // 16
        0x00, 0x00, 0x00, // 17
        0x00, 0x00, 0x00, // 18
        0x00, 0x00, 0x00, // 19
        0x00, 0x00, 0x00, // 20
        0x00, 0x00, 0x00, // 21
        0x00, 0x00, 0x00, // 22
        0x00, 0x00, 0x00, // 23
        0x00, 0x00, 0x00, // 24
        0x00, 0x00, 0x00, // 25
        0x00, 0x00, 0x00, // 26
        0x00, 0x00, 0x00, // 27
        0x00, 0x00, 0x00, // 28
        0x00, 0x00, 0x00, // 29
        0x00, 0x00, 0x00, // 30
        0x00, 0x00, 0x00, // 31
        0x00, 0x00, 0x00, // 32
        0x00, 0x17, 0x00, // 33
        0x03, 0x00, 0x03, // 34
        0x1F, 0x0A, 0x1F, // 35
        0x06, 0x1F, 0x0A, // 36
        0x19, 0x04, 0x13, // 37
        0x1A, 0x15, 0x12, // 38
        0x00, 0x03, 0x00, // 39
        0x0E, 0x11, 0x00, // 40
        0x00, 0x11, 0x0E, // 41
        0x01, 0x02, 0x01, // 42
        0x04, 0x0E, 0x04, // 43
        0x18, 0x00, 0x00, // 44
        0x04, 0x04, 0x04, // 45
        0x10, 0x00, 0x00, // 46
        0x18, 0x04, 0x03, // 47
        0x1F, 0x11, 0x1F, // 48
        0x02, 0x1F, 0x00, // 49
        0x1D, 0x15, 0x17, // 50
        0x15, 0x15, 0x1F, // 51
        0x07, 0x04, 0x1F, // 52
        0x17, 0x15, 0x1D, // 53
        0x1F, 0x15, 0x1D, // 54
        0x01, 0x1D, 0x03, // 55
        0x1F, 0x15, 0x1F, // 56
        0x17, 0x15, 0x1F, // 57
        0x00, 0x12, 0x00, // 58
        0x00, 0x1A, 0x00, // 59
        0x04, 0x0A, 0x0A, // 60
        0x0A, 0x0A, 0x0A, // 61
        0x0A, 0x0A, 0x04, // 62
        0x01, 0x15, 0x03, // 63
        0x0D, 0x11, 0x0F, // 64
        0x1F, 0x05, 0x1F, // 65
        0x1F, 0x15, 0x1B, // 66
        0x1F, 0x11, 0x11, // 67
        0x1F, 0x11, 0x0E, // 68
        0x1F, 0x15, 0x15, // 69
        0x1F, 0x05, 0x05, // 70
        0x1F, 0x11, 0x1D, // 71
        0x1F, 0x04, 0x1F, // 72
        0x11, 0x1F, 0x11, // 73
        0x08, 0x11, 0x0F, // 74
        0x1F, 0x04, 0x1B, // 75
        0x1F, 0x10, 0x10, // 76
        0x1F, 0x06, 0x1F, // 77
        0x1F, 0x01, 0x1F, // 78
        0x1F, 0x11, 0x1F, // 79
        0x1F, 0x05, 0x07, // 80
        0x1F, 0x11, 0x2F, // 81
        0x1F, 0x05, 0x1B, // 82
        0x17, 0x15, 0x1D, // 83
        0x01, 0x1F, 0x01, // 84
        0x1F, 0x10, 0x1F, // 85
        0x0F, 0x10, 0x0F, // 86
        0x1F, 0x0C, 0x1F, // 87
        0x1B, 0x04, 0x1B, // 88
        0x03, 0x1C, 0x03, // 89
        0x19, 0x15, 0x13, // 90
        0x1F, 0x11, 0x00, // 91
        0x03, 0x04, 0x18, // 92
        0x00, 0x11, 0x1F, // 93
        0x02, 0x01, 0x02, // 94
        0x10, 0x10, 0x10, // 95
        0x01, 0x02, 0x00, // 96
        0x00, 0x00, 0x00, // 97
        0x00, 0x00, 0x00, // 98
        0x00, 0x00, 0x00, // 99
        0x00, 0x00, 0x00, // 100
        0x00, 0x00, 0x00, // 101
        0x00, 0x00, 0x00, // 102
        0x00, 0x00, 0x00, // 103
        0x00, 0x00, 0x00, // 104
        0x00, 0x00, 0x00, // 105
        0x00, 0x00, 0x00, // 106
        0x00, 0x00, 0x00, // 107
        0x00, 0x00, 0x00, // 108
        0x00, 0x00, 0x00, // 109
        0x00, 0x00, 0x00, // 110
        0x00, 0x00, 0x00, // 111
        0x00, 0x00, 0x00, // 112
        0x00, 0x00, 0x00, // 113
        0x00, 0x00, 0x00, // 114
        0x00, 0x00, 0x00, // 115
        0x00, 0x00, 0x00, // 116
        0x00, 0x00, 0x00, // 117
        0x00, 0x00, 0x00, // 118
        0x00, 0x00, 0x00, // 119
        0x00, 0x00, 0x00, // 120
        0x00, 0x00, 0x00, // 121
        0x00, 0x00, 0x00, // 122
        0x04, 0x1B, 0x11, // 123
        0x00, 0x1F, 0x00, // 124
        0x11, 0x1B, 0x04, // 125
        0x04, 0x04, 0x04, // 126
        0x00, 0x00, 0x00 // 127
    };
}
