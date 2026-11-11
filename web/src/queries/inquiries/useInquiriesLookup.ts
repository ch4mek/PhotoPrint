import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { GET_INQUIRIES_LOOKUP_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { InquiryLookup } from '@/types/absences';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useInquiriesLookup = (
	queryOptions?: PatchedQueryOptions<
		Array<InquiryLookup>,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<Array<InquiryLookup>, AxiosError<ServerResponseError>>({
		...queryOptions,
		queryKey: [absencesQueriesKeys.getInquiriesLookup],
		queryFn: async () => {
			const { data } = await axios.get(GET_INQUIRIES_LOOKUP_API_ENDPOINT);

			return data;
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		retry: false,
	});
