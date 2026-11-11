import type booking from './locales/en/booking.json';
import type common from './locales/en/common.json';
import type dashboard from './locales/en/dashboard.json';
import type home from './locales/en/home.json';
import type profile from './locales/en/profile.json';
import type timesheet from './locales/en/timesheet.json';

export type Resources = {
	common: typeof common;
	profile: typeof profile;
	timesheet: typeof timesheet;
	home: typeof home;
	dashboard: typeof dashboard;
	booking: typeof booking;
};
