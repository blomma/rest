Välkommen till denna snutt kod, eftersom den är skriven i .Net core så kan man köra den direkt i linux, dock så kräver det att man har installerat SDK, så för att slippa det så finns det en Dockerfile inkluderad som man kan bygga och köra en instans ifrån.

Den är skriven i C#, där adsen persisterar till en sqlite databas via EntityFramework. Enjoy.

# Build

Givet att man står i samma folder som denna vackra README så är det bara att köra

    docker build -t adservice .

# Run

    docker run -p 5000:5000 -d --name ads adservice

# Use

## Get ads

Hämtar alla ads som finns, många eller få, det är helt upp till dig och hur många du har skapat.

    GET http://localhost:5000/api/ads

## Get ads sorterade

Samma som ovan fast sorterade efter önskemål, alla props kan sorteras på.

    GET http://localhost:5000/api/ads?OrderBy=createdUtc:desc&ThenBy=price

Sortering börjar med OrderBy={prop} som sen följs av 0 eller flera ThenBy={prop}, riktningen på sorteringen är som default asc. men kan anges som desc genom formen {prop}:desc

## Get ad

Hämtar enskild ad, id är propen id på den ad man vill hämta.

    GET http://localhost:5000/api/ads/{id}

## Delete ad

Sorgligt men sant, ibland så har en ad levt klart sitt liv och här finns möjligheten att säga hej då.

    DELETE http://localhost:5000/api/ads/{id}

## Create ad

Själva skapelseprocessen i arbete här, svaret tillbaka kommer, om allt har gått bra, innehålla två ytterligare props, nämligen CreatedUtc och Id.

    POST http://localhost:5000/api/ads/

med en json body på formen

    {
        "subject":"aSubject",
        "body": "aBody",
        "email": "aEmail",
        "price": 1
    }

där price är optional
