import axios from 'axios';
import buildQuery from 'odata-query';

import {
	BodyFilters,
	GetMerchElementsQueryParams,
	MerchElement,
} from '../../pages/Merch/components/ElementsTab/types';
import { getOrderByValue } from '../../pages/Merch/components/ElementsTab/utils/getOrderByValue';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { GET_POST_MERCH_ELEMENTS_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ByPageResponse } from '@/queries/types';
import { convertFilterArrayToQueryParam } from '@/queries/utils';
import { MerchItemStatusesType } from '@/types/configuration';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetMerchElements = (
	params: GetMerchElementsQueryParams,
	allStatuses: MerchItemStatusesType[],
	orderByCondition: string,
	body: BodyFilters,
	options?: PatchedQueryOptions<ByPageResponse<MerchElement>>,
	order?: SortDirection
) =>
	useQueryWithCallback({
		queryKey: [
			merchQueriesKeys.getMerchElementsData,
			params,
			order,
			orderByCondition,
		],
		queryFn: async () => {
			const { top, skip, filters } = params;

			const query = buildQuery({
				top,
				skip,
				orderBy: getOrderByValue(orderByCondition, order),
				filter: {
					and: [
						{
							and: [
								{
									status: convertFilterArrayToQueryParam(
										filters?.statusFilter ?? [],
										allStatuses.length
									),
								},
								filters?.descriptionFilter &&
									`contains(tolower(Description), tolower('${replaceSpecialCharacters(
										filters?.descriptionFilter
									)}'))`,
								filters?.nameFilter &&
									`contains(tolower(Name), tolower('${replaceSpecialCharacters(
										filters?.nameFilter
									)}'))`,
								filters?.supplierFilter &&
									`contains(tolower(Supplier), tolower('${replaceSpecialCharacters(
										filters?.supplierFilter
									)}'))`,
								filters?.idFilter &&
									`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
										filters?.idFilter
									)}', Edm.String)`,
							],
						},
					],
				},
			});
			const { data } = await axios.post<ByPageResponse<MerchElement>>(
				`${GET_POST_MERCH_ELEMENTS_ENDPOINT}/${query}`,
				body
			);

			return data;
		},
		refetchOnWindowFocus: false,
		...options,
	});
