#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "stdc.h"

void writeLine(int space, const char *file, int targetLine)
{
	char res[256] = "";
	for (int i = 0; i < space && strlen(res) < sizeof(res) - 1; i++)
	{
		strncat(res, " ", sizeof(res) - strlen(res) - 1);
	}

	FILE *fptr = fopen(file, "r");
	if (fptr == NULL)
	{
		return;
	}

	int currentLine = 0;
	char line[256];
	while (fgets(line, sizeof(line), fptr) != NULL)
	{
		currentLine++;
		if (currentLine == targetLine)
		{
			strncat(res, line, sizeof(res) - strlen(res) - 1);
			break;
		}
	}

	fclose(fptr);
	printf("%s", res);
}

void printStackTrace()
{
	int start = (top >= 4) ? top - 4 : 0;
	for (int i = top; i >= start; i--)
	{
		StackFrame frame = stack[i];
		char fileInfo[256];
		snprintf(fileInfo, sizeof(fileInfo), "     File '%s', Line: %d", frame.file, frame.line);
		printf("\x1B[31m%s\x1B[0m\n", fileInfo);
		writeLine(10, frame.file, frame.line);
	}
}

void pushFrame(int line, const char* file) {
	if (top >= STACK_SIZE - 1) {
		printf("\x1B[31mStackOverflow: Recursion depth exceeded!\x1B[0m\n");
		printStackTrace();
		fflush(stdout);
		exit(1);
	}
    
	StackFrame stackFrame;
	stackFrame.line = line;
	stackFrame.file = file;
	top += 1;
	stack[top] = stackFrame;
}

void popFrame() {
	if (top > -1) {
		top -= 1;
	}
}

void printError(const char *errorName, const char *message, const char *file, int line)
{
	pushFrame(line, file);
	char exceptionInfo[256];
	snprintf(exceptionInfo, sizeof(exceptionInfo), "%s: %s", errorName, message);

	printf("\x1B[31m%s\x1B[0m\n", exceptionInfo);
	printStackTrace();
    
	fflush(stdout);
	exit(1);
}