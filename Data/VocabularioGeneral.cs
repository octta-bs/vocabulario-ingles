using VocabularioIngles.Models;

namespace VocabularioIngles.Data;

/// <summary>
/// Vocabulario general por niveles del MCER (Pre A1, A1 y A2), para empezar desde cero antes del inglés
/// técnico. 168 palabras en 14 categorías, escritas a mano: la traducción, "se lee" y los
/// ejemplos deben revisarse. Ordenado por niveles del MCER, pero NO copia las listas oficiales de Cambridge.
/// La pronunciación (IPA, inglés de EE. UU.) sale de open-dict-data/ipa-dict (MIT) y las formas de los verbos
/// de monolithpl/verb.forms.dictionary (MIT); este archivo se generó a partir de ellos.
/// </summary>
public static class VocabularioGeneral
{
    /// <param name="Pasado">Solo verbos: el pasado. El participio, igual.</param>
    public record PalabraSemilla(string Ingles, string Espanol, string Pronunciacion, string ComoSeLee,
        string Ejemplo, string EjemploEs, string? Pasado = null, string? Participio = null);

    public record CategoriaSemilla(string Nombre, NivelMcer Nivel, PalabraSemilla[] Palabras);

    public static readonly CategoriaSemilla[] Categorias =
    {
        new("Colores", NivelMcer.PreA1, new PalabraSemilla[]
        {
            new("red", "rojo", "/ˈɹɛd/", "réd", "My car is red.", "Mi coche es rojo."),
            new("blue", "azul", "/ˈbɫu/", "blú", "The sky is blue.", "El cielo es azul."),
            new("green", "verde", "/ˈɡɹin/", "grín", "The tree is green.", "El árbol es verde."),
            new("yellow", "amarillo", "/ˈjɛɫoʊ/", "yé-lou", "The sun is yellow.", "El sol es amarillo."),
            new("black", "negro", "/ˈbɫæk/", "blák", "My cat is black.", "Mi gato es negro."),
            new("white", "blanco", "/ˈhwaɪt/", "uáit", "The wall is white.", "La pared es blanca."),
            new("orange", "naranja (color)", "/ˈɔɹəndʒ/", "ó-rinch", "I have an orange shirt.", "Tengo una camisa naranja."),
            new("pink", "rosa", "/ˈpɪŋk/", "pínk", "She has a pink bag.", "Ella tiene una bolsa rosa."),
            new("purple", "morado", "/ˈpɝpəɫ/", "pér-pol", "The flower is purple.", "La flor es morada."),
            new("brown", "café (color)", "/ˈbɹaʊn/", "bráun", "The dog is brown.", "El perro es café."),
            new("gray", "gris", "/ˈɡɹeɪ/", "gréi", "The sky is gray today.", "El cielo está gris hoy."),
            new("color", "color", "/ˈkəɫɝ/", "cá-ler", "What color is your phone?", "¿De qué color es tu teléfono?")
        }),
        new("Números", NivelMcer.PreA1, new PalabraSemilla[]
        {
            new("one", "uno", "/ˈwən/", "uán", "I have one brother.", "Tengo un hermano."),
            new("two", "dos", "/ˈtu/", "tú", "I have two cats.", "Tengo dos gatos."),
            new("three", "tres", "/ˈθɹi/", "zrí", "We have three meetings today.", "Tenemos tres reuniones hoy."),
            new("four", "cuatro", "/ˈfɔɹ/", "fór", "The table has four legs.", "La mesa tiene cuatro patas."),
            new("five", "cinco", "/ˈfaɪv/", "fáiv", "I work five days a week.", "Trabajo cinco días a la semana."),
            new("six", "seis", "/ˈsɪks/", "síks", "I wake up at six.", "Me despierto a las seis."),
            new("seven", "siete", "/ˈsɛvən/", "sé-ven", "There are seven days in a week.", "Hay siete días en una semana."),
            new("eight", "ocho", "/ˈeɪt/", "éit", "The class starts at eight.", "La clase empieza a las ocho."),
            new("nine", "nueve", "/ˈnaɪn/", "náin", "My son is nine.", "Mi hijo tiene nueve años."),
            new("ten", "diez", "/ˈtɛn/", "tén", "I have ten fingers.", "Tengo diez dedos."),
            new("twenty", "veinte", "/ˈtwɛni/", "tuén-ti", "She is twenty years old.", "Ella tiene veinte años."),
            new("hundred", "cien", "/ˈhəndɝd/", "jón-dred", "The book has a hundred pages.", "El libro tiene cien páginas.")
        }),
        new("Animales", NivelMcer.PreA1, new PalabraSemilla[]
        {
            new("dog", "perro", "/ˈdɔɡ/", "dóg", "My dog is very friendly.", "Mi perro es muy amigable."),
            new("cat", "gato", "/ˈkæt/", "kát", "The cat is on the sofa.", "El gato está en el sofá."),
            new("bird", "pájaro", "/ˈbɝd/", "bérd", "A bird is in the tree.", "Hay un pájaro en el árbol."),
            new("fish", "pez", "/ˈfɪʃ/", "fích", "The fish is in the water.", "El pez está en el agua."),
            new("horse", "caballo", "/ˈhɔɹs/", "jórs", "The horse is fast.", "El caballo es rápido."),
            new("cow", "vaca", "/ˈkaʊ/", "káu", "The cow gives milk.", "La vaca da leche."),
            new("pig", "cerdo", "/ˈpɪɡ/", "píg", "The pig is pink.", "El cerdo es rosa."),
            new("duck", "pato", "/ˈdək/", "dák", "The duck is in the lake.", "El pato está en el lago."),
            new("rabbit", "conejo", "/ˈɹæbət/", "rá-bit", "The rabbit eats carrots.", "El conejo come zanahorias."),
            new("lion", "león", "/ˈɫaɪən/", "lá-ion", "The lion is big.", "El león es grande."),
            new("elephant", "elefante", "/ˈɛɫəfənt/", "é-le-fant", "The elephant is gray.", "El elefante es gris."),
            new("monkey", "mono", "/ˈməŋki/", "món-ki", "The monkey eats a banana.", "El mono come un plátano.")
        }),
        new("Familia", NivelMcer.PreA1, new PalabraSemilla[]
        {
            new("mother", "madre", "/ˈməðɝ/", "má-der", "My mother is a teacher.", "Mi madre es maestra."),
            new("father", "padre", "/ˈfɑðɝ/", "fá-der", "My father works in an office.", "Mi padre trabaja en una oficina."),
            new("brother", "hermano", "/ˈbɹəðɝ/", "brá-der", "My brother plays soccer.", "Mi hermano juega futbol."),
            new("sister", "hermana", "/ˈsɪstɝ/", "sís-ter", "My sister lives in Monterrey.", "Mi hermana vive en Monterrey."),
            new("son", "hijo", "/ˈsən/", "són", "My son is in school.", "Mi hijo está en la escuela."),
            new("daughter", "hija", "/ˈdɔtɝ/", "dó-ter", "My daughter likes to draw.", "A mi hija le gusta dibujar."),
            new("baby", "bebé", "/ˈbeɪbi/", "béi-bi", "The baby is sleeping.", "El bebé está dormido."),
            new("grandmother", "abuela", "/ˈɡɹændˌməðɝ/", "gránd-ma-der", "My grandmother makes great tamales.", "Mi abuela hace tamales riquísimos."),
            new("grandfather", "abuelo", "/ˈɡɹænˌfɑðɝ/", "gránd-fa-der", "My grandfather tells good stories.", "Mi abuelo cuenta buenas historias."),
            new("family", "familia", "/ˈfæməɫi/", "fá-mi-li", "I have a big family.", "Tengo una familia grande."),
            new("friend", "amigo", "/ˈfɹɛnd/", "frénd", "He is my best friend.", "Él es mi mejor amigo."),
            new("parents", "padres (mamá y papá)", "/ˈpɛɹənts/", "pé-rents", "My parents live in Puebla.", "Mis padres viven en Puebla.")
        }),
        new("El cuerpo", NivelMcer.PreA1, new PalabraSemilla[]
        {
            new("head", "cabeza", "/ˈhɛd/", "jéd", "My head hurts.", "Me duele la cabeza."),
            new("eye", "ojo", "/ˈaɪ/", "ái", "I have something in my eye.", "Tengo algo en el ojo."),
            new("ear", "oreja, oído", "/ˈiɹ/", "íer", "My ear hurts.", "Me duele el oído."),
            new("nose", "nariz", "/ˈnoʊz/", "nóus", "My nose is cold.", "Tengo la nariz fría."),
            new("mouth", "boca", "/ˈmaʊθ/", "máuz", "Open your mouth, please.", "Abre la boca, por favor."),
            new("hand", "mano", "/ˈhænd/", "jánd", "Raise your hand.", "Levanta la mano."),
            new("arm", "brazo", "/ˈɑɹm/", "árm", "My arm is tired.", "Tengo el brazo cansado."),
            new("leg", "pierna", "/ˈɫɛɡ/", "lég", "He broke his leg.", "Se rompió la pierna."),
            new("foot", "pie", "/ˈfʊt/", "fút", "My foot hurts.", "Me duele el pie."),
            new("hair", "cabello", "/ˈhɛɹ/", "jér", "She has long hair.", "Ella tiene el cabello largo."),
            new("face", "cara", "/ˈfeɪs/", "féis", "Wash your face.", "Lávate la cara."),
            new("back", "espalda", "/ˈbæk/", "bák", "My back hurts after work.", "Me duele la espalda después del trabajo.")
        }),
        new("Comida y bebida", NivelMcer.A1, new PalabraSemilla[]
        {
            new("water", "agua", "/ˈwɔtɝ/", "uó-ter", "I drink water every day.", "Tomo agua todos los días."),
            new("coffee", "café", "/ˈkɑfi/", "kó-fi", "I drink coffee in the morning.", "Tomo café en la mañana."),
            new("bread", "pan", "/ˈbɹɛd/", "bréd", "I buy bread every morning.", "Compro pan todas las mañanas."),
            new("egg", "huevo", "/ˈɛɡ/", "ég", "I eat an egg for breakfast.", "Como un huevo en el desayuno."),
            new("rice", "arroz", "/ˈɹaɪs/", "ráis", "We eat rice and beans.", "Comemos arroz y frijoles."),
            new("chicken", "pollo", "/ˈtʃɪkən/", "chí-ken", "The chicken soup is hot.", "La sopa de pollo está caliente."),
            new("apple", "manzana", "/ˈæpəɫ/", "á-pol", "I eat an apple every day.", "Como una manzana todos los días."),
            new("milk", "leche", "/ˈmɪɫk/", "mílk", "The milk is in the fridge.", "La leche está en el refrigerador."),
            new("cheese", "queso", "/ˈtʃiz/", "chíis", "I love cheese.", "Me encanta el queso."),
            new("breakfast", "desayuno", "/ˈbɹɛkfəst/", "brék-fast", "Breakfast is ready.", "El desayuno está listo."),
            new("lunch", "comida (de mediodía)", "/ˈɫəntʃ/", "lánch", "We have lunch at two.", "Comemos a las dos."),
            new("dinner", "cena", "/ˈdɪnɝ/", "dí-ner", "Dinner is at eight.", "La cena es a las ocho.")
        }),
        new("La casa", NivelMcer.A1, new PalabraSemilla[]
        {
            new("house", "casa", "/ˈhaʊs/", "jáus", "My house is small.", "Mi casa es pequeña."),
            new("kitchen", "cocina", "/ˈkɪtʃən/", "kí-chen", "My mother is in the kitchen.", "Mi mamá está en la cocina."),
            new("bedroom", "recámara", "/ˈbɛˌdɹum/", "béd-rum", "My bedroom is upstairs.", "Mi recámara está arriba."),
            new("bathroom", "baño", "/ˈbæˌθɹum/", "báz-rum", "Where is the bathroom?", "¿Dónde está el baño?"),
            new("door", "puerta", "/ˈdɔɹ/", "dór", "Please close the door.", "Por favor, cierra la puerta."),
            new("window", "ventana", "/ˈwɪndoʊ/", "uín-dou", "Open the window, please.", "Abre la ventana, por favor."),
            new("table", "mesa", "/ˈteɪbəɫ/", "téi-bol", "The keys are on the table.", "Las llaves están en la mesa."),
            new("chair", "silla", "/ˈtʃɛɹ/", "chér", "This chair is comfortable.", "Esta silla es cómoda."),
            new("bed", "cama", "/ˈbɛd/", "béd", "I go to bed at eleven.", "Me voy a la cama a las once."),
            new("sofa", "sofá", "/ˈsoʊfə/", "sóu-fa", "We watch TV on the sofa.", "Vemos la tele en el sofá."),
            new("floor", "piso", "/ˈfɫɔɹ/", "flór", "The floor is clean.", "El piso está limpio."),
            new("key", "llave", "/ˈki/", "kí", "I can't find my key.", "No encuentro mi llave.")
        }),
        new("La ropa", NivelMcer.A1, new PalabraSemilla[]
        {
            new("shirt", "camisa", "/ˈʃɝt/", "shért", "I wear a white shirt to work.", "Uso una camisa blanca para trabajar."),
            new("pants", "pantalones", "/ˈpænts/", "pánts", "These pants are too long.", "Estos pantalones están muy largos."),
            new("shoes", "zapatos", "/ˈʃuz/", "shús", "My shoes are new.", "Mis zapatos son nuevos."),
            new("dress", "vestido", "/ˈdɹɛs/", "drés", "She wears a red dress.", "Ella lleva un vestido rojo."),
            new("jacket", "chamarra", "/ˈdʒækət/", "yá-ket", "Take a jacket, it's cold.", "Llévate una chamarra, hace frío."),
            new("hat", "sombrero, gorra", "/ˈhæt/", "ját", "He wears a hat in the sun.", "Usa sombrero bajo el sol."),
            new("socks", "calcetines", "/ˈsɑks/", "sóks", "I need clean socks.", "Necesito calcetines limpios."),
            new("skirt", "falda", "/ˈskɝt/", "skért", "Her skirt is blue.", "Su falda es azul."),
            new("coat", "abrigo", "/ˈkoʊt/", "kóut", "Put on your coat.", "Ponte el abrigo."),
            new("glasses", "lentes", "/ˈɡɫæsəz/", "glá-ses", "I need my glasses to read.", "Necesito mis lentes para leer."),
            new("sweater", "suéter", "/ˈswɛtɝ/", "sué-ter", "This sweater is warm.", "Este suéter es calientito."),
            new("clothes", "ropa", "/ˈkɫoʊðz/", "klóuz", "I wash my clothes on Sunday.", "Lavo mi ropa el domingo.")
        }),
        new("Días, meses y tiempo", NivelMcer.A1, new PalabraSemilla[]
        {
            new("Monday", "lunes", "/ˈmənˌdeɪ/", "mán-dei", "I have a meeting on Monday.", "Tengo una reunión el lunes."),
            new("Friday", "viernes", "/ˈfɹaɪˌdeɪ/", "frái-dei", "On Friday we finish early.", "El viernes salimos temprano."),
            new("Saturday", "sábado", "/ˈsæˌtɪˌdeɪ/", "sá-ter-dei", "On Saturday I play soccer.", "El sábado juego futbol."),
            new("Sunday", "domingo", "/ˈsənˌdeɪ/", "són-dei", "On Sunday I visit my parents.", "El domingo visito a mis padres."),
            new("week", "semana", "/ˈwik/", "uík", "I study English every week.", "Estudio inglés cada semana."),
            new("month", "mes", "/ˈmənθ/", "mónz", "I get paid every month.", "Me pagan cada mes."),
            new("year", "año", "/ˈjɪɹ/", "íer", "This year I want a new job.", "Este año quiero un trabajo nuevo."),
            new("today", "hoy", "/təˈdeɪ/", "tu-déi", "Today is a good day.", "Hoy es un buen día."),
            new("tomorrow", "mañana", "/təˈmɑˌɹoʊ/", "tu-mó-rou", "See you tomorrow.", "Nos vemos mañana."),
            new("yesterday", "ayer", "/ˈjɛstɝˌdeɪ/", "iés-ter-dei", "Yesterday I fixed a bug.", "Ayer arreglé un error."),
            new("morning", "mañana (parte del día)", "/ˈmɔɹnɪŋ/", "mór-ning", "I run in the morning.", "Corro en la mañana."),
            new("night", "noche", "/ˈnaɪt/", "náit", "I study at night.", "Estudio en la noche.")
        }),
        new("Verbos básicos", NivelMcer.A1, new PalabraSemilla[]
        {
            new("eat", "comer", "/ˈit/", "ít", "I eat breakfast at seven.", "Desayuno a las siete.", "ate", "eaten"),
            new("drink", "beber, tomar", "/ˈdɹɪŋk/", "drínk", "I drink coffee every morning.", "Tomo café todas las mañanas.", "drank", "drunk"),
            new("sleep", "dormir", "/ˈsɫip/", "slíp", "I sleep eight hours.", "Duermo ocho horas.", "slept", "slept"),
            new("go", "ir", "/ˈɡoʊ/", "góu", "I go to work by bus.", "Voy al trabajo en autobús.", "went", "gone"),
            new("have", "tener", "/ˈhæv/", "jáv", "I have two brothers.", "Tengo dos hermanos.", "had", "had"),
            new("like", "gustar", "/ˈɫaɪk/", "láik", "I like pizza.", "Me gusta la pizza.", "liked", "liked"),
            new("want", "querer", "/ˈwɑnt/", "uónt", "I want a new job.", "Quiero un trabajo nuevo.", "wanted", "wanted"),
            new("speak", "hablar", "/ˈspik/", "spík", "I speak Spanish and English.", "Hablo español e inglés.", "spoke", "spoken"),
            new("walk", "caminar", "/ˈwɑk/", "uók", "I walk to the park every day.", "Camino al parque todos los días.", "walked", "walked"),
            new("play", "jugar", "/ˈpɫeɪ/", "pléi", "I play soccer on Saturday.", "Juego futbol el sábado.", "played", "played"),
            new("watch", "ver (la tele, una película)", "/ˈwɑtʃ/", "uách", "I watch movies at night.", "Veo películas en la noche.", "watched", "watched"),
            new("live", "vivir", "/ˈɫaɪv/", "lív", "I live in Mexico City.", "Vivo en la Ciudad de México.", "lived", "lived")
        }),
        new("La ciudad", NivelMcer.A2, new PalabraSemilla[]
        {
            new("street", "calle", "/ˈstɹit/", "strít", "My street is very quiet.", "Mi calle es muy tranquila."),
            new("park", "parque", "/ˈpɑɹk/", "párk", "We walk in the park.", "Caminamos en el parque."),
            new("hospital", "hospital", "/ˈhɑˌspɪtəɫ/", "jós-pi-tal", "The hospital is near my house.", "El hospital está cerca de mi casa."),
            new("school", "escuela", "/ˈskuɫ/", "skúl", "The school is close to my house.", "La escuela está cerca de mi casa."),
            new("bank", "banco", "/ˈbæŋk/", "bánk", "I need to go to the bank.", "Necesito ir al banco."),
            new("store", "tienda", "/ˈstɔɹ/", "stór", "The store opens at nine.", "La tienda abre a las nueve."),
            new("restaurant", "restaurante", "/ˈɹɛsˌtɹɑnt/", "rés-to-rant", "We eat at a Mexican restaurant.", "Comemos en un restaurante mexicano."),
            new("supermarket", "supermercado", "/ˈsupɝˌmɑɹkɪt/", "sú-per-mar-ket", "I buy food at the supermarket.", "Compro comida en el supermercado."),
            new("bus", "autobús", "/ˈbəs/", "bás", "I take the bus to work.", "Tomo el autobús al trabajo."),
            new("car", "coche", "/ˈkɑɹ/", "kár", "I drive my car to work.", "Voy al trabajo en mi coche."),
            new("bridge", "puente", "/ˈbɹɪdʒ/", "brídch", "The bridge is very long.", "El puente es muy largo."),
            new("corner", "esquina", "/ˈkɔɹnɝ/", "kór-ner", "The bank is on the corner.", "El banco está en la esquina.")
        }),
        new("Viajes", NivelMcer.A2, new PalabraSemilla[]
        {
            new("airport", "aeropuerto", "/ˈɛɹˌpɔɹt/", "ér-port", "The airport is far from the city.", "El aeropuerto está lejos de la ciudad."),
            new("ticket", "boleto", "/ˈtɪkət/", "tí-ket", "I need a ticket to Cancún.", "Necesito un boleto a Cancún."),
            new("passport", "pasaporte", "/ˈpæsˌpɔɹt/", "pás-port", "Don't forget your passport.", "No olvides tu pasaporte."),
            new("hotel", "hotel", "/hoʊˈtɛɫ/", "jo-tél", "The hotel has a pool.", "El hotel tiene alberca."),
            new("luggage", "equipaje", "/ˈɫəɡədʒ/", "lá-guich", "My luggage is heavy.", "Mi equipaje pesa mucho."),
            new("flight", "vuelo", "/ˈfɫaɪt/", "fláit", "My flight leaves at six.", "Mi vuelo sale a las seis."),
            new("trip", "viaje", "/ˈtɹɪp/", "tríp", "We have a trip next month.", "Tenemos un viaje el próximo mes."),
            new("beach", "playa", "/ˈbitʃ/", "bích", "The beach is beautiful.", "La playa es hermosa."),
            new("map", "mapa", "/ˈmæp/", "máp", "I use a map on my phone.", "Uso un mapa en mi teléfono."),
            new("reservation", "reservación", "/ˌɹɛzɝˈveɪʃən/", "re-ser-véi-shon", "I have a reservation for two nights.", "Tengo una reservación por dos noches."),
            new("train", "tren", "/ˈtɹeɪn/", "tréin", "The train is late.", "El tren viene retrasado."),
            new("suitcase", "maleta", "/ˈsutˌkeɪs/", "sút-keis", "My suitcase is blue.", "Mi maleta es azul.")
        }),
        new("Salud", NivelMcer.A2, new PalabraSemilla[]
        {
            new("doctor", "doctor", "/ˈdɑktɝ/", "dók-ter", "I need to see a doctor.", "Necesito ver a un doctor."),
            new("medicine", "medicina", "/ˈmɛdəsən/", "mé-di-sin", "Take this medicine after lunch.", "Toma esta medicina después de comer."),
            new("headache", "dolor de cabeza", "/ˈhɛˌdeɪk/", "jé-deik", "I have a headache.", "Me duele la cabeza."),
            new("fever", "fiebre", "/ˈfivɝ/", "fí-ver", "My son has a fever.", "Mi hijo tiene fiebre."),
            new("cough", "tos", "/ˈkɑf/", "kóf", "I have a bad cough.", "Tengo mucha tos."),
            new("cold", "resfriado; frío", "/ˈkoʊɫd/", "kóuld", "I have a cold.", "Tengo un resfriado."),
            new("pain", "dolor", "/ˈpeɪn/", "péin", "I have pain in my back.", "Tengo dolor en la espalda."),
            new("appointment", "cita", "/əˈpɔɪntmənt/", "a-póint-ment", "I have an appointment on Monday.", "Tengo una cita el lunes."),
            new("healthy", "sano, saludable", "/ˈhɛɫθi/", "jél-zi", "I want to be healthy.", "Quiero estar sano."),
            new("tired", "cansado", "/ˈtaɪɝd/", "táierd", "I am tired after work.", "Estoy cansado después del trabajo."),
            new("sick", "enfermo", "/ˈsɪk/", "sík", "I am sick today.", "Hoy estoy enfermo."),
            new("exercise", "ejercicio", "/ˈɛksɝˌsaɪz/", "ék-ser-sais", "Exercise is good for your health.", "El ejercicio es bueno para la salud.")
        }),
        new("Verbos comunes", NivelMcer.A2, new PalabraSemilla[]
        {
            new("buy", "comprar", "/ˈbaɪ/", "bái", "I buy fruit at the market.", "Compro fruta en el mercado.", "bought", "bought"),
            new("pay", "pagar", "/ˈpeɪ/", "péi", "I pay with my card.", "Pago con mi tarjeta.", "paid", "paid"),
            new("cook", "cocinar", "/ˈkʊk/", "kúk", "I cook dinner at night.", "Cocino la cena en la noche.", "cooked", "cooked"),
            new("wait", "esperar", "/ˈweɪt/", "uéit", "Please wait a minute.", "Por favor, espera un minuto.", "waited", "waited"),
            new("call", "llamar", "/ˈkɔɫ/", "kól", "I call my mother on Sunday.", "Le llamo a mi mamá el domingo.", "called", "called"),
            new("help", "ayudar", "/ˈhɛɫp/", "jélp", "Can you help me?", "¿Me puedes ayudar?", "helped", "helped"),
            new("ask", "preguntar", "/ˈæsk/", "ásk", "I ask a question in class.", "Hago una pregunta en clase.", "asked", "asked"),
            new("answer", "contestar", "/ˈænsɝ/", "án-ser", "Please answer the phone.", "Por favor, contesta el teléfono.", "answered", "answered"),
            new("understand", "entender", "/ˌəndɝˈstænd/", "an-der-stánd", "I understand the problem.", "Entiendo el problema.", "understood", "understood"),
            new("forget", "olvidar", "/fɝˈɡɛt/", "for-guét", "I always forget my keys.", "Siempre olvido mis llaves.", "forgot", "forgotten"),
            new("remember", "recordar", "/ɹiˈmɛmbɝ/", "ri-mém-ber", "I remember your name.", "Recuerdo tu nombre.", "remembered", "remembered"),
            new("choose", "elegir", "/ˈtʃuz/", "chús", "I choose the blue shirt.", "Elijo la camisa azul.", "chose", "chosen")
        })
    };
}
