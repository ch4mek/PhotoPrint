import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getEmployeePayloadChangeEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';

import { EmployeePayloadResponse, ServerResponseError } from './types';

export const useUpdateEmployeePayload = <T>(
	userId: number,
	options?: UseMutationOptions<
		EmployeePayloadResponse,
		AxiosError<ServerResponseError>,
		T
	>
) =>
	useMutation({
		...options,
		mutationFn: (params: T) =>
			axios
				.put<EmployeePayloadResponse>(
					getEmployeePayloadChangeEndpoint(userId),
					params
				)
				.then((res) => res.data)
				.catch(handleRequestError),
	});
