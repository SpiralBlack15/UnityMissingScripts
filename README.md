# UNITY MISSING SCRIPTS
[English](#Overview "Goto English Documentation") | [Russian](#Обзор "Goto Russian Documentation")

---
ENG
---

## Overview

This util provides easy search and replace for the missing scripts GUIDs in your Unity3D scene. 

**Provided functionality**

+ Can find all GameObjects with missing scripts on currenty opened Scene.
+ Can find GUIDs of both alive and missing MonoBehaviour scripts (it does not apply to Unity "system" scripts such as Transform, Camera and so on because they don't have GUID field).
+ Can select group of objects with the same lost script (if these objects are not prefabed, see limitations), so you can mass replace broken script without having to do this with each script individually.
+ Allows you to browse and survey script's and GameObject's scene file's entry in Unity window.
+ Project has full and switchable ENG/RU localization in EditorWindow.

**Current limitations**: 

+ Works under **_Unity 2019.2, 2019.3 and higher_**. Earlier versions of Unity do not support some functions.
+ Works only in opened scene, not in whole project. 
+ Cannot find GUIDs if missing scripts objects are inside prefabs (still, objects with broken scripts can be found as well), except object in prefab's root. This limitation is associated with a scene file search. 
+ Can be very slow on the scenes with big amount (more than thousand) of objects.
+ Most of the comments in the code are still in Russian. Sorry, too lazy to rewrite everything into English (still you can use Google Translate, and most part are anyway obvious from the code).

## Installation, launch and usage 

Copy folder into your project Assets folder, than rename it into something like "DeadScriptSearcher" or whatever you want. After that, you can see _Spiral Tools_ added to your upper menu in UnityEditor. There are two menu items: _Dead Script Searcher_ and _Object Auditor_ that open two different EditorWindow's. 

+ _Dead Script Searcher_ opens windows to search missing scripts in your scene.
+ _Object Auditor_ allows you to inspect GameObjects selected in Scene Hierarchy, looking for GUID's and some other information on alive and dead scripts on it.

**Dead Script Searcher window**

Here are some elements:

+ _Editor_ - navigates you to Dead Script Searcher editor's script.
+ _Language_ - switch localization.
+ _Re-upload scene text_ - allows you to forcely re-import current %insertnamehere%.scene into Searcher. Be aware that this button loads _scene file_, not _current state of the scene_, so you need to save scene first to be sure Searcher and Auditor are working correctly (if not, you may have some warnings/errors in Unity's console).
+ _Debug mode_ - if you use debug mode, all actions during finding dead GUIDs procedure will be outputed in Unity's console along with corresponding data. This can _significally_ slowdown the checkup of the scenes with a large number of objects.
+ _Find and select objects with dead scripts_ - do exactly what it says. It's very fast, but it does not looks for dead GUID's. Just find dead objects, nothing more. But it can find missing scripts in prefabs (still you cannot see their data).
+ _Find dead GUIDs in the scene_ - collects and display all missing scripts data on the scene (excluding prefabed objects and scripts without m_Script fields). For each unique GUID, Searcher generates panel with:
    + Script GUID (selectable).
    + Number of MonoBehaviours (most often, but not necessarily equal to GameObjects count) with _this type of_ missing script. This is foldout filed with clickable MonoBehaviours' IDs, so you can find every script on scene individually. Every account have individual MonoBehaviour's data: its ID in scene file, button to select target GameObject, corresponding scene file's entry. If script has any custom serialized fields, you will see their variables names and values in this entry.
    + Button to select group of the objects with the same script. 
    
**Object Auditor window**

Like Dead Script Searcher window, it has an editor script field, localization switcher and button for re-uploading the scene file.

Everytime you select some object(s) on scene, an information panels appears in Object Auditor window. Each panel corresponds to one of the selected objects. It contains:

+ _File ID_ - GameObject's scene file ID.
+ _File caption IDX_ - the number of the string in scene file in which the GameObject's caption contains.
+ _Show components list_ - foldout that contains brief GameObject's components information such as: script name, scene file ID, script GUID (if has one, if not - there is an explanation why: because of prefabed or not MonoBehaviour). If script is dead, it shows light red, otherwise - light green.

## Useful information

You can see the implementation of rendering editor script fields in SpiralEditor.cs and use them freely in your projects. Just do not forget that the Editor script should always be in a separate file with a name identical to the class name, otherwise Unity will not see it as MonoScript.

## License comments

The MIT License (MIT)

Copyright © 2020 SpiralBlack https://github.com/SpiralBlack15

This project provides under the MIT License, look to the license file for more info. Most .cs files in project include part of license about **no warranity**. Just remember it :)

---
RU
---

## Обзор

Утилита предоставляет удобный инструментарий для поиска и замены GUID'ов от скриптов-потеряшек в сцене Unity3D.

**Функционал**:

+ Находит все объекты с потерянными скриптами на открытой сцене.
+ Может находить GUID'ы как живых, так и мёртвых MonoBehaviours (это не относится к таким "системным" скриптам Unity как Transform, Camera и подобным, поскольку они не имеют GUID-поля).
+ Может выделять группу объектов, имеющих одинаковый потерянный скрипт (если эти объекты не входят в состав префаба, см. ограничения), так что вы можете массово заменять потерянные скрипты существующими без необходимости делать это с каждым скриптом по отдельности.
+ Позволяет просматривать в окне Unity записи конкретных скриптов и объектов из файла сцены.
+ Проект имеет две равноценные переключаемые локализации ENG/RU в окне редактора.

**Текущие ограничения**:

+ Работает под версией **_Unity 2019.2, 2019.3 и выше_**. Ранние версии Unity не поддерживают часть функций.
+ Работает только с текущей открытой сценой, не с проектом в целом.
+ Не может идентифицировать GUID'ы мёртвых скрпитов, если те находятся на объектах, входящих в состав префаба (тем не менее, сами объекты она находит), если только объект не является корнем префаба. Это ограничение связано с поиском GUID'ов через файл сцены.
+ Может быть очень медленной для сцен с большим количеством (больше тысячи) объектов.
+ Большая часть комментариев в коде на русском. Хотя для русской локали это скорее плюс, чем минус.

## Установка и использование

Скопируйте папку в ваш проект, после чего переименуйте её во что-то типа "DeadScriptSearcher", на ваше усмотрение. После этого в верхнее меню Unity будет добавлен выпадающий список _Spiral Tools_. В нём два элемента: _Dead Script Searcher_ и _Object Auditor_, которые открывают отдельные перетаскиваемые окна в редакторе. 

+ _Dead Script Searcher_ (Поиск мёртвых скриптов) открывает окно поиска мёртвых скриптов в текущей сцене. 
+ _Object Auditor_ (Ревизор) позволяет инспектировать выделенные в иерархии объекты, просматривая GUID'ы и прочую информацию с их скриптов, как живых, так и мёртвых.

**Окно "Поиск мёртвых скриптов"**

Здесь есть несколько элементов:

+ _Editor_ - показывает EditorWindow-скрипт от Dead Script Searcher'a.
+ _Language_ - переключатель локализации.
+ _Синхронизировать текст сцены с её файлом_ - позволяет принудительно импортировать %названиесцены%.scene в Поисковик. Обратите внимание на то, что эта кнопка загружает _файл сцены_, не _текущее состояние сцены_, поэтому необходимо сначала сохранить сцену, чтобы быть уверенным, что Поисковик и Ревизор работают правильно (если нет, у вас могут изредка сыпаться сообщения в консоль, что что-то там не найдено).
+ _Режим отладки_ - если включен, все действия Поисковика во время поиска мёртвых GUID'ов будут выводиться в консоль вместе со всей сопутствующей информацией. Это может _существенно_ замедлить поиск по сценам с большим количеством объектов.
+ _Найти и выделить объекты с мёртвыми скриптами_ - делает именно то, что написано: просто быстро находит все объекты, имеющие на себе битые скрипты. Эта функция не ищет мёртвые GUID'ы, однако находит объекты, являющиеся частью префаба. 
+ _Найти мёртвые GUID в сцене_ - собирает и отображает всю информацию о потерянных скриптах на сцене (исключая объекты, являющиеся частью, но не корнем префаба, а также скрипты, не имеющие поля m_Script). Для каждого уникального GUID'а Поисковик формирует отдельную панель со следующими элементами:
    + GUID скрипта (выделяемый).
    + Количество MonoBehaviour (в основном, но необязательно соответствует количеству GameObject'ов), имеющих _данный тип_ потерянного скрипта. Это выпадающий список, содержащий кликабельные подпункты с номерами MonoBehaviour'ов, позволяющие найти каждый скрипт на сцене индивидуально. Каждый такой аккаунт содержит данные о конкретном MonoBehaviour: его ID в файле сцены, кнопку для выбора соответствующего GameObject'a, а также соответствующую запись из файла сцены (если скрипт имел сериализованные поля, вы увидите названия переменных и их значения в записи).
    + Кнопка для группового выделения скриптов с одинаковым GUID.
    
**Окно "Ревизор"**

Как и окно Поисковика, имеет поле, указывающее на скрипт редактора, переключатель локализации и кнопку для перезагрузки файла сцены.

Каждый раз, когда вы выделяете объект(ы) на сцене, в окне Ревизора появляются плашки, соответствующие выбранным объектам. Каждая плашка содержит:

+ _File ID_ - ID выбранного объекта в файле сцены.
+ _File caption IDX_ - номер строки, соответствующей заголовку данного объекта в файле сцены.
+ _Развернуть список компонент_ - разворачивающийся лист, содержащий краткую информацию по всем компонентам на объекте. Информация по каждому компоненту содержит: название скрипта компонента, индекс компонента в файле сцены, GUID скрипта (если есть, если нет - пояснение, почему нет: например, потому что объект является частью префаба ли не является MonoBehaviour). Если скрипт потерян, он будет высвечиваться светло-красным, если всё в порядке - светло-зелёным.

## Полезная информация

Реализацию отрисовки полей скрипта редактора можете посмотреть в SpiralEditor.cs и свободно использовать в своих проектах, просто не забывайте, что скрипт Editor'a всегда должен лежать в отдельном файле с названием, идентичным названию класса, иначе Unity его не воспринимает как MonoScript.

## Комментарии к лицензии

The MIT License (MIT)

Copyright © 2020 SpiralBlack https://github.com/SpiralBlack15

Проект предоставляется в комплекте с открытой лицензией MIT, по всем подробностям - в файл лицензии (или в Интернет, если нужен перевод на русский). Большая часть .cs файлов проекта содержит часть лицензии, относящуюся, в первую очередь, **к отсутствию каких бы то ни было гарантий и ответственности**. Мало ли, вдруг вы не читаете лицензии прежде чем копировать всё без разбору :)
