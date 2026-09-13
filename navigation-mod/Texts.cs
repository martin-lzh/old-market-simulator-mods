using System;
using System.Collections.Generic;
namespace OldMarket.Navigation
{
    public static class Texts
    {
        private static readonly string[] Languages={"en","zh","zh-Hant","de","fr","it","ja","ko","pt","ru","es","tr","uk"};
        private static readonly Dictionary<string,string[]> Values=new Dictionary<string,string[]> {
            ["ZoomHint"]=new[]{"Zoom out / in","缩小 / 放大","縮小 / 放大","Verkleinern / Vergrößern","Dézoomer / Zoomer","Riduci / Ingrandisci","縮小 / 拡大","축소 / 확대","Diminuir / Ampliar","Отдалить / Приблизить","Alejar / Acercar","Uzaklaştır / Yakınlaştır","Віддалити / Наблизити"},
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
