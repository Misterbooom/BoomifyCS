#ifndef STDC_H
#define STDC_H
#define STACK_SIZE 999
typedef struct StackFrame{
	int line;
	const char* file;

} StackFrame;
StackFrame stack[STACK_SIZE];
int top = -1;


void writeLine(int space, const char *file, int targetLine);
void printError(const char *errorName, const char *message, const char *file, int line);
void pushFrame(int line, const char* file);
void printStackTrace();
#endif // STDC_H
