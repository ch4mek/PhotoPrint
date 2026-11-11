import { useEffect } from 'react';

import { useConfiguration } from '../context/Configuration';

import { defaultConfiguration } from '@/constants/defaultValues';
import { useGetConfiguration } from '@/queries/useGetConfiguration';

export const useConfigurationContextValue = (token: null | string) => {
	const { setConfigurationContextValue } = useConfiguration();

	const { data, isFetching, dataUpdatedAt } = useGetConfiguration({
		enabled: Boolean(token),
		refetchOnWindowFocus: false,
	});

	useEffect(() => {
		setConfigurationContextValue({
			...(data || defaultConfiguration),
			isLoading: isFetching,
		});
	}, [dataUpdatedAt, isFetching]);
};
