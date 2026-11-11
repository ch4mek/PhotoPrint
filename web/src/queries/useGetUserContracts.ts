import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { DateFormat } from '@/constants/date';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_CONTRACTS_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { ToastContextValues } from '@/context/types';
import { capitalizeString } from '@/helpers/capitalizeString';
import { formatDate } from '@/helpers/formatDate';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ContractsFilters } from '@/pages/TeamBoardUser/components/ContractsTab/types';
import { ServerResponseError } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { GetUserContractsResponse } from './teamboard/types';
import { convertFilterArrayToQueryParam } from './utils';

const formatFilterDate = (date: Date) =>
	formatDate(date, DateFormat.YYYY_MM_DD);

export const useGetUserContracts = (
	userId: number,
	toast: ToastContextValues,
	skip: number,
	top: number,
	order: SortDirection,
	orderByCondition: string,
	filter: ContractsFilters,
	fireTypesCount?: number,
	queryOptions?: PatchedQueryOptions<
		GetUserContractsResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetUserContractsResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			TeamboardQueriesKeys.userContracts,
			skip,
			top,
			order,
			orderByCondition,
			filter,
		],
		queryFn: async () => {
			const {
				contractFrom,
				contractTo,
				fireDate,
				fireType,
				fireReason,
				fireDescription,
			} = filter;

			const searchFilter = {
				and: [
					`cast(userId, Edm.String) eq cast('${replaceSpecialCharacters(
						String(userId)
					)}', Edm.String)`,

					contractFrom?.from &&
						`contractFrom ge ${formatFilterDate(contractFrom.from)}`,
					contractFrom?.to &&
						`contractFrom le ${formatFilterDate(contractFrom.to)}`,
					contractTo?.from &&
						`contractTo ge ${formatFilterDate(contractTo.from)}`,
					contractTo?.to && `contractTo le ${formatFilterDate(contractTo.to)}`,
					fireDate?.from && `fireDate ge ${formatFilterDate(fireDate.from)}`,
					fireDate?.to && `fireDate le ${formatFilterDate(fireDate.to)}`,
					fireTypesCount && fireType.length
						? {
								fireType: convertFilterArrayToQueryParam(
									fireType,
									fireTypesCount
								),
						  }
						: undefined,
					fireReason
						? `contains(tolower(fireReason), tolower('${fireReason}'))`
						: undefined,
					fireDescription
						? `contains(tolower(fireDescription), tolower('${fireDescription}'))`
						: undefined,
				],
			};

			try {
				const query = buildQuery({
					filter: {
						and: [searchFilter],
					},
					orderBy: `${capitalizeString(orderByCondition)} ${order}`,
					top,
					skip: skip * top,
				});

				const res = await axios.get<GetUserContractsResponse>(
					`${GET_USER_CONTRACTS_API_ENDPOINT}${query}`
				);

				return res.data;
			} catch (error) {
				return Promise.reject(error);
			}
		},
		onError: createOnErrorHandler(toast),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...queryOptions,
	});
