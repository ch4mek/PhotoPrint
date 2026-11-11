import { PayloadProjectWithClientName } from '@/pages/Payload/types';

export const getProjectsWithActiveAssigment = (
	projects: PayloadProjectWithClientName[]
) => projects.filter(({ isActiveAssigment }) => isActiveAssigment);
