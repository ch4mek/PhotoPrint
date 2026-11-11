import { useMemo } from 'react';

import { useGetJiraImportProjects } from '@/queries/jiraImport/useGetJiraImportProjects';

export const useProjectOptions = () => {
	const { data = [], isFetching: isFetchingProjects } =
		useGetJiraImportProjects();

	const projects = useMemo(
		() =>
			data
				.filter(
					({ projectId }, idx, self) =>
						self.findIndex((t) => t.projectId === projectId) === idx
				)
				.map(({ projectId, projectName, jiraProjectTag }) => ({
					id: projectId,
					name: projectName,
					jiraProjectTag,
				})),
		[data]
	);

	return { projects, isFetchingProjects };
};
