import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError, AxiosResponse } from 'axios';

import { ServerResponseError } from '../types';

import { UPDATE_USER_LOCATION_ENDPOINT } from '@/constants/queryPaths';

import { UpdateUserLocationPayload } from './types';

export const useUpdateUserLocation = (
	options: UseMutationOptions<
		AxiosResponse<string>,
		AxiosError<ServerResponseError>,
		UpdateUserLocationPayload
	>
) =>
	useMutation({
		...options,
		mutationFn: (payload) =>
			axios.patch(UPDATE_USER_LOCATION_ENDPOINT, payload),
	});
