from transformers import AutoTokenizer, AutoModelForCausalLM

model_name = "Qwen/Qwen2.5-1.5B-Instruct"
tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForCausalLM.from_pretrained(model_name)



prompt = """
Тебе на ввод дадут текст. Текст был обработан ocr. Тебе надо попытаться восстановить исходную версию текста.
Вот текст:
"""
ocr_1 = """Исследование Расчетная дата Значение. выполнения измерения ОБЩИЙ АНАЛИЗ КРОВИ Нормальные значения Статус Гемоглобин 20!03!2020 115 г/л 108-132 выполнено Эритроциты 20103!2020 4.52 ++ х10121л 4.0 - 4.4 выполнено Гематокрит 20103!2020 35.0 % 32-42 выполнено Средний объем эритроцитов (МСУ) 20103!2020 77 фл 77-83 выполнено Среднее содержание НО в эритроците 20103!2020 25.4 пг 22.7-32.7 выполнено (МСН) Средняя концентрация НО в эритроцитах (МСНС) 20103!2020 329-- г/л 336-344 выполнено Цветовой показатель 20103!2020 0.76 -- 0,85- 1 выполнено Тромбоциты 20103!2020 306 х10*91л 196-344 выполнено Лейкоциты 20103!2020 7.2 х10*91л 5.5-15.5 выполнено Нейтрофилы сегментоядерные 20103!2020 1.72 х10*91л 1.5-8.5 выполнено Нейтрофилы сегментоядерные °/о 20103!2020 24.0 -- % 34-54 выполнено Эозинофилы 20103!2020 0.32 + х10*91л 0.02 - 0.3 выполнено Эозинофилы °!о 20103!2020 4.4 % 1 -5 выполнено Базофилы 20103!2020 0.05 х10*91л 0-0.07 выполнено Базофилы °/о 20103!2020 0.7 °/о 0-1  выполнено Моноциты 20103!2020 0.32 х10*91л 0.09-0.8 выполнено Моноциты °/о 20103!2020 4.4 °/о 4-8  выполнено Лимфоциты 20103!2020 4.79 х10*91л 1.5 - 7 выполнено Лимфоциты °/о 20103!2020 66.5 ++ % 33-53 выполнено СО3 (по Вестергрену) капиллярная кровь 20103!2020 30 ++ мм/час о - 10 выполнено Комментарий 19103!2020 выполнено 

1 2 3 4 
5 
6 7 
8 9 10 11 12 13 14 15 1б 17 18 19 
20 
В связи со сложностью стандартизации процедуры взятия капиллярной крови (физиологические и методические 21 особенности), результаты общего анализа крови, выполненные из капиллярной крови, могут отличаться друг от друга и результатов общего анализа, выполненных из венозной крови. Международный комитет по стандартизации в гематологии (ICSH)рекомендует использовать для проведения гематологических исследований венозную кровь. Взятие капиллярной крови для общего анализа крови рекомендовано при наличии медицинских показаний (новорожденные, ожоги, химиотерапия, труднодоступные вены и другое). """
ocr_2 = """ата рождении: 05.10.1978 Место жительства: Дата забора: 08.11.2021 
Общий клвввческвй анализ крове 
исследование результат единицы Рефереисное значение геюатокрит (НСТ) 41.8 % 39.1-51.6 геюоглобик (НОВ) 150 г/л 130-160 эритроциты (АВС) 4.89 10^12/л 4.0-5.5 MCV(ср.объем эритроц) 89 фл 80-100 RDW(итр.распр.эрит Р) 12.82 % 11.5-14.5 МСН(ср.содср.НЬ в эр) 29.21 иг 26.0-33.5 МСНС( ср.конц.Нб в эр) 359 г/л 300-360 тромбоциты (PLT) 259 10^9/л 150-400 лейкоциты(ИВС) 5.19 10^9/л 4.0-9.23 относит. кол.лимфоцятов % 39.0 % 19.0-45.0 относят. кол. моноцитов % 3.79 % 3.0-11.0 относит. кол.нейтрофилов % 46.6 % 44.0-72.0 яейтрофилы абс. 4.36 10^9/л 1.8-6.6 лиюфоциты абс. 1.70 10^9/л 1.26-3.2 моноцизтл абс. 0.5 , 10^9/л 0.09-1.01 относит. кол. эозянофялон % 2.98 % 0.7-5.0 эозянофштi абс. 0.11 10^9/л 0.04-0.58 относит. кол. базофилов % 0.0 °/о 0.0-1.5 базофвлы абс. 0.0 10^9/л 0.0-о.1 СОЭ по Вестергреиу 4 мм/ч 1-15 Средний объем тромб. в крови (МРУ) 9.15 фл 7.8-11.9 Одй объем тромб. в и (.крит,РСТ)  0.21 % 0.15-0.45 """
ocr_3 = ""
ocr_4 = ""

input_text = f"{prompt} {ocr_1}"

messages = [
    {"role": "system", "content": "You are a helpful assistant. Ты говоришь на русском. Тебе на ввод дают текст. Твоя задача восстановить этот текст"},
    {"role": "user", "content": input_text}
]

text = tokenizer.apply_chat_template(
    messages,
    tokenize=False,
    add_generation_prompt=True
)

model_inputs = tokenizer([text], return_tensors="pt").to(model.device)

generated_ids = model.generate(
    **model_inputs,
    max_new_tokens=512
)

generated_ids = [
    output_ids[len(input_ids):] for input_ids, output_ids in zip(model_inputs.input_ids, generated_ids)
]

response = tokenizer.batch_decode(generated_ids, skip_special_tokens=True)[0]

print(response)