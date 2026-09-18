using System;
using System.Collections.Generic;

namespace OldMarket.TreeInfo
{
    internal sealed class Texts
    {
        internal readonly string Season, AllSeasons, Ready, OffSeason, TooLate, Days, Dry;
        private Texts(string season, string all, string ready, string off, string late, string days, string dry)
        { Season = season; AllSeasons = all; Ready = ready; OffSeason = off; TooLate = late; Days = days; Dry = dry; }

        private static readonly Dictionary<string, Texts> Languages = new Dictionary<string, Texts>
        {
            ["en"] = new Texts("Production season: {0}", "All seasons", "Ripe — ready to harvest", "Not a production season", "Not enough days left this season", "Days until ripe: {0} (water daily)", "Not watered today"),
            ["zh"] = new Texts("生产季节：{0}", "四季", "已成熟，可采收", "本季不生产", "本季剩余天数不足以成熟", "预计还有 {0} 天成熟（每天浇水）", "今日未浇水"),
            ["zh-hant"] = new Texts("生產季節：{0}", "四季", "已成熟，可採收", "本季不生產", "本季剩餘天數不足以成熟", "預計還有 {0} 天成熟（每天澆水）", "今日未澆水"),
            ["de"] = new Texts("Erntesaison: {0}", "Alle Jahreszeiten", "Reif, bereit zur Ernte", "Keine Erntesaison", "Diese Jahreszeit reicht nicht mehr zum Reifen", "Tage bis zur Reife: {0} (täglich gießen)", "Heute noch nicht gegossen"),
            ["fr"] = new Texts("Saison de production : {0}", "Toutes les saisons", "Mûr, prêt à récolter", "Hors saison de production", "Pas assez de jours restants cette saison", "Jours avant maturité : {0} (arroser chaque jour)", "Pas encore arrosé aujourd’hui"),
            ["it"] = new Texts("Stagione di produzione: {0}", "Tutte le stagioni", "Maturo, pronto per la raccolta", "Fuori stagione di produzione", "Non restano abbastanza giorni in questa stagione", "Giorni alla maturazione: {0} (annaffiare ogni giorno)", "Non ancora annaffiato oggi"),
            ["ja"] = new Texts("生産する季節：{0}", "全季節", "成熟済み・収穫可能", "今の季節は生産しません", "今季中の成熟には日数が足りません", "成熟まであと約{0}日（毎日水やり）", "今日は水やりしていません"),
            ["ko"] = new Texts("생산 계절: {0}", "모든 계절", "다 익음 · 수확 가능", "이번 계절에는 생산하지 않음", "이번 계절 안에 익기에는 남은 날이 부족함", "익기까지 약 {0}일 (매일 물주기)", "오늘 물을 주지 않음"),
            ["pt"] = new Texts("Estação de produção: {0}", "Todas as estações", "Maduro, pronto para colher", "Fora da estação de produção", "Não restam dias suficientes nesta estação", "Dias até amadurecer: {0} (regar diariamente)", "Ainda não foi regado hoje"),
            ["ru"] = new Texts("Сезон плодоношения: {0}", "Все сезоны", "Созрело, можно собирать", "Сейчас не сезон плодоношения", "До конца сезона не успеет созреть", "Дней до созревания: {0} (поливать ежедневно)", "Сегодня ещё не полито"),
            ["es"] = new Texts("Temporada de producción: {0}", "Todas las estaciones", "Maduro, listo para cosechar", "Fuera de temporada de producción", "No quedan suficientes días esta temporada", "Días hasta madurar: {0} (regar a diario)", "Todavía no se ha regado hoy"),
            ["tr"] = new Texts("Üretim mevsimi: {0}", "Tüm mevsimler", "Olgun, hasada hazır", "Üretim mevsimi değil", "Bu mevsimde olgunlaşmak için yeterli gün kalmadı", "Olgunlaşmaya kalan gün: {0} (her gün sulayın)", "Bugün sulanmadı"),
            ["uk"] = new Texts("Сезон плодоношення: {0}", "Усі сезони", "Дозріло, можна збирати", "Зараз не сезон плодоношення", "До кінця сезону не встигне дозріти", "Днів до дозрівання: {0} (поливати щодня)", "Сьогодні ще не полито")
        };

        internal static Texts For(string locale)
        {
            string code = (locale ?? "en").Trim().Replace('_', '-').ToLowerInvariant();
            if (Languages.TryGetValue(code, out var exact)) return exact;
            var parts = code.Split('-');
            if (parts[0] == "zh")
            {
                // An explicit script takes precedence over the regional alias.
                if (Array.IndexOf(parts, "hans") >= 0) return Languages["zh"];
                if (Array.IndexOf(parts, "hant") >= 0 || Array.IndexOf(parts, "tw") >= 0 ||
                    Array.IndexOf(parts, "hk") >= 0 || Array.IndexOf(parts, "mo") >= 0) return Languages["zh-hant"];
            }
            return Languages.TryGetValue(parts[0], out var language) ? language : Languages["en"];
        }
    }
}
