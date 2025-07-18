# Pulse: A Real-Time Nightlife Discovery Platform

## Project Overview

**Project Lead:** Davis Kolakowski

## Executive Summary

Pulse is a cutting-edge, location-based platform that revolutionizes how users discover and engage with local nightlife venues. By leveraging real-time data, community-sourced "vibe checks," and AI-driven recommendations, Pulse helps users find the perfect venue based on what's happening right now.

## The Problem

Traditional review and discovery platforms (Yelp, Google Maps, etc.) offer static information that quickly becomes outdated and fails to capture what's happening right now:

- Users can't determine current crowd levels, ambiance, or special events in real-time
- Venues have no effective way to broadcast last-minute events, extended happy hours, or current atmosphere
- A venue's online reputation might not reflect tonight's experience
- Users often make decisions based on outdated information, leading to disappointing experiences

The ambiance of a venue can make or break your night, and traditional apps can't capture the current vibe. The result? Frustration, FOMO, and wasted time and money.

## Target Users

1. **Consumers (21+):**
    - Night-crawlers and social explorers
    - Foodies and drink enthusiasts
    - Live music and entertainment seekers
    - Young professionals looking for quality social experiences
2. **Venues:**
    - Small independent bars and restaurants
    - Cafés and lounges
    - Live music venues
    - Any establishment looking to increase foot traffic and engagement

## Core Value Propositions

### For Users:

- Real-time insight into venue atmosphere and crowd levels
- Community-sourced "vibe checks" providing authentic snapshots
- Personalized recommendations matching current mood and preferences
- Filters for specific experiences (lively, quiet, live music, etc.)
- Time-sensitive specials and promotions from nearby venues

### For Venues:

- Direct communication channel to potential customers at decision time
- Real-time broadcasting of events, specials, and atmosphere changes
- Analytics on user engagement and conversion
- Community building and loyalty development
- Cost-effective marketing that drives immediate results

## MVP Feature Set

For our initial release, we're focusing on delivering these essential features:

### User Features:

1. **Privacy-First Location System:**
    - Manual address entry as the default option
    - Search around a specific address with adjustable radius settings
    - Opt-in "Use My Current Location" button for users who choose to enable location services
    - No automatic location tracking or GPS permissions required by default
    - City/neighborhood selection for broader area discovery

2. **Real-Time Venue Discovery:**
    - List view of venues with currently active specials in the selected area
    - Each venue card displays basic info and active promotions
    - At-a-glance indicators showing venue activity level
    - Sorting options based on popularity, distance, or special type
    - One-tap access to venue details and live activity thread

3. **Three-Category Classification System:**
    - Three distinct categorization elements to improve user discovery:
      - **Venue Types**: Fixed categories for venues (bar, restaurant, cafe, etc.)
      - **Tags**: Event/promotion identifiers with # symbol for specials (#happyhour, #margaritanight, etc.)
      - **Vibes**: User-generated atmosphere descriptors with # symbol (#busy, #livemusic, etc.)
    - Each category serves a specific purpose in the discovery experience
    - Clear visual distinction between Types, Tags, and Vibes in the UI
    - Search and filter functionality specific to each category type

4. **Special-Driven Connections:**
    - Specials serve as the primary link between venues and users
    - Each special includes:
      - Description (e.g., "Half-Price Wings Happy Hour")
      - Type categorization (Food, Drink, Entertainment)
      - Timing details (start time, end time, expiration)
      - Recurrence pattern for regular events
      - Associated venue information
      - Tags for discovery (#wingsnight, #happyhour, etc.)
    - Users discover venues primarily through interesting specials
    - Active specials appear in search results with their Tags
    - Users can follow Tags to receive notifications about matching specials
    - Special verification through user activity threads
    - Time-sensitive nature creates urgency and real-time engagement

5. **Category-Specific Implementation:**
    - **Venue Types**: Selected by venue owners from a predefined list
      - Examples: Bar, Restaurant, Cafe, Lounge, Club, Brewery, Concert Venue
      - Each venue must select a primary Type and can add up to two secondary Types
      - Types are used for basic venue categorization and filtering
      - Displayed prominently on venue profiles and search results
    - **Tags**: Applied to specials for discovery and promotion
      - Examples: #happyhour, #wingsnight, #liveband, #djset, #karaoke
      - Tags are preceded by # symbol for visual distinction
      - Connect users with specific experiences they're seeking
      - Can be followed, searched, and filtered
    - **Vibes**: Created by users to describe current venue conditions
      - Examples: #busy, #quiet, #greatservice, #goodvibes, #nocover
      - Also preceded by # symbol for visual consistency with Tags
      - Help other users know what to expect right now
      - Temporary and tied to the 15-minute expiration of user posts

6. **Live Venue Activity Threads:**
    - Social media-style comment threads attached to each venue
    - Users can post text comments, photos, and short video clips about their current experience
    - All user posts automatically vanish after 15 minutes to ensure information is always current
    - Users can add Vibes to their posts describing the current atmosphere
    - Venue cards show the number of active posts in the thread
    - No historical data - only what's happening right now
    - Example posts:
        - "Just got in with no line! Great music playing right now #nodresscode #hiphopmix"
        - "Happy hour special is amazing, got 2-for-1 drinks #cheapdrinks #tequilatuesday"
        - [Photo of current crowd or performance] "#packed #goodvibes"
        - [10-second video clip of venue atmosphere] "#liveband #jazznight"

7. **Category-Based Discovery System:**
    - Users can browse venues through a multi-faceted approach:
      - By Venue Type (find all bars, restaurants, clubs, etc.)
      - By Tags (find all venues with #happyhour, #livemusic, etc.)
      - By current Vibes (find venues currently #packed, #mellow, etc.)
    - Filter results by combining categories:
      - Type + Tag (e.g., Bars with #happyhour)
      - Type + Vibe (e.g., Restaurants that are currently #quiet)
      - Tag + Vibe (e.g., #livemusic venues that are #energetic)
    - Distance filters can be applied to any search
    - Popular Tags and Vibes are highlighted to show trending options
    - Personalized suggestions based on preference history
    - Save favorite combinations for quick searches
    - Follow specific Tags to receive notifications

8. **Active Specials Feed:**
    - Stream of currently running specials near the user
    - Time-remaining indicators for limited-time offers
    - Special verification through user comments
    - Specials grouped by Tags for easy browsing
    - Specials vanish from the feed when they end
    - New specials are highlighted

9. **Venue Profiles:**
    - Basic information (hours, contact details, photos)
    - Venue Types (primary and secondary)
    - Current active specials with their Tags
    - Live activity thread showing real-time user comments with Vibes
    - Type-based related venues suggestions

10. **PostgreSQL-Based Recommendation Engine:**
    - Initial version using traditional database algorithms
    - Recommendations based on Type preferences, Tag interests, common Vibes, user history, and popularity metrics
    - Category affinity analysis for better matching
    - _Note: Advanced AI recommendations planned for future releases_

### Venue Features:

1. **Venue Management Portal:**
    - Simple web interface for venue owners/managers
    - Type selection for venue profile (primary + secondary Types)
    - Special creation and management with:
      - Content description
      - Type categorization (Food, Drink, Entertainment)
      - Start date and time
      - End time (optional)
      - Expiration date (for multi-day specials)
      - Recurring schedule options (for regular events)
      - Tag assignment
    - Photo uploads
    - Ability to monitor (but not delete) user activity threads
    - Tag performance analytics

2. **Special Management System:**
    - Create and edit specials with complete timing control
    - Set one-time or recurring specials (daily, weekly, monthly)
    - Assign Tags to each special
    - Schedule specials in advance
    - Cancel or modify active specials
    - View special performance metrics
    - Suggestions for effective tagging

3. **Basic Analytics:**
    - Profile view counts
    - Engagement metrics on specials
    - Tag performance analytics
    - Activity thread participation statistics
    - User demographic information (anonymized)
    - Special conversion tracking
    - Vibe trend analysis

4. **Free Tier Access:**
    - All core features available at no cost for initial adoption

### Administrative Features:

1. **Category Management System:**
    - Maintain the list of available Venue Types
    - Monitor Tag usage and trends
    - Track Vibe patterns and popularity
    - Feature specific Tags for promotion
    - Consolidate similar Tags and Vibes
    - Block inappropriate content
    - Create seasonal or event-specific featured Tags

2. **Content Moderation:**
    - Review reported content
    - Verify venue accounts
    - Monitor user activity for policy violations
    - Manage content expiration system

## Technical Architecture

### Frontend:

- **Vue.js 3 Progressive Web App (PWA)** built with **Vite** as the build tool for lightning-fast development and optimized production builds.
  - **Vue 3 Composition API** for reactive, maintainable component architecture
  - **TypeScript** integration for enhanced developer experience and code reliability
  - **Pinia** for intuitive state management with Vue 3 reactivity
  - **Vue Router 4** for client-side routing and navigation
  - **TailwindCSS 4** for utility-first styling and consistent design system
  - **Headless UI for Vue** for accessible, unstyled UI components
  - **Heroicons** for consistent iconography throughout the application
  - **@vueuse/core** for essential Vue 3 composition utilities
  - Installable on mobile devices as a PWA
  - Offline capabilities and caching strategies
  - Push notifications (with user permission)
  - Responsive design optimized for all screen sizes
  - **Cypress** for end-to-end testing and **Vitest** for unit testing
- **Auth0 Vue SDK** (@auth0/auth0-vue) for seamless authentication integration
- Future native mobile applications planned post-MVP (iOS & Android)
- Dedicated web portal for venue management built with the same Vue.js stack

### Backend:

- **.NET 9 (C#) API services** built with modern ASP.NET Core architecture
- **Entity Framework Core 9** with PostgreSQL for robust data persistence
- **PostgreSQL database** for data storage and initial recommendation algorithms
- **.NET Aspire** for cloud-native application orchestration and development
  - Microservices architecture with service discovery
  - Built-in observability and monitoring
  - Container orchestration for development and deployment
  - Redis integration for caching and session management
- Real-time messaging infrastructure using **SignalR** for instant updates
- **Auth0** integration for authentication and authorization services
  - Secure user registration and login flows
  - Social login options (Google, Facebook, etc.)
  - JWT token-based API authentication
  - Role-based access control for users, venues, and administrators
  - Single Sign-On (SSO) capabilities
- **Cronos** library for advanced cron expression parsing and special scheduling
- **FluentValidation** for robust input validation and business rules
- Content expiration system for managing ephemeral posts (15-minute auto-deletion)
- Advanced category indexing and search optimization
- Special scheduling with complex recurrence patterns (daily, weekly, monthly, custom cron)
- **Azure Maps** integration for location services and geocoding

### Identity and Authentication:

- **Auth0 as Identity Provider:**
  - Centralized user authentication and management using Auth0's identity platform.
  - Secure credential storage and handling.
  - Support for multi-factor authentication.
  - User profile management.
  - Compliance with security best practices.
  - Venue owner verification workflows.
  - Role assignment and management (users, venue owners, administrators).
  - Integration with frontend and backend services via OAuth/OpenID Connect.
  - User session management and token handling.

### Authentication & Authorization Model:

**📋 Updated Authorization System (December 2024)**

A comprehensive, production-ready authorization system has been designed and documented. See the `docs/auth/` directory for complete details:

- **Architecture Overview**: `docs/auth/authorization-architecture-overview.md`
- **Implementation Guide**: `docs/auth/api-authorization-patterns-v2.md`
- **Recommendation**: `docs/auth/authorization-approach-recommendation.md`
- **Implementation Roadmap**: `docs/auth/implementation-roadmap.md`
- **Developer Instructions**: `docs/auth/copilot-instructions.md`

The updated system uses a **hybrid approach** combining Auth0's enterprise authentication with database-driven venue permissions for optimal security, scalability, and maintainability.

**Legacy Documentation Below** (to be updated during implementation):

The application uses a two-tiered approach to user permissions:

- **Application-Level Authorization (via Auth0):**
  - **System Administrator**: Full access to all application features
    - All venue management capabilities
    - User administration
    - System configuration
    - Content moderation
    - Analytics access
  - **Venue Manager**: Access to manage venues through the global system
    - Create and manage venue information
    - Create specials for their venues
    - View analytics for their venues
    - Limited user management for their venues
  - **Application User**: Basic authenticated user permissions
    - Create posts about venues
    - View venues and specials
    - Delete their own posts
    - Follow tags and receive notifications

- **Venue-Specific Authorization (Database-Level):**
  - **manage:venue**: Edit venue details, hours, and basic information
  - **manage:specials**: Create and manage special offers and events
  - **respond:posts**: Respond to customer posts about the venue
  - **invite:users**: Add other users as venue managers
  - **manage:users**: Control permissions for venue users

This hybrid approach allows granular control over which users can manage specific venues, independent of their global application roles.

### Database Schema:

- **Venues**: Basic venue information, location data, primary and secondary Types
- **VenueTypes**: Predefined list of venue classifications (bar, restaurant, cafe, etc.)
- **OperatingSchedule**: Venue business hours and days of operation
- **Specials**: Event details, timing, recurrence, venue association (maintains existing SpecialTypes enum)
- **Tags**: Tag definitions for specials
- **TagToSpecialLink**: Associations between Tags and Specials
- **Posts**: Ephemeral user content with 15-minute expiration, includes foreign keys to user and venue
- **Vibes**: User-created atmosphere descriptors
- **VibeToPostLink**: Associations between Vibes and Posts
- **VenuePermission**: Permissions that can be granted to users for specific venues
- **VenueUser**: Associates users with venues they can manage
- **VenueUserToPermissionLink**: Links venue users with specific permissions
- **ApplicationUser**: User account data and application-specific information
  - Id: Auto-incrementing primary key in our database
  - Auth0Id: Auth0 identifier for linking to external identity provider
  - Account creation and last login timestamps
  - Default location and search radius preferences
  - Location services opt-in status
  - Profile status (active/inactive)
  - Note: Basic profile info (name, email, profile picture) managed by Auth0

### Infrastructure

- **Azure cloud hosting** with modern containerized deployment
- **Docker containerization** with multi-stage builds for optimal production images
- **.NET Aspire orchestration** for cloud-native application development
  - Service discovery and communication
  - Built-in health checks and monitoring
  - Redis for distributed caching and session management
  - PostgreSQL for primary data storage
- **Azure Container Apps** for scalable, serverless container hosting
- **Azure Database for PostgreSQL** for managed database services
- Real-time data synchronization with SignalR hubs
- Secure API communication with JWT tokens via Auth0
- **GitHub Actions** CI/CD pipelines for automated testing and deployment
- Environment-specific configuration management

## MVP Constraints & Limitations

For our initial release, we're implementing the following constraints to focus development:

1. **Limited Geography:**
    - Launch in 2-3 test cities with active nightlife scenes
    - Focus on downtown/central districts with high venue density

2. **Privacy-First Location Approach:**
    - Manual address entry as the default interaction method
    - Opt-in location services only when user explicitly chooses
    - No background location tracking
    - No persistent storage of precise location data

3. **Ephemeral Content Model:**
    - All user-generated content auto-expires after 15 minutes
    - No content archives or historical data
    - Focus on "right now" experience
    - Prevents outdated or misleading information
    - Encourages active, current participation

4. **Structured Categorization System:**
    - Predefined Venue Types for consistent classification
    - Open Tags and Vibes for flexibility and user expression
    - Visual distinction between the three category types
    - Basic moderation for inappropriate content
    - Tag/Vibe consolidation for common misspellings or variations

5. **Vue.js PWA-First Approach:**
    - **Vue 3 with Composition API** chosen over React for its intuitive reactivity system and gentler learning curve
    - **Pinia** state management selected over Redux for its simplicity and Vue 3 optimization
    - **TailwindCSS** utility-first approach for rapid, consistent styling
    - Progressive Web App capabilities for mobile-like experience without app store distribution
    - Native mobile apps planned for post-MVP development phase
    - Responsive design ensures optimal experience across all device types

6. **PostgreSQL for Recommendations:**
    - Using traditional database queries for initial recommendations
    - Full AI implementation with Azure Cognitive Services planned for V2

7. **Basic Analytics:**
    - Essential metrics only for MVP
    - Advanced analytics and insights planned for future releases

8. **Limited Integrations:**
    - No third-party integrations in initial release beyond Auth0
    - Future plans include social media, mapping, and POS system integrations

9. **Manual Verification:**
    - Venue accounts manually verified by our team
    - Automated verification system planned for scale

## Post-MVP Roadmap

After successful MVP launch and validation, we plan to implement:

1. **Native Mobile Applications:**
    - iOS and Android apps built with modern frameworks (React Native or Flutter)
    - Enhanced performance leveraging device-specific capabilities
    - Expanded offline capabilities with local data caching
    - Push notifications and background sync
    - Integration with device cameras and location services

2. **AI-Powered Recommendation Engine:**
    - Upgrade from PostgreSQL to Azure Cognitive Services
    - Advanced personalization based on user behavior and preferences
    - Type, Tag, and Vibe preference learning
    - Predictive analytics for venues

3. **Enhanced Social Features:**
    - Friend connections and group planning
    - Follower systems for venue updates
    - Enhanced community moderation tools

4. **Advanced Categorization System:**
    - Tag and Vibe suggestions and auto-completion
    - Category relationships and affinity mapping
    - Trending Tag and Vibe analytics
    - Type/Tag/Vibe journey recommendations
    - Translation for international users

5. **Premium Venue Tiers:**
    - Featured placement options
    - Push notification capabilities
    - Advanced analytics dashboard
    - Custom Tags for premium venues
    - Promoted Special placement

6. **Expanded Integrations:**
    - Social media connectivity
    - POS system integration for real-time specials
    - Reservation systems

7. **Geographic Expansion:**
    - Rollout to additional cities
    - International markets

8. **Enhanced Authentication and User Management:**
    - Advanced authentication rules and actions for user onboarding
    - Custom authentication flows for specific user segments
    - Enterprise SSO options for venue chains
    - Expanded user profile capabilities
    - Identity verification for age restrictions

## Monetization Strategy

Pulse will follow a freemium model:

1. **Free Tier (Venues):**
    - Basic profile management
    - Standard status updates and event posting
    - Essential analytics
    - Limited number of special Tags

2. **Premium Tier (Venues):**
    - Featured placement in searches and lists
    - Push notification capabilities to nearby users
    - Advanced analytics and insights
    - Priority listing for special events
    - Enhanced profile customization
    - Unlimited special Tags
    - Promoted Tag capabilities

3. **Revenue Projections:**
    - Targeting 10-15% conversion rate from free to premium venues
    - Tiered pricing based on venue size and market
    - Potential for additional revenue streams through verified ticket sales or special event promotions

## Playtest and Validation

For our upcoming playtesting phase, we'll focus on:

1. **User Experience Testing:**
    - Usability of manual address entry and radius selection
    - User engagement with venue activity threads
    - Type, Tag, and Vibe usage patterns and effectiveness
    - Satisfaction with 15-minute content expiration
    - Value of real-time versus historical data
    - Overall satisfaction with venue discovery process
    - User preferences regarding location input methods
    - Special discovery and engagement
    - Auth0 registration and login experience

2. **Venue Portal Testing:**
    - Ease of posting updates and specials
    - Type selection and Tag creation
    - Special creation and scheduling
    - Value of provided analytics
    - Time investment required to maintain presence
    - Perceived ROI on engagement
    - Interest in monitoring live activity threads
    - Auth0-based account management

3. **Technical Validation:**
    - Performance of the **Vue 3** application under various load conditions with Pinia state management
    - Real-time update delivery speed using **SignalR** integration in the Vue frontend
    - **TailwindCSS** responsiveness across different screen sizes and devices
    - Address geocoding accuracy with **Azure Maps** integration
    - Content expiration system reliability (15-minute auto-deletion)
    - Category indexing and search performance with PostgreSQL
    - Special recurrence handling accuracy using **Cronos** library
    - Database query performance for recommendation algorithms
    - PWA performance across different devices, browsers, and network conditions
    - **Auth0 Vue SDK** integration reliability and authentication flow performance
    - **Vite** build performance and hot module replacement during development
    - **Cypress** end-to-end testing coverage and **Vitest** unit test reliability
    - **.NET Aspire** service orchestration and health monitoring effectiveness

## Success Metrics

We'll measure MVP success through these key metrics:

1. **User Engagement:**
    - Daily/weekly active users
    - Average session time
    - Retention rates (7-day, 30-day)
    - Activity thread participation rate
    - Tag and Vibe usage frequency and diversity
    - Posts per venue per hour
    - Special click-through rates
    - Address search vs. opt-in location services usage rates
    - Registration completion rates

2. **Venue Performance:**
    - Venue sign-up and retention rates
    - Frequency of special and Tag updates
    - Tag diversity per venue
    - Special creation frequency
    - Self-reported traffic increase attributed to Pulse
    - Interest in premium features
    - Special engagement rates

3. **Platform Health:**
    - System performance and reliability
    - Data freshness (% of venues with active threads)
    - Category system health (usefulness, diversity, relevance)
    - Special timing accuracy
    - User feedback and satisfaction scores
    - Content expiration system accuracy
    - PWA install rate on mobile devices
    - Auth0 authentication success rates
    - Average login time and reliability

Pulse represents a significant opportunity to transform the nightlife discovery experience by bringing real-time, community-driven insights to both users and venues. Our MVP focuses on delivering essential functionality through a carefully structured Type, Tag, and Vibe categorization system that maintains simplicity while providing powerful discovery options.

By implementing a privacy-first location system, ephemeral content model, and three-category classification approach, we prioritize user privacy, real-time information, and intuitive discovery. This approach balances the needs of venues to promote their offerings with users' desire for authentic, current information.

## Architectural Decision Record

### Why Vue.js Over React

Our MVP implementation chose **Vue 3** over React for several strategic reasons:

1. **Developer Experience**: Vue's template syntax and Composition API provide a more intuitive development experience
2. **Learning Curve**: Faster onboarding for new team members compared to React's JSX and hooks complexity
3. **Bundle Size**: Vue 3's tree-shaking and smaller runtime footprint result in faster initial page loads
4. **Reactivity System**: Vue's proxy-based reactivity is more performant and easier to debug than React's reconciliation
5. **Ecosystem Maturity**: Pinia, Vue Router, and the official Vue tooling provide a cohesive, well-maintained ecosystem

### Technology Stack Rationale

- **Vite**: Chosen for its lightning-fast HMR and optimized production builds
- **TailwindCSS**: Utility-first approach enables rapid prototyping and consistent design system
- **Pinia**: Modern Vue 3 state management that's simpler and more intuitive than Vuex
- **TypeScript**: Provides type safety and enhanced developer experience across the entire stack
- **.NET Aspire**: Microsoft's cloud-native framework enables modern microservices architecture
- **Auth0**: Industry-leading authentication eliminates security implementation complexity