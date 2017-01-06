
#include <stdio.h>

#include <ft2build.h>
#include FT_FREETYPE_H

FT_Library library;
FT_Error error;
FT_Face face;

int main(int argc, char *argv[])
{
	// init library
	error = FT_Init_FreeType(&library);
	if (error) {
		printf("error|freetype-initiation|code:%d", error);
		exit(1);
	}

	// load font
	error = FT_New_Face(library, "D:\\dev\\UndefinedProject\\be-output\\verdana.ttf", 0, &face);
	if (error) {
		if (error == FT_Err_Unknown_File_Format)
		{
			printf("error|load-font|unknown-file-format|code:%d", error);
		}
		else
		{
			printf("error|load-font|undefined-error|code:%d", error);
		}
		exit(1);
	}

	// set char size
	error = FT_Set_Char_Size(face, 0, 32 * 64, 96, 96);
	if (error) {
		printf("error|char-size|code:%d", error);
	}

	// load glyph by given char
	int glyphIndex = FT_Get_Char_Index(face, '@');
	error = FT_Load_Glyph(face, glyphIndex, FT_LOAD_DEFAULT);
	if (error) {
		printf("error|load-glyph|code:%d", error);
	}
	FT_GlyphSlot glyph = face->glyph;
	FT_Outline outline = glyph->outline;

	// get metrics
	float width = glyph->metrics.width / (float)65536 * 64;

	// get outline
	for (int i = 0; i < outline.n_points; i++)
	{
		if (FT_CURVE_TAG(outline.tags[i])) {
			printf("curve\n");
		}
		printf("tag:%.6f\n", outline.points[i].x / (float)65536);
	}

	return 1;
}