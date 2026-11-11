import { useMemo } from 'react';

import { useConfiguration } from '@/context/Configuration';
import {
	ProjectAssignFormInitialValues,
	ProjectAssignFormOptions,
} from '@/forms/ProjectAccessAddingBase/types';
import { convertProjectAssignToInitialFormValues } from '@/forms/TeamboardForm/UserProject/utils';
import { getFullName } from '@/helpers/formatName';
import { useUserProjectAssignmentById } from '@/queries/teamboard/useUserProjectAssignmentById';
import { useUsersLookup } from '@/queries/useUsersLookup';
import { ProjectEntity } from '@/types/project';

export const useGetProjectAccessFormValues = (
	projectData: ProjectEntity,
	projectAssignId?: number
) => {
	const { data, isFetching } = useUserProjectAssignmentById(projectAssignId);

	const {
		configurableValues: { seniorityLevels, countries },
	} = useConfiguration();

	const { data: usersLookup } = useUsersLookup({
		isActive: true,
	});

	const userOptions = useMemo(
		() =>
			(usersLookup || []).map(({ id, lastName, firstName }) => ({
				id,
				fullName: getFullName(firstName, lastName),
			})),
		[usersLookup]
	);

	const formOptions: ProjectAssignFormOptions = useMemo(
		() => ({
			users: userOptions,
			projects: [projectData],
		}),
		[userOptions]
	);

	const formInitialValue: ProjectAssignFormInitialValues = useMemo(() => {
		if (!projectAssignId || isFetching) {
			return {
				project: {
					value: {
						id: projectData.id,
						name: projectData.name,
					},
					disabled: true,
				},
				billability: {
					value: projectData.isBillable,
				},
			};
		}

		if (!data) {
			return {};
		}

		const user = {
			id: data.userId,
			fullName: data.userFullName,
		};

		return convertProjectAssignToInitialFormValues(
			data,
			user,
			[projectData],
			seniorityLevels,
			countries,
			false
		);
	}, [data, projectAssignId, seniorityLevels, countries, isFetching]);

	return {
		isFetching,
		formOptions,
		formInitialValue,
		isAssignEditable: data?.isAssignEditable,
	};
};
