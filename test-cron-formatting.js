const cronstrue = require('cronstrue');

// Test function that mimics our updated formatCronSchedule function
function formatCronSchedule(cron) {
  if (!cron || cron.trim() === '') {
    return 'Custom schedule'
  }

  try {
    // Use cronstrue library to parse and format the cron expression
    const humanReadable = cronstrue.toString(cron, {
      throwExceptionOnParseError: false,
      use24HourTimeFormat: false
    })
    
    // Clean up the output for better readability
    return humanReadable
      .replace(/^At /, '') // Remove "At " prefix
      .replace(/every day/, 'daily')
      .replace(/every ([A-Z][a-z]+)/, 'every $1') // Keep "every Sunday", etc.
      .replace(/^([A-Z])/, m => m.toLowerCase()) // Lowercase first letter
  } catch (error) {
    // Fallback to original patterns for specific common cases
    const patterns = {
      '0 16 * * 1-5': 'Weekdays at 4:00 PM',
      '0 21 * * 5,6': 'Fri & Sat at 9:00 PM',
      '0 21 * * 3': 'Wednesdays at 9:00 PM',
      '0 11 * * 2': 'Tuesdays at 11:00 AM',
      '0 16 * * 2-6': 'Tue-Sat at 4:00 PM',
      '0 10 * * 0': 'Sundays at 10:00 AM',
      '* * * * *': 'Every minute',
      '0 19 * * 4': 'Thursdays at 7:00 PM',
      '* 19 * * 4': 'Thursdays 7:00-7:59 PM'
    }
    
    return patterns[cron] || 'Custom schedule'
  }
}

// Test cases that should now work correctly
const testCases = [
  '0 18 * * 6',      // Saturday at 6 PM (the problem case!)
  '0 10 * * 0',      // Sunday at 10 AM  
  '0 12 * * 1',      // Monday at 12 PM
  '0 14 * * 2',      // Tuesday at 2 PM
  '0 16 * * 3',      // Wednesday at 4 PM
  '0 18 * * 4',      // Thursday at 6 PM
  '0 20 * * 5',      // Friday at 8 PM
  '0 16 * * 1-5',    // Weekdays at 4 PM
  '0 21 * * 0,6',    // Weekends at 9 PM
  '0 12 * * *',      // Daily at noon
  'invalid cron'     // Invalid CRON expression
];

console.log('Testing CRON Schedule Formatting:');
console.log('==================================');

testCases.forEach(cron => {
  const result = formatCronSchedule(cron);
  console.log(`${cron.padEnd(15)} -> ${result}`);
});
