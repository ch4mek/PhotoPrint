import { useMemo } from 'react';

import {
	allAssignedPrivateProjectsPaginationParams,
	allAssignedPrivateProjectsFilters,
} from '../constants';
import { ProjectOption } from '../types';

import { useProjectsLookup } from '@/queries/useProjectsLookup';
import { useUserProjectAssignments } from '@/queries/useUserProjectAssignments';
import { ProjectType } from '@/types/project';

interface UseAllProjectsAssignedToUserProps {
	userId?: number;
	enabled?: boolean;
}

export const useAllProjectsAssignedToUser = ({
	userId,
	enabled,
}: UseAllProjectsAssignedToUserProps) => {
	const {
		data: privateUserProjects,
		isLoading: isPrivateProjectsLoading,
		isFetching: isPrivateProjectsFetching,
	} = useUserProjectAssignments(
		userId,
		allAssignedPrivateProjectsPaginationParams.page,
		allAssignedPrivateProjectsPaginationParams.top,
		allAssignedPrivateProjectsFilters,
		undefined,
		undefined,
		{ enabled }
	);

	const {
		data: publicUserProjects,
		isLoading: isPublicProjectsLoading,
		isFetching: isPublicProjectsFetching,
	} = useProjectsLookup({
		isPublic: true,
		isActive: true,
		enabled,
	});

	const userAvailableProjects: ProjectOption[] = useMemo(() => {
		if (publicUserProjects && privateUserProjects && enabled) {
			const privateProjects = Object.values(
				privateUserProjects.items.reduce<Record<number, ProjectOption>>(
					(acc, project) => {
						const projectId = project.projectId;

						if (!acc[projectId]) {
							acc[projectId] = {
								id: projectId,
								name: project.projectName,
								type: ProjectType.Private,
							};
						}

						return acc;
					},
					{}
				)
			);

			return [
				...privateProjects,
				...publicUserProjects.map((project) => ({
					...project,
					type: ProjectType.Public,
				})),
			];
		}

		return [];
	}, [publicUserProjects, privateUserProjects, enabled]);

	const isLoadingUserAvailableProjects =
		isPrivateProjectsLoading ||
		isPrivateProjectsFetching ||
		isPublicProjectsLoading ||
		isPublicProjectsFetching;

	return { userAvailableProjects, isLoadingUserAvailableProjects };
};
