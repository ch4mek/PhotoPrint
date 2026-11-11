import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { GET_PERMISSIONS_ASSIGNMENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';

import {
	BillableWorkingPermissionsAssignments,
	UseBillableWorkingHoursPermissionsOptions,
} from './types';

export const BILLABLE_WORKING_HOURS_PERMISSIONS_QUERY_KEY =
	'billable-working-hours-permissions';

export const useBillableWorkingHoursPermissions = ({
	page,
	rowsPerPage,
	order,
	orderBy,
	userIdFilter,
	userFullNameFilter,
	projectNameFilter,
	userDepartmentId,
	permissionName,
	permissionId,
}: UseBillableWorkingHoursPermissionsOptions) =>
	useQueryWithCallback<
		BillableWorkingPermissionsAssignments,
		ServerResponseError
	>({
		queryKey: [
			BILLABLE_WORKING_HOURS_PERMISSIONS_QUERY_KEY,
			page,
			rowsPerPage,
			order,
			orderBy,
			userIdFilter,
			userFullNameFilter,
			projectNameFilter,
			userDepartmentId,
			permissionName,
			permissionId,
		],

		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						userIdFilter &&
							`cast(userId, Edm.String) eq cast('${replaceSpecialCharacters(
								userIdFilter
							)}', Edm.String)`,

						projectNameFilter &&
							`contains(tolower(projectName), tolower('${replaceSpecialCharacters(
								projectNameFilter
							)}'))`,

						userFullNameFilter &&
							`contains(tolower(userFullname), tolower('${replaceSpecialCharacters(
								userFullNameFilter
							)}'))`,
					],
				},

				orderBy: `${orderBy} ${order}`,
				top: rowsPerPage,
				skip: page * rowsPerPage,
			});
			const { data } = await axios.get<BillableWorkingPermissionsAssignments>(
				`${GET_PERMISSIONS_ASSIGNMENTS_API_ENDPOINT}/${query}`,
				{
					params: { userDepartmentId, permissionName, permissionId },
				}
			);

			return data;
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
