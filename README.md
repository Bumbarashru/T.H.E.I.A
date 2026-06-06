Как работает RADMNIN VPN? Узнать протокол, научиться делать локальную сеть между компами 



# 1. Создаем проект
dotnet new console -n MyProject
cd MyProject

# 2. Устанавливаем все библиотеки-аналоги

# Windows уведомления (аналог winotify)
dotnet add package Microsoft.Toolkit.Uwp.Notifications

# Автоматизация клавиатуры и мыши (аналог pyautogui)
dotnet add package InputSimulatorPlus                    #ВИНДА 
ИЛИ 
# Установка
dotnet add package Desktop.Robot --version 1.5.0         #ЛИНУКС

# Браузерная автоматизация (аналог selenium + webdriver_manager)
dotnet add package Selenium.WebDriver
dotnet add package WebDriverManager

# WhatsApp автоматизация (аналог pywhatkit)
dotnet add package WhatsAppAutomation

# Воспроизведение звука (аналог playsound)
dotnet add package NAudio

# Системная информация (аналог psutil + wmi)
dotnet add package System.Management
dotnet add package Hardware.Info

# Веб-скрапинг (аналог webscout)
dotnet add package HtmlAgilityPack

# Очистка HTML (аналог lxml_html_clean)
dotnet add package HtmlSanitizer

# Клиент для AI API (аналог gradio_client) - через встроенный HttpClient, пакет не нужен

# Красивая консоль + спиннеры (аналог colorlog + yaspin)
dotnet add package Spectre.Console

# Компьютерное зрение (аналог opencv-python)
dotnet add package OpenCvSharp4
dotnet add package OpenCvSharp4.Windows

# Научные вычисления (аналог scipy)
dotnet add package MathNet.Numerics

# Управление громкостью Windows (аналог pycaw)
dotnet add package AudioSwitcher.AudioApi.CoreAudio

# Альтернатива Selenium для скрытой автоматизации
dotnet add package PlaywrightSharp

# Маскировка Selenium (Stealth)
dotnet add package SeleniumStealth

# Дополнительно: работа с JSON (аналог json)
dotnet add package Newtonsoft.Json

# Дополнительно: работа с Excel (аналог openpyxl/xlrd)
dotnet add package EPPlus

# Дополнительно: работа с PDF (аналог PyPDF2)
dotnet add package iTextSharp.LGPLv2.Core

# Проверяем, что всё установилось
dotnet restore


------------------------------------------------------------------------------------------------ 
❌ УДАЛИТЬ (работают только на Windows):
dotnet remove package Microsoft.Toolkit.Uwp.Notifications
dotnet remove package InputSimulatorPlus
dotnet remove package WhatsAppAutomation
dotnet remove package NAudio
dotnet remove package System.Management
dotnet remove package OpenCvSharp4.Windows
dotnet remove package AudioSwitcher.AudioApi.CoreAudio

✅ ДОБАВИТЬ (кроссплатформенные аналоги):
# 1. Уведомления (вместо Microsoft.Toolkit.Uwp.Notifications)
dotnet add package Desktop.Notifications

# 2. Автоматизация клавиатуры/мыши (вместо InputSimulatorPlus)
# Desktop.Robot уже установлен - он кроссплатформенный!
# Просто используйте его для обеих ОС

# 3. WhatsApp автоматизация (вместо WhatsAppAutomation)
# WhatsAppAutomation работает только с WhatsApp Desktop для Windows
# Используйте Selenium + WhatsApp Web (кроссплатформенно)
# Selenium.WebDriver уже установлен - используйте его

# 4. Воспроизведение звука (вместо NAudio)
dotnet add package ManagedBass
dotnet add package ManagedBass.Flac
dotnet add package ManagedBass.Mp3
# ManagedBass кроссплатформенный (Windows, Linux, macOS)

# 5. Системная информация (вместо System.Management/WMI)
# Hardware.Info уже установлен - он кроссплатформенный!
# Для дополнительной информации используйте:
dotnet add package System.Runtime.InteropServices.RuntimeInformation

# 6. OpenCV runtime для Linux (вместо OpenCvSharp4.Windows)
dotnet add package OpenCvSharp4.runtime.linux-x64

# 7. Управление громкостью (вместо AudioSwitcher.AudioApi.CoreAudio)
dotnet add package CSCore
# CSCore кроссплатформенный (работает на Windows и Linux через PulseAudio)



------------------------------------------------------------------------------------------------------
📋 ИТОГОВЫЙ СПИСОК для Arch Linux:
# Создаем проект
dotnet new console -n MyProject
cd MyProject

# КРОССПЛАТФОРМЕННЫЕ БИБЛИОТЕКИ (работают везде)

# Уведомления (кроссплатформенные)
dotnet add package Desktop.Notifications

# Автоматизация клавиатуры и мыши (кроссплатформенная)
dotnet add package Desktop.Robot --version 1.5.0

# Браузерная автоматизация (кроссплатформенная)
dotnet add package Selenium.WebDriver
dotnet add package WebDriverManager

# Воспроизведение звука (кроссплатформенное)
dotnet add package ManagedBass
dotnet add package ManagedBass.Mp3

# Системная информация (кроссплатформенная)
dotnet add package Hardware.Info
dotnet add package System.Runtime.InteropServices.RuntimeInformation

# Веб-скрапинг (кроссплатформенный)
dotnet add package HtmlAgilityPack

# Очистка HTML (кроссплатформенный)
dotnet add package HtmlSanitizer

# Красивая консоль + спиннеры (кроссплатформенные)
dotnet add package Spectre.Console

# Компьютерное зрение (кроссплатформенное)
dotnet add package OpenCvSharp4
dotnet add package OpenCvSharp4.runtime.linux-x64

# Научные вычисления (кроссплатформенные)
dotnet add package MathNet.Numerics

# Управление громкостью (кроссплатформенное)
dotnet add package CSCore

# Альтернатива Selenium для скрытой автоматизации (кроссплатформенная)
dotnet add package Microsoft.Playwright

# Маскировка Selenium (кроссплатформенная)
dotnet add package SeleniumStealth

# Работа с JSON (кроссплатформенная)
dotnet add package Newtonsoft.Json

# Работа с Excel (кроссплатформенная)
dotnet add package EPPlus

# Работа с PDF (кроссплатформенная)
dotnet add package iTextSharp.LGPLv2.Core

# Восстанавливаем пакеты
dotnet restore


🐧 ДОПОЛНИТЕЛЬНЫЕ СИСТЕМНЫЕ ЗАВИСИМОСТИ для Arch Linux:

# Для Desktop.Notifications
sudo pacman -S libnotify

# Для Desktop.Robot (автоматизация мыши/клавиатуры)
sudo pacman -S xdotool xclip

# Для ManagedBass (аудио)
sudo pacman -S alsa-lib

# Для CSCore (управление громкостью через PulseAudio)
sudo pacman -S pulseaudio pulseaudio-alsa

# Для OpenCV
sudo pacman -S opencv

# Для Playwright (браузерная автоматизация)
sudo pacman -S chromium


------------------------------------------------------------------------------------------------
    <PackageReference Include="Build5Nines.SharpVector" Version="2.2.1" />
    <PackageReference Include="CSCore" Version="1.2.1.2" />
    <PackageReference Include="KokoroSharp.CPU" Version="0.6.7" />
    <PackageReference Include="ManagedBass" Version="4.0.2" />
    <PackageReference Include="ManagedBass.Flac" Version="4.0.2" />
    <PackageReference Include="ManagedBass.Mp3" Version="4.0.2" />
    <PackageReference Include="Microsoft.SemanticKernel" Version="1.77.0" />
    <PackageReference Include="Microsoft.SemanticKernel.Connectors.Ollama" Version="1.66.0-alpha" />
    <PackageReference Include="MQTTnet" Version="5.1.0.1559" />
    <PackageReference Include="System.Runtime.InteropServices.RuntimeInformation" Version="4.3.0" />
    <PackageReference Include="Vosk" Version="0.3.38" />