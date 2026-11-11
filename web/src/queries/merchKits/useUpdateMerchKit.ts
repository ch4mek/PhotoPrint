import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { getMerchKitByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { MerchKits } from '@/pages/Merch/components/KitsTab/types';

import { UpdateMerchKitPayload } from './types';

export const useUpdateMerchKit = (
	options: UseMutationOptions<
		MerchKits,
		AxiosError<ServerResponseError>,
		UpdateMerchKitPayload
	>
) =>
	useMutation({
		mutationFn: (data) =>
			axios
				.put<MerchKits>(getMerchKitByIdEndpoint(data.id!), data)
				.then((response) => response.data)
				.catch(handleRequestError),
		...options,
	});
