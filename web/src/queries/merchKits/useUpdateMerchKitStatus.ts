import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { getMerchKitStatusByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';

import {
	UpdateMerchKitStatusPayload,
	UpdateMerchKitStatusResponse,
} from './types';

export const useUpdateMerchKitStatus = (
	options?: UseMutationOptions<
		UpdateMerchKitStatusResponse,
		AxiosError<ServerResponseError>,
		UpdateMerchKitStatusPayload
	>
) =>
	useMutation({
		mutationFn: ({ id, status }) =>
			axios
				.patch<UpdateMerchKitStatusResponse>(
					getMerchKitStatusByIdEndpoint(id),
					{ status }
				)
				.then((response) => response.data)
				.catch(handleRequestError),
		...options,
	});
