import sys
import asyncio
from g4f.client import AsyncClient
from g4f.Provider import OpenaiChat

async def analyze_error(error_text: str) -> str:
    client = AsyncClient()
    prompt = (
        "Please analyze the following C# error message. Response must be short. Provide a brief explanation, "
        "possible causes, and recommendations for resolution:\n\n"
        f"{error_text}"
    )
    response = await client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[{"role": "user", "content": prompt}]
    )
    return response.choices[0].message.content

async def main():
    if len(sys.argv) < 2:
        print("Usage: python analyzer.py \"error text\"")
        sys.exit(1)
    error_text = sys.argv[1]
    analysis = await analyze_error(error_text)
    print("Error Analysis:")
    print(analysis)

if __name__ == "__main__":
    asyncio.run(main())
