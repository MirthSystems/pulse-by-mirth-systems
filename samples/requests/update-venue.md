# Update Venue Request Sample

## Endpoint

PUT https://localhost:7309/api/venues/{id}

## Headers

```http
Content-Type: application/json
Authorization: Bearer <your-jwt-token>
```

## Body

```json
{
  "categoryId": 1,
  "name": "The Coffee Corner - Updated",
  "description": "A renovated cozy coffee shop with artisanal brews, homemade pastries, and live music",
  "phoneNumber": "+1-555-123-4567",
  "website": "https://thecoffeecorner.com",
  "email": "info@thecoffeecorner.com",
  "streetAddress": "123 Main Street",
  "secondaryAddress": "Suite 101",
  "locality": "Seattle",
  "region": "WA",
  "postalCode": "98101",
  "country": "United States",
  "latitude": 47.6062,
  "longitude": -122.3321,
  "isActive": true,
  "businessHours": [
    {
      "dayOfWeekId": 1,
      "openTime": "09:00",
      "closeTime": "19:00",
      "isClosed": false
    },
    {
      "dayOfWeekId": 2,
      "openTime": "07:00",
      "closeTime": "21:00",
      "isClosed": false
    },
    {
      "dayOfWeekId": 3,
      "openTime": "07:00",
      "closeTime": "21:00",
      "isClosed": false
    },
    {
      "dayOfWeekId": 4,
      "openTime": "07:00",
      "closeTime": "21:00",
      "isClosed": false
    },
    {
      "dayOfWeekId": 5,
      "openTime": "07:00",
      "closeTime": "23:00",
      "isClosed": false
    },
    {
      "dayOfWeekId": 6,
      "openTime": "08:00",
      "closeTime": "23:00",
      "isClosed": false
    },
    {
      "dayOfWeekId": 7,
      "openTime": "10:00",
      "closeTime": "18:00",
      "isClosed": false
    }
  ]
}
```

## Expected Response

```json
{
  "success": true,
  "message": "Venue updated successfully.",
  "data": {
    "id": 1,
    "categoryId": 1,
    "name": "The Coffee Corner - Updated",
    "description": "A renovated cozy coffee shop with artisanal brews, homemade pastries, and live music",
    "businessHours": [
      {
        "id": 1,
        "venueId": 1,
        "dayOfWeekId": 1,
        "dayOfWeekName": "Sunday",
        "openTime": "09:00",
        "closeTime": "19:00",
        "isClosed": false
      }
    ]
  }
}
```
