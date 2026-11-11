import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_MICROFRONTENDS_CONFIG } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import {
	MFERegistryResponse,
	MicroFrontendConfig,
} from '@/types/microfrontends';

interface UpdateMicrofrontendsPayload {
	microfrontends: MicroFrontendConfig[];
}

export const useUpdateMicrofrontends = (
	options?: UseMutationOptions<
		MFERegistryResponse,
		AxiosError<ServerResponseError>,
		UpdateMicrofrontendsPayload
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const raw = JSON.stringify(data);
				const escaped = JSON.stringify(raw);

				const res = await axios.put(GET_MICROFRONTENDS_CONFIG, escaped, {
					headers: {
						'Content-Type': 'application/json',
					},
				});

				return res.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
