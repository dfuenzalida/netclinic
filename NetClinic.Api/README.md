# NetClinic - a .NET core port of the Spring Petclinic

# Running

* Run `docker-compose up` on the parent directory to start the Postgres container that contains the data.
* Run `dotnet run` to start the API in development mode.

# API

## `GET /vets`

Lists veterinarians and their specialty details

```javascript
{
  "vetList": [
    {
      "id": 1,
      "firstName": "James",
      "lastName": "Carter",
      "specialties": []
    },
    {
      "id": 2,
      "firstName": "Helen",
      "lastName": "Leary",
      "specialties": [
        {
          "id": 1,
          "name": "radiology"
        }
      ]
    }
  ]
}
```

## `GET /owners?lastName=fra`

Lists owners whose Last Name starts with the given value, including their Pet names

```javascript
{
    "ownerList": [
        {
            "id": 1,
            "firstName": "George",
            "lastName": "Franklin",
            "address": "110 W. Liberty St.",
            "city": "Madison",
            "telephone": "6085551023"
            "pets": [
                "Leo"
            ]
        }
    ]
}
```

## `GET /owners/:id`

Shows the details of an owner, their pets and each visit of each pet

```javascript
{
  "owner": {
      "id": 1,
      "firstName": "George",
      "lastName": "Franklin",
      "address": "110 W. Liberty St.",
      "city": "Madison",
      "telephone": "6085551023"
      "pets": [
          {
            "id": 1,
            "name": "Leo",
            "birthDate": "2010-09-07",
            "type": "cat",
            "visits": [
              "date": "2025-11-12",
              "description": "Rabies shot and yearly check"
            ]
          }
      ]
    }
}

```