import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { dashboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_APPROVED_INQUIRIES_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GetApprovedAbsencesResponse, UseGetDashboardParams } from './types';

export const useGetApprovedAbsences = ({
	departmentId,
}: UseGetDashboardParams) =>
	useQueryWithCallback<GetApprovedAbsencesResponse, AxiosError>({
		queryKey: [dashboardQueriesKeys.getApprovedAbsences, departmentId],
		queryFn: async () =>
			axios
				.get<GetApprovedAbsencesResponse>(GET_APPROVED_INQUIRIES_ENDPOINT, {
					params: {
						departmentId,
					},
				})
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});
