using System;
using System.Collections.Generic;
namespace OldMarket.Navigation
{
    public static class Texts
    {
        private static readonly string[] Languages={"en","zh","zh-Hant","de","fr","it","ja","ko","pt","ru","es","tr","uk"};
        private static readonly Dictionary<string,string[]> Values=new Dictionary<string,string[]> {
            ["ZoomOut"]=new[]{"Zoom out","缩小","縮小","Verkleinern","Dézoomer","Riduci","縮小","축소","Diminuir","Отдалить","Alejar","Uzaklaştır","Віддалити"},
            ["ZoomIn"]=new[]{"Zoom in","放大","放大","Vergrößern","Zoomer","Ingrandisci","拡大","확대","Ampliar","Приблизить","Acercar","Yakınlaştır","Наблизити"},
            ["EditMarker"]=new[]{"Edit marker","编辑标记","編輯標記","Markierung bearbeiten","Modifier le repère","Modifica segnaposto","マーカーを編集","마커 편집","Editar marcador","Изменить метку","Editar marcador","İşareti düzenle","Редагувати мітку"},
            ["poi_engineer"]=new[]{"Engineer","工程师","工程師","Ingenieur","Ingénieur","Ingegnere","技師","기술자","Engenheiro","Инженер","Ingeniero","Mühendis","Інженер"},
            ["poi_decorations"]=new[]{"Decorations","装饰品店","裝飾品店","Dekorationen","Décorations","Decorazioni","装飾品店","장식품점","Decorações","Декорации","Decoraciones","Dekorasyon","Декорації"},
            ["poi_carpenter"]=new[]{"Carpenter","木匠","木匠","Tischler","Menuisier","Falegname","大工","목수","Carpinteiro","Плотник","Carpintero","Marangoz","Тесля"},
            ["poi_animals"]=new[]{"Animal market","动物市场","動物市場","Tiermarkt","Marché aux animaux","Mercato degli animali","動物市場","동물 시장","Mercado de animais","Рынок животных","Mercado de animales","Hayvan pazarı","Ринок тварин"},
            ["poi_garden"]=new[]{"Garden supplies","园艺用品店","園藝用品店","Gartenbedarf","Jardinerie","Articoli da giardino","園芸用品店","원예용품점","Artigos de jardim","Садовые товары","Artículos de jardín","Bahçe malzemeleri","Садові товари"},
            ["poi_clothing"]=new[]{"Clothing store","服装店","服裝店","Bekleidung","Vêtements","Abbigliamento","衣料品店","옷가게","Roupas","Одежда","Ropa","Giyim mağazası","Одяг"},
            ["poi_rest"]=new[]{"Rest point","休息点","休息點","Ruheplatz","Lieu de repos","Punto di riposo","休憩所","휴식 지점","Local de descanso","Место отдыха","Lugar de descanso","Dinlenme noktası","Місце відпочинку"},
            ["poi_orders"]=new[]{"Orders","订购处","訂購處","Bestellungen","Commandes","Ordini","注文所","주문소","Encomendas","Заказы","Pedidos","Siparişler","Замовлення"},
            ["poi_market"]=new[]{"Market","市场","市場","Markt","Marché","Mercato","市場","시장","Mercado","Рынок","Mercado","Pazar","Ринок"},
            ["poi_workshop"]=new[]{"Workshop","工坊","工坊","Werkstatt","Atelier","Officina","工房","작업장","Oficina","Мастерская","Taller","Atölye","Майстерня"},
            ["poi_farm"]=new[]{"Farm","农场","農場","Bauernhof","Ferme","Fattoria","農場","농장","Fazenda","Ферма","Granja","Çiftlik","Ферма"},
            ["poi_museum"]=new[]{"Museum","博物馆","博物館","Museum","Musée","Museo","博物館","박물관","Museu","Музей","Museo","Müze","Музей"},
            ["SelectMarker"]=new[]{"Select a marker to edit","选择标记以编辑","選擇標記以編輯","Markierung zum Bearbeiten wählen","Sélectionnez un repère à modifier","Seleziona un segnaposto da modificare","編集するマーカーを選択","편집할 마커를 선택하세요","Selecione um marcador para editar","Выберите метку для изменения","Selecciona un marcador para editar","Düzenlemek için işaret seçin","Виберіть мітку для редагування"},
            ["CurrentTarget"]=new[]{"Current target","当前目标","目前目標","Aktuelles Ziel","Destination actuelle","Destinazione attuale","現在の目標","현재 목표","Destino atual","Текущая цель","Destino actual","Mevcut hedef","Поточна ціль"},
            ["SetTarget"]=new[]{"Set as target","设为目标","設為目標","Als Ziel setzen","Définir comme destination","Imposta destinazione","目標に設定","목표로 설정","Definir como destino","Назначить целью","Fijar como destino","Hedef olarak belirle","Призначити ціллю"},
            ["NoTarget"]=new[]{"No target selected","尚未选择目标","尚未選擇目標","Kein Ziel ausgewählt","Aucune destination sélectionnée","Nessuna destinazione selezionata","目標が選択されていません","선택한 목표가 없습니다","Nenhum destino selecionado","Цель не выбрана","Ningún destino seleccionado","Hedef seçilmedi","Ціль не вибрано"},
            ["YourPosition"]=new[]{"Your position","你的位置","你的位置","Deine Position","Votre position","La tua posizione","現在地","내 위치","Sua posição","Вы здесь","Tu posición","Konumun","Ваше розташування"},
            ["NE"]=new[]{"NE","东北","東北","NO","NE","NE","北東","북동","NE","СВ","NE","KD","ПнСх"},
            ["SE"]=new[]{"SE","东南","東南","SO","SE","SE","南東","남동","SE","ЮВ","SE","GD","ПдСх"},
            ["SW"]=new[]{"SW","西南","西南","SW","SO","SO","南西","남서","SO","ЮЗ","SO","GB","ПдЗх"},
            ["NW"]=new[]{"NW","西北","西北","NW","NO","NO","北西","북서","NO","СЗ","NO","KB","ПнЗх"},
            ["North"]=new[]{"N","北","北","N","N","N","北","북","N","С","N","K","Пн"},
            ["East"]=new[]{"E","东","東","O","E","E","東","동","L","В","E","D","Сх"},
            ["South"]=new[]{"S","南","南","S","S","S","南","남","S","Ю","S","G","Пд"},
            ["West"]=new[]{"W","西","西","W","O","O","西","서","O","З","O","B","Зх"},
            ["NorthUp"]=new[]{"North up","固定正北","固定正北","Norden oben","Nord en haut","Nord in alto","北を上に固定","북쪽 고정","Norte para cima","Север сверху","Norte arriba","Kuzey yukarı","Північ угорі"},
            ["CameraUp"]=new[]{"Camera up","随视角转动","隨視角轉動","Blickrichtung oben","Selon la caméra","Segui visuale","視点に追従","시점 따라 회전","Seguir câmera","По направлению взгляда","Seguir cámara","Kamerayı izle","За напрямком погляду"},
            ["Map"]=new[]{"Map","地图","地圖","Karte","Carte","Mappa","地図","지도","Mapa","Карта","Mapa","Harita","Мапа"},
            ["NoMap"]=new[]{"Map unavailable for this region","此区域暂无地图","此區域暫無地圖","Keine Karte für dieses Gebiet","Carte indisponible pour cette zone","Mappa non disponibile per questa zona","このエリアの地図はありません","이 지역의 지도가 없습니다","Mapa indisponível nesta região","Карта этой области недоступна","Mapa no disponible en esta región","Bu bölgenin haritası yok","Мапа цієї області недоступна"},
            ["Target"]=new[]{"Target","目标","目標","Ziel","Destination","Destinazione","目標","목표","Destino","Цель","Destino","Hedef","Ціль"},
            ["Coordinates"]=new[]{"Coordinates","坐标","座標","Koordinaten","Coordonnées","Coordinate","座標","좌표","Coordenadas","Координаты","Coordenadas","Koordinatlar","Координати"},
            ["center"]=new[]{"Center on player","回到玩家","回到玩家","Zum Spieler","Centrer sur le joueur","Centra sul giocatore","プレイヤーに戻る","플레이어 중심","Centralizar jogador","К игроку","Centrar jugador","Oyuncuya dön","До гравця"},
            ["close"]=new[]{"Close","关闭","關閉","Schließen","Fermer","Chiudi","閉じる","닫기","Fechar","Закрыть","Cerrar","Kapat","Закрити"},
            ["help"]=new[]{"Scroll: zoom  •  Drag: pan  •  Right click: add marker","滚轮缩放 · 拖动平移 · 右键添加标记","滾輪縮放 · 拖動平移 · 右鍵新增標記","Mausrad: Zoom · Ziehen: Verschieben · Rechtsklick: Markierung","Molette : zoom · Glisser : déplacer · Clic droit : repère","Rotella: zoom · Trascina: sposta · Clic destro: segnaposto","ホイール：拡大縮小 · ドラッグ：移動 · 右クリック：マーカー","휠: 확대/축소 · 드래그: 이동 · 우클릭: 마커","Rolar: zoom · Arrastar: mover · Clique direito: marcador","Колесо: масштаб · Перетаскивание: сдвиг · ПКМ: метка","Rueda: zoom · Arrastrar: mover · Clic derecho: marcador","Tekerlek: yakınlaştır · Sürükle: kaydır · Sağ tık: işaret","Колесо: масштаб · Перетягування: зсув · ПКМ: мітка"},
            ["marker"]=new[]{"Marker","标记","標記","Markierung","Repère","Segnaposto","マーカー","마커","Marcador","Метка","Marcador","İşaret","Мітка"},
            ["rename"]=new[]{"Rename","重命名","重新命名","Umbenennen","Renommer","Rinomina","名前を変更","이름 변경","Renomear","Переименовать","Renombrar","Yeniden adlandır","Перейменувати"},
            ["delete"]=new[]{"Delete","删除","刪除","Löschen","Supprimer","Elimina","削除","삭제","Excluir","Удалить","Eliminar","Sil","Видалити"},
            ["color"]=new[]{"Color","颜色","顏色","Farbe","Couleur","Colore","色","색상","Cor","Цвет","Color","Renk","Колір"},
            ["icon"]=new[]{"Icon","图标","圖示","Symbol","Icône","Icona","アイコン","아이콘","Ícone","Значок","Icono","Simge","Значок"},
            ["clear_target"]=new[]{"Clear target","取消目标","取消目標","Ziel entfernen","Effacer la destination","Rimuovi destinazione","目標を解除","목표 해제","Limpar destino","Убрать цель","Quitar destino","Hedefi kaldır","Прибрати ціль"},
            ["temporary"]=new[]{"Markers are temporary: save identity unavailable","未识别存档，标记仅本次有效","未識別存檔，標記僅本次有效","Spielstand unbekannt: Markierungen temporär","Sauvegarde inconnue : repères temporaires","Salvataggio sconosciuto: segnaposti temporanei","セーブ未識別：マーカーは一時的です","저장 정보 없음: 임시 마커","Partida desconhecida: marcadores temporários","Сохранение не определено: метки временные","Partida desconocida: marcadores temporales","Kayıt bilinmiyor: işaretler geçici","Збереження не визначено: мітки тимчасові"},
            ["save_error"]=new[]{"Could not save markers","标记保存失败","標記儲存失敗","Markierungen konnten nicht gespeichert werden","Échec de l’enregistrement des repères","Impossibile salvare i segnaposti","マーカーを保存できません","마커를 저장할 수 없습니다","Não foi possível salvar marcadores","Не удалось сохранить метки","No se pudieron guardar los marcadores","İşaretler kaydedilemedi","Не вдалося зберегти мітки"},
            ["load_error"]=new[]{"Could not load markers; existing file preserved","标记读取失败，原文件已保留","標記讀取失敗，原檔案已保留","Markierungen nicht geladen; Datei erhalten","Repères non chargés ; fichier conservé","Segnaposti non caricati; file conservato","マーカー読込失敗・元ファイルは保持","마커 불러오기 실패; 원본 유지","Falha ao carregar; arquivo preservado","Метки не загружены; файл сохранён","Error al cargar; archivo conservado","İşaretler yüklenemedi; dosya korundu","Мітки не завантажено; файл збережено"}
        };
        public static string Normalize(string locale)
        {
            locale=(locale??"en").Replace('_','-').ToLowerInvariant();
            if(locale.StartsWith("zh")) return locale.Contains("hant")||locale.Contains("tw")||locale.Contains("hk")?"zh-Hant":"zh";
            var language=locale.Split('-')[0]; return Array.IndexOf(Languages,language)>=0?language:"en";
        }
        public static string Get(string locale,string key)
        { return Values.TryGetValue(key,out var row)?row[Array.IndexOf(Languages,Normalize(locale))]:key; }
    }
}
