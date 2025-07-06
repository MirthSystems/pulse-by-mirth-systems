import sharp from 'sharp';
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// SVG template for the Pulse icon
const createSVG = (size) => `
<svg width="${size}" height="${size}" viewBox="0 0 ${size} ${size}" xmlns="http://www.w3.org/2000/svg">
  <defs>
    <linearGradient id="gradient" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" style="stop-color:#3b82f6;stop-opacity:1" />
      <stop offset="100%" style="stop-color:#1d4ed8;stop-opacity:1" />
    </linearGradient>
  </defs>
  <rect width="${size}" height="${size}" fill="url(#gradient)" rx="${Math.round(size * 0.1)}"/>
  <text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" 
        font-size="${Math.round(size * 0.6)}" font-family="Arial, sans-serif" 
        font-weight="bold" fill="white">P</text>
</svg>
`;

// Create maskable icon SVG (with more padding)
const createMaskableSVG = (size) => `
<svg width="${size}" height="${size}" viewBox="0 0 ${size} ${size}" xmlns="http://www.w3.org/2000/svg">
  <defs>
    <linearGradient id="gradient" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" style="stop-color:#3b82f6;stop-opacity:1" />
      <stop offset="100%" style="stop-color:#1d4ed8;stop-opacity:1" />
    </linearGradient>
  </defs>
  <circle cx="${size/2}" cy="${size/2}" r="${size/2}" fill="url(#gradient)"/>
  <text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" 
        font-size="${Math.round(size * 0.45)}" font-family="Arial, sans-serif" 
        font-weight="bold" fill="white">P</text>
</svg>
`;

async function generateIcon(svgContent, outputPath) {
  try {
    const buffer = Buffer.from(svgContent);
    await sharp(buffer)
      .png()
      .toFile(outputPath);
    console.log(`Generated: ${outputPath}`);
  } catch (error) {
    console.error(`Error generating ${outputPath}:`, error);
  }
}

async function generateAllIcons() {
  const publicDir = path.join(__dirname, '..', 'public');
  
  // Standard icons
  await generateIcon(createSVG(64), path.join(publicDir, 'pwa-64x64.png'));
  await generateIcon(createSVG(192), path.join(publicDir, 'pwa-192x192.png'));
  await generateIcon(createSVG(512), path.join(publicDir, 'pwa-512x512.png'));
  await generateIcon(createSVG(180), path.join(publicDir, 'apple-touch-icon.png'));
  
  // Maskable icon (circular with padding for Android)
  await generateIcon(createMaskableSVG(512), path.join(publicDir, 'maskable-icon-512x512.png'));
  
  console.log('All icons generated successfully!');
}

generateAllIcons().catch(console.error);
