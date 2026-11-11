import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import {
	DEADLINE_DATE_FIELD_NAME,
	DEFAULT_PLANS_ORDER_BY_FIELD,
	ID_FIELD_NAME,
	STATUS_FIELD_NAME,
} from '@/constants/developmentPlan';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_DEVELOPMENT_PLANS_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import {
	DevelopmentPlanStatus,
	UserDevelopmentPlan,
} from '@/types/developmentPlan';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import {
	ByPageResponse,
	GetDevelopmentPlanQueryParams,
	ServerResponseError,
} from './types';
import { convertFilterArrayToQueryParam } from './utils';
import { convertDateFromToValueToQueryParam } from './utils/convertFromToValueToQueryParam';

export const useUserDevelopmentPlans = (
	{ filters = {}, ...params }: GetDevelopmentPlanQueryParams,
	options?: PatchedQueryOptions<
		ByPageResponse<UserDevelopmentPlan>,
		AxiosError<ServerResponseError>,
		ByPageResponse<UserDevelopmentPlan>
	>
) => {
	const { top, skip, userId, mode, orderBy, conversationId } = params;
	const { deadlineDate, status, endDate, notes, startDate, summary } = filters;

	const orderByOrderWeight = orderBy?.toLowerCase().includes(STATUS_FIELD_NAME)
		? orderBy
				.toLowerCase()
				.replace(STATUS_FIELD_NAME, DEFAULT_PLANS_ORDER_BY_FIELD)
				.concat(`,${DEADLINE_DATE_FIELD_NAME} ${SortDirection.ASC}, 
				${ID_FIELD_NAME} ${SortDirection.DESC}`)
		: orderBy;

	return useQueryWithCallback({
		queryKey: [
			TeamboardQueriesKeys.getUserDevelopmentPlans,
			`${userId}`,
			`${conversationId}`,
			filters,
			params,
		],
		queryFn: async () => {
			try {
				const query = buildQuery({
					top,
					skip,
					filter: {
						and: [
							userId && `userId eq ${userId}`,
							summary &&
								`contains(tolower(summary), tolower('${replaceSpecialCharacters(
									summary
								)}'))`,
							notes &&
								`contains(tolower(notes), tolower('${replaceSpecialCharacters(
									notes
								)}'))`,
							...convertDateFromToValueToQueryParam(startDate, 'startDate'),
							...convertDateFromToValueToQueryParam(endDate, 'endDate'),
							...convertDateFromToValueToQueryParam(
								deadlineDate,
								'deadlineDate'
							),
							status && {
								status: convertFilterArrayToQueryParam(
									status,
									Object.values(DevelopmentPlanStatus).length
								),
							},
						],
					},
					orderBy: orderByOrderWeight,
				});

				const { data } = await axios.get<ByPageResponse<UserDevelopmentPlan>>(
					`${GET_USER_DEVELOPMENT_PLANS_ENDPOINT}/${query}`,
					{
						params: { conversationId, source: mode },
					}
				);

				return data;
			} catch (error) {
				throw error as AxiosError<ServerResponseError>;
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
};
