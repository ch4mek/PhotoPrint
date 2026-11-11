import { UseQueryResult } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { convertFilterArrayToQueryParam } from '../utils';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { GET_INQUIRIES_CALENDAR_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { RequestFilterType } from '@/pages/Requests/components/ListTab/types';
import { UseGetInquiriesCalendarResponse } from '@/types/absences';

export const useGetInquiriesCalendar = (
	page: number,
	rowsPerPage: number,
	period: Date[],
	filters: RequestFilterType,
	orderBy: string,
	allDepartmentsLength: number,
	inquiryId?: string
): UseQueryResult<UseGetInquiriesCalendarResponse, unknown> => {
	const periodStartDay = period[0];
	const year = periodStartDay.getFullYear();
	const month = periodStartDay.getMonth() + 1;

	return useQueryWithCallback<UseGetInquiriesCalendarResponse, unknown>({
		queryKey: [
			absencesQueriesKeys.getInquiriesCalendar,
			page,
			rowsPerPage,
			filters,
			orderBy,
			year,
			month,
			inquiryId,
		],
		queryFn: async () => {
			const { userFullName, userDepartment } = filters || {};

			const query = buildQuery({
				filter: {
					and: [
						userFullName &&
							`contains(tolower(userFullName), tolower('${replaceSpecialCharacters(
								userFullName
							)}'))`,
						userDepartment && {
							departmentId: convertFilterArrayToQueryParam(
								userDepartment,
								allDepartmentsLength
							),
						},
					],
				},
				orderBy,
			});

			const { data } = await axios.get<UseGetInquiriesCalendarResponse>(
				`${GET_INQUIRIES_CALENDAR_ENDPOINT}/${query}`,
				{
					params: {
						year,
						month,
						templateId: inquiryId,
						top: rowsPerPage,
						skip: page * rowsPerPage,
					},
				}
			);

			return data;
		},
		refetchOnWindowFocus: false,
	});
};
