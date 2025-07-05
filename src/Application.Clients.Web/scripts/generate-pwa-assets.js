#!/usr/bin/env node

// Simple script to generate PWA icons
// You should replace this with actual icon generation using sharp or similar

const fs = require('fs')
const path = require('path')

const sizes = [64, 192, 512]
const publicDir = path.join(__dirname, '../public')

// Create simple placeholder icons
// In production, you should use proper icon generation tools

const createSimpleIcon = (size, filename) => {
  const svg = `<svg width="${size}" height="${size}" viewBox="0 0 ${size} ${size}" xmlns="http://www.w3.org/2000/svg">
  <rect width="${size}" height="${size}" fill="#1f2937" rx="${size * 0.1}"/>
  <text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" font-size="${size * 0.6}" font-family="Arial, sans-serif" font-weight="bold" fill="white">P</text>
</svg>`
  
  fs.writeFileSync(path.join(publicDir, filename), svg)
  console.log(`Created ${filename}`)
}

// Generate PWA icons
createSimpleIcon(64, 'pwa-64x64.png')
createSimpleIcon(192, 'pwa-192x192.png')
createSimpleIcon(512, 'pwa-512x512.png')
createSimpleIcon(512, 'maskable-icon-512x512.png')

// Create apple touch icon
createSimpleIcon(180, 'apple-touch-icon.png')

// Create favicon
const faviconSvg = `<svg width="32" height="32" viewBox="0 0 32 32" xmlns="http://www.w3.org/2000/svg">
  <rect width="32" height="32" fill="#1f2937" rx="3"/>
  <text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" font-size="20" font-family="Arial, sans-serif" font-weight="bold" fill="white">P</text>
</svg>`

fs.writeFileSync(path.join(publicDir, 'favicon.svg'), faviconSvg)

// Create safari pinned tab
const safariIcon = `<svg width="16" height="16" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg">
  <path d="M2 2h12a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2z" fill="black"/>
  <text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" font-size="10" font-family="Arial, sans-serif" font-weight="bold" fill="white">P</text>
</svg>`

fs.writeFileSync(path.join(publicDir, 'safari-pinned-tab.svg'), safariIcon)

console.log('PWA assets generated successfully!')
console.log('Note: For production, you should use proper icon generation tools with actual brand assets.')
