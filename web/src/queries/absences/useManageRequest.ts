import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { manageRequestEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ManageRequestType, MenageRequestProps } from '@/types/absences';

const { Accept, Reject } = ManageRequestType;

export const useManageRequest = (
	options?: UseMutationOptions<
		MenageRequestProps,
		AxiosError<ServerResponseError>,
		MenageRequestProps
	>
) =>
	useMutation({
		mutationFn: async (body) => {
			try {
				const { id, type } = body;
				const manageType =
					type === Accept ? Accept.toLowerCase() : Reject.toLowerCase();

				const { data } = await axios.post(
					manageRequestEndpoint(id, manageType),
					{}
				);

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		...options,
	});
