from transformers import pipeline
generator = pipeline("text-generation", model="gpt2")
print(generator("Привет, как дела?", max_length=50))
