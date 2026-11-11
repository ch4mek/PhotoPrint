import { UseMutationOptions } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { ServerResponseError } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export type KPIColorResponse = {
	id: number;
	name: string;
	rightIntervalBorder: number;
};

export type UseGetKPIColorsProps = PatchedQueryOptions<
	Array<KPIColorResponse>,
	AxiosError<ServerResponseError>
>;

export type UseUpdateKPIColorsProps = UseMutationOptions<
	Array<KPIColorResponse>,
	AxiosError<ServerResponseError>,
	Array<KPIColorResponse>
>;
