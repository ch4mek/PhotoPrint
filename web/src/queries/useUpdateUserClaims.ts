import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getClaimsByUserIdAPIEndpoint } from '@/constants/queryPaths';
import { AvailableUserClaimResponse } from '@/types/settings';

import { ServerResponseError, UserClaimsSubmitParams } from './types';

export const useUpdateUserClaims = (
	options: UseMutationOptions<
		AvailableUserClaimResponse[],
		AxiosError<ServerResponseError>,
		UserClaimsSubmitParams
	>
) =>
	useMutation<
		AvailableUserClaimResponse[],
		AxiosError<ServerResponseError>,
		UserClaimsSubmitParams
	>({
		...options,
		mutationFn: ({ userId, permissions }) =>
			axios
				.patch(getClaimsByUserIdAPIEndpoint(userId), {
					claimTypes: permissions,
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
