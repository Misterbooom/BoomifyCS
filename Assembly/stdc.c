#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <windows.h>
#include <dbghelp.h>
#include "stdc.h"

#pragma comment(lib, "dbghelp.lib")
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

void printStackTrace()
{
	int start = (top >= 4) ? top - 4 : 0;
	for (int i = top; i >= start; i--)
	{
		StackFrame frame = stack[i];
		char fileInfo[256];
		snprintf(fileInfo, sizeof(fileInfo), "     File '%s', Line: %d", frame.file, frame.line);
		printf("\033[38;2;255;0;0m%s\033[0m\n", fileInfo);
		writeLine(10, frame.file, frame.line);
	}
}
void pushFrame(int line,const char* file) {
	StackFrame stackFrame;
	stackFrame.line = line;
	stackFrame.file = file;
	if (top == STACK_SIZE - 1){
		printError("StackOverflow","Recursion depth exceeded!",file,line);
	}
	else{
		top += 1;
		stack[top] = stackFrame;

	}
}
void popFrame(){
	if (top == -1){
		return;
	}
	else{
		top -= 1;
	}
}

void printError(const char *errorName, const char *message, const char *file, int line)
{
	char exceptionInfo[256];
	snprintf(exceptionInfo, sizeof(exceptionInfo), "%s: %s", errorName, message);

	printf("\033[38;2;255;0;0m%s\033[0m\n", exceptionInfo);
	printStackTrace();
	exit(1);
}



// int main()
// {
// 	printError("Error", "Error message", "stdc.c", 2);
// 	return 0;
// }
