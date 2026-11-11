import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { CONFIGURATION_QUERY_KEY } from '@/constants/queriesKeys';
import { CreateCalendarTimeParams } from '@/queries/settings/types';
import { useCreateCalendarCountry } from '@/queries/settings/useCreateCalendarCountry';
import { useCreateCalendarTime } from '@/queries/settings/useCreateCalendarTime';
import { useDeleteCalendarCountry } from '@/queries/settings/useDeleteCalendarCountry';
import { useUpdateCalendarCountry } from '@/queries/settings/useUpdateCalendarCountry';
import { useUpdateCalendarTime } from '@/queries/settings/useUpdateCalendarTime';
import { CalendaryCountry } from '@/types/configuration';
import { WorkingHoursPerMonth } from '@/types/settings';

export const useCalendarMutations = () => {
	const { mutateAsync: createTimeAsync } = useCreateCalendarTime();
	const { mutateAsync: createCountryAsync } = useCreateCalendarCountry();
	const { mutateAsync: updateTimeAsync } = useUpdateCalendarTime();
	const { mutateAsync: updateCountryAsync } = useUpdateCalendarCountry();
	const { mutateAsync: deleteCountryAsync } = useDeleteCalendarCountry();

	const queryClient = useQueryClient();

	const refetchConfiguration = useCallback(() => {
		queryClient.refetchQueries({ queryKey: [CONFIGURATION_QUERY_KEY] });
	}, []);

	const createMonthTime = useCallback(
		(
			params: CreateCalendarTimeParams,
			onSuccess?: (newTime: WorkingHoursPerMonth) => void,
			onError?: () => void
		) => {
			createTimeAsync(params).then(onSuccess).catch(onError);
		},
		[]
	);

	const updateMonthTime = useCallback(
		(
			entityId: number,
			newTime: number,
			onSuccess?: (newTime: WorkingHoursPerMonth) => void,
			onError?: () => void
		) => {
			updateTimeAsync({ hours: newTime, id: entityId })
				.then(onSuccess)
				.catch(onError);
		},
		[]
	);

	const createCountry = useCallback(
		(
			countryName: string,
			onSuccess?: (newCountry: CalendaryCountry) => void,
			onError?: () => void
		) => {
			createCountryAsync(countryName)
				.then((data) => {
					refetchConfiguration();

					if (onSuccess) {
						onSuccess(data);
					}
				})
				.catch(onError);
		},
		[]
	);

	const updateCountry = useCallback(
		(
			countryId: number,
			newName: string,
			onSuccess?: (newCountry: CalendaryCountry) => void,
			onError?: () => void
		) => {
			updateCountryAsync({ id: countryId, name: newName })
				.then((data) => {
					refetchConfiguration();

					if (onSuccess) {
						onSuccess(data);
					}
				})
				.catch(onError);
		},
		[]
	);

	const deleteCountry = useCallback(
		(
			countryId: number,
			onSuccess?: (newCountry: CalendaryCountry) => void,
			onError?: () => void
		) => {
			deleteCountryAsync(countryId)
				.then((data) => {
					refetchConfiguration();

					if (onSuccess) {
						onSuccess(data);
					}
				})
				.catch(onError);
		},
		[]
	);

	return {
		createMonthTime,
		updateMonthTime,
		createCountry,
		updateCountry,
		deleteCountry,
	};
};
