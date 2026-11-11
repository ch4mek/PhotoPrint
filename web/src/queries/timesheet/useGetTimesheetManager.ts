import { UseQueryResult } from '@tanstack/react-query';
import axios from 'axios';

import { DateFormat } from '@/constants/date';
import { TimesheetQueryKeys } from '@/constants/queriesKeys';
import { GET_TIMESHEET_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs from '@/lib/dayjs';
import { timesheetModesIds } from '@/pages/TimeSheet/constants';
import { TimesheetModeFilterOption } from '@/pages/TimeSheet/types';

import { TimesheetManagerDataResponse } from './types';

const { departmentModeId, projectsModeId, personalModeId, groupModeId } =
	timesheetModesIds;

export const useGetTimesheetManager = (
	page: number,
	rowsPerPage: number,
	period: Date[],
	role: TimesheetModeFilterOption,
	clientId: number | null | string,
	projectId: number | null | string,
	userId: number | null | string,
	resourceGroupId?: number | string,
	isPersonalMode?: boolean
): UseQueryResult<TimesheetManagerDataResponse, unknown> =>
	useQueryWithCallback<TimesheetManagerDataResponse, unknown>({
		queryKey: [
			TimesheetQueryKeys.timesheetManager,
			page,
			rowsPerPage,
			period[0].getTime(),
			period.at(-1)?.getTime(),
			role,
			clientId,
			projectId,
			userId,
			resourceGroupId,
		],
		queryFn: async () => {
			const onlyManagedProjects = role?.id === projectsModeId;

			const departmentId =
				role?.id === departmentModeId ||
				role?.id === personalModeId ||
				role?.id === groupModeId ||
				onlyManagedProjects
					? null
					: role.id;

			const { data } = await axios.get<TimesheetManagerDataResponse>(
				`${GET_TIMESHEET_API_ENDPOINT}`,
				{
					params: {
						reportForDateFrom: dayjs(period[0]).format(DateFormat.YYYY_MM_DD),
						reportForDateTo: dayjs(period.at(-1)).format(DateFormat.YYYY_MM_DD),
						departmentId,
						projectId,
						clientId,
						userId,
						resourceGroupId,
						top: rowsPerPage,
						skip: page * rowsPerPage,
						onlyManagedProjects: onlyManagedProjects ? true : undefined,
						assignedForSelectedDate: projectId != null || clientId != null,
						isPersonalMode: !!isPersonalMode,
					},
				}
			);

			return data;
		},
		refetchOnWindowFocus: false,
	});
