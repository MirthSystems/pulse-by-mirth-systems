# Venue Search Request Sample

## Endpoint

POST https://localhost:7309/api/venues/search

## Headers

```http
Content-Type: application/json
```

## Body

```json
{
  "searchTerm": "coffee",
  "categoryId": 1,
  "latitude": 47.6062,
  "longitude": -122.3321,
  "radiusInMeters": 5000,
  "isActiveOnly": true,
  "hasActiveSpecials": false,
  "pageNumber": 1,
  "pageSize": 10,
  "sortBy": "distance",
  "sortOrder": "asc"
}
```

## Expected Response

```json
{
  "success": true,
  "message": "Venues found successfully.",
  "data": {
    "items": [
      {
        "id": 1,
        "name": "The Coffee Corner",
        "description": "A cozy coffee shop",
        "streetAddress": "123 Main Street",
        "locality": "Seattle",
        "categoryName": "Restaurant",
        "categoryIcon": "🍽️",
        "distanceInMeters": 250.5,
        "isActive": true,
        "activeSpecialsCount": 2
      }
    ],
    "totalItems": 1,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  }
}
```
