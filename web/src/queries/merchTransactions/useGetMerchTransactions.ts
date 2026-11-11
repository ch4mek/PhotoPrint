import { SortDirection } from '@mui/material';
import axios from 'axios';
import numeral from 'numeral';
import buildQuery from 'odata-query';

import { convertFilterArrayToQueryParam } from '../utils';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { GET_POST_MERCH_TRANSACTIONS_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import {
	BodyFilters,
	GetMerchTransactionsQueryParams,
	MerchTransaction,
} from '@/pages/Merch/components/TransactionsTab/types';
import { ByPageResponse } from '@/queries/types';
import { MerchStorageType, MerchTransactionType } from '@/types/configuration';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { LookupRecipientResponse } from './types';

export const useGetMerchTransactions = (
	params: GetMerchTransactionsQueryParams,
	order: SortDirection,
	allStorages: MerchStorageType[],
	allMerchTransactionTypes: MerchTransactionType[],
	allActiveUsers: LookupRecipientResponse[],
	body: BodyFilters,
	options?: PatchedQueryOptions<ByPageResponse<MerchTransaction>>
) =>
	useQueryWithCallback({
		queryKey: [merchQueriesKeys.getMerchTransactions, order, params],
		queryFn: async () => {
			const { top, skip, orderBy, filters } = params;

			const storageFilter = filters?.storageFilter
				?.map((item) => `name eq '${item}'`)
				.join(' or ');

			const query = buildQuery({
				filter: {
					and: [
						...convertDateFromToValueToQueryParam(filters?.dateFilter, 'date'),
						{
							senderName: convertFilterArrayToQueryParam(
								filters?.senderFilter ?? [],
								allStorages.length + 1
							),
						},
						{
							type: convertFilterArrayToQueryParam(
								filters?.transactionTypeFilter ?? [],
								allMerchTransactionTypes.length + 1
							),
						},
						{
							recipient: convertFilterArrayToQueryParam(
								filters?.recipientFilter ?? [],
								allStorages.length + allActiveUsers.length
							),
						},

						filters?.amountFilter?.from &&
							`sumAmount ge ${numeral(filters?.amountFilter?.from).value()}`,
						filters?.storageFilter?.length &&
							`warehouseNames/any(name: ${storageFilter})`,
						filters?.amountFilter?.to &&
							`sumAmount le ${numeral(filters?.amountFilter?.to).value()}`,
						filters?.nameFilter &&
							`contains(tolower(elementName), tolower('${replaceSpecialCharacters(
								filters.nameFilter
							)}'))`,
						filters?.creatorFilter &&
							`contains(tolower(creator), tolower('${replaceSpecialCharacters(
								filters.creatorFilter
							)}'))`,
						filters?.notesFilter &&
							`contains(tolower(notes), tolower('${replaceSpecialCharacters(
								filters.notesFilter
							)}'))`,
					],
				},
				top,
				skip,
				orderBy: `${orderBy} ${order}`,
			});

			const { data } = await axios.post<ByPageResponse<MerchTransaction>>(
				`${GET_POST_MERCH_TRANSACTIONS_ENDPOINT}${query}`,
				body
			);

			return data;
		},
		refetchOnWindowFocus: false,
		...options,
	});
