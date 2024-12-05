import sys
import os
import cv2
import numpy as np
from PIL import Image
import pytesseract
import fitz


pytesseract.pytesseract.tesseract_cmd = sys.argv[1]

def adapt_contrast(image):
    """
    Адаптивное увеличение контраста с помощью CLAHE.
    """
    clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8, 8))
    return clahe.apply(image)

def preprocess_image(image_path):
    """
    Предобработка изображения для улучшения качества текста.
    """
    img = cv2.imread(image_path, cv2.IMREAD_GRAYSCALE)
    img = cv2.resize(img, None, fx=2, fy=2, interpolation=cv2.INTER_CUBIC)
    img = adapt_contrast(img)
    img = cv2.fastNlMeansDenoising(img, None, 30, 7, 21)
    img = cv2.adaptiveThreshold(img, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 11, 2)
    kernel = np.ones((3, 3), np.uint8)
    img = cv2.morphologyEx(img, cv2.MORPH_CLOSE, kernel)
    img = cv2.morphologyEx(img, cv2.MORPH_OPEN, kernel)
    edges = cv2.Canny(img, 50, 150)
    img = cv2.bitwise_or(img, edges)
    temp_img_path = "temp_processed_image.png"
    cv2.imwrite(temp_img_path, img)
    return temp_img_path


def process_into_text(image):

    return pytesseract.image_to_string(image, lang='rus+eng', config='--psm 4 --oem 3')

def extract_text_from_files(file_path):
    """
    Обработка файлов из директории для извлечения текста.
    Возвращает список словарей с результатами.
    """
    results = []
    try:
        file_name = os.path.basename(file_path)
        if file_name.lower().endswith('.pdf'):
            doc = fitz.open(file_path)
            pdf_text = ""
            for page_num in range(doc.page_count):
                page = doc.load_page(page_num)
                text = page.get_text("text")
                if text.strip():
                    pdf_text += text

            if not pdf_text.strip():
                print(f"Текст не распознан для {file_name}, выполняется предварительная обработка...")
                for page_num in range(doc.page_count):
                    page = doc.load_page(page_num)
                    pix = page.get_pixmap()
                    image = Image.frombytes("RGB", [pix.width, pix.height], pix.samples)
                    image.save("temp_pdf_image.png")
                    processed_path = preprocess_image("temp_pdf_image.png")
                    preprocessed_image = Image.open(processed_path)
                    pdf_text += process_into_text(preprocessed_image)

            results.append({"file_name": file_name, "text": pdf_text})
            
        elif file_name.lower().endswith(('.png', '.jpg', '.jpeg')):
            processed_path = preprocess_image(file_path)
            preprocessed_image = Image.open(processed_path)
            image_text = process_into_text(preprocessed_image)
            results.append({"file_name": file_name, "text": image_text})

        else:
            print(f"Пропущен неподдерживаемый файл: {file_name}")

    except Exception as e:
        print(f"Ошибка при обработке файла {file_name}: {e}")

    return results



results = extract_text_from_files(sys.argv[2])

for result in results:
    print(f"Файл: {result['file_name']}")
    print(f"Текст:\n{result['text']}")


if __name__ == "__main__":
    for result in results:
        print(f"Файл: {result['file_name']}")
        print(f"Текст:\n{result['text']}")