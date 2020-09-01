# ThaiText
 **Unity Text UI for Thai language**<br>
 I combined all useful tools (Thai lexeme tokenizer & Thai font adjuster) and support Rich Text.<br>
 Thai Text is inherited from Unity UI.Text and some features are also come from these packages.

>**Rich Text**<br>
 The text for UI elements and text meshes can incorporate multiple font styles and sizes.<br>
 More information: [StyledText](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html)

## How it work?

### 1. LexTo

**Lexto** is Thai lexeme tokenizer. It's helpful for Thai text wrapping; automatic line-breaks.
It requires **lexitron.txt** as Thai dictionary to work.

### 2. lexitron.txt

**lexitron.txt** is text file to contain Thai words for tokenization.
This file must be in Resources folder, so it can be loaded.
Normally, it should be at **[Assets/DGN/ThaiText/Resources]**.
    
>Note: **lexitron.txt** must be encoded that supports Thai characters. Default encoding is UTF-16LE

You can edit **lexitron.txt** file by
- adding new word in new line
- delete word
- add # at the start of word to skip this word

### 3. Lexitron Watcher
    
**Lexitron Watcher** is a file system watcher for **lexitron.txt** file.
- If **lexitron.txt** is not existed in Resources folder, it will import at **[Assets/DGN/ThaiText/Resources]**
- If **lexitron.txt** has been changed, it will update dictionary in Lexto.

### 4. Thai Font Adjuster
    
**Thai Font Adjuster** will adjust Thai fonts to be rendered almost correctly.
Position of tone mark, upper vowel and lower vowel would be adjusted by surrounding characters.

**Limitation**
- ThaiFontAdjuster only can handle special fonts.
  - Font should have extended glyphs from U+F700 to U+F71A providing various position of Thai characters.
  - [NECTEC National Fonts](http://www.nectec.or.th/pub/review-software/font/national-fonts.html) (Garuda, Loma, Kinnari, Norasi) already provide it.
  - Modified NotoSansThai containing extended glyphs is included in this package.
  - If you want to use another font, check it contains extended glyphs, otherwise glyphs have to be added to font by yourself.

- Adjusting position is not same as true-type rendered one with GPOS and GSUB support.
  - This library adjusts position of glyph at best with limited extended set of character.
		But without GPOS, ideal positioning is impossibe.

 ### 5. Tag String Parser
**Tag String Parser** is used to separate markup tags for support Rich Text feature.

## Special Thanks
- [LexTo](http://www.sansarn.com/lexto/) by Choochart Haruechaiyasak
- [Thai Font Adjuster](https://github.com/SaladLab/Unity3D.ThaiFontAdjuster) by SaladLab
- Tag String Parser (sorry I lost the source of these scripts, will update when found it)
- Unity UI by Unity
