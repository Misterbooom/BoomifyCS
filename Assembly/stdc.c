#include <stdio.h>
#include <stdlib.h>
#include <string.h>

void writeLine(int space, const char *file, int targetLine)
{
	char res[256] = "";
	for (int i = 0; i < space && i < sizeof(res) - 1; i++)
	{
		strcat_s(res,sizeof(res) - 1," ");
	}

	FILE *fptr;
	fopen_s(&fptr,file, "r");
	if (fptr == NULL)
	{
		perror("Error opening file");
		return;
	}

	int currentLine = 0;
	char line[256];
	while (fgets(line, sizeof(line), fptr) != NULL)
	{
		currentLine++;
		if (currentLine == targetLine)
		{
			// strncat(res, line, sizeof(res) - strlen(res) - 1);
			strcat_s(res,sizeof(res) - strlen(res) - 1,line);
			break;
		}
	}

	fclose(fptr);
	printf("%s", res);
}

void printError(const char *errorName, const char *message, const char *file, int line)
{
	char exceptionInfo[256];
	snprintf(exceptionInfo, sizeof(exceptionInfo), "%s: %s", errorName, message);

	char fileInfo[256];
	snprintf(fileInfo, sizeof(fileInfo), "     File '%s', Line: %d", file, line);

	printf("\033[38;2;255;0;0m%s\n%s\033[0m\n", exceptionInfo, fileInfo);
	writeLine(10, file, line);
}


// int main()
// {
// 	printError("Error", "Error message", "stdc.c", 2);
// 	return 0;
// }
