import { createContext, useContext } from 'react';

import { CalendarCountries } from '@/types/settings';

export const SettingsCalendarCountriesContext =
	createContext<CalendarCountries>([] as CalendarCountries);

export const useSettingsCalendarCountriesContext = () =>
	useContext(SettingsCalendarCountriesContext);
