using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_RSA
{
    internal class Utils
    {





        // Функция для нахождения наибольшего общего делителя (НОД) двух чисел с использованием алгоритма Евклида
        public static BigInteger NOD(BigInteger a, BigInteger b)
        {
            a = BigInteger.Abs(a);  // Преобразуем числа в абсолютные значения
            b = BigInteger.Abs(b);
            return b == BigInteger.Zero ? a : NOD(b, a % b);  // Если b равно 0, то НОД - это a, иначе рекурсивно вызываем NOD
        }

        // Функция для быстрого возведения числа в степень по модулю (по методу бинарного возведения в степень)
        public static BigInteger StepenMod(BigInteger @base, BigInteger exp, BigInteger mod)
        {
            BigInteger result = 1;  // Начальное значение результата
            @base %= mod;  // Берем остаток от деления base на mod
            while (exp > 0)  // Пока экспонента больше 0
            {
                if (exp % 2 != 0)  // Если экспонента нечётная
                    result = (result * @base) % mod;  // Умножаем результат на base по модулю
                @base = (@base * @base) % mod;  // Удваиваем base по модулю
                exp /= 2;  // Делим экспоненту на 2
            }
            return result;  // Возвращаем результат
        }

        // Функция для нахождения обратного элемента по модулю (расширенный алгоритм Евклида)
        public static BigInteger ObrMod(BigInteger a, BigInteger m)
        {
            if (NOD(a, m) != BigInteger.One)  // Если НОД(a, m) не равен 1, то обратный элемент не существует
                return -1;  // Возвращаем -1 в случае отсутствия обратного элемента

            BigInteger x = 1, y = 0;  // Инициализируем переменные для алгоритма Евклида
            BigInteger m0 = m, t, q;
            while (a > 1)  // Алгоритм продолжается до тех пор, пока a не станет равным 1
            {
                q = a / m;  // Получаем частное
                t = m;  // Временная переменная для m
                m = a % m;  // Обновляем m
                a = t;  // Обновляем a
                t = y;  // Временная переменная для y
                y = x - q * y;  // Обновляем y
                x = t;  // Обновляем x
            }
            if (x < 0)  // Если x отрицательно, делаем его положительным
                x += m0;
            return x;  // Возвращаем обратный элемент
        }

        // Функция для кодирования сообщения
        public static string Encode(string message)
        {
            string result = "";  // Строка для хранения результата
            foreach (char c in message)  // Для каждого символа в сообщении
            {
                result += ((int)c + 100).ToString();  // Кодируем символ как число и добавляем в строку
            }
            return result;  // Возвращаем закодированное сообщение
        }

        // Функция для декодирования сообщения
        public static string Decode(string encodedMessage)
        {
            string decodedMessage = "";  // Строка для хранения расшифрованного сообщения
            string temp = "";  // Временная строка для хранения текущего числа

            foreach (char ch in encodedMessage)  // Для каждого символа в закодированном сообщении
            {
                temp += ch;  // Добавляем символ к временной строке

                if (temp.Length >= 3)  // Если временная строка содержит 3 символа
                {
                    if (int.TryParse(temp, out int value))  // Преобразуем строку в число
                    {
                        char decodedChar = (char)(value - 100);  // Декодируем символ
                        decodedMessage += decodedChar;  // Добавляем символ в результат
                        temp = "";  // Очищаем временную строку
                    }
                    else
                    {
                        throw new FormatException($"Не удалось преобразовать \"{temp}\" в число.");  // Ошибка преобразования
                    }
                }
            }

            return decodedMessage;  // Возвращаем расшифрованное сообщение
        }

        // Функция для шифрования блоков данных
        public static List<BigInteger> EncryptBlocks(string input, BigInteger exp, BigInteger n)
        {
            var encrypted = new List<BigInteger>();  // Список для хранения зашифрованных блоков
            string current = "";  // Временная строка для хранения текущего блока

            foreach (char c in input)  // Для каждого символа во вводе
            {
                current += c;  // Добавляем символ в текущий блок
                BigInteger value = BigInteger.Parse(current);  // Преобразуем текущий блок в число

                if (value >= n)  // Если значение блока больше, чем n
                {
                    current = current.Remove(current.Length - 1);  // Убираем последний символ из блока
                    BigInteger block = BigInteger.Parse(current);  // Преобразуем блок в число
                    encrypted.Add(StepenMod(block, exp, n));  // Шифруем блок и добавляем в список
                    current = c.ToString();  // Начинаем новый блок с текущего символа
                }
            }

            if (current.Length > 0)  // Если остался последний блок
            {
                BigInteger block = BigInteger.Parse(current);  // Преобразуем блок в число
                encrypted.Add(StepenMod(block, exp, n));  // Шифруем последний блок
            }

            return encrypted;  // Возвращаем список зашифрованных блоков
        }

        // Функция для парсинга зашифрованного текста
        public static List<BigInteger> ParseEncryptedText(string inputText)
        {
            var result = new List<BigInteger>();  // Список для хранения зашифрованных чисел
            var parts = inputText.Split(' ');  // Разбиваем входной текст на части по пробелам

            foreach (var part in parts)  // Для каждой части
            {
                if (!string.IsNullOrWhiteSpace(part))  // Если часть не пустая
                {
                    result.Add(BigInteger.Parse(part));  // Преобразуем в число и добавляем в список
                }
            }

            return result;  // Возвращаем список чисел
        }

        // Функция для расшифровки блоков
        public static string DecryptBlocks(List<BigInteger> encrypted, BigInteger d, BigInteger n)
        {
            string result = "";  // Строка для хранения результата расшифровки

            foreach (BigInteger block in encrypted)  // Для каждого зашифрованного блока
            {
                BigInteger decrypted = StepenMod(block, d, n);  // Расшифровываем блок
                result += decrypted.ToString();  // Добавляем расшифрованное значение в результат
            }

            return result;  // Возвращаем расшифрованное сообщение
        }

        // Функция для проверки простоты числа
        public static bool IsPrime(BigInteger n)
        {
            if (n < 2) return false;  // Число меньше 2 не является простым
            if (n == 2 || n == 3) return true;  // Числа 2 и 3 простые
            if (n % 2 == 0) return false;  // Число четное, значит, не простое

            for (BigInteger i = 3; i * i <= n; i += 2)  // Проверяем делители до квадратного корня из n
            {
                if (n % i == 0) return false;  // Если n делится на i, то n не простое
            }
            return true;  // Если не нашли делителей, число простое
        }

        // Функция для генерации случайного большого числа, меньшее max
        public static BigInteger RandomBigInteger(BigInteger max)
        {
            byte[] bytes = max.ToByteArray();  // Получаем байтовое представление максимального значения
            BigInteger value;

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())  // Генератор случайных чисел
            {
                do
                {
                    rng.GetBytes(bytes);  // Заполняем байты случайными данными
                    bytes[bytes.Length - 1] &= 0x7F;  // Убираем знак числа
                    value = new BigInteger(bytes);  // Преобразуем байты в число
                } while (value >= max);  // Генерируем до тех пор, пока число не будет меньше max
            }

            return value;  // Возвращаем случайное число
        }

        // Тестирование простоты числа с использованием теста Миллера-Рабина
        public static bool MillerRabinTest(BigInteger n, BigInteger d, int s)
        {
            BigInteger a = 2 + RandomBigInteger(n - 4);  // Генерируем случайное число a
            BigInteger x = StepenMod(a, d, n);  // Вычисляем a^d % n

            if (x == 1 || x == n - 1) return true;  // Если x равно 1 или n-1, то число вероятно простое

            for (int r = 1; r < s; ++r)  // Выполняем s раундов теста
            {
                x = (x * x) % n;  // Возводим x в квадрат по модулю
                if (x == 1) return false;  // Если x == 1, то число составное
                if (x == n - 1) return true;  // Если x == n-1, то число вероятно простое
            }

            return false;  // Если тест не прошел, то число составное
        }

        // Функция для проверки простоты числа с использованием теста Миллера-Рабина
        public static bool IsPrimeM(BigInteger n, int iterations = 40)
        {
            if (n <= 1 || n % 2 == 0) return false;  // Числа <= 1 и четные не простые
            if (n == 2 || n == 3) return true;  // Числа 2 и 3 простые

            BigInteger d = n - 1;
            int s = 0;
            while (d % 2 == 0)  // Разлагаем n-1 на 2^s * d
            {
                d /= 2;
                ++s;
            }

            for (int i = 0; i < iterations; ++i)  // Выполняем несколько итераций теста Миллера-Рабина
            {
                if (!MillerRabinTest(n, d, s)) return false;  // Если хотя бы один тест не прошел, число составное
            }

            return true;  // Если все тесты прошли, число вероятно простое
        }

        // Функция для нахождения следующего простого числа больше n
        public static BigInteger FindNextPrime(BigInteger n)
        {
            if (n < 2) return 2;  // Если n меньше 2, возвращаем 2
            if (n % 2 == 0) n += 1;  // Если n четное, начинаем с n+1

            while (true)  // Ищем следующее простое число
            {
                if (IsPrime(n)) return n;  // Если n простое, возвращаем его
                n += 2;  // Иначе проверяем следующее нечетное число
            }
        }

        // Функция для нахождения следующего простого числа с использованием теста Миллера-Рабина
        public static BigInteger FindNextPrimeM(BigInteger n)
        {
            if (n < 2) return 2;  // Если n меньше 2, возвращаем 2
            if (n % 2 == 0) n += 1;  // Если n четное, начинаем с n+1

            while (true)  // Ищем следующее простое число
            {
                if (IsPrimeM(n)) return n;  // Если n простое, возвращаем его
                n += 2;  // Иначе проверяем следующее нечетное число
            }
        }


    }
}
