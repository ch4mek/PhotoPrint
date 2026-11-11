import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError, isAxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { SettingsCalendarMutationKeys } from '@/constants/mutationKeys';
import { SETTINGS_CALENDAR_COUNTRIES_API_ENDPOINT } from '@/constants/queryPaths';
import { CalendaryCountry } from '@/types/configuration';

export const useCreateCalendarCountry = (
	options?: UseMutationOptions<CalendaryCountry, AxiosError, string>
) =>
	useMutation<CalendaryCountry, AxiosError, string>({
		mutationKey: [SettingsCalendarMutationKeys.createCountry],
		mutationFn: (countryName: string) =>
			axios
				.post<CalendaryCountry>(
					SETTINGS_CALENDAR_COUNTRIES_API_ENDPOINT,
					countryName,
					{ headers: { 'Content-Type': 'application/json' } }
				)
				.then((res) => res.data)
				.catch((error) => {
					if (isAxiosError(error)) {
						throw error;
					}
					throw new Error(SOMETHING_WENT_WRONG);
				}),
		...options,
	});
