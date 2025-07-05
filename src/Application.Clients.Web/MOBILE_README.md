# Pulse Mobile App

This is the mobile app version of Pulse, built with Vue 3, Vite, and Capacitor for cross-platform native functionality.

## 🚀 Features

### Progressive Web App (PWA)
- ✅ Service Worker with offline caching
- ✅ Install prompt for web and mobile browsers  
- ✅ Web App Manifest with proper icons
- ✅ Splash screens for various device sizes
- ✅ Network status detection
- ✅ Background sync capabilities

### Native Mobile Features
- ✅ iOS and Android platform support via Capacitor
- ✅ Geolocation with high accuracy positioning
- ✅ Camera access for taking photos and selecting from gallery
- ✅ Haptic feedback for better user experience
- ✅ Push notifications (configured)
- ✅ Status bar and splash screen customization
- ✅ Keyboard handling and responsive UI
- ✅ Network status monitoring
- ✅ Share API integration
- ✅ Clipboard access
- ✅ App state management (foreground/background)
- ✅ Deep linking support

## 📱 Getting Started

### Prerequisites
- Node.js 18+
- For iOS development: Xcode and iOS Simulator
- For Android development: Android Studio and Android SDK

### Installation

1. Install dependencies:
```bash
npm install
```

2. Build the web app:
```bash
npm run build
```

3. Add native platforms:
```bash
# Android
npx cap add android

# iOS  
npx cap add ios
```

### Development

#### Web Development
```bash
# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

#### Native Development

##### Android
```bash
# Build web app and sync with native project
npm run cap:build

# Open in Android Studio
npm run cap:android

# Run on connected device/emulator
npm run cap:run:android

# Build APK/AAB
npm run cap:build:android
```

##### iOS
```bash
# Build web app and sync with native project  
npm run cap:build

# Open in Xcode
npm run cap:ios

# Run on simulator/device
npm run cap:run:ios

# Build for App Store
npm run cap:build:ios
```

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run cap:sync` - Sync web assets with native projects
- `npm run cap:build` - Build web app and sync with native
- `npm run cap:android` - Open Android project in Android Studio
- `npm run cap:ios` - Open iOS project in Xcode
- `npm run cap:run:android` - Build and run on Android
- `npm run cap:run:ios` - Build and run on iOS

## 🔧 Configuration

### Capacitor Configuration

The app is configured in `capacitor.config.ts`:

```typescript
{
  appId: 'com.mirth.pulse',
  appName: 'Pulse',
  webDir: 'dist',
  // ... additional native configurations
}
```

### PWA Configuration

PWA settings are configured in `vite.config.ts` using the VitePWA plugin:

```typescript
VitePWA({
  registerType: 'autoUpdate',
  workbox: {
    // Caching strategies
  },
  manifest: {
    // Web app manifest
  }
})
```

## 🎨 Native UI Components

The app includes several native-aware components:

- `PWAInstaller.vue` - Handles PWA installation prompts
- `NetworkStatus.vue` - Shows connection status
- `NativeFeaturesDemo.vue` - Demonstrates native capabilities

## 🔌 Using Native Features

Import and use native features via composables:

```vue
<script setup>
import { 
  useGeolocation, 
  useCamera, 
  useHaptics,
  usePlatform 
} from '@/composables/useNative'

const { getCurrentPosition } = useGeolocation()
const { takePicture } = useCamera()
const { vibrate } = useHaptics()
const { isNative, platform } = usePlatform()

// Get user location
const location = await getCurrentPosition()

// Take a photo (native only)
if (isNative) {
  const photo = await takePicture()
}

// Haptic feedback
await vibrate('medium')
</script>
```

## 📱 Platform-Specific Features

### iOS
- Custom splash screens for all device sizes
- Status bar styling
- Safe area handling
- App Store deployment ready

### Android
- Adaptive icons
- Status bar and navigation bar theming
- Hardware back button handling
- Google Play Store deployment ready

### Web/PWA
- Install prompts for supported browsers
- Offline functionality
- Add to home screen capability
- Progressive enhancement

## 🔒 Permissions

The app requests the following permissions:

### iOS (Info.plist)
- `NSLocationWhenInUseUsageDescription` - For location services
- `NSCameraUsageDescription` - For camera access
- `NSPhotoLibraryUsageDescription` - For photo gallery access

### Android (AndroidManifest.xml)
- `ACCESS_FINE_LOCATION` - For precise location
- `ACCESS_COARSE_LOCATION` - For approximate location  
- `CAMERA` - For camera access
- `READ_EXTERNAL_STORAGE` - For photo gallery access

## 🚀 Deployment

### Web/PWA
Deploy the `dist` folder to any static hosting service:
- Vercel, Netlify, Azure Static Web Apps, etc.

### iOS App Store
1. Open the iOS project in Xcode: `npm run cap:ios`
2. Configure signing and provisioning profiles
3. Build and upload to App Store Connect

### Google Play Store  
1. Open the Android project in Android Studio: `npm run cap:android`
2. Generate signed APK/AAB
3. Upload to Google Play Console

## 🐛 Troubleshooting

### Common Issues

**Build failures:**
```bash
# Clean and rebuild
npm run clean
npm install
npm run build
npx cap sync
```

**iOS simulator issues:**
```bash
# Reset simulator
xcrun simctl erase all
```

**Android build issues:**
```bash
# Clean Android project
cd android
./gradlew clean
cd ..
```

## 📚 Documentation

- [Capacitor Documentation](https://capacitorjs.com/docs)
- [Vue 3 Documentation](https://vuejs.org/)
- [Vite Documentation](https://vitejs.dev/)
- [VitePWA Documentation](https://vite-pwa-org.netlify.app/)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test on web, iOS, and Android
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.
