# Galactus
Демо:https://van-fim.itch.io/testproject/<br>
Для мультиплеера использовал mirror
1) Реализована генерация галактик и систем (планет и солнц) из xml файлов:<br>
Assets\Resources\Content\Universe.xml<br>
Assets\Resources\Content\Galaxies.xml<br>
Assets\Resources\Content\Systems.xml<br>
Assets\Resources\Content\Sectors.xml<br>
Assets\Resources\Content\Zones.xml<br>
Assets\Resources\Content\Planets.xml<br>
Assets\Resources\Content\Suns.xml<br>
xml файлы вынес во внешнюю паку Content:<br>
Их можно модифицировать, добавлять шаблоны кораблей, увеличивать скорость и тд.

3) Реализована систеа аккаунтов и создания персонажей (вводим логин и выбираем старт - персонаж создан!)<br>Старты и позиции игрока задаются в файле Assets\Resources\Content\GameStarts.xml<br>Старт 01 игрок стартует в скафандре
4) Реализована карта при помощи которой игрок может путешествовать по системам и секторам
5) Реализована система локализации
6) Реализовал свое решение для фикса точности системы координат ))). Можно летать на огромнейшие расстояния.<br>На самом деле большой белый куб находится в абсолютном центре мира. Границы этого куба игрок никогда не перелетит.
7) Есть собственная коммандная консоль: нажать ~<br>
Список доступных команд:<br>
StartHost<br>
StartServer<br>
StartClient<br>
ClearPage<br>
SetAccountResource<br>
SetClientResource<br>
AddAccountResource<br>
AddClientResource<br>
SetPageMaxLines<br>
CreatePage<br>
SetPageSettings<br>
ShowPage<br>
SaveAccountData<br>
ShowClientsList<br>
Quit<br>
Help<br>
Управление:

Миникарта<br>
W,S,A,D движение камеры<br>
колесо мыши меняет скорость<br>
зажать ПКМ для поворота камеры<br>

Игра
<br>Белый и зеленый куб нужны для понимания нашего движения<br>
W,S,A,D,Q,E вращение игрока по разным осям<br>
колесо мыши меняет скорость<br>
зажать ПКМ для поворота камеры<br>
