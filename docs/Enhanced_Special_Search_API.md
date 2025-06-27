# Enhanced Special Search API Documentation

## Overview

The enhanced special search API allows you to find venues in a specific area that have active specials, with the specials categorized by type (food, drink, entertainment). This provides a comprehensive view of all nearby venues and their current offerings.

## Endpoint

**POST** `/api/specials/search/venues`

## Request Model

```json
{
  "searchTerm": "string (optional)",
  "latitude": "number (optional)",
  "longitude": "number (optional)", 
  "radiusInMeters": "number (default: 5000)",
  "date": "string (optional, format: yyyy-MM-dd, defaults to today)",
  "time": "string (optional, format: HH:mm, defaults to current time)",
  "activeOnly": "boolean (default: true)",
  "pageNumber": "number (default: 1)",
  "pageSize": "number (default: 20)",
  "sortBy": "string (default: 'distance')", // "distance", "name", "special_count"
  "sortOrder": "string (default: 'asc')" // "asc", "desc"
}
```

## Response Model

```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Downtown Bar & Grill",
        "description": "Popular sports bar with great food",
        "phoneNumber": "+1-555-0123",
        "website": "https://example.com",
        "email": "contact@example.com",
        "profileImage": "https://example.com/image.jpg",
        "streetAddress": "123 Main St",
        "secondaryAddress": "Suite 100",
        "locality": "Downtown",
        "region": "CA",
        "postalCode": "90210",
        "country": "US",
        "latitude": 34.0522,
        "longitude": -118.2437,
        "categoryId": 1,
        "categoryName": "Restaurant",
        "categoryIcon": "restaurant-icon",
        "distanceInMeters": 1250.5,
        "specials": {
          "food": [
            {
              "id": 101,
              "venueId": 1,
              "venueName": "Downtown Bar & Grill",
              "specialCategoryId": 1,
              "title": "Happy Hour Appetizers",
              "description": "50% off all appetizers",
              "startDate": "2025-06-26",
              "startTime": "15:00",
              "endTime": "18:00",
              "endDate": null,
              "isRecurring": true,
              "isActive": true,
              "categoryName": "Food",
              "categoryIcon": "food-icon",
              "distanceInMeters": 1250.5
            }
          ],
          "drink": [
            {
              "id": 102,
              "venueId": 1,
              "venueName": "Downtown Bar & Grill",
              "specialCategoryId": 2,
              "title": "2-for-1 Cocktails",
              "description": "Buy one get one free on all cocktails",
              "startDate": "2025-06-26",
              "startTime": "17:00",
              "endTime": "19:00",
              "endDate": null,
              "isRecurring": true,
              "isActive": true,
              "categoryName": "Drink",
              "categoryIcon": "drink-icon",
              "distanceInMeters": 1250.5
            }
          ],
          "entertainment": [
            {
              "id": 103,
              "venueId": 1,
              "venueName": "Downtown Bar & Grill",
              "specialCategoryId": 3,
              "title": "Live Jazz Night",
              "description": "Live jazz music every Thursday",
              "startDate": "2025-06-26",
              "startTime": "20:00",
              "endTime": "23:00",
              "endDate": null,
              "isRecurring": true,
              "isActive": true,
              "categoryName": "Entertainment",
              "categoryIcon": "music-icon",
              "distanceInMeters": 1250.5
            }
          ]
        },
        "totalSpecialCount": 3
      }
    ],
    "totalCount": 25,
    "pageNumber": 1,
    "pageSize": 20
  },
  "message": null
}
```

## Example Usage

### Find venues with specials near current location (default 5km radius)
```bash
curl -X POST "https://api.example.com/api/specials/search/venues" \
  -H "Content-Type: application/json" \
  -d '{
    "latitude": 34.0522,
    "longitude": -118.2437
  }'
```

### Find venues with specials for a specific date/time
```bash
curl -X POST "https://api.example.com/api/specials/search/venues" \
  -H "Content-Type: application/json" \
  -d '{
    "latitude": 34.0522,
    "longitude": -118.2437,
    "date": "2025-06-27",
    "time": "18:00",
    "radiusInMeters": 2000
  }'
```

### Search for venues with text and location
```bash
curl -X POST "https://api.example.com/api/specials/search/venues" \
  -H "Content-Type: application/json" \
  -d '{
    "searchTerm": "pizza",
    "latitude": 34.0522,
    "longitude": -118.2437,
    "radiusInMeters": 1000
  }'
```

### Get all venues with specials (no location filter)
```bash
curl -X POST "https://api.example.com/api/specials/search/venues" \
  -H "Content-Type: application/json" \
  -d '{
    "pageNumber": 1,
    "pageSize": 50
  }'
```

## Special Categorization

The system automatically categorizes specials into three main types:

- **Food**: Specials with categories containing keywords like "food", "meal", "dining", "appetizer", "entree", "dessert"
- **Drink**: Specials with categories containing keywords like "drink", "beverage", "cocktail", "beer", "wine", "alcohol"  
- **Entertainment**: Specials with categories containing keywords like "entertainment", "music", "event", "show", "performance", "karaoke"

## Integration with Azure Maps and PostGIS

This endpoint uses a hybrid approach:

1. **PostGIS** for precise geospatial queries of venue locations and radius filtering
2. **Azure Maps** for enhanced location services and geocoding (if needed)
3. **Database queries** for active special filtering based on date/time criteria

## Performance Considerations

- Default radius is 5km to balance performance with useful results
- Pagination is strongly recommended for large result sets  
- Date/time filtering is performed efficiently using indexed database queries
- Specials are categorized in-memory for optimal performance

## Related Endpoints

- `GET /api/specials/near` - Get just the specials near a location (not venues)
- `POST /api/specials/search` - Search individual specials
- `GET /api/venues/search` - Search venues (without special categorization)
