# Create Special Request Sample

## Endpoint

POST https://localhost:7309/api/specials

## Headers

```http
Content-Type: application/json
Authorization: Bearer <your-jwt-token>
```

## Body

```json
{
  "venueId": 1,
  "categoryId": 1,
  "title": "Happy Hour Special",
  "description": "50% off all coffee drinks from 3-5 PM",
  "startDate": "2025-06-27",
  "endDate": "2025-12-31",
  "startTime": "15:00",
  "endTime": "17:00",
  "isAllDay": false,
  "isRecurring": true,
  "recurringPattern": "daily",
  "recurringDays": [1, 2, 3, 4, 5],
  "isActive": true,
  "maxRedemptions": 100,
  "currentRedemptions": 0
}
```

## Expected Response

```json
{
  "success": true,
  "message": "Special created successfully.",
  "data": {
    "id": 123,
    "venueId": 1,
    "categoryId": 1,
    "title": "Happy Hour Special",
    "description": "50% off all coffee drinks from 3-5 PM",
    "startDate": "2025-06-27",
    "endDate": "2025-12-31",
    "startTime": "15:00",
    "endTime": "17:00",
    "isAllDay": false,
    "isRecurring": true,
    "recurringPattern": "daily",
    "recurringDays": [1, 2, 3, 4, 5],
    "isActive": true,
    "maxRedemptions": 100,
    "currentRedemptions": 0,
    "venue": {
      "id": 1,
      "name": "The Coffee Corner"
    },
    "category": {
      "id": 1,
      "name": "Food & Drink",
      "icon": "🍽️"
    }
  }
}
```
