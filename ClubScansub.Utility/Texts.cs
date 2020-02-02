using System;
using System.Collections.Generic;
using System.Text;

namespace ClubScansub.Utility
{
    public class Texts
    {
        public static string WelcomeMail(string name, string username)
        {
            var text = $"Hej {name}" +
                $"Du er blevet oprettet med et basis medlemskab og en turkonto i Dykkerklubben Scansub." +
                $"<br/>" +
                $"Dit medlemsnummer er: <strong>{username}</strong> <br/>" +
                $"<br/>" +
                $"Dit kodeord blive tilsendt på en SMS af sikkerhedsmæssige årsager, så hvis du ikke har opgivet et mobiltelefon i din registreringsformular, " +
                $"bedes du henvende dig i butikken eller på nedenstående telefonnummer.<br/>" +
                $"<<br/>" +
                $"Du kan ændre dit password ved at logge ind på <a href='http://kalender.scansub.dk'>kalender.scansub.dk</a> og gå til min 'Min konto' i øverste højre hjørne.<br/>" +
                $"Her kan du så administrere kodeord, kontaktoplysninger samt tilknytte en facebook-konto så du fremover kan logge på med denne. <br/>" +
                $"Husk at få verificeret dit tlf. nr. samt din mail adresse" +
                $"";
            return text;
        }

        public static string StudentWelcomeMail(string name, string username)
        {
            var text = $"Hej {name}" +
                $"Du er blevet oprettet som kursist i Dykkerklubben Scansub." +
                $"<br/>" +
                $"Dit medlemsnummer er: <strong>{username}</strong> <br/>" +
                $"<br/>" +
                $"Dit kodeord blive tilsendt på en SMS af sikkerhedsmæssige årsager, så hvis du ikke har opgivet et mobiltelefon i din registreringsformular, " +
                $"bedes du henvende dig i butikken eller på nedenstående telefonnummer.<br/>" +
                $"<<br/>" +
                $"Du kan ændre dit password ved at logge ind på <a href='http://kalender.scansub.dk'>kalender.scansub.dk</a> og gå til min 'Min konto' i øverste højre hjørne.<br/>" +
                $"Her kan du så administrere kodeord, kontaktoplysninger samt tilknytte en facebook-konto så du fremover kan logge på med denne. <br/>" +
                $"Husk at få verificeret dit tlf. nr. samt din mail adresse" +
                $"";
            return text;
        }

        public static string RegardsText()
        {
            return $"Med venlig hilsen <br />" +
                $"<br/>" +
                $"John Karlsen <br/>" +
                $"<br/>" +
                $"Scansub DK Diver<br/>" +
                $"Industrivej 51F<br/>" +
                $"4000 Roskilde<br/>" +
                $"Tlf. + 45 46 75 05 75<br/>" +
                $"e - mail            info@dkdiver.dk<br/>" +
                $"web               www.dkdiver.dk<br/>" +
                $"<br/>" +
                $"Forretningens åbningstider:<br/>" +
                $"Man - Tors      Kl.  09.00 - 18.00<br/>" +
                $"Fredag           Kl.  09.00 - 18.00<br/>" +
                $"Lørdag           Kl.  09.00 - 15.00<br/>" +
                $"";

        }

        public static string DepositText()
        {
            return $"<br/>" +
                $"Hvis du ønsker at indsætte penge på din Turkonto, kan det gøres på følgende 3 måder:<br/>" +
                $"<br/>" +
                $"1.<br/>" +
                $"Via Webbank til vores bankkonto i Sparekassen Sjælland-Fyn:<br/>" +
                $"Reg.nr. 0520 - Konto nr. 727989<br/>" +
                $"opgiv blot dit navn og debitornummer så indsætter vi beløbet på din Turkonto.<br/>" +
                $"<br/>" +
                $"2.<br/>" +
                $"Eller du kan indsætte penge på din konto ved at henvende dig i vores forretning.<br/>" +
                $"<br/>" +
                $"3.<br/>" +
                $"Eller du kan gå ind i vores <a href='http://www.dkdiver.dk'>internet butik</a>  <br/>" +
                $"- under ture, rejser og klub<br/>" +
                $"- vælg varen der hedder indsæt på Turkonto til 1 kr.<br/>" +
                $"- Husk at rette varen, til det stk.antal du ønsker at indsætte på Turkontoen<br/>" +
                $"- eller vælg inde på varen hvilket beløb du ønsker at indsætte<br/>" +
                $"-Put varen i indkøbskurven og gå til dankort betaling<br/>" +
                $"<br/>";

        }

        public static string EventAccountTerms()
        {
            return $"<ul><li>Turkontoen kan <strong>kun</strong> anvendes i forbindelse med tilmelding til aktiviter.</li>" +
                $"<li>Turkontoen kan <strong>IKKE</strong> anvendes til kursus tilmelding, tilmelding til rejser eller vare køb i butikken!</li>" +
                $"<li>Det er <strong>IKKE</strong> muligt at få refunderet indbetalinger på tur kontoen, de kan kun anvendes til dykker ture!</li></ul>" +
                $"<br/>" +
                $"<br/>" +
                $"Ved tilmelding til aktiviteter trækkes tur gebyret automatisk fra din Turkonto, det " +
                $"vil derfor kun være muligt at tilmelde sig ture hvis saldoen på din Turkonto " +
                $"som minimum modsvarer aktivitetsgebyret.<br/>" +
                $"<br/>";
        }

        public static string EventTerms()
        {
            return $"Du kan tilmelde dig aktiviteter online ved at logge på din konto på <a href='http://kalender.scansub.dk'>kalender.scansub.dk</a><br/>" +
                $"Du kan kun tilmelde dig aktiviteter, hvis du har dækning på din turkonto.<br/>" +
                $"<br/>" +
                $"Under menupunktet 'Klubmedlem' kan du følge med i dine tilmeldinger og status på de aktiviteter, du har tilmeldt dig.<br/>" +
                $"Under 'Min konto' kan du se din turkonto samt vedligeholde dine medlemsdata, certifikater, ændre password m.m.<br/>" +
                $"<br/>" +
                $"Bemærk venligst!<br/>" +
                $"Tilmeldinger er som standard bindende, men hvis du får behov kan du godt afmelde dig fra en aktivitet, men vi har følgende regler:<br/>" +
                $"<br/>" +
                $"<ol><li>Hvis du afmelder dig mere end 7 dage før afvikling af aktiviteten refunderer vi det fulde beløb til din turkonto.</li>" +
                $"<li>Afmelder du dig mellem 7 dage og 24 timer før, refunderer vi kun halvdelen til din turkonto.</li>" +
                $"<li>Hvis din afmelding sker senere end 24 timer før, refunderer vi intet.</li></ol>" +
                $"<br/>" +
                $"Flerdagsaktiviteter og Udlandsrejser kan dog have særlige regler for tilmelding og afmelding " +
                $"som vil være beskrevet under den enkelte aktivitet.<br/>" +
                $"<br/>" +
                $"Afmelding fra aktiviter, udlandsture og rejser kan kun ske ved at ringe på tlf. 46 75 05 75 " +
                $"eller skrive mail til info@dkdiver.dk<br/>" +
                $"<br/>";

        }

        public static string StudentTerms()
        {
            return $"Dit kursus medlemesskab giver dig adgang til vores kursus kalender på <a href='http://kalender.scansub.dk'>kalender.scansub.dk</a><br />" +
                $"Her akn du se dine kurser og finde kursusplnaner, mødetider og steder, og du kan samtidig kan skrive dig op til andre kurser. <br />" +
                $"Du kan også se vores andre aktivitieter Dykkerture, ferier, rejser m.m. men for at kunne tilmelde dig disse skal du bede om et gratis basis-medlemsskab af klubben<br />" +
                $"Dette er giver dig en række fordele - se disse på <a href='http:// www.dkdiver.dk/klubben'>Dykkerklubben Scansub DK Diver</a><br />";
        }
    }
}
