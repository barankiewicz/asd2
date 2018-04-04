
namespace ASD
{

[System.Serializable]
public class Cut
    {

    public int length;       // wymiar pionowy elementu (przed cięciem)
    public int width;        // wymiar poziomy elementu (przed cięciem)
    public int price;        // sumaryczna wartość wszystkich elementów uzyskanych z pocięcia tego elementu

    public bool vertical;    // true dla cięcia pionowego, false dla cięcia poziomego
    public int n;            // odległość od lewej (dla cięcia pionowego) lub górnej (dla cięcia poziomego) krawędzi elementu
                             // UWAGA:  wartość 0 oznacza brak cięcia, składowe topleft i bottomright muszą być równe null,
                             //         a do składowej price wpisujemy zadaną wartość elementu (gdy jest jednym z pożądanych
                             //         lub 0 gdy nie jest (czyli jest bezwartościowym ścinkiem)

    public Cut topleft;      // informacje o lewym/górnym elemencie uzustanym w wyniku dokonanego cięcia
    public Cut bottomright;  // informacje o prawym/dolnym elemencie uzustanym w wyniku dokonanego cięcia

    public Cut() {}  // wymagame przez system

    public Cut(int length, int width, int price, bool vertical=true, int n=0, Cut topleft=null, Cut bottomright=null)
        {
        this.length = length;
        this.width = width;
        this.price = price;
        this.vertical = vertical;
        this.n = n;
        this.topleft = topleft;
        this.bottomright = bottomright;
        }

    }

}