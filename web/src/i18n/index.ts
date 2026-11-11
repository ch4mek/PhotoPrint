import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import enBooking from './locales/en/booking.json';
import enCommon from './locales/en/common.json';
import enDashboard from './locales/en/dashboard.json';
import enHome from './locales/en/home.json';
import enProfile from './locales/en/profile.json';
import enTimesheet from './locales/en/timesheet.json';
import ruBooking from './locales/ru/booking.json';
import ruCommon from './locales/ru/common.json';
import ruDashboard from './locales/ru/dashboard.json';
import ruHome from './locales/ru/home.json';
import ruProfile from './locales/ru/profile.json';
import ruTimesheet from './locales/ru/timesheet.json';

const defaultNamespace = 'common';

i18n.use(initReactI18next).init({
	lng: 'en',
	fallbackLng: 'en',
	supportedLngs: ['en', 'ru'],
	defaultNS: defaultNamespace,
	ns: [
		defaultNamespace,
		'profile',
		'home',
		'timesheet',
		'dashboard',
		'booking',
	],
	resources: {
		en: {
			common: enCommon,
			profile: enProfile,
			timesheet: enTimesheet,
			home: enHome,
			dashboard: enDashboard,
			booking: enBooking,
		},
		ru: {
			common: ruCommon,
			profile: ruProfile,
			timesheet: ruTimesheet,
			home: ruHome,
			dashboard: ruDashboard,
			booking: ruBooking,
		},
	},
	interpolation: {
		escapeValue: false,
	},
	react: {
		useSuspense: false,
	},
});
