# TestProjectIvan

# Описание решений:
- FileParser Service - сервис для получения, обработки и передачи в RabbitMQ данных полученных из XML документа.
- DataProcessor Service - сервис для получения данных из RabbitMQ, обработки и сохранения в БД SQLite.
  
# Параметры публикации:

 ## Публикация через Docker:
 
#### Требования к окружению:
- Битность 64
- Ядро не старее 3.10
  
### Процесс публикации:

Арендуем сервер или устанавливаем через Windows Store (Не забудьте включить WSL) Ubuntu 20.04.3 LTS.
### Подготовим Docker к установке.

- Первым делом обновите существующий список пакетов:<br>
`sudo apt update`

- Затем установите несколько необходимых пакетов, которые позволяют apt использовать пакеты через HTTPS:<br>
`sudo apt install apt-transport-https ca-certificates curl software-properties-common`

- Добавьте ключ GPG для официального репозитория Docker в вашу систему:<br>
`curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -`

- Добавьте репозиторий Docker в источники APT:<br>
`sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable"`

- Потом обновите базу данных пакетов и добавьте в нее пакеты Docker из недавно добавленного репозитория:<br>
`sudo apt update`

- Убедитесь, что установка будет выполняться из репозитория Docker, а не из репозитория Ubuntu по умолчанию:<br>
`apt-cache policy docker-ce`

Вы должны получить следующий вывод, хотя номер версии Docker может отличаться:<br>

![Вывод Docker](https://i.imgur.com/Ht03tts.png)<br>
Обратите внимание, что docker-ce не установлен, но является кандидатом на установку из репозитория Docker для Ubuntu

### Установите Docker:

`sudo apt install docker-ce`<BR>

Docker должен быть установлен, демон-процесс запущен, а для процесса активирован запуск при загрузке. Проверьте, что он запущен:

`sudo systemctl status docker`
Вывод должен выглядеть примерно следующим образом, указывая, что служба активна и запущена:

![Docker состояние](https://i.imgur.com/X2VTDiB.png)<br>
### Развертывание RabbitMQ в Docker (ВНИМАНИЕ. Я не выключал RMQ у себя на сервере, поэтому вы можете пропустить этот этап.)
- Скачайте официальный образ RabbitMQ<br>
`docker pull rabbitmq:3.6.14-management`
- Создайте docker volume для RabbitMQ<br>
`docker volume create rabbitmq_data`<br>
- Запустите контейнер с RabbitmMQ<br>
`docker run -d --hostname rabbitmq --log-driver=journald --name rabbitmq -p 5672:5672 -p 15672:15672 -p 15674:15674 -p 25672:25672 -p 61613:61613 -v rabbitmq_data:/var/lib/rabbitmq rabbitmq:3.6.14-management`<br>
### Настройка приложения перед публикацией (Если вы используете мой RMQ, то можете пропустить этот этап)
#### Пулим Git к себе и изменяем конфигурационные файлы:
- TestProjectIvan/app.config
- DataProcessorService/app.config<br>

Необходимо изменить RMQHost на адрес сервера, на котором у вас стоит RMQ. Аутентификационные данные не придусмотрены.
Загружаем проекты с измененными конфигурационными файлами на свой GIT.
### Публикация и запуск приложений
#### Создаем 2 образа приложений (измените ссылку с моего GIT на свой)
`docker build -f DockerfileFileParser https://github.com/bag162/TestProjectIvan.git -t fileparser/app:1.0`<br>
`docker build -f DockerfileDataProcessor https://github.com/bag162/TestProjectIvan.git -t dataprocessor/app:1.0`<br>
#### Смотрим созданные образы
`docker images`
#### Запускаем приложения
`docker run -d fileparser/app:1.0`<br>
`docker run -d dataprocessor/app:1.0`<br>
