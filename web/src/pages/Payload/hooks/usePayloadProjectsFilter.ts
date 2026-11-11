import { useMemo } from 'react';

import { normalizeProjects } from '@/helpers/normalizeProjects';
import { useProjectsLookup } from '@/queries/useProjectsLookup';

export const usePayloadProjectsFilter = () => {
	const { data: projects, isFetching } = useProjectsLookup({
		isPublic: false,
	});

	return useMemo(normalizeProjects(projects, isFetching), [isFetching]);
};
