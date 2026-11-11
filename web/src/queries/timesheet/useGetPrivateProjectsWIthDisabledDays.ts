import { UseQueryResult } from '@tanstack/react-query';
import axios from 'axios';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { TimesheetQueryKeys } from '@/constants/queriesKeys';
import { createUserProjectWithDisabledDaysEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs from '@/lib/dayjs';

import { ProjectsWithBlockedDaysResponse } from './types';

const getRequestDate = (date: Date) =>
	dayjs(date).startOf('day').format(YYYY_MM_DD_DAYJS_DATE_FORMAT);

export const useGetPrivateProjectsWithDisabledDays = (
	userId: number,
	startDate: Date,
	endDate: Date,
	managedByPMO?: boolean
): UseQueryResult<ProjectsWithBlockedDaysResponse, unknown> =>
	useQueryWithCallback<ProjectsWithBlockedDaysResponse, unknown>({
		queryKey: [
			TimesheetQueryKeys.projectsWithDisabledDays,
			userId,
			getRequestDate(startDate),
			getRequestDate(endDate),
		],
		queryFn: () => {
			const trackIntervalFrom = getRequestDate(startDate);
			const trackIntervalTo = getRequestDate(endDate);

			return axios
				.get<ProjectsWithBlockedDaysResponse>(
					createUserProjectWithDisabledDaysEndpointUrl(userId),
					{
						params: {
							onlyManagedProjects: managedByPMO,
							trackIntervalFrom,
							trackIntervalTo,
						},
					}
				)
				.then((res) => res.data);
		},
		refetchOnWindowFocus: false,
		enabled: !!startDate && !!endDate,
	});
