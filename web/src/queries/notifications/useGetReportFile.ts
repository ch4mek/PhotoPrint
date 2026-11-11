import { UseQueryResult } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationQueriesKeys } from '@/constants/queriesKeys';
import { getReportFileEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetReportFile = (
	fileId: string,
	notificationId: number,
	options?: PatchedQueryOptions<Blob, AxiosError>
): UseQueryResult<Blob, AxiosError> =>
	useQueryWithCallback<Blob, AxiosError>({
		queryKey: [NotificationQueriesKeys.reportFile, fileId, notificationId],
		queryFn: () =>
			axios
				.get<Blob>(`${getReportFileEndpointUrl(fileId)}`, {
					responseType: 'blob',
					paramsSerializer: {
						indexes: true,
					},
				})
				.then((respone) => respone.data)
				.catch((error) => {
					throw error;
				}),
		refetchOnWindowFocus: false,
		retry: false,
		gcTime: 0,
		...options,
	});
