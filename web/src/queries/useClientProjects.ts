import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { ClientQuerieKeys } from '@/constants/queriesKeys';
import { getClientProjectsEndpointUrl } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ClientProjectsResponse } from '@/types/clients';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useClientProjects = (
	clientId: number,
	toast?: ToastContextValues,
	options?: PatchedQueryOptions<Array<ClientProjectsResponse>, AxiosError>
) =>
	useQueryWithCallback({
		...options,
		queryKey: [ClientQuerieKeys.clientsProjects, clientId],
		queryFn: () =>
			axios
				.get<Array<ClientProjectsResponse>>(
					getClientProjectsEndpointUrl(clientId)
				)
				.then((res) => res.data)
				.catch((error) => {
					toast?.open(
						createToast(
							NotificationType.ERROR,
							error.response.data.Message || SOMETHING_WENT_WRONG
						)
					);

					throw error;
				}),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
