import { ProjectLookupResponse } from '@/types/project';

export const normalizeProjects =
	(projects: ProjectLookupResponse[] | undefined, isFetching: boolean) => () =>
		isFetching || !projects?.length
			? []
			: projects.map(({ id, name }) => ({
					id,
					name,
			  }));
