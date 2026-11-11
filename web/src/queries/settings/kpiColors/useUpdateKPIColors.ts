import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { KPI_COLORS_ENDPOINT } from '@/constants/queryPaths';
import { ServerResponseError } from '@/queries/types';

import { KPIColorResponse, UseUpdateKPIColorsProps } from './types';

export const useUpdateKPIColors = (options?: UseUpdateKPIColorsProps) =>
	useMutation<
		Array<KPIColorResponse>,
		AxiosError<ServerResponseError>,
		Array<KPIColorResponse>
	>({
		...options,
		mutationFn: (kpiColors: Array<KPIColorResponse>) =>
			axios.put(KPI_COLORS_ENDPOINT, kpiColors).then(({ data }) => data),
	});
