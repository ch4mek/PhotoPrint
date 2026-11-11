import { TeamboardUserProjectsFormProps } from '@/forms/TeamboardForm/UserProject/types';
import { ProjectEntity } from '@/types/project';

export interface ProjectAccessAddingFormProps
	extends Omit<TeamboardUserProjectsFormProps, 'user'> {
	projectData: ProjectEntity;
}
