import axios, { AxiosError } from 'axios';

import { SettingsKPIColorsQueriesKeys } from '@/constants/queriesKeys';
import { KPI_COLORS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';

import { KPIColorResponse, UseGetKPIColorsProps } from './types';

export const useGetKPIColors = (options?: UseGetKPIColorsProps) =>
	useQueryWithCallback<
		Array<KPIColorResponse>,
		AxiosError<ServerResponseError>
	>({
		...options,
		queryKey: [SettingsKPIColorsQueriesKeys.kpiColors],
		queryFn: async () => {
			const { data } = await axios.get<Array<KPIColorResponse>>(
				KPI_COLORS_ENDPOINT
			);

			return data;
		},
		refetchOnWindowFocus: false,
	});
