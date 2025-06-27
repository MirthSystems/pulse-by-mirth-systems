# Create Venue Request Sample

## Endpoint
POST https://localhost:7309/api/venues

## Headers
```
Content-Type: application/json
Authorization: Bearer <your-jwt-token>
```

## Body
```json
{
  "categoryId": 1,
  "name": "The Coffee Corner",
  "description": "A cozy coffee shop with artisanal brews and homemade pastries",
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
      "openTime": "08:00",
      "closeTime": "20:00",
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
      "closeTime": "22:00",
      "isClosed": false
    },
    {
      "dayOfWeekId": 6,
      "openTime": "08:00",
      "closeTime": "22:00",
      "isClosed": false
    },
    {
      "dayOfWeekId": 7,
      "isClosed": true
    }
  ]
}
```

## Expected Response
```json
{
  "success": true,
  "message": "Venue created successfully.",
  "data": {
    "id": 123,
    "categoryId": 1,
    "name": "The Coffee Corner",
    "description": "A cozy coffee shop with artisanal brews and homemade pastries",
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
    "category": {
      "id": 1,
      "name": "Restaurant",
      "icon": "🍽️"
    },
    "businessHours": [
      {
        "id": 1,
        "venueId": 123,
        "dayOfWeekId": 1,
        "dayOfWeekName": "Sunday",
        "openTime": "08:00",
        "closeTime": "20:00",
        "isClosed": false
      }
    ],
    "activeSpecials": []
  }
}
```
