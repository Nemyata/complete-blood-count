import sys
from huggingface_hub import InferenceClient
import re

client = InferenceClient(api_key=sys.argv[1])
def extract_data(response):
    data = {}
    for line in response.split('\n'):
        if ':' in line:
            key, value = line.split(':', 1)
            value = value.strip()

            clean_value = re.match(r'[\d.]+', value)
            if clean_value:
                data[key.strip()] = clean_value.group()
            else:
                data[key.strip()] = value
    return data

prompt = """На вход дан текст, полученный через OCR с результатами медицинского анализа крови. 

Восстанови и верни следующую информацию:
- Пол
- Гемоглобин  
- Эритроциты
- Тромбоциты
- Лейкоциты

Ответ должен быть в следующем формате:

Пол: <значение>
Гемоглобин: <значение>
Эритроциты: <значение>
Тромбоциты: <значение>
Лейкоциты: <значение>

Текст анализа:
"""


input_text = f"{prompt}{sys.argv[2]}"
input_text += "Ответ должен быть кратким, строго на русском и строго соответствовать указанному формату. Не добавляй комментарии или пояснения."

messages = [{"role": "system", "content": "Ты — помощник, который извлекает значения из текста медицинских анализов и выдаёт их строго в заданном формате."},
            {"role": "user", "content": input_text}]

completion = client.chat.completions.create(
    model="Qwen/QwQ-32B-Preview",
    messages=messages,
    max_tokens=100,
    temperature=0.2,
    top_p=0.9
)

result = extract_data(completion.choices[0].message.content)

if __name__ == "__main__":
    print(result)