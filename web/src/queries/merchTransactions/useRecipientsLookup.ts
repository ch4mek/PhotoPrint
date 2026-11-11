import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { GET_MERCH_RECIPIENTS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { LookupRecipientResponse } from './types';

export const useRecipientsLookup = () =>
	useQueryWithCallback<Array<LookupRecipientResponse>, AxiosError>({
		queryKey: [merchQueriesKeys.getMerchRecipients],
		queryFn: () =>
			axios
				.get<Array<LookupRecipientResponse>>(GET_MERCH_RECIPIENTS_ENDPOINT)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
