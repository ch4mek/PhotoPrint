import { ProjectOption } from '../types';

export const getProjectKey = (option: ProjectOption) =>
	`${option.id}-${option.type}`;
