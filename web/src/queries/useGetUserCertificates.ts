import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { CERTIFICATES_QUERY_KEY } from '@/constants/queriesKeys';
import { GET_CERTIFICATES_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { ToastContextValues } from '@/context/types';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import type {
	CertificateFilters,
	CertificateResponse,
} from '@/types/certificate';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { ServerResponseError } from './types';
import { convertFromToDateObjToQueryParam } from './utils';

export const useGetUserCertificates = (
	userId: number,
	toast: ToastContextValues,
	top: number,
	skip: number,
	order?: SortDirection,
	orderByCondition?: string,
	filters: Partial<CertificateFilters> = {},
	options: PatchedQueryOptions<
		CertificateResponse,
		AxiosError<ServerResponseError>
	> = {}
) =>
	useQueryWithCallback<CertificateResponse, AxiosError<ServerResponseError>>({
		...options,
		queryKey: [
			CERTIFICATES_QUERY_KEY,
			userId,
			order,
			orderByCondition,
			filters,
			top,
			skip,
		],
		queryFn: async () => {
			const { certificateId, stack, receiptDate } = filters;

			try {
				const query = buildQuery({
					filter: {
						and: [
							...convertFromToDateObjToQueryParam(receiptDate, 'receiptDate'),
							certificateId &&
								`contains(tolower(certificateId), tolower('${replaceSpecialCharacters(
									certificateId
								)}'))`,
							stack &&
								`contains(tolower(stack), tolower('${replaceSpecialCharacters(
									stack
								)}'))`,
						],
					},
					top,
					skip: skip * top,
					orderBy:
						orderByCondition && order ? `${orderByCondition} ${order}` : '',
				});

				const { data } = await axios.get<CertificateResponse>(
					`${GET_CERTIFICATES_API_ENDPOINT}/${userId}${query}`
				);

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		onError: createOnErrorHandler(toast),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
