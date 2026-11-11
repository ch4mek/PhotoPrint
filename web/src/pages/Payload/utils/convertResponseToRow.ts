import { findById } from '@/helpers/findById';
import {
	DEFAULT_CIRCLE_COLOR_PICKER_COLOR,
	PAGE_SIZE,
} from '@/pages/Payload/contants';
import {
	ConvertResponseRowProps,
	HoursDetail,
	PayloadGridRow,
	PayloadProjectWithClientName,
} from '@/pages/Payload/types';
import { getNewCellHeight } from '@/pages/Payload/utils/getNewCellHeight';
import { getProjectsWithActiveAssigment } from '@/pages/Payload/utils/getProjectsWithActiveAssigment';

import { convertHourDetailsArrayToObject } from './convertHourDetailsArrayToObject';
import { formatHoursAndFTEValues } from './formatHoursAndFTEValues';

export function convertResponseToRow({
	handleEdit,
	responseData,
	departmentsData,
	page,
}: ConvertResponseRowProps) {
	if (!responseData) {
		return [];
	}

	const generalRows = responseData.users.map(
		(
			{
				comments,
				departmentId,
				firstName,
				lastName,
				firstNameRu,
				lastNameRu,
				hourDetails,
				needProject,
				plan,
				projects: userProjectsList,
				specializationName,
				specializationSeniority,
				userId,
				resourceGroupName,
				color: userColor,
				isActive,
				canUserInfoBeRetrievedByCurrentUser,
			},
			ind
		) => {
			const formattedHoursDetails: HoursDetail[] = hourDetails.map(
				({ projectDetails, ...rest }) => {
					const formattedMainValues = formatHoursAndFTEValues(rest);
					const formattedProjectDetailsValues = projectDetails.map(
						formatHoursAndFTEValues
					);

					return {
						...formattedMainValues,
						projectDetails: formattedProjectDetailsValues,
					};
				}
			);

			const months = convertHourDetailsArrayToObject(
				formattedHoursDetails,
				userProjectsList
			);

			const { projects, year, clients, usersTotal } = responseData;

			const userProjectsMap = new Map(
				userProjectsList.map(
					({
						projectId,
						isActiveAssignement,
						isTimeOnProjectBillable,
						isFutureActiveAssignment,
					}) => [
						projectId,
						{
							isActiveAssignement,
							isTimeOnProjectBillable,
							isFutureActiveAssignment,
						},
					]
				)
			);

			const clientMap = new Map(
				clients.map(({ clientId, name, color }) => [clientId, { name, color }])
			);

			const userProjects: PayloadProjectWithClientName[] = projects
				.filter(({ projectId }) => userProjectsMap.has(projectId))
				.map((project) => {
					const client = clientMap.get(project.clientId) || null;
					const userProject = userProjectsMap.get(project.projectId);

					return {
						...project,
						color: client?.color || '',
						isActiveAssigment: userProject?.isActiveAssignement || false,
						isTimeOnProjectBillable:
							userProject?.isTimeOnProjectBillable || false,
						isFutureActiveAssignment:
							userProject?.isFutureActiveAssignment || false,
						clientName: client?.name || '',
					};
				})
				.sort((first, second) => first.projectId - second.projectId);
			const userInfo = {
				firstName,
				lastName,
				firstNameRu,
				lastNameRu,
				specializationName,
				specializationSeniority,
				isActive: isActive && canUserInfoBeRetrievedByCurrentUser,
			};

			const projectWithActiveAssigment =
				getProjectsWithActiveAssigment(userProjects);
			const cellHeight = getNewCellHeight(
				false,
				userProjects.length,
				projectWithActiveAssigment.length
			);

			const rowIndex = ind + page * PAGE_SIZE + 1;

			return {
				id: userId,
				department: findById(departmentsData, departmentId)?.fullName,
				group: resourceGroupName,
				employee: userInfo,
				comments,
				needProject,
				projects: userProjects,
				...months,
				plan,
				year,
				handleEditClick: handleEdit,
				expanded: false,
				color: userColor ?? DEFAULT_CIRCLE_COLOR_PICKER_COLOR,
				projectItemsRowHeight: cellHeight,
				disabled: !isActive,
				isLastRow: usersTotal > 1 && rowIndex === usersTotal,
			};
		}
	);

	return generalRows as PayloadGridRow[];
}
